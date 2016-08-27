using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telegram.Bot.Types;

namespace de.yarmolaev.TelegramCommandBot.Controller
{
    class BasicController
    {
        #region Definitions
        #region Fields
        private RichTextBox OutputTextbox;
        private MainWindow MainWindow;
        private string Username;
        private string BotId;
        #endregion

        #region Colors
        static BrushConverter converter = new System.Windows.Media.BrushConverter();
        Brush BrushRequest = (Brush)converter.ConvertFromString("#FF006606");
        Brush BrushResponse = (Brush)converter.ConvertFromString("#FF00287F");
        Brush BrushInfo = (Brush)converter.ConvertFromString("#FFAAAAAA");
        Brush BrushWarning = (Brush)converter.ConvertFromString("#FFCC9000");
        Brush BrushError = (Brush)converter.ConvertFromString("#FF990000");
        #endregion

        #region Messages
        string MessageSettingDisabled = "This function is disabled in the settings. Please check to be able to use.";
        #endregion

        #region Delegates
        public delegate void AppendLineCallback(string text, Brush brush, TextAlignment textAlignment);
        #endregion
        #region Instance
        private static BasicController Controller { get; set; }
        #endregion
        #endregion

        #region Methods

        #region Controller operations
        /// <summary>
        /// Constructor for BasicController
        /// </summary>
        /// <param name="mainWindow">MainWindow needed to call methods defined in it.</param>
        /// <param name="botId">Bot ID needed to start bot</param>
        /// <param name="username">Username needed to start bot</param>
        private BasicController(MainWindow mainWindow, string botId, string username)
        {
            #region Setting fields
            MainWindow = mainWindow;
            OutputTextbox = mainWindow.tb_Output;

            Username = username;
            BotId = botId;
            #endregion
        }

        /// <summary>
        /// Returns the instance of BasicController
        /// </summary>
        /// <param name="mainWindow">MainWindow needed to call methods defined in it.</param>
        /// <param name="botId">Bot ID needed to start bot</param>
        /// <param name="username">Username needed to start bot</param>
        /// <returns></returns>
        public static BasicController GetInstance(MainWindow mainWindow, string botId, string username)
        {
            if (Controller == null)
                Controller = new BasicController(mainWindow, botId, username);
            return Controller;
        }

        /// <summary>
        /// Easy way to get telegram bot controller cnstance
        /// </summary>
        /// <returns></returns>
        private TelegramBotController GetTelegramBotControllerInstance()
        {
            return TelegramBotController.GetInstance(MainWindow, BotId, Username);
        }

        /// <summary>
        /// Easy way to get cmd controller instance
        /// </summary>
        /// <returns></returns>
        private CmdController GetCmdControllerInstance()
        {
            return CmdController.GetInstance();
        }

        /// <summary>
        /// Resets all controller to get a clean restart
        /// </summary>
        public static void ResetController()
        {
            CmdController.ResetController();
            TelegramBotController.ResetController();
            Controller = null;
        }
        #endregion

        #region Bot communication
        /// <summary>
        /// Starts the Telegram Bot
        /// </summary>
        /// <returns></returns>
        public bool StartBot()
        {
            return GetTelegramBotControllerInstance().StartBot();
        }

        /// <summary>
        /// Stops the telegram bot
        /// </summary>
        /// <returns></returns>
        public bool StopBot()
        {
            return GetTelegramBotControllerInstance().StopBot();
        }

