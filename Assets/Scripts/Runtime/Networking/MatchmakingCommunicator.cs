using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    public class MatchmakingCommunicator : MonoBehaviour
    {
       public string matchmakingServer = "jlot.tk"; 
        public bool advertise = false;
        private NetworkManager _networkManager;

        public void Start()
        {
            _networkManager = GetComponent<NetworkManager>();
        }

        public void StartAdvertisingGame()
        {
            Debug.Log("MISSING IMPLEMENTATION FOR MATCHMAKING COMMUNICATOR");
        }

        public void DelistGame()
        {
            Debug.Log("MISSING DELIST IMPLEMENTATION");
        }
    }
}