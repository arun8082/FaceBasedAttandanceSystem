using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testdlibdotnetNuget
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static void log(string message) {
            string logFile = Application.StartupPath + @"\log\error.log";
            Console.WriteLine("\n\nLog file ");
            try
            {
                if (!Directory.Exists(Application.StartupPath + @"\log"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\log");
                }
                if (!File.Exists(logFile)) {
                    File.Create(logFile);
                }
                if (!string.IsNullOrEmpty(message))
                {
                    File.AppendAllText(logFile, "\n"+DateTime.Now+"\t\t"+message);
                }
            }
            catch (Exception dex) {

            }
        }
    }
}
