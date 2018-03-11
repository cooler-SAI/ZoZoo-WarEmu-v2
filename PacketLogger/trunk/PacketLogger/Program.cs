using System;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System.IO;
using System.Net;

namespace PacketLogger
{
    class Program
    {
        
        private static PacketBuffer ClientBuffer = new PacketBuffer();
        private static PacketBuffer ServerBuffer = new PacketBuffer();
        private static PacketHandler phandler = new PacketHandler();
        private static Log log = new Log(); 
        static void Main(string[] args)
        {
            CaptureDeviceList devices = CaptureDeviceList.Instance;

            if (devices.Count < 1)
            {
                log.Error("No devices were found!");
                return;
            }
            try
            {
                foreach (PcapDevice dev in devices) // capture all
                {
                   
                    dev.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
                    dev.Open();
                    dev.Filter = "tcp port 10622";
                    dev.StartCapture();
                }
            }
            catch (Exception e)
            { log.Error(e.ToString()); }

            Console.ReadLine();

        }
        private static bool IsFromClient(TcpPacket packet)
        {
            return packet.SourcePort != 10622 && packet.DestinationPort == 10622;
        }

        private static bool FlagCheck(byte flag)
        {
            return (flag & (1 << 3)) != 0; // Push flag
        }


        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            try
            {

                var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                var tcpPacket = TcpPacket.GetEncapsulated(packet);

                if (tcpPacket.PayloadData != null && tcpPacket.PayloadData.Length > 0)
                {
                    bool isClientPacket = IsFromClient(tcpPacket);
                    bool IsPacketComplete = FlagCheck(tcpPacket.AllFlags);
                    var buffer = isClientPacket ? ClientBuffer : ServerBuffer;

                   // Console.WriteLine("Flag :{0} \t Size: {2} \t IsClient: {3}", IsPacketComplete, tcpPacket.AcknowledgmentNumber, tcpPacket.PayloadData.Length, isClientPacket);
                    if (IsPacketComplete)
                        {
                            buffer.Append(tcpPacket.PayloadData);
                           phandler.Packethandler(buffer.GetPacketAndReset(), isClientPacket);
                        }
                        else
                        {
                            buffer.Append(tcpPacket.PayloadData);
                        }
                }
            }
            catch{ }
        }
        }
    }

