using System.IO;
using System.Numerics;
using System.Text;
using UnityEngine;

namespace Networking
{
    // LAYOUT: [0, mushType, long id, vec2int pos, vec2int sourcePos]
    public class WorldInitCommand : BaseCommand
    {
        public int size;
        public int seed;
        
        public static int COMMAND_LENGTH = 16;

        public WorldInitCommand(int size, int seed)
        {
            this.size = size;
            this.seed = seed;
            SerializeCommand();
        }

        public override string ToString()
        {
            return $"MAP INIT: {nameof(size)}: {size}, {nameof(seed)}: {seed}";
        }

        public WorldInitCommand(BinaryReader reader)
            : base(reader)
        { }
        
        public override void SerializeCommand()
        {
            PackageWrapper pw = new PackageWrapper(COMMAND_LENGTH, (byte)CommandType.CUT);
            pw.Write(size);
            pw.Write(seed);
            Buffer = pw.Buffer;
//            Debug.Log("SERIALIZED INTO BUFFER: " + Buffer.ToBitString());
        }

        public override void DeserializeCommand(BinaryReader binaryReader)
        {
            size = binaryReader.ReadBytes(4).DeserializeInt();
            seed = binaryReader.ReadBytes(4).DeserializeInt();
        }
    }
}