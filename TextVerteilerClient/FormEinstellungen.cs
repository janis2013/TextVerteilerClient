using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;

namespace TextVerteilerClient
{
    public partial class FormEinstellungen : Form
    {

        ToolTip tip1 = new ToolTip();
        ToolTip tip2 = new ToolTip();


        ToolTip[] Tooltips;

        public FormEinstellungen()
        {
            InitializeComponent();
        }


        private void FormEinstellungen_Load(object sender, EventArgs e)
        {
            this.TopMost = true;

            Tooltips = new ToolTip[] { tip1 };

            foreach (var tip in Tooltips)
            {
                tip.UseFading = false;
                tip.UseAnimation = false;
                tip.ShowAlways = false;
                tip.InitialDelay = 500;
            }

            tip1.SetToolTip(numAutomatischVerstecken, "Startet, wenn die Maus außerhalb des Fensters ist.");
            tip2.SetToolTip(numHistoryStack, "Gibt an, wie viele Narichten zwischengespeichert werden.");


        }



        private void FormEinstellungen_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }

        public void btnConnect_Click(object sender, EventArgs e)
        {

                IPAddress tempIp;

                if (IPAddress.TryParse(tbIP.Text, out tempIp))
                {
                    Program.fmMain.SetFormText("Connecting...");
                    Program.fmMain.Connect(tempIp);

                }
                else
                {
                    MessageBox.Show("Bitte gültige Ip eingeben!", "Fehler");
                }

                btnDisconnect.Enabled = true;
                btnConnect.Enabled = false;

        }

        public void btnDisconnect_Click(object sender, EventArgs e)
        {

            Program.fmMain.Disconnect();
            Program.fmMain.SetFormText("Disconnected");

            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;

        }


        public void SetBtnConnectVisible(bool Visible)
        {
            if (btnDisconnect.InvokeRequired)
            {
                Action<bool> set = new Action<bool>(SetBtnConnectVisible);
                this.Invoke(set, new object[] { Visible });
            }
            else
            {
                btnConnect.Enabled = Visible;
            }
        }

        public void SetBtnDisconnectVisible(bool Visible)
        {
            if (btnDisconnect.InvokeRequired)
            {
                Action<bool> set = new Action<bool>(SetBtnDisconnectVisible);
                this.Invoke(set, new object[] { Visible });
            }
            else
            {
                btnDisconnect.Enabled = Visible;
            }
        }
  

        private void numOpcaity_ValueChanged(object sender, EventArgs e)
        {
            FormMain.LeaveOpacity = (double)numOpcaity.Value;
        }

        private void btnResetPosition_Click(object sender, EventArgs e)
        {
            Program.fmMain.ResetPosition();
        }

        private void numAutomatischVerstecken_ValueChanged(object sender, EventArgs e)
        {
            if (Program.fmMain != null)
            {
                Program.fmMain.MaxLazy = (int)numAutomatischVerstecken.Value * 10;
            }
        }

        private void numHistoryStack_ValueChanged(object sender, EventArgs e)
        {
            FormMain.HistoryStackSize = (int)numHistoryStack.Value;
        }
    }
}
