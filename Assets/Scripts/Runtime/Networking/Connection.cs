using System;
using ComradeVanti.CSharpTools;
using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    public class Connection
    {
        private readonly ITransferLayer transfer;


        private Connection(
            ITransferLayer transfer,
            ITransferLayer.Type type)
        {
            this.transfer = transfer;
            Type = type;
        }


        public ITransferLayer.Type Type { get; }


        public IOpt<byte[]> CheckForMessages()
        {
            var bytes = transfer.NetUpdate();
            if (bytes == null || bytes.Length <= 0) return Opt.None<byte[]>();
            Debug.Log("RECEIVED DATA");
            return Opt.Some(bytes);
        }

        public void Send(BaseCommand command, byte channelId)
        {
            transfer.Send(command, channelId);
        }

        public static Connection AsHost(ITransferLayer.Type type)
        {
            var transfer = MakeTransfer(type);

            transfer.SetServer(true);
            transfer.CreateServer();

            return new Connection(transfer, type);
        }

        public static Connection AsGuest(
            ITransferLayer.Type type, string ip, ushort port)
        {
            var transfer = MakeTransfer(type);

            transfer.SetConnectionInfo(ip, port);
            transfer.SetServer(false);
            transfer.CreateClient();

            return new Connection(transfer, type);
        }

        private static ITransferLayer MakeTransfer(ITransferLayer.Type type)
        {
            return type switch
            {
                ITransferLayer.Type.ENET => new ENetTransfer(),
                ITransferLayer.Type.DUMMY_TRANSFER => new StubTransfer(),
                _ => throw new Exception("Unknown layer type")
            };
        }
    }
}