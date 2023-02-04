using System.IO;
using System.Numerics;
using System.Text;
using TeamShrimp.GGJ23;
using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    // LAYOUT: [0, mushType, long id, vec2int pos, vec2int sourcePos]
    public class CutCommand : BaseCommand
    {
        public Vector2Int pos;
        public Vector2Int sourcePos;
        
        public static int COMMAND_LENGTH = 16;

        public CutCommand(Vector2Int pos, Vector2Int sourcePos)
        {
            this.pos = pos;
            this.sourcePos = sourcePos;
            SerializeCommand();
        }

        public override string ToString()
        {
            return $"CUT COMMAND {nameof(pos)}: {pos}, {nameof(sourcePos)}: {sourcePos}";
        }

        public CutCommand(BinaryReader reader)
            : base(reader)
        { }
        
        public override void SerializeCommand()
        {
            PackageWrapper pw = new PackageWrapper(COMMAND_LENGTH, (byte)CommandType.CUT);
            pw.Write(pos);
            pw.Write(sourcePos);
            Buffer = pw.Buffer;
//            Debug.Log("SERIALIZED INTO BUFFER: " + Buffer.ToBitString());
        }

        public override void DeserializeCommand(BinaryReader binaryReader)
        {
            pos = binaryReader.ReadVector2Int();
            sourcePos = binaryReader.ReadVector2Int();
        }
    }
}