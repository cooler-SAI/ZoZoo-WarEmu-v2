using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace WarhammerEmu.GameServer
{
    public class Connection
    {
        public Socket socket;
        private const int bufferSize = 16 * 1024;
        private byte[] buffer = new byte[bufferSize];
        private Listener listener;

        public Connection(Socket socket,Listener listener)
        {

            PacketOut.SizeLen = sizeof(UInt16);
            PacketOut.OpcodeInLen = false;
            PacketOut.SizeInLen = false;
            PacketOut.OpcodeReverse = false;
            PacketOut.SizeReverse = false;
            PacketOut.Struct = PackStruct.SizeAndOpcode;


            Form1.activeConnection = this;
            this.socket = socket;
            this.listener = listener;
            Log.Debug("New connection");

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
                    OnReceive(buffer.Take(nBytesRec).ToArray());
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

        private ushort Opcode = 0;
        private long PacketSize = 0;
        public bool ReadingData = false;
        public UInt16 SequenceID, SessionID, Unk1;
        public byte Unk2;
        public PacketIn packet = null;

        protected void OnReceive(byte[] bytes)
        {
            PacketIn _Packet = new PacketIn(bytes, 0, bytes.Length);
            packet = _Packet;

            lock (this)
            {
                long PacketLength = packet.Length;

                while (PacketLength > 0)
                {
                    // Lecture du Header
                    if (!ReadingData)
                    {
                        if (PacketLength < 2)
                        {
                            Log.Error("OnReceive", "invalid header " + PacketLength);
                            break;
                        }

                        PacketSize = packet.GetUint16();
                        PacketLength -= 2;

                        if (PacketLength < PacketSize + 10)
                        {
                            Log.Error("OnReceive", "Incomplete Packet Header " + PacketLength + "<" + PacketSize);
                            break;
                        }

                        packet.Size = (ulong)PacketSize + 10;
                       packet = DeCrypt(packet);

                        SequenceID = packet.GetUint16();
                        SessionID = packet.GetUint16();
                        Unk1 = packet.GetUint16();
                        Unk2 = packet.GetUint8();
                        Opcode = packet.GetUint8();
                        PacketLength -= 8;

                        if (PacketLength > PacketSize + 2)
                        {
                               Log.Debug("OnReceive::Packet contain multiple opcodes " + PacketLength + ">" + (PacketSize + 2));
                        }
                        ReadingData = true;
                    }
                    else
                    {
                        ReadingData = false;

                        if (PacketLength >= PacketSize + 2)
                        {
                            byte[] BPack = new byte[PacketSize + 2];
                            packet.Read(BPack, 0, (int)(PacketSize + 2));

                            PacketIn Packet = new PacketIn(BPack, 0, BPack.Length);
                            Packet.Opcode = Opcode;
                            Packet.Size = (ulong)PacketSize;


                           // handler.HandlePacket(Packet);
                            listener.HandlePacket(this, Packet);

                            PacketLength -= PacketSize + 2;
                        }
                        else
                        {
                            Log.Error("OnReceive", "The size of the packet is less than the total received :" + PacketLength + "<" + (PacketSize + 2));
                            break;
                        }
                    }
                }
            }
        }



        public void SendTCP(PacketOut packet)
        {
            //Fix the packet size
            packet.WritePacketLength();
            packet = Crypt(packet);
            byte[] buf = packet.ToArray(); //packet.WritePacketLength sets the Capacity

            //Send the buffer

            socket.Send(buf);

        }

        public void SendCustomPacket(string hexStringBytes)
        {
            try
            {
                hexStringBytes = hexStringBytes.Replace(" ", "");

                byte opcode = Convert.ToByte(hexStringBytes.Substring(0, 2), 16);

                PacketOut packet = new PacketOut(opcode);
                packet.WriteHexStringBytes(hexStringBytes.Remove(0, 2));

                packet.WritePacketLength();
                packet = Crypt(packet);
                byte[] buf = packet.ToArray(); //packet.WritePacketLength sets the Capacity

                socket.Send(buf);
            }
            catch
            {
                Log.Error("SendCustomPacket: Failed to send packet");
            }
        }





        #region Crypto

        private Dictionary<ICryptHandler, CryptKey[]> m_crypts = new Dictionary<ICryptHandler, CryptKey[]>();

        public bool AddCrypt(string name, CryptKey CKey, CryptKey DKey)
        {
            ICryptHandler Handler = listener.GetCrypt(name);

            if (Handler == null)
                return false;

            if (CKey == null)
                CKey = Handler.GenerateKey(this);

            if (DKey == null)
                DKey = Handler.GenerateKey(this);

            if (m_crypts.ContainsKey(Handler))
                m_crypts[Handler] = new CryptKey[] { CKey, DKey };
            else
                m_crypts.Add(Handler, new CryptKey[] { CKey, DKey });

            return true;
        }

        public PacketIn DeCrypt(PacketIn packet)
        {
            if (m_crypts.Count <= 0)
                return packet;

            ulong opcode = packet.Opcode;
            ulong size = packet.Size;
            long StartPos = packet.Position;
            foreach (KeyValuePair<ICryptHandler, CryptKey[]> Entry in m_crypts)
            {
                try
                {

                    byte[] Buf = new byte[size];

                    long Pos = packet.Position;
                    packet.Read(Buf, 0, (int)Buf.Length);
                    packet.Position = Pos;

                    PacketIn Pack = Entry.Key.Decrypt(Entry.Value[1], Buf);
                    packet.Write(Pack.ToArray(), 0, Pack.ToArray().Length);

                    packet.Opcode = opcode;
                    packet.Size = size;
                }
                catch (Exception e)
                {
                    Log.Error("Decrypt Error : " + e.ToString());
                    continue;
                }
            }

            packet.Position = StartPos;
            return packet;
        }

        public PacketOut Crypt(PacketOut packet)
        {
            if (m_crypts.Count <= 0)
                return packet;

            byte[] Packet = packet.ToArray();

            int Hpos = 0;
            Hpos += PacketOut.SizeLen;
            if (PacketOut.OpcodeInLen)
                Hpos += packet.OpcodeLen;

            byte[] Header = new byte[Hpos];
            byte[] ToCrypt = new byte[(packet.Length - Hpos)];

            for (int i = 0; i < Hpos; ++i)
                Header[i] = Packet[i];

            for (int i = Hpos; i < Packet.Length; ++i)
                ToCrypt[i - Hpos] = Packet[i];

            try
            {
                foreach (KeyValuePair<ICryptHandler, CryptKey[]> Entry in m_crypts)
                {
                    ToCrypt = Entry.Key.Crypt(Entry.Value[0], ToCrypt);
                }
            }
            catch (Exception e)
            {
                Log.Error("Crypt Error : " + e.ToString());
                return packet;
            }

            PacketOut Out = new PacketOut((byte)0);
            Out.Opcode = packet.Opcode;
            Out.OpcodeLen = packet.OpcodeLen;
            Out.Position = 0;
            Out.SetLength(0);

            byte[] Total = new byte[Header.Length + ToCrypt.Length];

            for (int i = 0; i < Total.Length; ++i)
            {
                if (i < Header.Length)
                    Total[i] = Header[i];
                else
                    Total[i] = ToCrypt[i - Header.Length];
            }

            Out.Write(Total, 0, Total.Length);

            return Out;
        }

        #endregion


    }
}
