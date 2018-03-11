using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Google.ProtocolBuffers;
using WarProtocol;
namespace WarhammerEmu.LoginServer
{
    public class PacketHandler
    {
        private Socket socket;
        public PacketHandler(Socket socket)
        {
            this.socket = socket;
        }

        private ushort Opcode = 0;
        private int m_expectSize = 0;
        private bool m_expectData = false;

        public void OnRecieve(byte[] packetin)
        {
            lock (this)
            {
                PacketIn packet = new PacketIn(packetin, 0, packetin.Length);
                long byteLeft = packet.Length;

                while (byteLeft > 0)
                {
                    if (!m_expectData)
                    {
                        long StartPos = packet.Position;
                        m_expectSize = packet.DecodeMythicSize();
                        long EndPos = packet.Position;

                        long Diff = EndPos - StartPos;
                        byteLeft -= Diff;
                        if (m_expectSize <= 0)
                        {
                            packet.Opcode = packet.GetUint8();
                            packet.Size = (ulong)m_expectSize;  
                            HandlePacket(packet);
                            return;
                        }

                        if (byteLeft <= 0)
                            return;

                        Opcode = packet.GetUint8();
                        byteLeft -= 1;

                        m_expectData = true;
                    }
                    else
                    {
                        m_expectData = false;
                        if (byteLeft >= m_expectSize)
                        {
                            long Pos = packet.Position;

                            packet.Opcode = Opcode;
                            packet.Size = (ulong)m_expectSize;

                           // _srvr.HandlePacket(this, packet);
                            HandlePacket(packet);

                            byteLeft -= m_expectSize;
                            packet.Position = Pos;
                            packet.Skip(m_expectSize);
                        }
                        else
                        {
                            Log.Error("OnReceive", "Data count incorrect :" + byteLeft + " != " + m_expectSize);
                        }
                    }
                }

                packet.Dispose();
            }
            
        }
        public void SendTCPCuted(PacketOut Out)
        {

            long PSize = Out.Length - Out.OpcodeLen - PacketOut.SizeLen; // Size = Size-len-opcode

            byte[] Packet = new byte[PSize];
            Out.Position = Out.OpcodeLen + PacketOut.SizeLen;
            Out.Read(Packet, 0, (int)(PSize));

            List<byte> Header = new List<byte>(5);
            int itemcount = 1;
            while (PSize > 0x7f)
            {
                Header.Add((byte)((byte)(PSize) | 0x80));
                PSize >>= 7;
                itemcount++;
                if (itemcount >= Header.Capacity + 10)
                    Header.Capacity += 10;
            }

            Header.Add((byte)(PSize));
            Header.Add((byte)(Out.Opcode));

            socket.Send(Header.ToArray());
            socket.Send(Packet);
            Out.Dispose();
        }


        private void HandlePacket(PacketIn packet)
        {

            switch ((Opcodes)packet.Opcode)
            {
                case Opcodes.CMSG_VerifyProtocolReq:
                    onVerifyProtocolReq(packet);
                    break;
                case Opcodes.CMSG_AuthSessionTokenReq:
                    onAuthSessionTokenReq(packet);
                    break;
                case Opcodes.CMSG_GetCharSummaryListReq:
                    onGetCharSummaryListReq(packet);
                    break;
                case Opcodes.CMSG_GetClusterListReq:
                    onGetClusterListReq(packet);
                    break;
                case Opcodes.CMSG_GetAcctPropListReq:
                    onGetAcctPropListReq(packet);
                    break;
                case Opcodes.CMSG_MetricEventNotify:
                    onMetricEventNotify(packet);
                    break;
                default:
                    Log.Warn("unknown opcode " + packet.Opcode);
                        break;
                    
            }
            //Log.Trace("PacketIn > " + packet.Opcode);
            //Log.Trace("PacketIn > " + BitConverter.ToString(packet.ToArray()).Replace("-", " "));
        }

        private void onVerifyProtocolReq(PacketIn packet)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.SMSG_VerifyProtocolReply);

            byte[] IV_HASH1 = { 0x01, 0x53, 0x21, 0x4d, 0x4a, 0x04, 0x27, 0xb7, 0xb4, 0x59, 0x0f, 0x3e, 0xa7, 0x9d, 0x29, 0xe9 };
            byte[] IV_HASH2 = { 0x49, 0x18, 0xa1, 0x2a, 0x64, 0xe1, 0xda, 0xbd, 0x84, 0xd9, 0xf4, 0x8a, 0x8b, 0x3c, 0x27, 0x20 };

            ByteString iv1 = ByteString.CopyFrom(IV_HASH1);
            ByteString iv2 = ByteString.CopyFrom(IV_HASH2);
            VerifyProtocolReply.Builder verify = VerifyProtocolReply.CreateBuilder();
            verify.SetResultCode(VerifyProtocolReply.Types.ResultCode.RES_SUCCESS);

