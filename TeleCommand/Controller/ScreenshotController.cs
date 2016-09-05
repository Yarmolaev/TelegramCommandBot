using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace de.yarmolaev.TelegramCommandBot.Controller
{
    class ScreenshotController
    {
        /// <summary>
        /// Makes a screenshot
        /// </summary>
        /// <returns>Path to the made screenshot</returns>
        public static string GetScreenshot()
        {
                int screenLeft = SystemInformation.VirtualScreen.Left;
                int screenTop = SystemInformation.VirtualScreen.Top;
                int screenWidth = SystemInformation.VirtualScreen.Width;
                int screenHeight = SystemInformation.VirtualScreen.Height;

                using (Bitmap bmp = new Bitmap(screenWidth, screenHeight))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);
                    }

                return SaveImage(bmp);
                }
        }

        /// <summary>
        /// Saves a binmap with current timestamp in mydocuments/TCB/<timestamp>.jpg
        /// </summary>
        /// <param name="bmp">Bitmap to be saved</param>
        /// <returns>Path to the saved image</returns>
        private static string SaveImage(Bitmap bmp)
        {
            DateTime now = DateTime.Now;
            string filename = now.ToString("yyyy_MM_dd_HH_mm_ss_fff")+".jpg";
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
