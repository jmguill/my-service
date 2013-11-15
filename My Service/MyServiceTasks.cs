using System;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Security.AccessControl;

namespace MyService
{
    public class MyServiceTasks
    {
        private const string logSource = "MyService";
        private const string logName = "Application";
        private const string inputFilePath = "C:\\myserviceinput.txt";

        private NamedPipeServerStream pipe;

        public MyServiceTasks()
        {
            SetupEventLog();

            PipeSecurity ps = new PipeSecurity();
            ps.AddAccessRule(new PipeAccessRule("Users", PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, AccessControlType.Allow));
            ps.AddAccessRule(new PipeAccessRule("CREATOR OWNER", PipeAccessRights.FullControl, AccessControlType.Allow));
            ps.AddAccessRule(new PipeAccessRule("SYSTEM", PipeAccessRights.FullControl, AccessControlType.Allow));

            pipe = new NamedPipeServerStream("MyServicePipe", PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 1024, 1024, ps);
            InterProcessSecurity.SetLowIntegrityLevel(pipe.SafePipeHandle);
        }

        public void MyServiceTasksEntryPoint(object stateObject)
        {
            MainMethod(stateObject);
        }

        private void MainMethod(object stateObject)
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
            AsyncCallback pipeConnectioCallback = new AsyncCallback(AsyncPipeCallback);
            if (!pipe.IsConnected)
            {
                try
                {
                    pipe.BeginWaitForConnection(pipeConnectioCallback, stateObject);
                }
                catch (Exception e)
                {
                    Log(string.Format("Oops: {0}", e.Message));
                }
            }
            else
            {
                ReadFromPipe();
            }
        }

        private void AsyncPipeCallback(IAsyncResult Result)
        {
            pipe.EndWaitForConnection(Result);
        }

        private void ReadFromPipe()
        {
            byte[] pipeBuffer = new byte[100];
            
            try
            {
                int bytesRead = pipe.Read(pipeBuffer, 0, 100);
                if (bytesRead > 0)
                {
                    char[] chars = new char[bytesRead];
                    System.Buffer.BlockCopy(pipeBuffer, 0, chars, 0, bytesRead);
                    string pipeString = new string(chars);
                    Log(string.Format("Read {0} bytes from pipe: {1}", bytesRead, pipeString));
                }
            }
            catch (Exception e)
            {
                Log(string.Format("Could not read from pipe: {0}", e.Message));
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

        public static class NativeMethods
        {
            public const string LOW_INTEGRITY_SSL_SACL = "S:(ML;;NW;;;LW)";
            public static int ERROR_SUCCESS = 0x0;
            public const int LABEL_SECURITY_INFORMATION = 0x00000010;
            public enum SE_OBJECT_TYPE
            {
                SE_UNKNOWN_OBJECT_TYPE = 0,
                SE_FILE_OBJECT,
                SE_SERVICE,
                SE_PRINTER,
                SE_REGISTRY_KEY,
                SE_LMSHARE,
                SE_KERNEL_OBJECT,
                SE_WINDOW_OBJECT,
                SE_DS_OBJECT,
                SE_DS_OBJECT_ALL,
                SE_PROVIDER_DEFINED_OBJECT,
                SE_WMIGUID_OBJECT,
                SE_REGISTRY_WOW64_32KEY
            }

            [DllImport("advapi32.dll", EntryPoint = "ConvertStringSecurityDescriptorToSecurityDescriptorW")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean ConvertStringSecurityDescriptorToSecurityDescriptor(
                [MarshalAs(UnmanagedType.LPWStr)] String strSecurityDescriptor,
                UInt32 sDRevision,
                ref IntPtr securityDescriptor,
                ref UInt32 securityDescriptorSize);

            [DllImport("kernel32.dll", EntryPoint = "LocalFree")]
            public static extern UInt32 LocalFree(IntPtr hMem);

            [DllImport("Advapi32.dll", EntryPoint = "SetSecurityInfo")]
            public static extern int SetSecurityInfo(SafeHandle hFileMappingObject,
                                                        SE_OBJECT_TYPE objectType,
                                                        Int32 securityInfo,
                                                        IntPtr psidOwner,
                                                        IntPtr psidGroup,
                                                        IntPtr pDacl,
                                                        IntPtr pSacl);
            [DllImport("advapi32.dll", EntryPoint = "GetSecurityDescriptorSacl")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean GetSecurityDescriptorSacl(
                IntPtr pSecurityDescriptor,
                out IntPtr lpbSaclPresent,
                out IntPtr pSacl,
                out IntPtr lpbSaclDefaulted);
        }

        public class InterProcessSecurity
        {

            public static void SetLowIntegrityLevel(SafeHandle hObject)
            {
                IntPtr pSD = IntPtr.Zero;
                IntPtr pSacl;
                IntPtr lpbSaclPresent;
                IntPtr lpbSaclDefaulted;
                uint securityDescriptorSize = 0;

                if (NativeMethods.ConvertStringSecurityDescriptorToSecurityDescriptor(NativeMethods.LOW_INTEGRITY_SSL_SACL, 1, ref pSD, ref securityDescriptorSize))
                {
                    if (NativeMethods.GetSecurityDescriptorSacl(pSD, out lpbSaclPresent, out pSacl, out lpbSaclDefaulted))
                    {
                        var err = NativeMethods.SetSecurityInfo(hObject,
                                                      NativeMethods.SE_OBJECT_TYPE.SE_KERNEL_OBJECT,
                                                      NativeMethods.LABEL_SECURITY_INFORMATION,
                                                      IntPtr.Zero,
                                                      IntPtr.Zero,
                                                      IntPtr.Zero,
                                                      pSacl);
                        if (err != NativeMethods.ERROR_SUCCESS)
                        {
                            throw new Win32Exception(err);
                        }
                    }
                    NativeMethods.LocalFree(pSD);
                }
            }
        }
    }
}
