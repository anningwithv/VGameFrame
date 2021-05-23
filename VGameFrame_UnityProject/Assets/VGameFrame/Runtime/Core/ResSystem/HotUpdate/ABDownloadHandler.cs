//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFramework. 
//  Author:      V 
//-------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace VGameFramework
{
    public class ABDownloadHandler : DownloadHandlerScript, IDisposable, ICloneable
    {
        #region ICloneable implementation

        public object Clone()
        {
            return new ABDownloadHandler()
            {
                id = id,
                hash = hash,
                url = url,
                len = len,
                savePath = savePath,
                completed = completed,
                name = name
            };
        }

        #endregion

        public int id { get; set; }

        public string error { get; private set; }

        public long len { get; set; }

        public string hash { get; set; }

        public string url { get; set; }

        public long position { get; private set; }

        public string name { get; set; }

        public string tempPath
        {
            get
            {
                var dir = Path.GetDirectoryName(savePath);
                return string.Format("{0}/{1}", dir, hash);
            }
        }

        public string savePath;

        public Action<ABDownloadHandler> completed { get; set; }

        private UnityWebRequest _request;
        private FileStream _stream;
        private bool _running;
        private bool _finished = false;

        protected override float GetProgress()
        {
            return position * 1f / len;
        }

        protected override byte[] GetData()
        {
            return null;
        }

        protected override void ReceiveContentLength(int contentLength)
        {
        }

        protected override bool ReceiveData(byte[] buffer, int dataLength)
        {
            if (!string.IsNullOrEmpty(_request.error))
            {
                error = _request.error;
                Complete();
                return true;
            }

            _stream.Write(buffer, 0, dataLength);
            position += dataLength;
            return _running;
        }

        protected override void CompleteContent()
        {
            Complete();
        }

        public override string ToString()
        {
            return string.Format("{0}, size:{1}, hash:{2}", url, len, hash);
        }

        public void Start()
        {
            if (_running)
            {
                return;
            }

            Debug.Log("Start Download��" + url);

            error = null;
            finished = false;
            _running = true;
            _stream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.Write);
            position = _stream.Length;
            if (position < len)
            {
                _stream.Seek(position, SeekOrigin.Begin);
                _request = UnityWebRequest.Get(url);
                _request.SetRequestHeader("Range", "bytes=" + position + "-");
                _request.downloadHandler = this;
                _request.SendWebRequest();
                Debug.Log("Send web request��" + url);
            }
            else
            {
                Complete();
            }
        }

        public void Update()
        {
            if (_running)
            {
                if (_request.isDone && _request.downloadedBytes < (ulong)len)
                {
                    error = "unknown error: downloadedBytes < len";
                }
                if (!string.IsNullOrEmpty(_request.error))
                {
                    error = _request.error;
                }
            }
        }

        public new void Dispose()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
                _stream = null;
            }
            if (_request != null)
            {
                _request.Abort();
                _request.Dispose();
                _request = null;
            }
            base.Dispose();
            _running = false;
            finished = true;
        }

        public void Complete(bool stop = false)
        {
            Dispose();
            if (stop)
            {
                return;
            }
            CheckError();
        }

        private void CheckError()
        {
            if (File.Exists(tempPath))
            {
                if (string.IsNullOrEmpty(error))
                {
                    using (var fs = File.OpenRead(tempPath))
                    {
                        if (fs.Length != len)
                        {
                            error = "�����ļ������쳣:" + fs.Length;
                        }
                        if (ABVersions.verifyBy == VerifyBy.Hash)
                        {
                            const StringComparison compare = StringComparison.OrdinalIgnoreCase;
                            if (!hash.Equals(Utility.GetCRC32Hash(fs), compare))
                            {
                                error = "�����ļ���ϣ�쳣:" + hash;
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(error))
                {
                    File.Copy(tempPath, savePath, true);
                    File.Delete(tempPath);
                    Debug.Log("Complete Download��" + url);
                    if (completed == null)
                        return;
                    completed.Invoke(this);
                    completed = null;
                }
                else
                {
                    File.Delete(tempPath);
                }
            }
            else
            {
                error = "�ļ�������";
            }
        }

        public void Retry()
        {
            Dispose();
            Start();
        }

        public bool finished
        {
            get { return _finished; }
            private set { _finished = value; }
        }
    }

}