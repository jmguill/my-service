using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string pipeString = "Hello hello hello!";
            byte[] pipeBuffer = new byte[pipeString.Length * sizeof(char)];
            System.Buffer.BlockCopy(pipeString.ToCharArray(), 0, pipeBuffer, 0, pipeString.Length);

            NamedPipeClientStream pipe = new NamedPipeClientStream(".", "MyServicePipe", PipeDirection.Out, PipeOptions.WriteThrough, System.Security.Principal.TokenImpersonationLevel.Impersonation);
            pipe.Connect();
            pipe.Write(pipeBuffer, 0, pipeBuffer.Length);
            Thread.Sleep(10000);
            pipe.Close();
        }
    }
}
