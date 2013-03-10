using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace TextVerteilerClient
{
    public class Config
    {

        public static string ConfigPath = Path.Combine(Environment.CurrentDirectory, "config.config");

        static FileInfo info = new FileInfo(ConfigPath);

        public static void SaveIp(string Ip)
        {
            info.Refresh();

            if (info.Exists)
            {
                var stream = info.Open(FileMode.OpenOrCreate);

                stream.SetLength(0);
                stream.Close();
                File.WriteAllText(ConfigPath, Ip);
            }
            else
            {
                info.Create().Close();
                File.WriteAllText(ConfigPath, Ip);
            }
        }


        public static string LoadIp()
        {
            info.Refresh();

            if (info.Exists && info.Length < 20) //ip add ist nicht groß
            {
                string ip = File.ReadAllText(ConfigPath);
                return ip;
            }

            return FormMain.IPADDRESS;

        }
    }
}
