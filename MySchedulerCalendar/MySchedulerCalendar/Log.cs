using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace MySchedulerCalendar
{
 

    
        public class Log
        {
            public static string LogFile { get; set; }

            public static void AppendOnFile(string severity, string message, params object[] args)
            {
                StreamWriter Writer;
                if (File.Exists(LogFile))
                    Writer = new StreamWriter(LogFile, true);
                else
                    Writer = new StreamWriter(LogFile);

                StringBuilder sb = new StringBuilder();



                DateTime n = DateTime.Now;
                sb.AppendFormat("[{0:00}:{1:00}:{2:00}.{3:000}]",
                    new object[] { n.Hour, n.Minute, n.Second, n.Millisecond });
                sb.AppendFormat("<{0:0000}>", System.Threading.Thread.CurrentThread.ManagedThreadId);
                sb.AppendFormat("({0}): ", severity);
                Writer.WriteLine(sb.Append(args.Length > 0 ? String.Format(message, args) : message).ToString());
                Writer.Close();

            }

        }
    
}
