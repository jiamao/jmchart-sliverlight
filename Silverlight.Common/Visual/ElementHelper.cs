using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Silverlight.Common.Visual
{
    public static class ElementHelper
    {
        /// <summary>
        /// 获取控件的图片
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.WriteableBitmap GetImageFromElement(UIElement el)
        {
            return new System.Windows.Media.Imaging.WriteableBitmap(el, null);
        }

        /// <summary>
        /// 把控件保存为PNG图片
        /// </summary>
        /// <param name="el"></param>
        public static void SaveElementToPNG(UIElement el)
        {
            try
            {
                //var sfo = new SaveFileDialog();
                //sfo.Filter = "png|*.png";

                //if (sfo.ShowDialog() == true)
                //{
                //    var img = GetImageFromElement(el);

                //    var imageData = new EditableImage(bitmap.PixelWidth, bitmap.PixelHeight);
                //    try
                //    {
                //        for (int y = 0; y < bitmap.PixelHeight; ++y)
                //        {
                //            for (int x = 0; x < bitmap.PixelWidth; ++x)
                //            {
                //                int pixel = bitmap.Pixels[bitmap.PixelWidth * y + x];
                //                imageData.SetPixel(x, y,
                //                (byte)((pixel >> 16) & 0xFF),
                //                (byte)((pixel >> 8) & 0xFF),
                //                (byte)(pixel & 0xFF), (byte)((pixel >> 24) & 0xFF)
                //                );
                //            }
                //        }
                //    }
                //    catch (System.Security.SecurityException)
                //    {
                //        throw new Exception("Cannot print images from other domains");
                //    } 


                //    using (var stream = sfo.OpenFile())
                //    {
                //        encoder.Save(stream);
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
