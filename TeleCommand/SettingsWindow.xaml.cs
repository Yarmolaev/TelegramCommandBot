using System.Windows;

namespace de.yarmolaev.TelegramCommandBot
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void cmd_save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void cmd_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmd_reset_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
        }
    }
}
