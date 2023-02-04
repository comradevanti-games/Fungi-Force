using ENet;
using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    [RequireComponent(typeof(IncomingCommandHandler))]
    public class NetworkManager : MonoBehaviour
    {
        private bool isInitialized = false;
        public static NetworkManager netman;
        public static NetworkManager server;
        public static NetworkManager client;
        public bool debug;
        public ushort port = 808;
        public string ip = "127.0.0.1";
        public ITransferLayer.Type networkingBackend;
        private IncomingCommandHandler _incomingCommandHandler;
        public Host host;
        public Peer otherClient;
        private ITransferLayer transferLayer;

        public void FixedUpdate()
        {
            if(!isInitialized) return;
            
            // Debug.Log("ENT UPDATE");
            var bytes = transferLayer.NetUpdate();
            if (bytes != null && bytes.Length > 0)
            {
                Debug.Log("RECEIVED DATA");
                ManageIncomingPacket(bytes);
            }
        }

        public void Init(bool isServer)
        {
            _incomingCommandHandler = GetComponent<IncomingCommandHandler>();
            if (isServer)
                server = this;
            else
                client = this;
            netman = this;
            switch (networkingBackend)
            {
                case ITransferLayer.Type.ENET:
                    transferLayer = new ENetTransfer();
                    break;
                case ITransferLayer.Type.DUMMY_TRANSFER:
                    transferLayer = new StubTransfer();
                    break;
            }
            if(debug) Debug.Log("INIT CALLED WITH " + ip + ":" + port);
            transferLayer.SetConnectionInfo(ip, port);
            transferLayer.SetServer(isServer);

            if (isServer)
                transferLayer.CreateServer();
            else
                transferLayer.CreateClient();

            isInitialized = true;
        }
        
        private void ManageIncomingPacket(byte[] incoming)
        {
            _incomingCommandHandler.HandleCommand(incoming);
        }

        public void SendCommand(BaseCommand baseCommand, byte channelId = 0)
        {
            transferLayer.Send(baseCommand, channelId);
        }
    }
}