using System;

namespace PacketLogger
{
    public class PacketBuffer
    {
        public byte[] Data = new byte[65536];
        public int Length = 0;  
   
        public void Append(byte[] data)
        {
            Array.Copy(data, 0, Data, Length, data.Length);
            Length += data.Length;
        }
        public byte[] GetPacketAndReset()
        {
            byte[] ret = new byte[Length];
            Array.Copy(Data, ret, Length);
            Length = 0;
            return ret;
        }

    }
}
