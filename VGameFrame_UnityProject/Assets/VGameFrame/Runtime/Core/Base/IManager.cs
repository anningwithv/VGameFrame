using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGameFrame
{
    public interface IManager
    {
        void OnInit();
        void OnUpdate();
        void OnDestroy();
    }
}