using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyService
{
    public class MyServiceTasks
    {
        private const string logSource = "MyService";
        private const string logName = "Application";
        private const string inputFilePath = "C:\\myserviceinput.txt";

        public MyServiceTasks()
        {
            SetupEventLog();
        }

        public void MyServiceTasksEntryPoint(object stateObject)
        {
            MainMethod();
        }

        private void MainMethod()
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(inputFilePath))
                {
                    String line = streamReader.ReadToEnd();
                    Log(string.Format("Read from file: {0}", line));
                }
            }
            catch (Exception e)
            {
                Log(string.Format("Could not read from file: {0}", e.Message));
            }
        }

        private void SetupEventLog()
        {
            if (!EventLog.SourceExists(logSource))
            {
                EventLog.CreateEventSource(logSource, logName);
            }
        }

        private void Log(string logMsg)
        {
            EventLog.WriteEntry(logSource, logMsg);
        }
    }
}