            verify.SetIv1(ByteString.CopyFrom(IV_HASH1));
            verify.SetIv2(ByteString.CopyFrom(IV_HASH2));
            Out.Write(verify.Build().ToByteArray());

            SendTCPCuted(Out);
        }

        private void onAuthSessionTokenReq(PacketIn packet)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.SMSG_AuthSessionTokenReply);


            AuthSessionTokenReq.Builder authReq = AuthSessionTokenReq.CreateBuilder();
            authReq.MergeFrom(packet.ToArray());

            string session = Encoding.ASCII.GetString(authReq.SessionToken.ToByteArray());
            Log.Trace(session);

            AuthSessionTokenReply.Builder authReply = AuthSessionTokenReply.CreateBuilder();
            authReply.SetResultCode(AuthSessionTokenReply.Types.ResultCode.RES_SUCCESS);


            Out.Write(authReply.Build().ToByteArray());
            SendTCPCuted(Out);
        }
        private void onGetCharSummaryListReq(PacketIn packet)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.SMSG_GetCharSummaryListReply);
            Out.Write(new byte[2] { 0x08, 00 });
            SendTCPCuted(Out);
        }

        private void onGetClusterListReq(PacketIn packet)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.SMSG_GetClusterListReply);
            GetClusterListReply.Builder ClusterListReplay = GetClusterListReply.CreateBuilder();
            ClusterListReplay.ResultCode = ResultCode.RES_SUCCESS;
            ClusterListReplay.AddClusterList(BuildCluster());

            byte[] cluster = ClusterListReplay.Build().ToByteArray();
            Out.Write(cluster);
            SendTCPCuted(Out);

        }
        private void onGetAcctPropListReq(PacketIn packet)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.SMSG_GetAcctPropListReply);
            byte[] val = { 0x08, 0x00 };
            Out.Write(val);
            SendTCPCuted(Out);
        }
        private void onMetricEventNotify(PacketIn packet)
        {

        }



        #region cluster
        private static ClusterProp setProp(string name, string value)
        {
            return ClusterProp.CreateBuilder().SetPropName(name)
                                              .SetPropValue(value)
                                              .Build();
        }
        private static ClusterInfo.Builder BuildCluster()
        {
            ClusterInfo.Builder cluster = ClusterInfo.CreateBuilder();
            cluster.SetClusterId(122)
                   .SetClusterName("Cluster 1")
                   .SetLobbyHost("127.0.0.1")
                   .SetLobbyPort(10622)
                   .SetLanguageId(0)
                   .SetMaxClusterPop(500)
                   .SetClusterPopStatus(ClusterPopStatus.POP_HIGH)
                   .SetLanguageId(0)
                   .SetClusterStatus(ClusterStatus.STATUS_ONLINE);

            cluster.AddServerList(
                ServerInfo.CreateBuilder().SetServerId(122)
                                          .SetServerName("Emulator")
                                          .Build());

            cluster.AddPropertyList(setProp("setting.allow_trials", "1"));
            cluster.AddPropertyList(setProp("setting.charxferavailable", "0"));
            cluster.AddPropertyList(setProp("setting.language", "EN"));
            cluster.AddPropertyList(setProp("setting.legacy", "0"));
            cluster.AddPropertyList(setProp("setting.manualbonus.realm.destruction", "100"));
            cluster.AddPropertyList(setProp("setting.manualbonus.realm.order", "100"));
            cluster.AddPropertyList(setProp("setting.min_cross_realm_account_level", "0"));
            cluster.AddPropertyList(setProp("setting.name", "Emulator"));
            cluster.AddPropertyList(setProp("setting.net.address", "127.0.0.1"));
            cluster.AddPropertyList(setProp("setting.net.port", "10622"));
            cluster.AddPropertyList(setProp("setting.redirect", "0"));
            cluster.AddPropertyList(setProp("setting.region", "STR_REGION_NORTHAMERICA"));
            cluster.AddPropertyList(setProp("setting.retired", "0"));
            cluster.AddPropertyList(setProp("status.queue.Destruction.waiting", "0"));
            cluster.AddPropertyList(setProp("status.queue.Order.waiting", "0"));
            cluster.AddPropertyList(setProp("status.realm.destruction.density", "1"));
            cluster.AddPropertyList(setProp("status.realm.order.density", "1"));
            cluster.AddPropertyList(setProp("status.servertype.openrvr", "0"));
            cluster.AddPropertyList(setProp("status.servertype.rp", "0"));
            cluster.AddPropertyList(setProp("status.status", "0"));

            cluster.Build();
            return cluster;
        }
        #endregion

      

    }
}
