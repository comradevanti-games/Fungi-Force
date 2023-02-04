using System;
using ENet;
using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    [RequireComponent(typeof(IncomingCommandHandler))]
    public class NetworkManager : MonoBehaviour
    {
        public bool debug;
        public ushort port = 808;
        public bool isServer = true;
        public string ip = "127.0.0.1";
        public Host host;
        public Peer otherClient;
        public static NetworkManager netman;
        public static NetworkManager server;
        public static NetworkManager client;
        private IncomingCommandHandler _incomingCommandHandler;
        private ITransferLayer transferLayer;
        public TransferLayer networkingBackend;
        
        public enum TransferLayer
        {
            ENET
        }
        
        private void Start()
        {
            _incomingCommandHandler = GetComponent<IncomingCommandHandler>();
            if (isServer)
                server = this;
            else
                client = this;
            netman = this;
            switch (networkingBackend)
            {
                case TransferLayer.ENET:
                    transferLayer = new ENetTransfer();
                    break;
            }
            transferLayer.SetConnectionInfo(ip, port);
            transferLayer.SetServer(isServer);
            StartNewNetworkConnection();
            
        }

        private void StartNewNetworkConnection()
        {
            if (isServer)
            {
                transferLayer.CreateServer();
            }
            else
            {
                transferLayer.CreateClient();
            }
        }

        
        
        private void CreateClient()
        {
            Library.Initialize();
            // CREATING CLIENT
            host = new Host();
            Address address = new Address();
            address.SetHost(ip);
            address.Port = port;
            
            host.Create();
            if(debug) Debug.Log("CONNECTING TO " + ip + ":" + port);
            otherClient = host.Connect(address);
            if(debug) Debug.Log("CONNECTION ESTABLISHED");
        }

        public void FixedUpdate()
        {
            // Debug.Log("ENT UPDATE");
            byte[] bytes = transferLayer.NetUpdate();
            if (bytes != null && bytes.Length > 0)
            {
                Debug.Log("RECEIVED DATA");
                ManageIncomingPacket(bytes);
            }
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