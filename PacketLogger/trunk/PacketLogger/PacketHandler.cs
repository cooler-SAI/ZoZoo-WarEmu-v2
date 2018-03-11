using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PacketLogger
{
    public class PacketHandler
    {
        bool KeyFound = false;
        int PacketCount = 0;
        byte[] RC4Key = new byte[256];
        private Memory mem= new Memory();
        private Crypto crypto = new Crypto();
        private Log log = new Log();
        bool IsClient;
        string filename = "packet_d" + 
            DateTime.Now.ToString("dd") + 
            "h" + DateTime.Now.ToString("hh") + 
            "m" + DateTime.Now.ToString("mm") + ".txt";

        public IniFile config = new IniFile(".\\config.ini");
  

        private byte[] Decode(byte[] packet)
        {
            if (KeyFound)
                return crypto.Decode(packet, RC4Key, IsClient);
            else
                return null;
        }
        public void Packethandler(byte[] Packet, bool IsFromClient)
        {

         
            
            IsClient = IsFromClient;
            if (!IsClient)
            {
                if (!KeyFound && Packet.Length == 219)
                {
                    RC4Key = mem.GetRC4Key();
                    KeyFound = true;
                    log.Info("RC4 key found...");
                }
            }
            if (KeyFound) PacketCount++; //First 2 packets are for RSA encryption, so we don't need them.
            if (PacketCount <= 2) return;

            
             var decoded = Decode(Packet);
             if (IsClient)
             {
                 var opcode = decoded[9].ToString("X2");
                 if (!config.Filter(opcode))
                 {
                     string box = log.PacketBox(decoded.Length, opcode, decoded, true);
                     log.Info(box);
                     log.WriteFile(box, filename);
                 }
             }
             else
             {
                 var opcode = decoded[2].ToString("X2");
                 if (!config.Filter(opcode))
                 {
                     string box = log.PacketBox(decoded.Length, opcode, decoded, false);
                     log.Debug(box);
                     log.WriteFile(box, filename);
                 }
             }

        }










    }
}
