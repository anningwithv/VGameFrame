using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

namespace VGameFrame
{
	public class VersionFile
	{
		public string hash { get; set; }

		public long id { get; set; }

		public long len { get; set; }

		public string name { get; set; }

		public long offset { get; set; }

		public VersionFile ()
		{
			offset = -1;
		}

		public void Serialize (BinaryWriter writer)
		{
			writer.Write (name);
			writer.Write (len);
			writer.Write (hash);
		}

		public void Deserialize (BinaryReader reader)
		{
			name = reader.ReadString ();
			len = reader.ReadInt64 ();
			hash = reader.ReadString ();
		}
	}

	public class VersionDisk
	{
		private readonly byte[] _buffers = new byte[1024 * 4];
		private readonly Dictionary<string, VersionFile> _data = new Dictionary<string, VersionFile> ();
		private readonly List<VersionFile> _files = new List<VersionFile>();
		public  List<VersionFile> files { get { return _files; }}
		public string name { get; set; } 
		private long _pos;
		private long _len;

		public VersionDisk ()
		{
		}

		public bool Exists ()
		{
			return files.Count > 0;
		}

		private void AddFile (VersionFile file)
		{
			_data [file.name] = file;
			files.Add (file);
		}

		public void AddFile (string path, long len, string hash)
		{ 
			var file = new VersionFile{ name = path, len = len, hash = hash };
			AddFile (file);
		}

		private void WriteFile (string path, BinaryWriter writer)
		{
			using (var fs = File.OpenRead (path)) {
				var len = fs.Length;
				WriteStream (len, fs, writer);
			}
		}

		private void WriteStream (long len, Stream stream, BinaryWriter writer)
		{
			var count = 0L;
			while (count < len) {
				var read = (int)Math.Min (len - count, _buffers.Length);
				stream.Read (_buffers, 0, read);
				writer.Write (_buffers, 0, read);
				count += read;
			}
		}

		public bool Load (string path)
		{
			if (!File.Exists (path))
				return false;

			Clear ();

			name = path;
			using (var reader = new BinaryReader (File.OpenRead (path))) {
				var count = reader.ReadInt32 ();
				for (var i = 0; i < count; i++) {
					var file = new VersionFile { id = i };
					file.Deserialize (reader);
					AddFile (file); 
				} 
				_pos = reader.BaseStream.Position;  
			}
			Reindex ();
			return true;
		}

		public void Reindex ()
		{
			_len = 0L;
			for (var i = 0; i < files.Count; i++) {
				var file = files [i];
				file.offset = _pos + _len;
				_len += file.len;
			}
		} 

		public VersionFile GetFile (string path, string hash)
		{
			var key = Path.GetFileName (path);
			VersionFile file;
			_data.TryGetValue (key, out file);
			return file;
		}

		public void Update(string dataPath, List<VersionFile> newFiles, List<VersionFile> saveFiles)
		{
			var dir = Path.GetDirectoryName(dataPath); 
			using (var stream = File.OpenRead(dataPath))
			{
				foreach (var item in saveFiles)
				{
					var path = string.Format("{0}/{1}", dir, item.name);
					if (File.Exists(path)) { continue; }  
					stream.Seek(item.offset, SeekOrigin.Begin); 
					using (var fs = File.OpenWrite(path))
					{
						var count = 0L;
						var len = item.len;
						while (count < len)
						{
							var read = (int) Math.Min(len - count, _buffers.Length);
							stream.Read(_buffers, 0, read);
							fs.Write(_buffers, 0, read);
							count += read;
						}
					}    
					newFiles.Add(item);
				}
			}

			if (File.Exists(dataPath))
			{
				File.Delete(dataPath);
			}
			
			using (var stream = File.OpenWrite (dataPath)) {
				var writer = new BinaryWriter (stream);
				writer.Write (newFiles.Count);
				foreach (var item in newFiles) {
					item.Serialize (writer);
				}  
				foreach (var item in newFiles) {
					var path = string.Format("{0}/{1}", dir, item.name);
					WriteFile (path, writer);
					File.Delete (path);
					Debug.Log ("Delete:" + path);
				} 
			} 
		}

		public void Save ()
		{
			var dir = Path.GetDirectoryName (name);   
			using (var stream = File.OpenWrite (name)) {
				var writer = new BinaryWriter (stream);
				writer.Write (files.Count);
				foreach (var item in files) {
					item.Serialize (writer);
				}  
				foreach (var item in files) {
					var path = dir + "/" + item.name;
					WriteFile (path, writer);
				}
			} 
		}

		public void Clear ()
		{
			_data.Clear ();
			files.Clear ();
		}
	}
}