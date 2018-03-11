using System;

namespace WarhammerEmu.GameServer
{

    public delegate void PacketFunction(Connection conn, PacketIn packet);

    [AttributeUsage(AttributeTargets.Method)]
    public class PacketHandlerAttribute : Attribute
    {
        protected int m_opcode;
        public PacketHandlerAttribute(int opcode)
        {
            m_opcode = opcode;
        }
        public int Opcode
        {
            get
            {
                return m_opcode;
            }
        }
    }
}
