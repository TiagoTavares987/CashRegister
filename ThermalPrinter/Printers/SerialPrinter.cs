using System.Collections.Generic;

using ThermalPrinter.ContentConfig;
using ThermalPrinter.PrinterConfig;
using ThermalPrinter.Utils;

namespace ThermalPrinter.Printers
{
    internal class SerialPrinter : IPrinter
    {
        private int _imageMaxWidth = 100;

        public SerialPrinter(SerialPrinterConfig config) => Config = config;

        public SerialPrinterConfig Config { get; }

        public void Print(IEnumerable<ContentLine> content)
        {
            using (var port = new System.IO.Ports.SerialPort(Config.PortName, Config.BaudRate, Config.Parity, Config.DataBits, Config.StopBits))
            {
                port.Open();
                foreach (var line in content)
                {
                    var list = new List<byte[]>();
                    switch (line.Type)
                    {
                        case ContentLineType.Text:
                            list.AddRange(EscCodes.AddData(EscCodes.AddText(line.Text), port.WriteBufferSize));
                            break;

                        case ContentLineType.Barcode:
                            list.AddRange(EscCodes.AddBarcode(line.Text, line.Width));
                            break;

                        case ContentLineType.QrCode:
                            list.AddRange(EscCodes.AddQrCode(line.Text, line.Width));
                            break;

                        case ContentLineType.Image:
                            if (line.Height == int.MinValue)
                                list.Add(EscCodes.AddImage(Bitmap.GetBitmapBytes(Bitmap.ScaleBitmap(line.Bitmap, line.Width > _imageMaxWidth ? _imageMaxWidth : line.Width))));
                            else
                                list.Add(EscCodes.AddImage(Bitmap.GetBitmapBytes(Bitmap.ScaleBitmap(line.Bitmap, line.Width, line.Height, _imageMaxWidth))));
                            break;

                        case ContentLineType.Data:
                            list.AddRange(EscCodes.AddData(line.Data, port.WriteBufferSize));
                            break;

                        case ContentLineType.Instruction:
                            switch (line.Instruction)
                            {
                                case ContentLineInstruction.Reset:
                                    list.Add(EscCodes.Reset);
                                    break;

                                case ContentLineInstruction.ChangeLine:
                                    list.Add(EscCodes.ChangeLine);
                                    break;

                                case ContentLineInstruction.AlignLeft:
                                    list.Add(EscCodes.AlignLeft);
                                    break;
                                case ContentLineInstruction.AlignCenter:
                                    list.Add(EscCodes.AlignCenter);
                                    break;
                                case ContentLineInstruction.AlignRight:
                                    list.Add(EscCodes.AlignRight);
                                    break;

                                case ContentLineInstruction.FontWeightNormal:
                                    list.Add(EscCodes.FontWeightNormal);
                                    break;
                                case ContentLineInstruction.FontWeightBold:
                                    list.Add(EscCodes.FontWeightBold);
                                    break;

                                case ContentLineInstruction.FontSize6:
                                    list.Add(EscCodes.FontSize8);
                                    break;
                                case ContentLineInstruction.FontSize8:
                                    list.Add(EscCodes.FontSize8);
                                    break;
                                case ContentLineInstruction.FontSize10:
                                    list.Add(EscCodes.FontSize10);
                                    break;
                                case ContentLineInstruction.FontSize12:
                                    list.Add(EscCodes.FontSize12);
                                    break;
                                case ContentLineInstruction.FontSize14:
                                    list.Add(EscCodes.FontSize12);
                                    break;

                                case ContentLineInstruction.PaperCut:
                                    list.Add(EscCodes.PaperCut);
                                    break;

                                case ContentLineInstruction.OpenDrawer:
                                    list.Add(EscCodes.OpenDrawer);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        default:
                            break;
                    }

                    list.ForEach(x => port.Write(x, 0, x.Length));
                }
                port.Close();
            }
        }
    }
}
