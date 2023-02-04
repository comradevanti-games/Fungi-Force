using System.IO;
using System.Text;
using TeamShrimp.GGJ23;

namespace TeamShrimp.GGJ23.Networking
{
    public class ConnectionInitCommand : BaseCommand
    {
        public string playerName;

        public ConnectionInitCommand(string playerName)
        {
            this.playerName = playerName;
            this.SerializeCommand();
        }
        
        public ConnectionInitCommand(BinaryReader reader)
            : base(reader) { }

        public override string ToString() => $"CONNECTION INIT COMMAND {nameof(playerName)}: {playerName}";

        public override void SerializeCommand()
        {
            var nameBytes = Encoding.Unicode.GetBytes(playerName);
            PackageWrapper pw = new PackageWrapper(nameBytes.Length+1, (byte)CommandType.CONNECT);
            pw.Write((byte)nameBytes.Length);
            pw.Write(nameBytes);
            Buffer = pw.Buffer;
        }

        public override void DeserializeCommand(BinaryReader binaryReader)
        {
            var length = binaryReader.ReadByte();
            var nameBytes = binaryReader.ReadBytes(length);
            playerName = Encoding.Unicode.GetString(nameBytes);
        }
    }
}