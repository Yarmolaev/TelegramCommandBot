using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using TeleCommand.Controller;

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

            Properties.Settings.Default.Save();

        }

        /// <summary>
        /// Stopping the Bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            BotController.StopBot();
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
