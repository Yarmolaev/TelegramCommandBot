using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace de.yarmolaev.TelegramCommandBot.Controller
{
    class ScreenshotController
    {
        public static string GetScreenshot()
        {
                // Determine the size of the "virtual screen", which includes all monitors.
                int screenLeft = SystemInformation.VirtualScreen.Left;
                int screenTop = SystemInformation.VirtualScreen.Top;
                int screenWidth = SystemInformation.VirtualScreen.Width;
                int screenHeight = SystemInformation.VirtualScreen.Height;

                // Create a bitmap of the appropriate size to receive the screenshot.
                using (Bitmap bmp = new Bitmap(screenWidth, screenHeight))
                {
                    // Draw the screenshot into our bitmap.
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);
                    }

                // Do something with the Bitmap here, like save it to a file:
                return SaveImage(bmp);
                }
        }

        private static string SaveImage(Bitmap bmp)
        {
            DateTime now = DateTime.Now;
            string filename = now.ToString("yyyy_MM_dd_HH_mm_ss")+".jpg";
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TCB");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, filename);

            try
            {
                bmp.Save(path, ImageFormat.Jpeg);
                return path;
            }catch(Exception e)
            {
                return null;
            }
        }
    }
}
