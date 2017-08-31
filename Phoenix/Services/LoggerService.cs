using System;
using System.IO;
using System.Web.Hosting;

namespace Phoenix.Services
{

    /// <summary>
    /// Simple logging class to log information to external sources e.g. files, maybe a table later on?
    /// If you want to log to the console use the builtin Debug.WriteLine().
    /// 
    /// Later on, we can use this class to modify how things are logged at different levels, without changing code elsewhere.
    /// </summary>
    public class LoggerService
    {
        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Info(string message)
        {
            Log("INFO", message);
        }

        /// <summary>
        /// Log Errors. Provide as much context information as possible.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Error(string message)
        {
            Log("ERROR", message);
        }

        private void Log(string level, string message)
        {
            string today = DateTime.Today.ToShortDateString().Replace("/", "_");

            // Msdn doesn't have enough documentation on DateTime, so I used:
            // http://www.c-sharpcorner.com/uploadfile/mahesh/working-with-datetime-using-C-Sharp/
            // to figure out the type of timestamp format I wanted
            string timestamp = DateTime.Today.ToString("G");

            string folderPath = "\\Logs\\";
            Directory.CreateDirectory(HostingEnvironment.MapPath(folderPath));

            var stream = File.AppendText(HostingEnvironment.MapPath(folderPath + today + ".log"));

            stream.WriteLine(timestamp + " --- " + "[" + level + "]");
            stream.Write(message);
            stream.WriteLine();
            stream.Dispose();

        }
    }
}