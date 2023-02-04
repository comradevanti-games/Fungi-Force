using System.IO;
using System.Text;

namespace Networking
{
    public class ReadyCommand : BaseCommand
    {
        private string playerName;

        public ReadyCommand(string playerName)
        {
            this.playerName = playerName;
            this.SerializeCommand();
        }
        
        public ReadyCommand(BinaryReader reader)
            : base(reader) { }
        
        public override void SerializeCommand()
        {
            var nameBytes = Encoding.Unicode.GetBytes(playerName);
            PackageWrapper pw = new PackageWrapper(nameBytes.Length, (byte)CommandType.READY);
            pw.Write(nameBytes);
            Buffer = pw.Buffer;
        }

        public override void DeserializeCommand(BinaryReader binaryReader)
        {
            
            var length = binaryReader.ReadByte();
            var nameBytes = binaryReader.ReadBytes(length);
            this.playerName = Encoding.Unicode.GetString(nameBytes);
        }
    }
}