using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WarhammerEmu.LoginServer;
using System.Threading;

namespace WarhammerEmu
{
    public partial class Form1 : Form
    {

        public static WarhammerEmu.GameServer.Connection activeConnection;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;



        IntPtr consoleHandle;
        bool isListening = false;
        Thread LobbyThread;
        Thread GameServerThread;
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
            consoleHandle = GetConsoleWindow();
            checkBox1.Checked = true;
        }




        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                ShowWindow(consoleHandle, SW_SHOW);
            }
            else if (!checkBox1.Checked)
            {
                ShowWindow(consoleHandle, SW_HIDE);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (activeConnection != null)
            {   
                    activeConnection.SendCustomPacket(richTextBox1.Text);   
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isListening == false)
            {
                LoginServer.Listener loginserv = new LoginServer.Listener("127.0.0.1", int.Parse(textBox1.Text));
                LobbyThread = new Thread(new ThreadStart(loginserv.Run));
                LobbyThread.Start();



                GameServer.Listener gameServer = new GameServer.Listener("127.0.0.1", int.Parse(textBox2.Text));
                GameServerThread = new Thread(new ThreadStart(gameServer.Run));
                GameServerThread.Start();    

                isListening = true;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isListening)
            {
                LobbyThread.Abort();
                GameServerThread.Abort();
            }
        }
    }
}
