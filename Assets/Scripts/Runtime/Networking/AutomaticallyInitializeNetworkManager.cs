using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    public class AutomaticallyInitializeNetworkManager : MonoBehaviour
    {
        public bool isServer;
        public string ip;
        public ushort port;

        public void Start()
        {
            if (isServer)
                GetComponent<NetworkManager>().InitAsHost();
            else
                GetComponent<NetworkManager>().InitAsGuest(ip, port);
        }
    }
}