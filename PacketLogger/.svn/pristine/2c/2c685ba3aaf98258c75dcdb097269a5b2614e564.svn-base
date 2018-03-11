using System;
using System.IO;

namespace PacketLogger
{
    public class Log:IDisposable
    {
        private StreamWriter Stream;

        public void Debug(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(obj);
            Console.ResetColor();
        }
        public void Error(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(obj);
            Console.ResetColor();
        }
        public void Info(object obj)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(obj);
            Console.ResetColor();
        }

        public void WriteFile(string s, string filename)
        {
            if (!File.Exists(filename))
            {
                Stream = new StreamWriter(filename);
            }
            else
            {
                Stream = File.AppendText(filename);
            }
            Stream.WriteLine(s);
            Stream.WriteLine("\r\n");
            Stream.Close();
        }
        public string PacketBox(int size, string opcode, byte[] data, bool isFromClient)
        {
            if (data.Length <= 0) return "";
            if (opcode == null) return "";

            string output = "";
            byte[] packet = data;
            var op = Convert.ToInt32(opcode, 16);

            string opname = Enum.GetName(typeof(Opcodes), op);
            if (opname == null) { opname = "UNKNOWN"; }
            string sname = isFromClient ? "[Client]" : "[Server]";
            output = String.Format(sname + ": Opcode = 0x{0} {1} PacketSize = {2} \r\n", opcode, opname, packet.Length);
            output += "|------------------------------------------------|----------------|\r\n";
            output += "|00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F |0123456789ABCDEF|\r\n";
            output += "|------------------------------------------------|----------------|\r\n";
            int countpos = 0;
            int charcount = 0;
            int charpos = 0;
            int line = 1;

            if (size > 0)
            {
                output += "|";
                for (int count = 0; count < size; count++)
                {
                    if (line == 0) { output += "|"; line = 1; }
                    output += packet[count].ToString("X2") + " ";
                    countpos++;
                    if (countpos == 16)
                    {
                        output += "|";
                        for (int c = charcount; c < size; c++)
                        {
                            if (((int)packet[charcount] < 32) || ((int)packet[charcount] > 126))
                            {
                                output += ".";
                            }
                            else { output += (char)packet[charcount]; }

                            charcount++;
                            charpos++;
                            if (charpos == 16)
                            {
                                charpos = 0;
                                break;
                            }
                        }
                        if (charcount < size) { output += "|\r\n"; } else { output += "|"; }
                        countpos = 0;
                        line = 0;
                    }
                }
                if (countpos < 16)
                {
                    for (int k = 0; k < 16 - countpos; k++)
                    { output += "   "; }
                }
                if (charcount < size)
                {
                    output += "|";

                    for (int c = charcount; c < size; c++)
                    {
                        output += ".";
                        charcount++;
                        charpos++;
                    }

                    if (charpos < 16)
                    {
                        for (int j = 0; j < 16 - charpos; j++)
                        { output += " "; }
                    }
                    output += "|";
                }
            }

            output += "\r\n-------------------------------------------------------------------";

            return output;

        }
        public void Dispose()
        {
            Stream.Dispose();
            GC.SuppressFinalize(Stream);
        }
    }
}
