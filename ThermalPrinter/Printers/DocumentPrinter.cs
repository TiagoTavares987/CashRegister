using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using ThermalPrinter.ContentConfig;
using ThermalPrinter.PrinterConfig;

namespace ThermalPrinter.Printers
{
    internal class DocumentPrinter : IPrinter
    {
        private int _defaultWidth = 380;
        private const int _defaultHeight = 1169;

        List<ContentLine> _content;
        //private Font _printingFont = new Font("mono", 10, FontStyle.Regular);
        private Font _printingFont = new Font("Courier New", 10, FontStyle.Regular);
        private Brush _printingBrush = new SolidBrush(Color.Black);
        private float _printingAlign = 0f;
        private int _index = 0;

        private float _printingX;
        private float _printingY;

        public DocumentPrinter(DocumentPrinterConfig config) => Config = config;

        public DocumentPrinterConfig Config { get; }

        public void Print(IEnumerable<ContentLine> content)
        {
            _defaultWidth = 365;
            _content = content.ToList();
            var controller = new PreviewPrintController();
            var printDocument = new PrintDocument() { PrintController = controller };
            printDocument.DefaultPageSettings.PaperSize = new PaperSize("Custom", _defaultWidth, _defaultHeight) { RawKind = 119 };
            printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            //printDocument.PrinterSettings.PrinterName = Config.PrinterName;
            printDocument.PrintPage += PrintDocumentPage;
            printDocument.Print();

            var image = GetDocumentImage(controller.GetPreviewPageInfo(), printDocument.DefaultPageSettings.PaperSize);
            ExportPdf(image);
        }

        private void PrintDocumentPage(object sender, PrintPageEventArgs e)
        {
            _printingX = 0f;
            _printingY = 0f;

            e.HasMorePages = false;

            for (; _index < _content.Count; _index++)
            {
                var line = _content[_index];
                switch (line.Type)
                {
                    case ContentLineType.Text:
                        PrintText(e, line.Text);
                        break;

                    case ContentLineType.Barcode:
                        PrintImage(e, Utils.Bitmap.GetBarcodeBitmap(line.Text, line.Width * 50, line.Width * 8), line.Width * 40);
                        break;

                    case ContentLineType.QrCode:
                        PrintImage(e, Utils.Bitmap.GetQrcodeBitmap(line.Text, line.Width));
                        break;

                    case ContentLineType.Image:
                        PrintImage(e, line.Bitmap, line.Width);
                        break;

                    case ContentLineType.Data:
                        break;

                    case ContentLineType.Instruction:
                        switch (line.Instruction)
                        {
                            case ContentLineInstruction.ChangeLine:
                                _printingY += _printingFont.GetHeight(e.Graphics);
                                break;

                            case ContentLineInstruction.AlignLeft:
                                _printingAlign = 0;
                                break;
                            case ContentLineInstruction.AlignCenter:
                                _printingAlign = .5f;
                                break;
                            case ContentLineInstruction.AlignRight:
                                _printingAlign = 1;
                                break;

                            case ContentLineInstruction.FontWeightNormal:
                                _printingFont = new Font(_printingFont.FontFamily, _printingFont.Size, FontStyle.Regular);
                                break;
                            case ContentLineInstruction.FontWeightBold:
                                _printingFont = new Font(_printingFont.FontFamily, _printingFont.Size, FontStyle.Bold);
                                break;

                            case ContentLineInstruction.FontSize6:
                                _printingFont = new Font(_printingFont.FontFamily, 6, _printingFont.Style);
                                break;
                            case ContentLineInstruction.FontSize8:
                                _printingFont = new Font(_printingFont.FontFamily, 8, _printingFont.Style);
                                break;
                            case ContentLineInstruction.FontSize10:
                                _printingFont = new Font(_printingFont.FontFamily, 10, _printingFont.Style);
                                break;
                            case ContentLineInstruction.FontSize12:
                                _printingFont = new Font(_printingFont.FontFamily, 12, _printingFont.Style);
                                break;
                            case ContentLineInstruction.FontSize14:
                                _printingFont = new Font(_printingFont.FontFamily, 20, _printingFont.Style);
                                break;

                            case ContentLineInstruction.PaperCut:
                                break;

                            case ContentLineInstruction.OpenDrawer:
                                break;

                            default:
                                break;
                        }
                        break;

                    default:
                        return;
                }

                if (e.HasMorePages)
                    return;
            }
        }

