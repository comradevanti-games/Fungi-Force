using System;
using ENet;
using Networking;
using UnityEngine;

namespace TeamShrimp.GGJ23.Runtime.Networking
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
        private int reconnectAttempts = 0;
        
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
            if(debug)
                Debug.Log("CREATING SERVER");
            host.Create(address, 1);
            if(debug) Debug.Log("CREATED AS SERVER");
        }
        
        
        private void CreateClient()
        {
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
            HandleENetUpdate();
        }

        private void HandleENetUpdate()
        {
            ENet.Event netEvent;
            try
            {
                if (host.CheckEvents(out netEvent) <= 0)
                {
                    if (host.Service(0, out netEvent) <= 0)
                        return;
                    var i = 0;
                }
            }
            catch (Exception e)
            {
                if (reconnectAttempts > 5)
                {
                    Debug.LogError("DESTROYING NETWORK MANAGER BECAUSE CONNECTION COULD NOT BE REESTABLISHED");
                    Destroy(gameObject);
                }

                Reconnect();
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
                        if(debug) Debug.Log("Client connected to server - ID: " + otherClient.ID);
                    }
                    else
                    {
                        if(debug) Debug.Log("Client established connection to server - server ID: " + otherClient.ID);
                    }
                    break;

                case ENet.EventType.Disconnect:
                    if(debug) Debug.Log("Client disconnected from server");
                    break;

                case ENet.EventType.Timeout:
                    if(debug) Debug.Log("Client connection timeout");
                    break;

                case ENet.EventType.Receive:
                    if(debug) Debug.Log("Packet received from server - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
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
            reconnectAttempts++;
            try
            {
                StopNetworkConnection();
                StartNewNetworkConnection();
            }
            catch (Exception e)
            {
                return;
            }

            reconnectAttempts = 0;
        }
        
        private void StopNetworkConnection()
        {
            Library.Deinitialize();
        }
    }
}