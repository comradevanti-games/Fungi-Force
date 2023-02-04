using System.IO;
using System.Text;

namespace Networking
{
    public class ReadyCommand : BaseCommand
    {
        public ReadyCommand()
        {
            this.SerializeCommand();
        }
        
        public ReadyCommand(BinaryReader reader)
            : base(reader) { }
        
        public override void SerializeCommand()
        {
            Buffer = new byte[1] {(byte)CommandType.READY};
        }

        public override void DeserializeCommand(BinaryReader binaryReader)
        {
        }
    }
}