        /// <summary>
        /// Evaluates the given command
        /// </summary>
        /// <param name="command">Command to be runed</param>
        /// <param name="args">Arguments to specify the command</param>
        public async void EvaluateCommand(string command, string[] args)
        {

            switch (command.Replace("/", string.Empty).ToLower())
            {
                #region screenshot
                case "screenshot":
                    if (!Properties.Settings.Default.AllowScreenshotRequest)
                    {
                        GetTelegramBotControllerInstance().SendMessage(MessageSettingDisabled);
                        return;
                    }
                    GetTelegramBotControllerInstance().SendMessage("Screenshot gets rendered...");
                    string screenshotPath = ScreenshotController.GetScreenshot();
                    FileToSend fts = new FileToSend();
                    if (Properties.Settings.Default.SendScreenshotFile)
                        using (var stream = System.IO.File.Open(screenshotPath, FileMode.Open))
                        {
                            fts.Content = stream;
                            fts.Filename = screenshotPath.Split('\\').Last();
                            try
                            {
                                await GetTelegramBotControllerInstance().Bot.SendDocumentAsync(GetTelegramBotControllerInstance().LastReceivedChatId, fts, "Screenshot Document (HQ)");
                            }
                            catch (Exception e)
                            {
                                AppendAsyncInfoLine(e.Message);
                            }
                        }

                    if (Properties.Settings.Default.SendScreenshotImage)
                        using (var stream = System.IO.File.Open(screenshotPath, FileMode.Open))
                        {
                            fts.Content = stream;
                            fts.Filename = screenshotPath.Split('\\').Last();
                            try
                            {
                                await GetTelegramBotControllerInstance().Bot.SendPhotoAsync(GetTelegramBotControllerInstance().LastReceivedChatId, fts, "Screenshot Image (LQ)");
                            }
                            catch (Exception e)
                            {
                                AppendAsyncInfoLine(e.Message);
                            }
                        }

                    break;
                #endregion
                #region cmd
                case "cmd":
                    if (!Properties.Settings.Default.AllowCmdRequest)
                    {
                        GetTelegramBotControllerInstance().SendMessage(MessageSettingDisabled);
                        return;
                    }
                    if (args.Length < 1)
                    {
                        GetTelegramBotControllerInstance().SendMessage("Command cmd needs at least one parameter");
                        return;
                    }

                    string[] forbiddenExpressions = Properties.Settings.Default.ForbiddenExpressions.Split(';');

                    string expression = string.Join(" ", args);

                    if (!(forbiddenExpressions.Length == 1 && forbiddenExpressions[0] == "") && forbiddenExpressions.Any(s => expression.Trim().Contains(s)))
                    {
                        GetTelegramBotControllerInstance().SendMessage("Your expression contains forbidden characters");
                        return;
                    }

                    CmdController.GetInstance().RunCommand(expression);
                    break;
                #endregion
                #region file
                case "file":
                    if (!Properties.Settings.Default.AllowSendDocuments)
                    {
                        GetTelegramBotControllerInstance().SendMessage(MessageSettingDisabled);
                        return;
                    }
                    string path = string.Join(" ", args);
                    string[] forbiddenPaths = Properties.Settings.Default.ForbiddenPaths.Split(';');
                    if (!(forbiddenPaths.Length == 1 && forbiddenPaths[0] == "") && forbiddenPaths.Any(s => path.Trim().Contains(s)))
                    {
                        GetTelegramBotControllerInstance().SendMessage("Your parameter contains forbidden characters");
                        return;
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        GetTelegramBotControllerInstance().SendMessage($"No document found at '{path}'");
                    }
                    else
                    {
                        using (var stream = System.IO.File.Open(path, FileMode.Open))
                        {
                            fts.Content = stream;
                            fts.Filename = path.Split('\\').Last();
                            try
                            {
                                await GetTelegramBotControllerInstance().Bot.SendDocumentAsync(GetTelegramBotControllerInstance().LastReceivedChatId, fts);
                            }
                            catch (Exception e)
                            {
                                AppendAsyncInfoLine(e.Message);
                            }
                        }
                    }

                    break;
                #endregion

                default:
                    GetTelegramBotControllerInstance().SendMessage($"Command {command} cound not be interpreted.");
                    break;
            }
        }

        /// <summary>
        /// Sends a message to the user
        /// </summary>
        /// <param name="text"></param>
        public void SendMessage(string text)
        {
            GetTelegramBotControllerInstance().SendMessage(text);
        }

        #region Append<definition>Line

        /// <summary>
        /// Appends a line to the Outputtextbox in gray color.
        /// </summary>
        /// <param name="text">Line to be appended</param>
        public void AppendAsyncInfoLine(String text)
        {
            MainWindow.AppendResultLine(text, BrushInfo, TextAlignment.Center);
        }

        /// <summary>
        /// Appends a line to the Outputtextbox in blue color.
        /// </summary>
        /// <param name="text">Line to be appended</param>
        public void AppendAsyncResultLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendLineCallback(MainWindow.AppendResultLine), text, BrushResponse, TextAlignment.Right);
        }

        /// <summary>
        /// Appends a line to the Outputtextbox in green color.
        /// </summary>
        /// <param name="text">Line to be appended</param>
        public void AppendAsyncRequestLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendLineCallback(MainWindow.AppendResultLine), text, BrushRequest, TextAlignment.Left);
        }

        /// <summary>
        /// Appends a line to the Outputtextbox in orange color.
        /// </summary>
        /// <param name="text">Line to be appended</param>
        public void AppendAsyncWarningLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendLineCallback(MainWindow.AppendResultLine), text, BrushWarning, TextAlignment.Right);
        }

        /// <summary>
        /// Appends a line to the Outputtextbox in red color.
        /// </summary>
        /// <param name="text">Line to be appended</param>
        public void AppendAsyncErrorLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendLineCallback(MainWindow.AppendResultLine), $"{text}\r\nAll Controller will be reset.", BrushError, TextAlignment.Right);
            ResetController();
        }
        #endregion
        #endregion

        #endregion

    }
}
