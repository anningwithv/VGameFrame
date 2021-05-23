using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGameFramework
{
    public interface IManager
    {
        void OnInit();
        void OnUpdate();
        void OnDestroy();
    }
}