using System.IO;
using System.Numerics;
using System.Text;
using TeamShrimp.GGJ23;
using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    // LAYOUT: [0, ... winner name ...]
    public class WinCommand : BaseCommand
    {
        public string winner;
        

        public WinCommand(string winner)
        {
            this.winner = winner;
            SerializeCommand();
        }

        public override string ToString()
        {
            return $"CUT COMMAND {nameof(winner)}: {winner}";
        }

        public WinCommand(BinaryReader reader)
            : base(reader)
        { }
        
        public override void SerializeCommand()
        {
            
            var nameBytes = Encoding.Unicode.GetBytes(winner);
            PackageWrapper pw = new PackageWrapper(nameBytes.Length+1, (byte)CommandType.CONNECT);
            pw.Write((byte)nameBytes.Length);
            pw.Write(nameBytes);
            Buffer = pw.Buffer;
        }

        public override void DeserializeCommand(BinaryReader binaryReader)
        {
            var length = binaryReader.ReadByte();
            var nameBytes = binaryReader.ReadBytes(length);
            winner = Encoding.Unicode.GetString(nameBytes);
        }
    }
}