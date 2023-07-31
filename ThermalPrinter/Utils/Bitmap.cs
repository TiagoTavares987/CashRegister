using MessagingToolkit.QRCode.Codec;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

using ZXing;
using ZXing.Common;

namespace ThermalPrinter.Utils
{
    internal class Bitmap
    {
        public static byte[] GetBitmapBytes(System.Drawing.Bitmap bitmap)
        {
            try
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                int size = bitmapData.Stride * bitmapData.Height;
                byte[] data = new byte[size];
                System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, data, 0, size);
                List<float> pixelsBrightness = new List<float>();

                switch (bitmap.PixelFormat)
                {
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppPArgb:
                    case PixelFormat.Format32bppRgb:
                        for (int i = 0; i < size - 3; i += 4) // i += 32 / 8
                        {
                            int Blue = data[i];
                            int Green = data[i + 1];
                            int Red = data[i + 2];
                            int Alpha = data[i + 3];
                            pixelsBrightness.Add(Color.FromArgb(Alpha, Red, Green, Blue).GetBrightness());
                        }
                        break;
                    case PixelFormat.Format24bppRgb:
                        {
                            int strideRounding = 0;
                            int strideOffset = bitmapData.Stride - bitmap.Width * 3;
                            for (int i = 0; i + strideOffset < size; i += 3)
                            {
                                if (strideOffset > 0 && strideRounding != 0 && strideRounding % bitmap.Width == 0)
                                    i += strideOffset;

                                int Blue = data[i];
                                int Green = data[i + 1];
                                int Red = data[i + 2];
                                pixelsBrightness.Add(Color.FromArgb(Red, Green, Blue).GetBrightness());

                                if (strideOffset > 0)
                                    strideRounding++;
                            }
                        }
                        break;
                    case PixelFormat.Format8bppIndexed:
                        for (int i = 0; i < size; i++) // i += 8 / 8
                        {
                            int Grey = data[i];
                            pixelsBrightness.Add(Color.FromArgb(Grey, Grey, Grey).GetBrightness());
                        }
                        break;
                    default:
                        //Weird image format. BREAK. RETURN DON'T PROCESS
                        return null;
                }

                float brightness = pixelsBrightness.Sum() / pixelsBrightness.Count;

                #region test to txt
                //StringBuilder text = new StringBuilder();
                //for (int y = 0; y < bitmap.Height; ++y)
                //{
                //    for (int x = 0; x < bitmap.Width; ++x)
                //    {
                //        if (y * bitmap.Width + x >= pixelsBrightness.Count)
                //        {

                //        }
                //        else
                //        {
                //            if (pixelsBrightness[y * bitmap.Width + x] < brightness)
                //            {
                //                text.Append("X");
                //            }
                //            else
                //            {
                //                text.Append(" ");
                //            }
                //        }
                //    }
                //    text.AppendLine();
                //}
                //File.WriteAllText("teste.txt", text.ToString());
                #endregion

                List<byte> bytes = new List<byte>() { 0x1b, 0x33, 0x24 };
                for (int line = 0; line <= bitmap.Height / 24; ++line)
                {
                    bytes.Add(0x1b);
                    bytes.Add(0x2a);
                    bytes.Add(0x21);
                    bytes.Add((byte)((bitmap.Width >> 0) & 0xff));
                    bytes.Add((byte)((bitmap.Width >> 8) & 0x03));
                    for (int x = 0; x < bitmap.Width; ++x)
                    {
                        for (int s = 0; s < 3; ++s)
                        {
                            byte slice = 0;
                            for (int b = 0; b < 8; ++b)
                            {
                                int y = (line * 24) + (s * 8) + b;
                                slice |= (byte)((y < bitmap.Height && pixelsBrightness[y * bitmap.Width + x] < brightness ? 1 : 0) << (7 - b));
                            }
                            bytes.Add(slice);
                        }
                    }
                    bytes.Add(0x0a);
                }
                bytes.Add(0x1b);
                bytes.Add(0x21);
                bytes.Add(0x1e);

                // test to printer
                //System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort("COM4", 38400, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                //port.Open();
                //port.Write(bytes.ToArray(), 0, bytes.Count);
                //port.Close();

                return bytes.ToArray();
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        public static System.Drawing.Bitmap ScaleBitmap(System.Drawing.Bitmap inputBitmap, int scaleWidth, int scaleHeight, int maxWidth)
        {
            int width = inputBitmap.Width;
            int height = inputBitmap.Height;
            if (scaleWidth > 0 && scaleWidth != 100)
                width = inputBitmap.Width * scaleWidth / 100;
            if (scaleHeight > 0 && scaleHeight != 100)
                height = inputBitmap.Height * scaleHeight / 100;

            if (width > maxWidth)
            {
                height = height * maxWidth / width;
                width = maxWidth;
            }

            return ResizeBitmap(inputBitmap, width, height);
        }

        public static System.Drawing.Bitmap ScaleBitmap(System.Drawing.Bitmap inputBitmap, int width)
        {
            if (inputBitmap.Width == width)
                return inputBitmap;

            return ResizeBitmap(inputBitmap, width, inputBitmap.Height * width / inputBitmap.Width);
        }

        public static System.Drawing.Bitmap ResizeBitmap(System.Drawing.Bitmap inputBitmap, int width, int height)
        {
            if (width > 0 && height > 0 && (inputBitmap.Width != width || inputBitmap.Height != height))
            {
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height);
                using (Graphics graphics = Graphics.FromImage((Image)bitmap))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(inputBitmap, 0, 0, width, height);
                }
                return bitmap;
            }
            else
                return inputBitmap;
        }

        public static System.Drawing.Bitmap GetBarcodeBitmap(string barcode, int width, int height)
        {
            try
            {
                BarcodeFormat barcodeFormat;
                switch (barcode.Length)
                {
                    case 7:
                        barcodeFormat = BarcodeFormat.EAN_8;
                        break;

                    case 8:
                        barcodeFormat = Barcode.Verify(BarcodeType.EAN_8, barcode) ? BarcodeFormat.EAN_8 : BarcodeFormat.CODE_39;
                        break;

                    case 12:
                        barcodeFormat = BarcodeFormat.EAN_13;
                        break;

                    case 13:
                        barcodeFormat = Barcode.Verify(BarcodeType.EAN_13, barcode) ? BarcodeFormat.EAN_13 : BarcodeFormat.CODE_39;
                        break;

                    default:
                        barcodeFormat = BarcodeFormat.CODE_39;
                        break;
                }

                var codeWriter = new BarcodeWriter
                {
                    Format = barcodeFormat,
                    Options = new EncodingOptions
                    {
                        PureBarcode = true,
                        Width = width,
                        Height = height,
                        Margin = 1
                    }
                };
                return codeWriter.Write(barcode);
            }
            catch { return null; }            
        }

        public static System.Drawing.Bitmap GetQrcodeBitmap(string text, int size)
        {
            try
            {
                if (size < 1 || size > 16)
                    size = 4;

                var qrCodecEncoder = new QRCodeEncoder();
                qrCodecEncoder.QRCodeBackgroundColor = Color.White;
                qrCodecEncoder.QRCodeForegroundColor = Color.Black;
                qrCodecEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodecEncoder.QRCodeScale = size;
                qrCodecEncoder.QRCodeVersion = 0;
                qrCodecEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                return qrCodecEncoder.Encode(text);
            }
            catch { return null; }
        }
    }
}
