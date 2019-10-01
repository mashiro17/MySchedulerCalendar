using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySchedulerCalendar
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log.LogFile =  String.Format("{0}_MySchedulerCalendar_log.txt", DateTime.Now.ToString("yyyyMMdd"));
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new CalendarForm());
            }
            catch(Exception e)
            {
               // Log.AppendOnFile("E",e.Message);
            }
        }
    }
}
