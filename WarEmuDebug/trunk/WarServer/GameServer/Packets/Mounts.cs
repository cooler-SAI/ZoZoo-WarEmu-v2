using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarhammerEmu.GameServer
{
    public static class Mounts
    {
        public static void SummonMount1(Connection conn)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_MOUNT_UPDATE);
            Out.WriteHexStringBytes("29D806BE1DF20000000000000000000000000000");
            conn.SendTCP(Out);
        }
        public static void SummonMount2(Connection conn)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_MOUNT_UPDATE);
            Out.WriteHexStringBytes("29D800660C780000000000000000000000000000");
            conn.SendTCP(Out);
        }

        public static void UpdateSpeed(Connection conn)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_MAX_VELOCITY);
            Out.WriteHexStringBytes("00C80100");// speed 200
            conn.SendTCP(Out);
        }

        public static void MountPacketTest(Connection conn)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_MOUNT_UPDATE);
            Out.WriteUInt16(0x29D8); // Object Id
           // Out.WriteHexStringBytes("29D806BE1DF20000000000000000000000000000");
            Out.WriteUInt16(1286); // Model ID of mount
            conn.SendTCP(Out);
        }
    }
}
