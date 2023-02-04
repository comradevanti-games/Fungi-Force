using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class ButtonFunctions : MonoBehaviour
    {
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        public void SetHost(bool isHost)
        {
            Blackboard.IsHost = isHost;
        }
    }
    
}
