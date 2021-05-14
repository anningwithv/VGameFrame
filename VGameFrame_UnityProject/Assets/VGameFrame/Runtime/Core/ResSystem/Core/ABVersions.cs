using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VGameFrame
{
    public enum VerifyBy
    {
        Size,
        Hash,
    }

    public static class ABVersions
    {
        public const string bundleDetail = "BundleDetail";
        public const string versionDetail = "VersionDetail";
        public static readonly VerifyBy verifyBy = VerifyBy.Hash;
        private static readonly VersionDisk _disk = new VersionDisk();
        private static readonly Dictionary<string, VersionFile> _updateData = new Dictionary<string, VersionFile>();
        private static readonly Dictionary<string, VersionFile> _baseData = new Dictionary<string, VersionFile>();

        public static void BuildVersions(string outputPath, string[] bundles, int version)
        {
            var versionDetailPath = outputPath + "/" + versionDetail;
            if (File.Exists(versionDetailPath))
            {
                File.Delete(versionDetailPath);
            }

            var bundleDetailPath = outputPath + "/" + bundleDetail;
            if (File.Exists(bundleDetailPath))
            {
                File.Delete(bundleDetailPath);
            }

            //将各个bundle的名称、长度、Hash值写入BundleDetail文件
            var disk = new VersionDisk();
            foreach (var file in bundles)
            {
                using (var fs = File.OpenRead(outputPath + "/" + file))
                {
                    disk.AddFile(file, fs.Length, Utility.GetCRC32Hash(fs));
                }
            }

            disk.name = bundleDetailPath;
            disk.Save();

            //将版本号和文件个数写入VersionDetail文件
            using (var stream = File.OpenWrite(versionDetailPath))
            {
                var writer = new BinaryWriter(stream);
                writer.Write(version);
                writer.Write(disk.files.Count + 1);
                using (var fs = File.OpenRead(bundleDetailPath))
                {
                    var file = new VersionFile { name = bundleDetail, len = fs.Length, hash = Utility.GetCRC32Hash(fs) };
                    file.Serialize(writer);
                }
                foreach (var file in disk.files)
                {
                    file.Serialize(writer);
                }
            }
        }

        public static int LoadVersion(string filename)
        {
            if (!File.Exists(filename))
                return -1;
            try
            {
                using (var stream = File.OpenRead(filename))
                {
                    var reader = new BinaryReader(stream);
                    return reader.ReadInt32();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return -1;
            }
        }

        public static List<VersionFile> LoadVersions(string filename, bool update = false)
        {
            var rootDir = Path.GetDirectoryName(filename);
            var data = update ? _updateData : _baseData;
            data.Clear();
            using (var stream = File.OpenRead(filename))
            {
                var reader = new BinaryReader(stream);
                var list = new List<VersionFile>();
                var ver = reader.ReadInt32();
                Debug.Log("LoadVersions:" + ver);
                var count = reader.ReadInt32();
                for (var i = 0; i < count; i++)
                {
                    var version = new VersionFile();
                    version.Deserialize(reader);
                    list.Add(version);
                    data[version.name] = version;
                    var dir = string.Format("{0}/{1}", rootDir, Path.GetDirectoryName(version.name));
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
                return list;
            }
        }
        public static void UpdateDisk(string savePath, List<VersionFile> newFiles)
        {
            var saveFiles = new List<VersionFile>();
            var files = _disk.files;
            foreach (var file in files)
            {
                if (_updateData.ContainsKey(file.name))
                {
                    saveFiles.Add(file);
                }
            }
            _disk.Update(savePath, newFiles, saveFiles);
        }

        public static bool LoadDisk(string filename)
        {
            return _disk.Load(filename);
        }

        public static bool IsNew(string path, long len, string hash)
        {
            VersionFile file;
            var key = Path.GetFileName(path);
            if (_baseData.TryGetValue(key, out file))
            {
                if (key.Equals(bundleDetail) ||
                    file.len == len && file.hash.Equals(hash, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (_disk.Exists())
            {
                var vdf = _disk.GetFile(path, hash);
                if (vdf != null && vdf.len == len && vdf.hash.Equals(hash, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (!File.Exists(path))
            {
                return true;
            }

            using (var stream = File.OpenRead(path))
            {
                if (stream.Length != len)
                {
                    return true;
                }
                if (verifyBy != VerifyBy.Hash)
                    return false;
                return !Utility.GetCRC32Hash(stream).Equals(hash, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
