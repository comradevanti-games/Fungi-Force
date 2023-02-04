using System;
using System.IO;
using System.Numerics;
using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    public static class SerializerExtension
    {
        public static byte[] Serialize(this Vector2Int v2i)
        {
            var bytes = new byte[8];
            BinaryWriter bw = new BinaryWriter(new MemoryStream(bytes));
            
            bw.Write(v2i.x.Serialize());
            bw.Write(v2i.y.Serialize());
            bw.Flush();
            return bytes;
        }

        public static byte[] Serialize(this int i)
        {
            byte[] bytes = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }
        
        public static byte[] Serialize(this long l)
        {
            byte[] bytes = BitConverter.GetBytes(l);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }
        
        
        public static int DeserializeInt(this byte[] l, int offset = 0)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(l);
            return BitConverter.ToInt32(l, offset);
        }
        
        
        public static long DeserializeLong(this byte[] l)
        {
            
            if (BitConverter.IsLittleEndian)
                Array.Reverse(l);
            return BitConverter.ToInt64(l);
        }

        public static Vector2Int ReadVector2Int(this BinaryReader br)
        {
            var x = br.ReadBytes(4).DeserializeInt();
            var y = br.ReadBytes(4).DeserializeInt();
            return new Vector2Int(x, y);
        }

        public static string ToBitString(this byte[] bytes)
        {
            var s = "";
            foreach (var b in bytes)
            {
                s += "     " + b.ToBitString();
            }
            return s;
        }

        public static string ToBitString(this byte b)
        {
            return Convert.ToString(b, 2).PadLeft(8, '0');
        }
    }
}