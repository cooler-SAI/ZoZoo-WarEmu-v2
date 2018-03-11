using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarhammerEmu.GameServer
{
    public class Authentication : IPacketHandler
    {
        [PacketHandlerAttribute((int)Opcodes.F_CONNECT)]
        static public void F_CONNECT(Connection conn, PacketIn packet)
        {
            Log.Debug("F_CONNECT");
            packet.Skip(8);
            UInt32 Tag = packet.GetUint32();
            string Token = packet.GetString(80);
            packet.Skip(21);
            string Username = packet.GetString(23);

            Log.Debug("New Connection : " + Token + ",User=" + Username);



            PacketOut Out = new PacketOut((byte)Opcodes.S_CONNECTED);
            Out.WriteUInt32(0);
            Out.WriteUInt32(Tag);
            Out.WriteByte(112);
            Out.WriteUInt32(1);
            Out.WritePascalString(Username);
            Out.WritePascalString("Emulator");
            Out.WriteByte(0);
            Out.WriteUInt16(0);
            conn.SendTCP(Out);
        }

        [PacketHandlerAttribute((int)Opcodes.F_PING)]
        static public void F_PING(Connection conn, PacketIn packet)
        {
            uint Timestamp = packet.GetUint32();

            PacketOut Out = new PacketOut((byte)Opcodes.S_PONG);
            Out.WriteUInt32(Timestamp);
            Out.WriteUInt64((UInt64)Listener.GetTimeStamp());
            Out.WriteUInt32((UInt32)(conn.SequenceID + 1));
            Out.WriteUInt32(0);
            conn.SendTCP(Out);
            Log.Trace("ping");
        }

    }
}
