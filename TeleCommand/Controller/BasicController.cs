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

        private RichTextBox OutputTextbox;
        private MainWindow MainWindow;

        private string Username;
        private string BotId;


        //Colors
        static BrushConverter converter = new System.Windows.Media.BrushConverter();
        Brush BrushRequest = (Brush)converter.ConvertFromString("#FF006606");
        Brush BrushResponse = (Brush)converter.ConvertFromString("#FF00287F");
        Brush BrushInfo = (Brush)converter.ConvertFromString("#FFAAAAAA");
        Brush BrushWarning = (Brush)converter.ConvertFromString("#FFCC9000");
        Brush BrushError = (Brush)converter.ConvertFromString("#FF990000");


        //Messages
        string MessageSettingDisabled = "This function is disabled in the settings. Please check to be able to use.";


        public delegate void AppendLineCallback(string text, Brush brush, TextAlignment textAlignment);

        private static BasicController Controller { get; set; }

        private BasicController(MainWindow mainWindow, string username, string botId)
        {
            MainWindow = mainWindow;
            OutputTextbox = mainWindow.tb_Output;

            Username = username;
            BotId = botId;
        }

        public bool StartBot()
        {
            return BotController.GetInstance(Username, BotId, MainWindow).StartBot();
        }

        public bool StopBot()
        {
            return BotController.GetInstance(Username, BotId, MainWindow).StopBot();
        }

        public static BasicController GetInstance(MainWindow mainWindow, string username, string botId)
        {
            if (Controller == null)
                Controller = new BasicController(mainWindow, username, botId);
            return Controller;
        }

        public void AppendAsyncInfoLine(String text)
        {
            MainWindow.AppendResultLine(text, BrushInfo, TextAlignment.Center);
        }

        public void AppendAsyncResultLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendLineCallback(MainWindow.AppendResultLine), text, BrushResponse, TextAlignment.Right);
        }

        public void AppendAsyncRequestLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendLineCallback(MainWindow.AppendResultLine), text, BrushRequest, TextAlignment.Left);
        }

        public void AppendAsyncWarningLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendLineCallback(MainWindow.AppendResultLine), text, BrushWarning, TextAlignment.Right);
        }

        public async void EvaluateCommand(string command, string[] args, BotController botController)
        {

            switch (command.Replace("/", string.Empty).ToLower())
            {
                #region Screenshot
                case "screenshot":
                    if (!Properties.Settings.Default.AllowScreenshotRequest)
                    {
                        botController.SendMessage(MessageSettingDisabled);
                        return;
                    }
                    BotController.GetInstance(Username, BotId, MainWindow).SendMessage("Screenshot gets rendered...");
                    string screenshotPath = ScreenshotController.GetScreenshot();
                    FileToSend fts = new FileToSend();
                    if (Properties.Settings.Default.SendScreenshotFile)
                        using (var stream = System.IO.File.Open(screenshotPath, FileMode.Open))
                        {
                            fts.Content = stream;
                            fts.Filename = screenshotPath.Split('\\').Last();
                            try
                            {
                                await BotController.GetInstance(Username, BotId, MainWindow).Bot.SendDocumentAsync(BotController.GetInstance(Username, BotId, MainWindow).LastReceivedChatId, fts, "Screenshot Document (HQ)");
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
                                await BotController.GetInstance(Username, BotId, MainWindow).Bot.SendPhotoAsync(BotController.GetInstance(Username, BotId, MainWindow).LastReceivedChatId, fts, "Screenshot Image (LQ)");
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
                        botController.SendMessage(MessageSettingDisabled);
                        return;
                    }
                    if (args.Length < 1)
                    {
                        BotController.GetInstance(Username, BotId, MainWindow).SendMessage("Command cmd needs at least one parameter");
                        return;
                    }

                    string[] forbiddenExpressions = Properties.Settings.Default.ForbiddenExpressions.Split(';');

                    string expression = string.Join(" ", args);

                    if (forbiddenExpressions.Any(s => expression.Trim().Contains(s)))
                    {
                        botController.SendMessage("Your expression contains forbidden characters");
                        return;
                    }

                    CmdController.GetInstance().RunCommand(expression);
                    break;
                #endregion
                #region file
                case "file":
                    if (!Properties.Settings.Default.AllowSendDocuments)
                    {
                        botController.SendMessage(MessageSettingDisabled);
                        return;
                    }
                    string path = string.Join(" ", args);
                    string[] forbiddenPaths = Properties.Settings.Default.ForbiddenPaths.Split(';');
                    if(forbiddenPaths.Any(s => path.Trim().Contains(s)))
                    {
                        botController.SendMessage("Your parameter contains forbidden characters");
                        return;
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        BotController.GetInstance(Username, BotId, MainWindow).SendMessage($"No document found at '{path}'");
                    }
                    else
                    {
                        using (var stream = System.IO.File.Open(path, FileMode.Open))
                        {
                            fts.Content = stream;
                            fts.Filename = path.Split('\\').Last();
                            try
                            {
                                await BotController.GetInstance(Username, BotId, MainWindow).Bot.SendDocumentAsync(BotController.GetInstance(Username, BotId, MainWindow).LastReceivedChatId, fts);
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
                    BotController.GetInstance(Username, BotId, MainWindow).SendMessage($"Command {command} cound not be interpreted");
                    break;
            }
        }

        public void SendMessage(string text)
        {
            BotController.GetInstance(Username, BotId, MainWindow).SendMessage(text);
        }

    }
}
