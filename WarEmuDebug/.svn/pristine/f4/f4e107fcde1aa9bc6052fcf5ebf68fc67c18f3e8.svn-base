using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MYPHandler;
using nsHashDictionary;
using System.IO;
using System.Configuration;
using System.Diagnostics;
namespace WarEmu
{
    public partial class Form1 : Form
    {
        string path = "";
        string username = "";
        string password = ""; 
        public Form1()
        {
            InitializeComponent();
           // readConfig();
        }

     

        private void patch()
        {
            using (Stream stream = new FileStream(path + @"\WAR.exe", FileMode.OpenOrCreate))
            {
                int encryptAddress = (0x00957FBE + 3) - 0x00400000;
                stream.Seek(encryptAddress, SeekOrigin.Begin);
                stream.WriteByte(0x01);



                byte[] decryptPatch1 = { 0x90, 0x90, 0x90, 0x90, 0x57, 0x8B, 0xF8, 0xEB, 0x32 };
                int decryptAddress1 = (0x009580CB) - 0x00400000;
                stream.Seek(decryptAddress1, SeekOrigin.Begin);
                stream.Write(decryptPatch1, 0, 9);

                byte[] decryptPatch2 = { 0x90, 0x90, 0x90, 0x90, 0xEB, 0x08 };
                int decryptAddress2 = (0x0095814B) - 0x00400000;
                stream.Seek(decryptAddress2, SeekOrigin.Begin);
                stream.Write(decryptPatch2, 0, 6);

                //stream.WriteByte(0x01);
            }
        }
        private void updateMYP()
        {
            
            FileStream fs = new FileStream(Application.StartupPath + "\\PortalSettings.xml", FileMode.Open, FileAccess.Read);
            Directory.SetCurrentDirectory(path);
            HashDictionary hashDictionary = new HashDictionary();
            hashDictionary.AddHash(0x3FE03665, 0x349E2A8C, "F4FCD464_3FE03665349E2A8C.xml", 0);
            MYPHandler.MYPHandler mypHandler = new MYPHandler.MYPHandler("data.myp", null, null, hashDictionary);
            mypHandler.GetFileTable();
            FileInArchive theFile = mypHandler.SearchForFile("F4FCD464_3FE03665349E2A8C.xml");
            mypHandler.ReplaceFile(theFile, fs);

            fs.Close();

            if (theFile != null)
                MessageBox.Show("patch success!");

        }
        private void unpatch()
        {
            using (Stream stream = new FileStream(path + @"\WAR.exe", FileMode.OpenOrCreate))
            {
                int encryptAddress = (0x00957FBE + 3) - 0x00400000;
                stream.Seek(encryptAddress, SeekOrigin.Begin);
                stream.WriteByte(0x00);


                int decryptAddress = (0x009580CB + 3) - 0x00400000;
                stream.Seek(decryptAddress, SeekOrigin.Begin);
                stream.WriteByte(0x00);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
           
            patch();
            updateMYP();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string startupPath = Application.StartupPath;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Open the folder for Warhammer";
                dialog.ShowNewFolderButton = false;
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    path = dialog.SelectedPath;
                    textBox3.Text = path;
                    UpdateSetting("path", path);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (path != "")
            {
                Process Pro = new Process();
                Pro.StartInfo.FileName = path + @"\WAR.exe";
                if (username != "")
                {
                    Pro.StartInfo.Arguments = " --acctname=" + System.Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username)) + " --sesstoken=REIxREUwMEEwQjFEMzlFMkNFMzdBODEwMDBFRjY3REM=";
                }
                else
                {
                    Pro.StartInfo.Arguments = " --acctname=bG9naW5Vc2Vy --sesstoken=REIxREUwMEEwQjFEMzlFMkNFMzdBODEwMDBFRjY3REM=";
                }
                Pro.Start();
            }
            else
            {
                MessageBox.Show("Error: path is missing");
            }

        }

        private static void UpdateSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            path = ConfigurationManager.AppSettings["path"];
            textBox3.Text = path;

            username = ConfigurationManager.AppSettings["username"];
            textBox1.Text = username;

            password = ConfigurationManager.AppSettings["password"];
            textBox2.Text = password;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            path = textBox3.Text;
            UpdateSetting("path", path);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateSetting("username", textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            UpdateSetting("password", textBox2.Text);
        }
    }
}
