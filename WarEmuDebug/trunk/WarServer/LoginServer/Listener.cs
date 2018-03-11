using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace WarhammerEmu.LoginServer
{
    public class Listener
    {
        private string hostIp;
        private int hostPort;
        protected static Socket m_socket;

        public Listener(string hostIp, int hostPort)
        {
            this.hostIp = hostIp;
            this.hostPort = hostPort;
        }
        public void Run()
        {
            Log.Debug("LoginServer: Listening for connections..");
            try
            {
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                m_socket.Bind(new IPEndPoint(IPAddress.Parse(hostIp), hostPort));
                m_socket.Listen(10);
                m_socket.BeginAccept(new AsyncCallback(OnConnect), null);
            }
            catch (SocketException se)
            {
                Log.Warn("LoginServer: Listener exception.." + se);
            }
        }
        private void OnConnect(IAsyncResult asyn)
        {

            var socket = m_socket.EndAccept(asyn);
            Connection connection = new Connection(socket);
            m_socket.BeginAccept(new AsyncCallback(OnConnect), null);
        }
    }
}
