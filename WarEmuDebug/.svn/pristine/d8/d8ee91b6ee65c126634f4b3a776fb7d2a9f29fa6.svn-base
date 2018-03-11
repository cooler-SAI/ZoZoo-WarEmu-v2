using System;
using System.Text;
using System.IO;
namespace WarhammerEmu
{
    public static class Log
    {

        private static readonly ConsoleColor warn = ConsoleColor.Red;
        private static readonly ConsoleColor trace = ConsoleColor.Yellow;
        private static readonly ConsoleColor debug = ConsoleColor.Green;
        private static readonly ConsoleColor dump = ConsoleColor.Cyan;

        private static void write(object obj, ConsoleColor color)
        {
            Console.ResetColor();
            Console.ForegroundColor = color;
            Console.WriteLine(obj);
        }
        public static void Debug(object obj)
        {

            string sformat = string.Format("[Debug]: {0}", obj);
            write(sformat, debug);
        }
        public static void Trace(object obj)
        {
            string sformat = string.Format("[Trace]: {0}", obj);
            write(sformat, trace);
        }
        public static void Warn(object obj)
        {
            string sformat = string.Format("[Warn]: {0}", obj);
            write(sformat, warn);
        }
        public static void Error(object obj)
        {
            string sformat = string.Format("[Error]: {0}", obj);
            write(sformat, warn);
        }
        public static void Error(string t, object obj)
        {
            string sformat = string.Format("[Error]: {0} {1}", t, obj);
            write(sformat, warn);
        }

        public static void DumpHex(byte[] packet)
        {

            string sformat = string.Format("[Dump]: {0}", Hex(packet, 0, packet.Length));
            write(sformat, dump);
        }

        public static string Hex(byte[] dump, int start, int len)
        {
            var hexDump = new StringBuilder();

            try
            {
                int end = start + len;
                for (int i = start; i < end; i += 16)
                {
                    StringBuilder text = new StringBuilder();
                    StringBuilder hex = new StringBuilder();
                    hex.Append("\n");

                    for (int j = 0; j < 16; j++)
                    {
                        if (j + i < end)
                        {
                            byte val = dump[j + i];
                            hex.Append(" ");
                            hex.Append(dump[j + i].ToString("X2"));
                            if (j == 3 || j == 7 || j == 11)
                                hex.Append(" ");
                            if (val >= 32 && val <= 127)
                            {
                                text.Append((char)val);
                            }
                            else
                            {
                                text.Append(".");
                            }
                        }
                        else
                        {
                            hex.Append("   ");
                            text.Append("  ");
                        }
                    }
                    hex.Append("  ");
                    hex.Append("//" + text.ToString());
                    hexDump.Append(hex.ToString());
                }
            }
            catch (Exception e)
            {
                Log.Error("HexDump", e.ToString());
            }

            return hexDump.ToString();
        }

    }
}
