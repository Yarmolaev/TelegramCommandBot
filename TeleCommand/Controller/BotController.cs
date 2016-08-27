using System;
using System.Collections.Generic;
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

        DateTime NextMessageAt;
        List<string> WaitingMessages;

        public string Username;
        public long LastReceivedChatId;

        public TelegramBotClient Bot;

        MainWindow MainWindow;

        private static BotController Controller;
        private string BotId;

        private BotController(string username, string botId, MainWindow mainWindow)
        {
            Username = username;
            this.BotId = botId;
            MainWindow = mainWindow;
        }

        public static BotController GetInstance(string username, string botId, MainWindow mainWindow)
        {
            if (Controller == null)
                Controller = new BotController(username, botId, mainWindow);
            return Controller;
        }

        public bool StartBot()
        {

            try
            {
                Bot = new TelegramBotClient(BotId);

                Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
                Bot.OnMessage += BotOnMessageReceived;
                Bot.OnMessageEdited += BotOnMessageReceived;
                Bot.OnInlineQuery += BotOnInlineQueryReceived;
                Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
                Bot.OnReceiveError += BotOnReceiveError;

                Bot.StartReceiving();

                BasicController.GetInstance(MainWindow, Username, BotId).AppendAsyncInfoLine("Bot started successfully");

                var me = Bot.GetMeAsync().Result;



                return true;
            }
            catch (Exception e)
            {
                return false;
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
                BasicController.GetInstance(MainWindow, Username, BotId).AppendAsyncInfoLine("Bot stopped successfully");
                return true;
            }
            BasicController.GetInstance(MainWindow, Username, BotId).AppendAsyncInfoLine("Error ocured while stopping bot.");
            return false;
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            SendMessage(e.ToString());
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
                BasicController.GetInstance(MainWindow, Username, BotId).AppendAsyncResultLine(e.Message.Text);
                EvaluateMessage(e.Message.Text);
            }
            else
            {
                string message = "You have no power here";
                SendMessage(message);
                LastReceivedChatId = e.Message.Chat.Id;
            }
        }

        public void SendMessage(String message)
        {
            WaitingMessages.Add(message);
            if (NextMessageAt == null || DateTime.Now > NextMessageAt)
            {
                //Message can be sent
                string completeMessage = string.Join("\r\n", WaitingMessages.ToArray());
                Bot.SendTextMessageAsync(LastReceivedChatId, completeMessage);
                BasicController.GetInstance(MainWindow, Username, BotId).AppendAsyncRequestLine(completeMessage);

                NextMessageAt = DateTime.Now.AddSeconds(2d);
            }
        }


        public bool CheckUser(string username)
        {
            return username.Equals(Username, StringComparison.OrdinalIgnoreCase);
        }



        public void EvaluateMessage(string message)
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
                arguments = new string[(tmp.Length - 1)];
                for (int i = 1; i < tmp.Length; i++)
                {
                    arguments[i - 1] = tmp[i];
                }
            }

            BasicController.GetInstance(MainWindow, Username, BotId).EvaluateCommand(command, arguments, this);
        }






    }
}
