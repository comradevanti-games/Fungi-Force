using ENet;
using Networking;
using UnityEngine;

namespace TeamShrimp.GGJ23.Runtime.Networking
{
    public class NetworkManager : MonoBehaviour
    {
        public ushort port = 808;
        public bool isServer = true;
        public string ip = "127.0.0.1";
        public Host host;
        public Peer otherClient;
        public static NetworkManager netman;
        public static NetworkManager server;
        public static NetworkManager client;
        private IncomingCommandHandler _incomingCommandHandler;
        
        
        private void Start()
        {
            _incomingCommandHandler = GetComponent<IncomingCommandHandler>();
            if (isServer)
                server = this;
            else
                client = this;
            netman = this;
            StartNewNetworkConnection();
            
        }
        
        
        private void StartNewNetworkConnection()
        {
            Library.Initialize();
            if (isServer)
            {
                CreateServer();
            }
            else
            {
                CreateClient();
            }
        }

        private void CreateServer()
        {
            // CREATING SERVER
            host = new Host();
            Address address = new Address {Port = port};

            Debug.Log("CREATING SERVER");
            host.Create(address, 1);
            Debug.Log("CREATED AS SERVER");
        }
        
        
        private void CreateClient()
        {
            // CREATING CLIENT
            host = new Host();
            Address address = new Address();
            address.SetHost(ip);
            address.Port = port;
            
            host.Create();
            Debug.Log("CONNECTING TO " + ip + ":" + port);
            otherClient = host.Connect(address);
            Debug.Log("CONNECTION ESTABLISHED");
        }

        public void FixedUpdate()
        {
            HandleENetUpdate();
        }

        private void HandleENetUpdate()
        {
            ENet.Event netEvent;
            
            if (host.CheckEvents(out netEvent) <= 0)
            {
                if (host.Service(0, out netEvent) <= 0)
                    return;
            }

            switch (netEvent.Type)
            {
                case ENet.EventType.None:
                    break;

                case ENet.EventType.Connect:
                    if (isServer)
                    {
                        otherClient = netEvent.Peer;
                        Debug.Log("Client connected to server - ID: " + otherClient.ID);
                    }
                    else
                    {
                        Debug.Log("Client established connection to server - server ID: " + otherClient.ID);
                    }
                    break;

                case ENet.EventType.Disconnect:
                    Debug.Log("Client disconnected from server");
                    break;

                case ENet.EventType.Timeout:
                    Debug.Log("Client connection timeout");
                    break;

                case ENet.EventType.Receive:
                    Debug.Log("Packet received from server - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
                    byte[] bytes = new byte[netEvent.Packet.Length];
                    netEvent.Packet.CopyTo(bytes);
                    ManageIncomingPacket(bytes);
                    netEvent.Packet.Dispose();
                    break;
            }
        }


        private void ManageIncomingPacket(byte[] incoming)
        {
            _incomingCommandHandler.HandleCommand(incoming);
        }

        public void SendCommand(BaseCommand baseCommand, byte channelId = 0)
        {
            var p = default(Packet);
            p.Create(baseCommand.Buffer);
            otherClient.Send(channelId, ref p);
            p.Dispose();
        }


        private void Reconnect()
        {
            StopNetworkConnection();
            StartNewNetworkConnection();
        }
        
        private void StopNetworkConnection()
        {
            Library.Deinitialize();
        }
    }
}