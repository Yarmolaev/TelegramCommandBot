using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TeleCommand.Controller
{
    class BotController
    {
        private TelegramBotClient Bot;
        private string Username;
        private long LastReceivedChatId;
        private RichTextBox OutputTextbox;
        private MainWindow MainWindow;

        public delegate void AppendResultLineCallback(string text);
        public delegate void AppendRequestLineCallback(string text);

        public bool StartBot(string username, string botId, RichTextBox tb, MainWindow mainWindow)
        {
            OutputTextbox = tb;
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

                //AppendAsyncInfoLine("Bot started successfully");

                var me = Bot.GetMeAsync().Result;

                /*btn_start.IsEnabled = false;
                btn_stop.IsEnabled = true;
                tb_Bot_ID.IsEnabled = false;
                tb_Username.IsEnabled = false;*/
                Username = username;
                MainWindow = mainWindow;

                return true;
            }
            catch
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
                return true;
            }
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
            //AppendAsyncResultLine(e.Message.Text);
            if (CheckUser(e.Message.From.Username))
            {
                //string message = "User Accepted";
                //sendMessage(message);
                LastReceivedChatId = e.Message.Chat.Id;
                this.OutputTextbox.Dispatcher.Invoke(new AppendRequestLineCallback(MainWindow.AppendResultLine), new object[] { e.Message.Text } );
                //TODO Meldung auswerten
            }
            else
            {
                string message = "You have no power here";
                sendMessage(message);
                LastReceivedChatId = e.Message.Chat.Id;
            }
        }

        private void sendMessage(String message)
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
            OutputTextbox.Dispatcher.Invoke(new AppendResultLineCallback(MainWindow.AppendResultLine), $"{text}");
        }

        private void AppendAsyncResultLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendResultLineCallback(MainWindow.AppendResultLine), $"Received:  {text}");
        }

        private void AppendAsyncRequestLine(String text)
        {
            OutputTextbox.Dispatcher.Invoke(new AppendResultLineCallback(MainWindow.AppendResultLine), $"Sent: \t   {text}");
        }
    }
}
