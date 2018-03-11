using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PacketLogger
{
    public class IniFile
    {
        public string path;
        
            [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
            [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);
        public IniFile(string INIPath)
        {
            path = INIPath;
        }
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            255, this.path);
            return temp.ToString();

        }

        public bool Filter( string op)
        {
            string[] OpcodeFilter = IniReadValue("Filter", "Opcodes").Split(',');
            string Level = IniReadValue("Filter", "Level");
            if (Level != "0")
            {

                foreach (string code in OpcodeFilter)
                {
                    if (code.ToLower() == op.ToLower())
                    {
                        if (Level == "1")
                            return true;

                        if (Level == "2")
                            return true;
                    }
                }

            }
            return false;
        }

    }
}