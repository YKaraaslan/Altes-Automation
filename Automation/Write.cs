using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public class Write
    {
        static string path = Properties.Resources.LogsPath;

        public static void ToFile(string message)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string pathYear = path + @"\" + DateTime.Now.Date.Year.ToString();
            if (!Directory.Exists(pathYear))
            {
                Directory.CreateDirectory(pathYear);
            }
            string pathMonth = pathYear + @"\" + DateTime.Now.Date.ToString("MMMM");
            if (!Directory.Exists(pathMonth))
            {
                Directory.CreateDirectory(pathMonth);
            }
            string filePath = pathMonth + @"\" + DateTime.Now.Date.ToString("dd") + "_" + DateTime.Now.ToString("dddd") + ".txt";

            if (!File.Exists(filePath))
            {
                using(StreamWriter sw = File.CreateText(filePath))
                {
                    string text = String.Format("{0} - {1}", DateTime.Now.ToString("HH:mm:ss"), message);
                    sw.WriteLine(text);
                }
            }
            else
            {
                using(StreamWriter sw = File.AppendText(filePath))
                {
                    string text = String.Format("{0} - {1}", DateTime.Now.ToString("HH:mm:ss"), message);
                    sw.WriteLine(text);
                }
            }
        }
    }
}
