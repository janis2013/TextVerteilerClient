/*
 * 
 * This Program is free for ANY use.
 * 
 * 
 * Program written by Janis Dähne
 * 
 * To change network stuff
 * change port: Port
 * 
 * 
 * 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using TextVerteilerClient.Networking;

using System.Net;
using System.Net.Sockets;



namespace TextVerteilerClient
{
    public partial class FormMain : Form
    {

        public static string IPADDRESS = "192.168.30.51";

        private int Port = 8008;

        public ClientContext Client { get; set; }

        public static double LeaveOpacity = 0.3D;

        public List<string> TextHistory;


        public int Pointer { get; set; }

        public static int MAXBits = 42000; // = 21.000 chars

        public int LazyCounter { get; set; }//nach XX sec automatisch verstecken

        public int MaxLazy { get; set; }


        public static int HistoryStackSize { get; set; }


        public int CheckStatusCounter { get; set; } //0 to 10

        public FormMain()
        {
            InitializeComponent();
        }

        public void FormMain_Load(object sender, EventArgs e)
        {
          //weil visible das hier auslöst
            ResetPosition();
        }

        public void IniFormMain()
        {
            TextHistory = new List<string>();

            ResetPosition();
            this.TopMost = true;

            FormMain.HistoryStackSize = 20;

            string ip = Config.LoadIp();

            Program.fmEinstellungen.tbIP.Text = ip;

            //connect on startup
            Program.fmEinstellungen.btnConnect_Click(null, new EventArgs());
            
        }


        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIconFmMain.ShowBalloonTip(1000);
            this.Visible = false;
            e.Cancel = true;

        }

        public void SetText(string Text)
        {
            if (tbMessage.InvokeRequired)
            {
                Action<string> set = new Action<string>(SetText);
                this.Invoke(set, new object[] { Text });
            }
            else
            {
                tbMessage.Clear();
                tbMessage.Text = Text;
            }

        }

        public void SetFormText(string Text)
        {
            if (this.InvokeRequired)
            {
                Action<string> set = new Action<string>(SetFormText);
                this.Invoke(set, new object[] { Text });
            }
            else
            {
                this.Text = Text;
            }
        }

        public void SetFormVisibility(bool visible)
        {
            if (this.InvokeRequired)
            {
                Action<bool> set = new Action<bool>(SetFormVisibility);
                this.Invoke(set, new object[] { visible });
            }
            else
            {
                this.Visible = visible;
            }
        }

        public void Connect(IPAddress RemoteIp)
        {
            Client = new ClientContext(ref TextHistory, MAXBits);
            Client.OnDataReceived += SetText;
            Client.BeginConnect(RemoteIp, this.Port);

        }

        public void Disconnect()
        {
            if (Client != null)
            {
                Client.Close();
            }

        }


        public void ResetPosition()
        {
            Point p = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width,
                   Screen.PrimaryScreen.WorkingArea.Bottom - this.Height);
            //((Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom) = taskbar height);
            this.Location = p;
        }

        private void SetEnterOpacity()
        {
            this.Opacity = 1.0;
        }

        private void SetLeaveOpacity()
        {
            this.Opacity = LeaveOpacity;
        }


        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (this.DesktopBounds.IntersectsWith(new Rectangle(Cursor.Position.X, Cursor.Position.Y, 1, 1)))
            {
                SetEnterOpacity();
            }
            else
            {
                SetLeaveOpacity();

                //nur wenn fenster auch wirklich sichtbar
                if (this.Visible == true)
                {
                    if (LazyCounter >= MaxLazy && MaxLazy > 0)
                    {
                        this.Visible = false;
                        LazyCounter = 0;
                    }

                    if (MaxLazy > 0)
                    {
                        LazyCounter++;
                    }
                    else
                    {
                        LazyCounter = 0;
                    }
                }
            }

            if (CheckStatusCounter >= 10)
            {
                if (!Client.isConnected())
                {
                    Program.fmEinstellungen.btnDisconnect_Click(null, new EventArgs());
                }
                this.CheckStatusCounter = 0;
            }
            this.CheckStatusCounter++;

        }

        private void einstellungenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.fmEinstellungen.Visible = true;
        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shutdown();
        }

        public void Shutdown()
        {
            Disconnect();
            notifyIconFmMain.Dispose();
            Program.ExitAndSaveIp();
        }

        private void notifyIconFmMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.Visible == true)
                {
                    this.Visible = false;
                }
                else
                {
                    this.Visible = true;
                }

            }
        }


        public void SetNewPointerPos(int Position)
        {
            this.Pointer = Position - 1;
            if (Position == 2) // jetzt haben wir 2 einträge zum navigieren
            {
                if (btnPrevious.InvokeRequired)
                {
                    Action<int> set = new Action<int>(SetNewPointerPos);
                    this.Invoke(set, new object[] { Position });
                }
                else
                {
                    btnPrevious.Enabled = true;
                }              
            }
            else if ( Position > 2)
            {
                if (btnPrevious.InvokeRequired)
                {
                    Action<int> set = new Action<int>(SetNewPointerPos);
                    this.Invoke(set, new object[] { Position });
                }
                else
                {
                    btnPrevious.Enabled = true;
                    btnNext.Enabled = false;
                }              
               
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            Pointer--;
            SetTextFromHistory();
            if (Pointer == 0)
            {
                btnPrevious.Enabled = false;
                btnNext.Enabled = true;
            }
            else
            {
                btnPrevious.Enabled = true;
                btnNext.Enabled = true;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Pointer++;
            SetTextFromHistory();
            if (Pointer == TextHistory.Count - 1)
            {
                btnNext.Enabled = false;
                btnPrevious.Enabled = true;
            }
            else
            {
                btnPrevious.Enabled = true;
                btnNext.Enabled = true;
            }
        }

        private void SetTextFromHistory()
        {
            if (Pointer >= 0 && Pointer < this.TextHistory.Count)
            {
                this.tbMessage.Clear();
                tbMessage.Text = this.TextHistory[Pointer];
                tbMessage.SelectAll();
            }

        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                tbMessage.SelectAll();
            }
        }



    }
}
