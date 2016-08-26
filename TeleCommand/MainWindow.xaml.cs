using System;
using System.Windows;
using System.Windows.Documents;
using TeleCommand.Controller;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TeleCommand
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        BotController BotController;
           
        //private readonly static int apiId = 88462;
        //private readonly static string apiHash = "e28d2b32dec3f3246894e87eb4b858c3";
        public MainWindow()
        {
            InitializeComponent();
        }

        

        

        

        

        /// <summary>
        /// Starting the bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            this.BotController = new BotController();
            this.BotController.StartBot(tb_Username.Text, tb_Bot_ID.Text, tb_Output, this);

            /*try
            {
                AppendAsyncInfoLine("Starting ...");
                Bot = new TelegramBotClient(tb_Bot_ID.Text);

                Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
                Bot.OnMessage += BotOnMessageReceived;
                Bot.OnMessageEdited += BotOnMessageReceived;
                Bot.OnInlineQuery += BotOnInlineQueryReceived;
                Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
                Bot.OnReceiveError += BotOnReceiveError;

                Bot.StartReceiving();

                AppendAsyncInfoLine("Bot started successfully");

                var me = Bot.GetMeAsync().Result;

                btn_start.IsEnabled = false;
                btn_stop.IsEnabled = true;
                tb_Bot_ID.IsEnabled = false;
                tb_Username.IsEnabled = false;
                Username = tb_Username.Text;
            }
            catch
            {
                AppendAsyncInfoLine("Failed to start bot!");
            }*/

        }

        /// <summary>
        /// Stopping the Bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            /*if (Bot != null)
            {
                Bot.StopReceiving();
                Bot = null;
                AppendResultLine("Bot stopped successfully");
            }
            else
            {
                AppendResultLine("Bot is not running");
            }
            btn_stop.IsEnabled = false;
            btn_start.IsEnabled = true;
            tb_Username.IsEnabled = true;
            tb_Bot_ID.IsEnabled = true;*/
            BotController.StopBot();
        }

        

        public void AppendResultLine(string text)
        {
            string breakLine = "\r\n";
            try
            {
                if (tb_Output.Document.Blocks != null && tb_Output.Document.Blocks.Count > 0)
                    tb_Output.AppendText(breakLine);
            }
            catch (Exception)
            {
            }
            //tb_Output.AppendText($"{text}");
            tb_Output.Document.Blocks.Add(new Paragraph(new Run(text)));

            tb_Output.Focus();
            //tb_Output.CaretIndex = tb_Output.Text.Length;
            tb_Output.ScrollToEnd();

        }


    }
}