        private void PrintText(PrintPageEventArgs e, string text)
        {
            var numberLines = text.Split('\n').Length;
            var finalY = _printingY + (_printingFont.GetHeight(e.Graphics) * numberLines);

            if (finalY > e.PageSettings.PaperSize.Height)
                e.HasMorePages = true;
            else
            {
                var printingWidth = e.PageSettings.PaperSize.Width;
                if (_printingAlign == 1f)
                {
                    SizeF textWidth = e.Graphics.MeasureString(text, _printingFont);
                    _printingX = printingWidth - textWidth.Width;
                    if (_printingX < 0)
                        _printingX = 0;
                }
                else
                {
                    if (_printingAlign == .5f)
                    {
                        try
                        {
                            SizeF scriptWidth = e.Graphics.MeasureString(text, _printingFont);
                            _printingX = (printingWidth - scriptWidth.Width) / 2;
                        }
                        catch (Exception)
                        {
                            _printingX = 0;
                        }
                        if (_printingX < 0)
                            _printingX = 0;
                    }
                    else
                        _printingX = 0;
                }

                e.Graphics.DrawString(text, _printingFont, _printingBrush, _printingX, _printingY, StringFormat.GenericDefault);
                _printingY += _printingFont.GetHeight(e.Graphics) * numberLines;
            }
        }

        public void PrintImage(PrintPageEventArgs e, Bitmap bitmap, int width = 0)
        {
            if (bitmap == null)
                return;

            int height = bitmap.Height;
            if(width <= 0)
                width = bitmap.Width;
            else if (width != bitmap.Width)
                height = (int)Math.Ceiling((decimal)width * bitmap.Height / bitmap.Width);

            if (width > e.PageSettings.PaperSize.Width)
            {
                width = e.PageSettings.PaperSize.Width;
                height = (int)Math.Ceiling((decimal)width * bitmap.Height / bitmap.Width);
            }

            if (_printingAlign == 1)
                _printingX = e.PageSettings.PaperSize.Width - width;
            else if (_printingAlign == .5)
            {
                try { _printingX = (e.PageSettings.PaperSize.Width - width) / 2; }
                catch { _printingX = 0; }
            }

            if (_printingX < 0)
                _printingX = 0;

            e.Graphics.DrawImage(bitmap, Convert.ToInt32(_printingX), Convert.ToInt32(_printingY), width, height);				
            _printingY += height;
        }

        private Bitmap GetDocumentImage(PreviewPageInfo[] pages, PaperSize size)
        {
            if (pages.Length > 0)
            {
                int width = size.Width;
                int height = size.Height;
                int areaHeight = height * (pages.Length - 1) + (int)Math.Ceiling(_printingY);
                if (areaHeight < width * 2)
                    areaHeight = width * 2;

                Bitmap bitmap = new Bitmap(width, areaHeight);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    int y = 0;
                    //if (isThumbnail)
                    //    g.FillRectangle(Brushes.LightGray, (Rectangle)new Rectangle(0, 0, areaWidth, areaHeight));
                    g.FillRectangle(Brushes.White, new Rectangle(0, 0, width, areaHeight));
                    foreach (PreviewPageInfo page in pages)
                    {
                        g.DrawImage(page.Image, new Rectangle(0, y, width, height));
                        y += height;
                    }
                }
                return bitmap;
            }
            return null;
        }

        public void ExportPdf(Bitmap image)
        {
            var pdfImage = iTextSharp.text.Image.GetInstance(image, ImageFormat.Png);
            var doc = new iTextSharp.text.Document(pdfImage, 0, 0, 0, 0);
            using (FileStream fs = new FileStream(Config.PdfPath, FileMode.Create))
            {
                PdfWriter.GetInstance(doc, fs);
                doc.Open();
                doc.Add(pdfImage);
                doc.Close();
            }
        }
    }
}
