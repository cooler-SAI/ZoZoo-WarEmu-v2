using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarhammerEmu.GameServer
{
    public class Encryptions : IPacketHandler
    {

        public struct sEncrypt
        {
            public byte cipher, application, major, minor, revision, unk1;
        };

        [PacketHandlerAttribute((int)Opcodes.F_ENCRYPTKEY)]
        static public void F_ENCRYPTKEY(Connection conn, PacketIn packet)
        {
            sEncrypt Result = RC4Crypto.ByteToType<sEncrypt>(packet);
            string Version = Result.major + "." + Result.minor + "." + Result.revision;
            Log.Trace(Version + "cipher:" + Result.cipher);

            if (Result.cipher == 0)
            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_RECEIVE_ENCRYPTKEY);
                Out.WriteByte(1);
                conn.SendTCP(Out);
            }
            else if (Result.cipher == 1)
            {
                byte[] EncryptKey = new byte[256];
                packet.Read(EncryptKey, 0, EncryptKey.Length);


                conn.AddCrypt("RC4Crypto", new CryptKey(EncryptKey), new CryptKey(EncryptKey));

                Log.Trace("KEY:" + BitConverter.ToString(EncryptKey));
            }
        }

    
    }
}
