using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
namespace WarhammerEmu.GameServer
{
    public class Listener
    {
        private string hostIp;
        private int hostPort;
        protected static Socket m_socket;
        public readonly PacketFunction[] m_packetHandlers = new PacketFunction[0xFFFFFF];

        public Listener(string hostIp, int hostPort)
        {
            this.hostIp = hostIp;
            this.hostPort = hostPort;
        }
        public void Run()
        {
            Log.Debug("GameServer: Listening for connections..");
            try
            {
                LoadPacketHandler();
                LoadCryptHandler();
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                m_socket.Bind(new IPEndPoint(IPAddress.Parse(hostIp), hostPort));
                m_socket.Listen(10);
                m_socket.BeginAccept(new AsyncCallback(OnConnect), null);
            }
            catch (SocketException se)
            {
                Log.Warn("GameServer: Listener exception.." + se);
            }
        }
        private void OnConnect(IAsyncResult asyn)
        {

            var socket = m_socket.EndAccept(asyn);
            Connection connection = new Connection(socket,this);
            m_socket.BeginAccept(new AsyncCallback(OnConnect), null);
        }

        public readonly Dictionary<string, ICryptHandler> m_cryptHandlers = new Dictionary<string, ICryptHandler>();
        public void LoadCryptHandler()
        {
           // Log.Info("TCPManager", "Loading Crypt Handler");

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    // Pick up a class
                    if (type.IsClass != true)
                        continue;

                    CryptAttribute[] crypthandler =
                        (CryptAttribute[])type.GetCustomAttributes(typeof(CryptAttribute), true);

                    if (crypthandler.Length > 0)
                    {
                     //   Log.Debug("TCPManager", "Registering crypt " + crypthandler[0]._CryptName);
                        m_cryptHandlers.Add(crypthandler[0]._CryptName, (ICryptHandler)Activator.CreateInstance(type));
                    }
                }
            }
        }

        public ICryptHandler GetCrypt(string name)
        {
            if (m_cryptHandlers.ContainsKey(name))
                return m_cryptHandlers[name];
            else
                return null;
        }


        public void LoadPacketHandler()
        {


            Log.Debug("GameServer: Loading the Packet Handler");

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    // Pick up a class
                    if (type.IsClass != true)
                        continue;

                    if (type.IsSubclassOf(typeof(IPacketHandler)))
                        continue;

                    foreach (MethodInfo m in type.GetMethods())
                        foreach (object at in m.GetCustomAttributes(typeof(PacketHandlerAttribute), false))
                        {
                            PacketHandlerAttribute attr = at as PacketHandlerAttribute;
                            PacketFunction handler = (PacketFunction)Delegate.CreateDelegate(typeof(PacketFunction), m);

                            Log.Debug("GameServer: Registering handler for opcode : " + attr.Opcode.ToString("X8"));
                            m_packetHandlers[attr.Opcode] = handler;
                        }
                }
            }
        }



        public void HandlePacket(Connection conn, PacketIn Packet)
        {
           
            PacketFunction packetHandler = null;

            if (Packet.Opcode < (ulong)m_packetHandlers.Length)
                packetHandler = m_packetHandlers[Packet.Opcode];
            else
                Log.Error("HandlePacket: ", "Can not handle :" + Packet.Opcode + "(" + Packet.Opcode.ToString("X8") + ")");

            if (packetHandler != null)
            {
                PacketHandlerAttribute[] packethandlerattribs = (PacketHandlerAttribute[])packetHandler.GetType().GetCustomAttributes(typeof(PacketHandlerAttribute), true);

                try
                {
                    packetHandler.Invoke(conn, Packet);
                }
                catch (Exception e)
                {
                    Log.Error("HandlePacket: ", "Packet handler error :" + Packet.Opcode + " " + e.ToString());
                }
            }
            else
                Log.Error("HandlePacket: ", "Can not Handle opcode :" + Packet.Opcode + "(" + Packet.Opcode.ToString("X8") + ")");
        }

        static public int GetTimeStamp()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
