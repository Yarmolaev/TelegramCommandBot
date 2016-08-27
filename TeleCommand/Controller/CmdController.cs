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
        #region Definitions
        #region Fields
        Process Process;
        StreamWriter StreamWriter;

        #endregion
        #region Instance
        private static CmdController Controller;
        #endregion
        #endregion

        #region Functions
        /// <summary>
        /// Constructor for CmdController
        /// </summary>
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

        /// <summary>
        /// Gets the instance of CmdController
        /// </summary>
        /// <returns></returns>
        public static CmdController GetInstance()
        {
            if (Controller == null || Controller.Process.HasExited)
                Controller = new CmdController();
            return Controller;
        }

        /// <summary>
        /// Resets the cmd controller
        /// </summary>
        public static void ResetController()
        {
            Controller = null;
        }

        /// <summary>
        /// Stops cmd reading
        /// </summary>
        public void CmdStop()
        {
            Process.WaitForExit(2000);
        }

        /// <summary>
        /// Runs a cmd command
        /// </summary>
        /// <param name="command"></param>
        public void RunCommand(string command)
        {
            StreamWriter.WriteLine(command);
        }

        /// <summary>
        /// Sends the Output to Output Textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessOutputHandler(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            BasicController.GetInstance(null, null, null).SendMessage(e.Data);
        }

        #endregion
    }
}
