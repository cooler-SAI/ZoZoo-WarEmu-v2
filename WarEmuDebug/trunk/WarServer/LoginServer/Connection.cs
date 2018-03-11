using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace WarhammerEmu.LoginServer
{
    public class Connection
    {
        private Socket socket;
        private const int bufferSize = 16 * 1024;
        private byte[] buffer = new byte[bufferSize];
        private PacketHandler handler;
        public Connection(Socket socket)
        {
            this.socket = socket;
            Log.Debug("New connection");
            handler = new PacketHandler(socket); 

            this.socket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(RecieveCallBack), this.socket);
        }
        private static byte[] newBuffer(int size)
        {
            return new byte[size];
        }

        private void RecieveCallBack(IAsyncResult ar)
        {
            if (!socket.Connected) return;

            Socket sock = (Socket)ar.AsyncState;

            try
            {

                int nBytesRec = socket.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    handler.OnRecieve(buffer.Take(nBytesRec).ToArray());
                    socket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(RecieveCallBack), socket);
                }
                else
                {
                    socket.Dispose();
                    Log.Trace("RecieveCallBack: client disconnected");
                }
            }
            catch
            {
                Log.Warn("RecieveCallBack: connection lost");
            }
        }
    }
}
