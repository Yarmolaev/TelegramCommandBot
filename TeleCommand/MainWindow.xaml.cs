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
            

        }

        /// <summary>
        /// Stopping the Bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Fit the UI to current bot state
        /// </summary>
        /// <param name="botStarted"></param>
        private void StartStopBotUI(bool botStarted)
        {
            //btn_start.IsEnabled = !botStarted;
            //btn_stop.IsEnabled = botStarted;
            if (botStarted)
                cmd_start_stop.Content = "Stop";
            else
                cmd_start_stop.Content = "Start";
            tb_Bot_ID.IsEnabled = !botStarted;
            tb_Username.IsEnabled = !botStarted;
        }


        /// <summary>
        /// Appends given text to Result TextBox
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="textAlignment"></param>
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

        private void cmd_start_stop_click(object sender, RoutedEventArgs e)
        {
            if(cmd_start_stop.Content.Equals("Start"))
            {
                if (BasicController.GetInstance(this, tb_Bot_ID.Text, tb_Username.Text).StartBot())
                {
                    StartStopBotUI(true);
                }
                else
                {
                    StartStopBotUI(false);
                }

                Properties.Settings.Default.Save();
            }else
            {
                if (BasicController.GetInstance(this, tb_Username.Text, tb_Bot_ID.Text).StopBot())
                {
                    StartStopBotUI(false);
                }
            }
        }
    }
}
