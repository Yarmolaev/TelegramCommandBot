using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace de.yarmolaev.TelegramCommandBot.Controller
{
    class TelegramBotController
    {
        #region Instance
        private static TelegramBotController Controller;
        #endregion

        #region Fields
        public TelegramBotClient Bot;
        DateTime NextMessageAt;
        List<string> WaitingMessages;
        MainWindow MainWindow;
        public string Username;
        private string BotId;
        public long LastReceivedChatId;
        int count = 0;
        Timer timer;
        #endregion

        /// <summary>
        /// Contructor for the TelegramBotController
        /// </summary>
        /// <param name="mainWindow">MainWindow needed to call methods defined in it.</param>
        /// <param name="botId">Bot ID needed to start bot</param>
        /// <param name="username">Username needed to start bot</param>
        private TelegramBotController(MainWindow mainWindow, string botId, string username)
        {
            Username = username;
            BotId = botId;
            MainWindow = mainWindow;
        }

        /// <summary>
        /// Returns the instance of TelegramBotController
        /// </summary>
        /// <param name="mainWindow">MainWindow needed to call methods defined in it.</param>
        /// <param name="botId">Bot ID needed to start bot</param>
        /// <param name="username">Username needed to start bot</param>
        /// <returns></returns>
        public static TelegramBotController GetInstance(MainWindow mainWindow, string botId, string username)
        {
            if (Controller == null)
                Controller = new TelegramBotController(mainWindow, botId, username);
            return Controller;
        }

        /// <summary>
        /// Resets the Controller
        /// </summary>
        public static void ResetController()
        {
            Controller = null;
        }

        /// <summary>
        /// Starts the bot
        /// </summary>
        /// <returns></returns>
        public bool StartBot()
        {
            try
            {
                #region New bot instance
                Bot = new TelegramBotClient(BotId);
                #endregion

                #region Defining for bot result methods
                Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
                Bot.OnMessage += BotOnMessageReceived;
                Bot.OnMessageEdited += BotOnMessageReceived;
                Bot.OnInlineQuery += BotOnInlineQueryReceived;
                Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
                Bot.OnReceiveError += BotOnReceiveError;
                #endregion

                Bot.StartReceiving();

                GetBasicControllerInstance().AppendAsyncInfoLine("Bot started successfully");

                var me = Bot.GetMeAsync().Result;

                WaitingMessages = new List<string>();

                return true;
            }
            catch (Exception e)
            {
                GetBasicControllerInstance().AppendAsyncErrorLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets easy the BasicController instance
        /// </summary>
        /// <returns></returns>
        private BasicController GetBasicControllerInstance()
        {
            return BasicController.GetInstance(MainWindow, BotId, Username);
        }
        
        /// <summary>
        /// Stopps the bot
        /// </summary>
        /// <returns>True if successfull, false if not</returns>
        public bool StopBot()
        {
            if (Bot != null)
            {
                Bot.StopReceiving();
                Bot = null;
                GetBasicControllerInstance().AppendAsyncInfoLine("Bot stopped successfully");
                return true;
            }
            GetBasicControllerInstance().AppendAsyncInfoLine("Error ocured while stopping bot.");
            return false;
        }

        #region SimpleReceiver
        private void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            throw new NotImplementedException();
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
        #endregion

        /// <summary>
        /// Evaluates the received message and user rights
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            if (CheckUser(e.Message.From.Username))
            {
                LastReceivedChatId = e.Message.Chat.Id;
                GetBasicControllerInstance().AppendAsyncResultLine(e.Message.Text);
                EvaluateMessage(e.Message.Text);
            }
            else
            {
                string message = "You have no power here";
                SendMessage(message);
                LastReceivedChatId = e.Message.Chat.Id;
            }
        }

        /// <summary>
        /// Prepares a message to be sent
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(String message)
        {
            if (timer == null)
                Loopy(2);
            WaitingMessages.Add(message);
        }

        /// <summary>
        /// Sends all prepared messages after 2 seconds
        /// </summary>
        private void SendMessage()
        {
            if (NextMessageAt == null)
            {
                NextMessageAt = DateTime.Now.AddSeconds(2d);
            }
            if (DateTime.Now > NextMessageAt && WaitingMessages.Count > 0)
            {
                //Message can be sent
                string completeMessage = string.Join("\r\n", WaitingMessages.ToArray());
                try
                {
                    Bot.SendTextMessageAsync(LastReceivedChatId, completeMessage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                GetBasicControllerInstance().AppendAsyncRequestLine(completeMessage);

                NextMessageAt = DateTime.Now.AddSeconds(2d);
                WaitingMessages.Clear();
            }
            else if (WaitingMessages.Count == 0)
            {
                timer.Dispose();
            }
        }
        
        /// <summary>
        /// Waits 2 seconds in a loop
        /// </summary>
        /// <param name="times">loop times</param>
        void Loopy(int times)
        {
            count = times;
            timer = new Timer(2000);
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Disposed += new EventHandler(timer_Disposed);
            timer.Start();
        }

        /// <summary>
        /// Resets the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Disposed(object sender, EventArgs e)
        {
            timer = null;
        }

        /// <summary>
        /// Being called if timer is runed once
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendMessage();
        }

        /// <summary>
        /// Checks if user has the right to call commands
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool CheckUser(string username)
        {
            return username.Equals(Username, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Evaluates the message
        /// </summary>
        /// <param name="message"></param>
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

            GetBasicControllerInstance().EvaluateCommand(command, arguments);
        }

    }
}
