﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using Svg;
using Image = System.Drawing.Image;

namespace c3IDE.Utilities.Helpers
{
    public class ImageHelper : Singleton<ImageHelper>
    {
        public Dictionary<string, BitmapImage> iconCache = new Dictionary<string, BitmapImage>();
        public string ImageToBase64(Image img)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                var imgArray = ms.ToArray();
                return Convert.ToBase64String(imgArray);
            }
        }

        public string ImageToBase64(string imgPath)
        {
            var img = Image.FromFile(imgPath);
            return ImageToBase64(img);
        }

        public Image Base64ToImage(string base64)
        {
            var img = Image.FromStream(new MemoryStream(Convert.FromBase64String(base64)));
            return img;
        }

        public BitmapImage Base64ToBitmap(string base64)
        {
            try
            {
                var img = Base64ToImage(base64);
                var bmp = new Bitmap(img);

                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BitmapImage();
            }
        }

        public string BitmapImageToBase64(BitmapImage imageC)
        {
            var memStream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return Convert.ToBase64String(memStream.ToArray());
        }

        public byte[] BitmapImageToBytes(BitmapImage image)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                var data = ms.ToArray();
                return data;
            }
        }

        public SvgDocument SvgFromFile(string path)
        {
             var svgDocument = SvgDocument.Open(path);
            return svgDocument;           
        }

        public string SvgToBase64(SvgDocument svg)
        {
            using (var stream = new MemoryStream())
            {
                svg.Write(stream);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public SvgDocument SvgFromXml(string xml)
        {
            try
            {
                using (var xmlStream = new MemoryStream(Encoding.ASCII.GetBytes(xml)))
                {
                    xmlStream.Position = 0;
                    return SvgDocument.Open<SvgDocument>(xmlStream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new SvgDocument();
            }
        }

        public SvgDocument Base64ToSvg(string base64)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(new MemoryStream(Convert.FromBase64String(base64)));
            var svg = new SvgDocument();
            SvgDocument.Open(xmlDoc);
            return svg;
        }

        public BitmapImage XmlToBitmapImage(string xml)
        {
            if (iconCache.ContainsKey(xml)) return iconCache[xml];
            var bmp = SvgToBitmapImage(SvgFromXml(xml));
            iconCache[xml] = bmp;
            return bmp;
        }

        public BitmapImage SvgToBitmapImage(SvgDocument svg)
        {
            //var bmp = svg.Draw(150,150);
            try
            {
                var bmp = svg.Draw();

        
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BitmapImage();
            }
        }
    }
}
