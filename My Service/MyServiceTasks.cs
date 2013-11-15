using System;
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
        public MyServiceTasks()
        {
            SetupEventLog();
        }

        public void MyServiceTasksEntryPoint(object stateObject)
        {
            MainMethod();
        }

        private void SetupEventLog()
        {
            if (!EventLog.SourceExists("MyService"))
            {
                EventLog.CreateEventSource("MyService", "Applicaion");
            }
        }

        private void MainMethod()
        {
            Log("Dang this is swag yo!");
        }

        private void Log(string logMsg)
        {
            EventLog.WriteEntry("MyService", logMsg);
        }
    }
}
