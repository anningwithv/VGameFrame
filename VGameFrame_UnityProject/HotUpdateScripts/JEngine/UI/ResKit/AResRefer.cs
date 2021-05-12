﻿//
// AResRefer.cs
//
// Author:
//       L-Fone <275757115@qq.com>
//
// Copyright (c) 2020 JEngine
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using JEngine.Core;
using UnityEngine;
using UnityEngine.UI;

namespace JEngine.UI.ResKit
{
    public abstract class AResRefer
    {
        protected List<ReferInfo> _referList = new List<ReferInfo>();

        public abstract void Refer();

        public abstract void LoadRefer(Action call, bool isloadFromResource = false);

        public abstract void ReleaseRefer();

        public virtual void AddRefer(ResType type, string referStr, AssetType assetType)
        {
            ReferInfo refer = null;

            if (!string.IsNullOrEmpty(referStr))
            {
                refer = new ReferInfo(type, referStr, assetType);
                _referList.Add(refer);
            }
        }
        public List<ReferInfo> GetReferList { get { return _referList; } }
    }
}
