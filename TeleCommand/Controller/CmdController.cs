using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.yarmolaev.TelegramCommandBot.Controller
{
    class CmdController
    {
        Process Process;
        StreamWriter StreamWriter;
        private static CmdController Controller;

        private CmdController()
        {
            if (Process == null)
            {
                Process = new Process();
                Process.StartInfo.FileName = "cmd.exe";

                // Set UseShellExecute to false for redirection.
                Process.StartInfo.UseShellExecute = false;

                // Redirect the standard output of the sort command.  
                // This stream is read asynchronously using an event handler.
                Process.StartInfo.RedirectStandardOutput = true;
                Process.StartInfo.RedirectStandardInput = true;
                Process.StartInfo.RedirectStandardError = true;
                Process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(850);
                var sortOutput = new StringBuilder("");

                // Set our event handler to asynchronously read the sort output.
                Process.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputHandler);
                Process.ErrorDataReceived += new DataReceivedEventHandler(ProcessOutputHandler);

                // Redirect standard input as well.  This stream
                // is used synchronously.
                Process.StartInfo.RedirectStandardInput = true;
                Process.Start();

                // Use a stream writer to synchronously write the sort input.
                StreamWriter = Process.StandardInput;

                // Start the asynchronous read of the sort output stream.
                Process.BeginOutputReadLine();

            }
        }

        public static CmdController GetInstance()
        {
            if (Controller == null || Controller.Process.HasExited)
                Controller = new CmdController();
            return Controller;
        }

        public void CmdStop()
        {
            Process.WaitForExit(2000);
        }

        public void RunCommand(string command)
        {
            StreamWriter.WriteLine(command);
        }

        private void Test(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        public delegate void AppendTextCallback(string text);

        private void ProcessOutputHandler(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            Console.WriteLine(e.Data);
            if (e.Data.Contains("ÿ"))
                Console.WriteLine("t");
            BasicController.GetInstance(null, null, null).SendMessage(e.Data);
        }

        private void AppendText(string text)
        {
            Console.WriteLine(text);
        }
    }
}
