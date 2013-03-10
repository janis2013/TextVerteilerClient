using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TextVerteilerClient
{
    static class Program
    {

        public static FormMain fmMain;
        public static FormEinstellungen fmEinstellungen;

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            fmMain = new FormMain();

            fmEinstellungen = new FormEinstellungen();

            fmMain.IniFormMain();


            Application.Run();

        }

        public static void ExitAndSaveIp()
        {
            Config.SaveIp(Program.fmEinstellungen.tbIP.Text);

            Application.ExitThread();
        }
    }

}
