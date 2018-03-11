using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PacketLogger
{
    public class Memory
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Boolean bInheritHandle, UInt32 dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
        byte[] lpBuffer, UIntPtr nSize, ref uint lpNumberOfBytesWritten);
        public IniFile config = new IniFile(".\\config.ini");

        public byte[] ReadBytes(IntPtr Handle, int addr, int size)
        {
            byte[] bytestoread = new byte[size];
            uint rw = 0;
            ReadProcessMemory(Handle, (IntPtr)addr, bytestoread, (UIntPtr)size, ref rw);
            return bytestoread;
        }

        public byte[] GetRC4Key()
        {
            try
            {
                int address = Convert.ToInt32(config.IniReadValue("Offsets", "RC4Address"), 16);

                Process[] Processes = Process.GetProcessesByName("war");
                Process war = Processes[0];
                IntPtr Handle = OpenProcess(0x10, false, (uint)war.Id);
                return ReadBytes(Handle, address, 256); // RC4 addr 0x001844C8 [1.4.5a]

            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return null; }
            
        }

    }
}
