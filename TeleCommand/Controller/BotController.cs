using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace de.yarmolaev.TelegramCommandBot.Controller
{
    class BotController
    {
        enum Commands
        {
            cmd,
            screenshot
        }

        private TelegramBotClient Bot;
        private string Username;
        private long LastReceivedChatId;
        private RichTextBox OutputTextbox;
        private MainWindow MainWindow;

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

        public bool StartBot(string username, string botId, RichTextBox tb, MainWindow mainWindow)
        {
            OutputTextbox = tb;
            MainWindow = mainWindow;
            try
            {
                //AppendAsyncInfoLine("Starting ...");
                Bot = new TelegramBotClient(botId);

                Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
                Bot.OnMessage += BotOnMessageReceived;
                Bot.OnMessageEdited += BotOnMessageReceived;
                Bot.OnInlineQuery += BotOnInlineQueryReceived;
                Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
                Bot.OnReceiveError += BotOnReceiveError;

                Bot.StartReceiving();

                AppendAsyncInfoLine("Bot started successfully");

                var me = Bot.GetMeAsync().Result;


                Username = username;

                return true;
            }
            catch (Exception e)
            {
                return false;
                //AppendAsyncInfoLine("Failed to start bot!");
            }
        }

        

        private void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        public bool StopBot()
        {
            if (Bot != null)
            {
                Bot.StopReceiving();
                Bot = null;
                AppendAsyncInfoLine("Bot stopped successfully");
                return true;
            }
            AppendAsyncInfoLine("Error ocured while stopping bot.");
            return false;
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            if (CheckUser(e.Message.From.Username))
            {
                LastReceivedChatId = e.Message.Chat.Id;
                this.AppendAsyncResultLine(e.Message.Text);
                EvaluateMessage(e.Message.Text);
            }
            else
            {
                string message = "You have no power here";
                SendMessage(message);
                LastReceivedChatId = e.Message.Chat.Id;
            }
        }

        private void SendMessage(String message)
        {
            Bot.SendTextMessageAsync(LastReceivedChatId, message);
            AppendAsyncRequestLine(message);
        }


        private bool CheckUser(string username)
        {
            return username.Equals(Username, StringComparison.OrdinalIgnoreCase);
        }

        private void AppendAsyncInfoLine(String text)
        {
            MainWindow.AppendResultLine(text, BrushInfo, TextAlignment.Center);
        }

        private void AppendAsyncResultLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendLineCallback(MainWindow.AppendResultLine), text, BrushResponse, TextAlignment.Left);
        }

        private void AppendAsyncRequestLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendLineCallback(MainWindow.AppendResultLine), text, BrushRequest, TextAlignment.Right);
        }

        private void AppendAsyncWarningLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendLineCallback(MainWindow.AppendResultLine), text, BrushWarning, TextAlignment.Left);
        }

        private void EvaluateMessage(string message)
        {
            if (!message.ToCharArray()[0].Equals('/'))
            {
                SendMessage($"Your message '{message}' was not recognized as a command. '/' is missing. ");
                return;
            }
            string command;
            string[] arguments = { };
            if (message.IndexOf(" ") < 0)
            {
                command = message;
            }
            else
            {
                string[] tmp = message.Split(' ');
                command = tmp[0];
                arguments = new string[(command.Length - 1)];
                for (int i = 1; i < tmp.Length; i++)
                {
                    arguments[i + 1] = tmp[i];
                }
            }

            EvaluateCommand(command, arguments);
        }


        private async void EvaluateCommand(string command, string[] args)
        {

            switch (command.Replace("/", string.Empty).ToLower())
            {
                case "screenshot":
                    if (!Properties.Settings.Default.AllowScreenshotRequest)
                    {
                        SendMessage(MessageSettingDisabled);
                        return;
                    }
                    SendMessage("Screenshot gets rendered...");
                    string screenshotPath = ScreenshotController.GetScreenshot();
                    FileToSend fts = new FileToSend();
                    if (Properties.Settings.Default.SendScreenshotFile)
                        using (var stream = System.IO.File.Open(screenshotPath, FileMode.Open))
                        {
                            fts.Content = stream;
                            fts.Filename = screenshotPath.Split('\\').Last();
                            //await Bot.SendPhotoAsync(this.LastReceivedChatId, fts, "My Text");
                            try
                            {
                                await Bot.SendDocumentAsync(this.LastReceivedChatId, fts, "My Text");
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
                                await Bot.SendPhotoAsync(this.LastReceivedChatId, fts, "My Text");
                                //    await Bot.SendDocumentAsync(this.LastReceivedChatId, fts, "My Text");
                            }
                            catch (Exception e)
                            {
                                AppendAsyncInfoLine(e.Message);
                            }
                        }

                    break;
                default:
                    SendMessage($"Command {command} cound not be interpreted");
                    break;
            }
        }

    }
}
