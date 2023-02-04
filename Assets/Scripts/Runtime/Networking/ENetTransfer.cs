using System;
using ENet;
using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    public class ENetTransfer : ITransferLayer
    {
        public bool isServer = false;

        public Host host;
        public Peer otherClient;

        public string ip;
        public ushort port;

        public bool debug;
        private int reconnectAttempts = 0;

        public void SetServer(bool isServer)
        {
            this.isServer = isServer;
        }

        public void SetConnectionInfo(string ip, ushort port)
        {
            this.ip = ip;
            this.port = port;
        }

        public byte[] NetUpdate()
        {
            ENet.Event netEvent;
            try
            {
                if (host.CheckEvents(out netEvent) <= 0)
                {
                    if (host.Service(0, out netEvent) <= 0)
                        return new byte[0];
                    var i = 0;
                }
            }
            catch (Exception e)
            {
                Reconnect();
                return new byte[0];
            }

            switch (netEvent.Type)
            {
                case ENet.EventType.None:
                    break;

                case ENet.EventType.Connect:
                    if (isServer)
                    {
                        otherClient = netEvent.Peer;
                        if (debug) Debug.Log("Client connected to server - ID: " + otherClient.ID);
                    }
                    else
                    {
                        if (debug) Debug.Log("Client established connection to server - server ID: " + otherClient.ID);
                    }

                    break;

                case ENet.EventType.Disconnect:
                    if (debug) Debug.Log("Client disconnected from server");
                    break;

                case ENet.EventType.Timeout:
                    if (debug) Debug.Log("Client connection timeout");
                    break;

                case ENet.EventType.Receive:
                    if (debug)
                        Debug.Log("Packet received from server - Channel ID: " + netEvent.ChannelID +
                                  ", Data length: " + netEvent.Packet.Length);
                    byte[] bytes = new byte[netEvent.Packet.Length];
                    netEvent.Packet.CopyTo(bytes);

                    netEvent.Packet.Dispose();
                    return bytes;
            }

            return null;
        }

        public void CreateServer()
        {
            Library.Initialize();
            // CREATING SERVER
            host = new Host();
            Address address = new Address {Port = port};
            if (debug)
                Debug.Log("CREATING SERVER");
            host.Create(address, 1);
            if (debug) Debug.Log("CREATED AS SERVER");
        }

        public void StopNetworkConnection()
        {
            Library.Deinitialize();
        }


        public void CreateClient()
        {
            Library.Initialize();
            // CREATING CLIENT
            host = new Host();
            Address address = new Address();
            address.SetHost(ip);
            address.Port = port;

            host.Create();
            if (debug) Debug.Log("CONNECTING TO " + ip + ":" + port);
            otherClient = host.Connect(address);
            if (debug) Debug.Log("CONNECTION ESTABLISHED");
        }


        private void Reconnect()
        {
            reconnectAttempts++;
            try
            {
                StopNetworkConnection();
                if (isServer)
                    CreateServer();
                else
                    CreateClient();
            }
            catch (Exception e)
            {
                return;
            }

            reconnectAttempts = 0;
        }
        
        
        public void Send(BaseCommand baseCommand, byte channelId = 0)
        {
            var p = default(Packet);
            p.Create(baseCommand.Buffer);
            otherClient.Send(channelId, ref p);
            p.Dispose();
        }

    }
}