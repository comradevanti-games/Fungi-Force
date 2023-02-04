using System.Collections.Generic;
using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    public class StubTransfer : ITransferLayer
    {
        public Queue<byte[]> bytesLast = new Queue<byte[]>();
        
        public void SetServer(bool isServer)
        {
            
        }

        public void SetConnectionInfo(string ip, ushort port)
        {
        }

        public byte[] NetUpdate() => bytesLast.Count > 0 ? bytesLast.Dequeue() : null;

        public void CreateClient()
        {
        }

        public void CreateServer()
        {
        }

        public void StopNetworkConnection()
        {
        }

        public void Send(BaseCommand baseCommand, byte channelId = 0)
        {
            if (baseCommand is ConnectionInitCommand c)
            {
                bytesLast.Enqueue(null);
                bytesLast.Enqueue(null);
                ConnectionInitCommand ci = new ConnectionInitCommand("DUMMY CLIENT");
                bytesLast.Enqueue(ci.Buffer);
                var wic = new WorldInitCommand(5,42);
                wic.SerializeCommand();
                bytesLast.Enqueue(wic.Buffer);
                return;
            }
            Debug.Log(baseCommand);
            for(int i = 0; i < 5; i++)
                bytesLast.Enqueue(null);
            bytesLast.Enqueue(baseCommand.Buffer);
        }
    }
}