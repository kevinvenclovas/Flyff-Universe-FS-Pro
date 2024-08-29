using CefSharp.Wpf;
using DeviceId;
using FlyffUAutoFSPro._Script.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FlyffUAutoFSPro._Script
{
    public static class Utils
    {
        public static double GetWindowsScale()
        {
            return (100 * System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth) / 100f;
        }
        public static bool IsNumberInRange(int number, int must, int range)
        {
            int min = must - range;
            int max = must + range;
            return (number <= max && number >= min);
        }

        public static bool IsNumberInRange(double number, double must, double range)
        {
            double min = must - range;
            double max = must + range;
            return (number <= max && number >= min);
        }

        public static Task<Bitmap> TakeScreenshotAsync(this ChromiumWebBrowser source)
        {
            var tcs = new TaskCompletionSource<Bitmap>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            source.Paint += ChromiumWebBrowser_Paint;
            return tcs.Task;

            void ChromiumWebBrowser_Paint(object sender, PaintEventArgs e)
            {
                source.Paint -= ChromiumWebBrowser_Paint;
                using (var temp = new Bitmap(e.Width, e.Height, 4 * e.Width,
                    PixelFormat.Format32bppRgb, e.Buffer))
                {
                    tcs.SetResult(new Bitmap(temp));
                }
            }
        }

        public static void SetKeyValueToCombobox<T,I>(this ComboBox box, List<KeyValuePair<T, I>> values, int selectedIndex)
        {
            box.SelectedValuePath = "Key";
            box.DisplayMemberPath = "Value";

            foreach (var v in values)
            {
                box.Dispatcher.Invoke(() => box.Items.Add(v));
            }

            box.Dispatcher.Invoke(() => box.SelectedIndex = selectedIndex);
        }

        public static void SetMotionsToCombobox(this ComboBox box, List<Motion> motions, int selectedId)
        {
            foreach (var m in motions)
            {
                var cItem = new ImageComboboxItem()
                {
                    ID = m.Id,
                    ImageSource = m.Image.BitmapToImageSource()
                };

                box.Dispatcher.Invoke(() => box.Items.Add(cItem));
                if(selectedId == m.Id)
                {
                    box.Dispatcher.Invoke(() => box.SelectedItem = cItem);
                }
            }

        }

        public static BitmapImage BitmapToImageSource(this Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, bitmap.RawFormat);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private static byte[] ToByteArray(this Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
       
        public static string BitmapToSVG(this Bitmap image)
        {

            List<List<Color>> PixelColors = new List<List<Color>>();

            int imwid = 0;
            int imhei = 0;
            using (var ms = new MemoryStream(image.ToByteArray()))
            {
                Bitmap mypic = new Bitmap(ms);

                imwid = mypic.Width;
                imhei = mypic.Height;

                for (int w = 0; w < imwid; w++)
                {
                    List<Color> Row = new List<Color>();
                    for (int h = 0; h < imhei; h++)
                    {
                        Color pixelColor = mypic.GetPixel(w, h);
                        Row.Add(pixelColor);
                    }

                    PixelColors.Add(Row);
                }

            }

            StringBuilder sb = new StringBuilder();

            sb.Append($"<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink= \"http://www.w3.org/1999/xlink\" width=\"{imwid}\" height=\"{imhei}\" viewBox =\"0 0 {imwid} {imhei}\">");

            for (int z = 0; z < PixelColors.Count(); z++)
            {
                for (int i = 0; i < PixelColors[z].Count(); i++)
                {
                    if (PixelColors[z][i].A == 255)
                        sb.Append($"<rect shape-rendering=\"crispEdges\" fill=\"rgb({PixelColors[z][i].R}, {PixelColors[z][i].G}, {PixelColors[z][i].B})\"  x=\"{z.ToString().Replace(',', '.')}\" y=\"{i.ToString().Replace(',', '.')}\" width=\"1\" height=\"1\" />");
                }
            }

            sb.Append($"</svg>");
            return sb.ToString();

        }

        public static string CreateMD5(this string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string GetDeviceId()
        {
            return new DeviceIdBuilder()
               .AddMachineName()
               .AddOsVersion()
               .OnWindows(windows => windows
                   .AddProcessorId()
                   .AddMotherboardSerialNumber()
                   .AddSystemDriveSerialNumber())
               .ToString();
        }
   

        public static float GetRectangleMiddleX(this Rectangle rect)
        {
            return rect.X + (rect.Width / 2f);
        }

        public static float GetRectangleMiddleY(this Rectangle rect)
        {
            return rect.Y + (rect.Height / 2f);
        }

        public static Vector2 CalculateCenterSquare(Rectangle leftBottom, Rectangle rightBottom, Rectangle rightTop, Rectangle leftTop)
        {
            // Calculate with 2 SideBySide
            Vector2 center = Vector2.Zero;

            if (leftBottom != Rectangle.Empty && rightBottom != Rectangle.Empty)
            {
                var x = (GetRectangleMiddleX(rightBottom) + GetRectangleMiddleX(leftBottom)) / 2f;
                var y = ((GetRectangleMiddleY(rightBottom) + GetRectangleMiddleY(leftBottom)) / 2f) - ((GetRectangleMiddleX(rightBottom) - GetRectangleMiddleX(leftBottom)) / 2f);
                return new Vector2(x, y);
            }
            else if (rightBottom != Rectangle.Empty && rightTop != Rectangle.Empty)
            {
                var y = (GetRectangleMiddleY(rightBottom) + GetRectangleMiddleY(rightTop)) / 2f;
                var x = ((GetRectangleMiddleX(rightBottom) + GetRectangleMiddleX(rightTop)) / 2f) - ((rightBottom.Bottom - rightTop.Top) / 2f);
                return new Vector2(x, y);
            }
            else if (rightTop != Rectangle.Empty && leftTop != Rectangle.Empty)
            {
                var x = (GetRectangleMiddleX(rightTop) + GetRectangleMiddleX(leftTop)) / 2f;
                var y = ((GetRectangleMiddleY(rightTop) + GetRectangleMiddleY(leftTop)) / 2f) + ((GetRectangleMiddleX(rightTop) - GetRectangleMiddleX(leftTop)) / 2f);
                return new Vector2(x, y);
            }
            else if (leftTop != Rectangle.Empty && leftBottom != Rectangle.Empty)
            {
                var y = (GetRectangleMiddleY(leftBottom) + GetRectangleMiddleY(leftTop)) / 2f;
                var x = ((GetRectangleMiddleX(leftBottom) + GetRectangleMiddleX(leftTop)) / 2f) + ((GetRectangleMiddleY(leftBottom) - GetRectangleMiddleY(leftTop)) / 2f);
                return new Vector2(x, y);
            }
            
            // Calculate with 2 Diagonal
            if (leftBottom != Rectangle.Empty && rightTop != Rectangle.Empty)
            {
                var x = (GetRectangleMiddleX(rightTop) + GetRectangleMiddleX(leftBottom)) / 2f;
                var y = ((GetRectangleMiddleY(leftBottom) + GetRectangleMiddleY(rightTop)) / 2f);
                return new Vector2(x, y);
            }
            else if (rightBottom != Rectangle.Empty && leftTop != Rectangle.Empty)
            {
                var x = (GetRectangleMiddleX(rightBottom) + GetRectangleMiddleX(leftTop)) / 2f;
                var y = ((GetRectangleMiddleY(rightBottom) + GetRectangleMiddleY(leftTop)) / 2f);
                return new Vector2(x, y);
            }

            // Calculate with 1

            if (leftBottom != Rectangle.Empty)
            {
                return new Vector2(GetRectangleMiddleX(leftBottom), GetRectangleMiddleY(leftBottom));
            }
            else if (rightBottom != Rectangle.Empty)
            {
                return new Vector2(GetRectangleMiddleX(rightBottom), GetRectangleMiddleY(rightBottom));
            }
            else if (rightTop != Rectangle.Empty)
            {
                return new Vector2(GetRectangleMiddleX(rightTop), GetRectangleMiddleY(rightTop));
            }
            else if (leftTop != Rectangle.Empty)
            {
                return new Vector2(GetRectangleMiddleX(leftTop), GetRectangleMiddleY(leftTop));
            }

            return Vector2.Zero;
        }


        public static T CombineIntEnums<T>(this IEnumerable<T> values) where T : Enum
        {
            int? result = null;

            foreach (var enumVal in values)
            {
                // convert enum to int
                var intVal = Convert.ToInt32(enumVal);

                if (result.HasValue == false)
                    result = intVal;

                result |= intVal;
            }

            // convert int to enum
            var val = (T)Enum.ToObject(typeof(T), result ?? 0);

            return val;
        }


        /// <summary>Returns true if the current application has focus, false otherwise</summary>
        public static bool ApplicationIsOnFocus()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
    }
}
