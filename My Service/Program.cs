using System;
using System.ServiceProcess;

namespace MyService
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ServiceBase.Run(new MyService());
        }
    }
}