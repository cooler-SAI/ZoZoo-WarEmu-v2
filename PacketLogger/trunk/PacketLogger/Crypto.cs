using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketLogger
{
    public class Crypto
    {
        public byte[] Decode(byte[] Buffer, byte[] Key,bool IsClient)
        {
            try
            {
                if (Buffer == null) return null;
                if (Key == null) return null;
                byte[] tmpsbox = new byte[256];
                Array.Copy(Key, 0, tmpsbox, 0, Key.Length);
                byte i = 0;
                byte j = 0;
                ushort len = (ushort)((Buffer[0] << 8) | Buffer[1]); 

                if (IsClient)    //if from Client +10 bytes for session,param,code,checksum
                {len += 10;}   
                else { len += 1; }

                int k;
                for (k = (len / 2) + 2; k < len + 2; k++)
                {
                    i++;
                    byte tmp = tmpsbox[i];
                    j += tmp;
                    tmpsbox[i] = tmpsbox[j];
                    tmpsbox[j] = tmp;
                    byte xorKey = tmpsbox[(byte)(tmpsbox[i] + tmpsbox[j])];
                    Buffer[k] ^= xorKey;
                    j += Buffer[k];
                }
                for (k = 2; k < (len / 2) + 2; k++)
                {
                    i++;
                    byte tmp = tmpsbox[i];
                    j += tmp;
                    tmpsbox[i] = tmpsbox[j];
                    tmpsbox[j] = tmp;
                    byte xorKey = tmpsbox[(byte)(tmpsbox[i] + tmpsbox[j])];
                    Buffer[k] ^= xorKey;
                    j += Buffer[k];
                }
                return Buffer;
            }
            catch { return null; }
        }
    }
}
