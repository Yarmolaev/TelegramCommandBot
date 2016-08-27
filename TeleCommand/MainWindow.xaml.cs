using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using de.yarmolaev.TelegramCommandBot.Controller;

namespace de.yarmolaev.TelegramCommandBot
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



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
            //BasicController.GetInstance(this, tb_Username.Text, tb_Bot_ID.Text);
            //this.BotController = new BotController();
            //if(this.BotController.StartBot())
            if (BasicController.GetInstance(this, tb_Username.Text, tb_Bot_ID.Text).StartBot())
            {
                StartStopBotUI(true);
            }
            /*else
            {
                StartStopBotUI(false);
            }

            Properties.Settings.Default.Save();*/

        }

        /// <summary>
        /// Stopping the Bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            if (BasicController.GetInstance(this, tb_Username.Text, tb_Bot_ID.Text).StopBot())
            {
                StartStopBotUI(false);
            }
        }

        private void StartStopBotUI(bool botStarted)
        {
            btn_start.IsEnabled = !botStarted;
            btn_stop.IsEnabled = botStarted;
            tb_Bot_ID.IsEnabled = !botStarted;
            tb_Username.IsEnabled = !botStarted;
        }



        public void AppendResultLine(string text, Brush color, TextAlignment textAlignment)
        {
            Run r = new Run(text);
            r.Foreground = color;
            Paragraph p = new Paragraph(r);
            p.TextAlignment = textAlignment;
            tb_Output.Document.Blocks.Add(p);

            tb_Output.ScrollToEnd();

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow s = new SettingsWindow();
            s.ShowDialog();
        }
    }
}
