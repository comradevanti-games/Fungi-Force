using System;
using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    public class AutomaticallyInitializeNetworkManager : MonoBehaviour
    {
        public bool isServer;
        public void Start()
        {
            GetComponent<NetworkManager>().Init(isServer);
        }
    }
}