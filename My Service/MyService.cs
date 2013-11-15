using System;
using System.Threading;
using System.ServiceProcess;
using System.Diagnostics;
namespace MyService
{
    public class MyService : ServiceBase
    {
        private MyServiceTasks myServiceTasks;
        private Timer stateTimer;
        private TimerCallback timerDelegate;

        public MyService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            myServiceTasks = new MyServiceTasks();
            timerDelegate = new TimerCallback(myServiceTasks.MyServiceTasksEntryPoint);
            stateTimer = new Timer(timerDelegate, null, 5000, 10000);
            this.EventLog.WriteEntry("MyService Service Has Started");
        }

        protected override void OnStop()
        {
            stateTimer.Dispose();
            this.EventLog.WriteEntry("MyService Service Has Stopped");
        }

        private void InitializeComponent()
        {
            this.ServiceName = "MyService";
            this.CanStop = true;
            this.AutoLog = false;
            this.EventLog.Log = "Application";
            this.EventLog.Source = "MyService";
        }
    }
}