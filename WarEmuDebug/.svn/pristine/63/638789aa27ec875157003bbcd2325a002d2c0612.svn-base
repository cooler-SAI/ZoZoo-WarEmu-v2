using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
namespace WarhammerEmu.GameServer
{
    [Crypt("RC4Crypto")]
    public class RC4Crypto : ICryptHandler
    {
        public byte[] Crypt(CryptKey Key, byte[] packet)
        {
            packet = PacketOut.EncryptMythicRC4(Key.GetbKey(), packet);

            return packet;
        }

        public PacketIn Decrypt(CryptKey Key, byte[] packet)
        {
            PacketIn Packet = new PacketIn(packet, 0, packet.Length);
            Packet = Packet.DecryptMythicRC4(Key.GetbKey());
            return Packet;
        }

        public CryptKey GenerateKey(Connection conn)
        {
            return new CryptKey(new byte[0]);
        }

        public static T ByteToType<T>(PacketIn packet)
        {
            BinaryReader reader = new BinaryReader(packet);
            byte[] bytes = reader.ReadBytes(System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)));

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)System.Runtime.InteropServices.Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }
    }
}
