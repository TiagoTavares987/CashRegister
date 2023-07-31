using System;
using System.Collections.Generic;
using System.Text;

namespace ThermalPrinter.Utils
{
    internal static class EscCodes
    {
        private static readonly byte[] _LF = Encoding.ASCII.GetBytes("\n");
        private static readonly byte[] _RESETPRINTER = Encoding.ASCII.GetBytes("\x1B\x40");
        private static readonly byte[] _BOLD_ON = Encoding.ASCII.GetBytes("\x1B\x21\x08");
        private static readonly byte[] _BOLD_OFF = Encoding.ASCII.GetBytes("\x1B\x21\x00");
        private static readonly byte[] _DOUBLE_HEIGHT_ON = Encoding.ASCII.GetBytes("\x1B\x21\x10");
        private static readonly byte[] _DOUBLE_HEIGHT_OFF = Encoding.ASCII.GetBytes("\x1B\x21\x00");
        private static readonly byte[] _FONTSIZE_SMALL = Encoding.ASCII.GetBytes("\x1B\x21" + "1");
        private static readonly byte[] _DOUBLE_WIDTH_ON = Encoding.ASCII.GetBytes("\x1B\x21\x20");
        private static readonly byte[] _DOUBLE_WIDTH_OFF = Encoding.ASCII.GetBytes("\x1B\x21\x00");
        private static readonly byte[] _UNDERLINE_ON = Encoding.ASCII.GetBytes("\x1B\x2D" + "1");
        private static readonly byte[] _UNDERLINE_OFF = Encoding.ASCII.GetBytes("\x1B\x2D" + "0");
        private static readonly byte[] _EMPHASIZED_ON = Encoding.ASCII.GetBytes("\x1B\x45" + "1");
        private static readonly byte[] _EMPHASIZED_OFF = Encoding.ASCII.GetBytes("\x1B\x45" + "0");
        private static readonly byte[] _DOUBLESTRIKE_ON = Encoding.ASCII.GetBytes("\x1B\x47" + "1");
        private static readonly byte[] _DOUBLESTRIKE_OFF = Encoding.ASCII.GetBytes("\x1B\x47" + "0");
        private static readonly byte[] _CHAR_A = Encoding.ASCII.GetBytes("\x1B\x4D" + "0");
        private static readonly byte[] _CHAR_B = Encoding.ASCII.GetBytes("\x1B\x4D" + "1");
        private static readonly byte[] _CHAR_C = Encoding.ASCII.GetBytes("\x1B\x4D" + "2");
        private static readonly byte[] _JUSTIFY_LEFT = Encoding.ASCII.GetBytes("\x1B\x61" + "0");
        private static readonly byte[] _JUSTIFY_CENTER = Encoding.ASCII.GetBytes("\x1B\x61" + "1");
        private static readonly byte[] _JUSTIFY_RIGHT = Encoding.ASCII.GetBytes("\x1B\x61" + "2");
        private static readonly byte[] _RF = Encoding.ASCII.GetBytes("\x1B\x65" + "1");
        private static readonly byte[] _PAPERCUT = Encoding.ASCII.GetBytes("\x1D\x56" + "1");
        private static readonly byte[] _PAPERCUT_FULL = Encoding.ASCII.GetBytes("\x1D\x56" + "0");
        private static readonly byte[] _COLOR_1 = Encoding.ASCII.GetBytes("\x1B\x72" + "0");
        private static readonly byte[] _COLOR_2 = Encoding.ASCII.GetBytes("\x1B\x72" + "1");
        private static readonly byte[] _PULSE = Encoding.ASCII.GetBytes("\x1B\x70" + "<<PIN>>" + "<<TIMEON>>" + "<<TIMEOFF>>");
        private static readonly byte[] _CODETABLE = Encoding.ASCII.GetBytes("\x1B\x74" + "<<CHARCODE>>");
        private static readonly byte[] _OPENDRAWER = Encoding.ASCII.GetBytes((char)(27) + "p" + (char)(0) + (char)(10) + (char)(100));

        public static byte[] AddText(string text) => Encoding.ASCII.GetBytes(ClearString(text) + "\n");
        public static IEnumerable<byte[]> AddBarcode(string text, int size)
        {
            int height;
            switch (size)
            {
                case 1:
                    height = 1;
                    break;

                case 2:
                    height = 28;
                    break;

                case 3:
                    height = 42;
                    break;

                case 4:
                    height = 56;
                    break;

                case 5:
                    height = 70;
                    break;

                case 6:
                    height = 83;
                    break;

                case 7:
                    height = 97;
                    break;

                default:
                    size = 4;
                    height = 56;
                    break;
            }
            height = (int)Math.Ceiling(57.6 * height);

            var barcode = new List<byte[]>();
            barcode.Add(new byte[] { 0x1D, 0x77, (byte)size });
            barcode.Add(new byte[] { 0x1D, 0x68, (byte)height });
            barcode.Add(new byte[] { 0x1D, 0x48, 0x00 });

            if (text.Length == 8)
                barcode.Add(new byte[] { 0x1D, 0x6B, 0x03 });
            else if (text.Length == 13)
                barcode.Add(new byte[] { 0x1D, 0x6B, 0x02 });
            else
                barcode.Add(new byte[] { 0x1D, 0x6B, 0x04 });

            barcode.Add(Encoding.ASCII.GetBytes(text));
            return barcode;
        }
        public static IEnumerable<byte[]> AddQrCode(string text, int size)
        {
            var qrCode = new List<byte[]>();
            int scriptLenght = text.Length + 3;
            qrCode.Add(new byte[] { 0x1d, 0x28, 0x6b, 0x04, 0x00, 0x31, 0x41, 0x32, 0x00 });
            qrCode.Add(new byte[] { 0x1d, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x43, (byte)size });
            qrCode.Add(new byte[] { 0x1d, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x45, 0x33 });
            qrCode.Add(new byte[] { 0x1d, 0x28, 0x6b, (byte)(scriptLenght % 256), (byte)(scriptLenght / 256), 0x31, 0x50, 0x30 });
            qrCode.Add(Encoding.ASCII.GetBytes(text));
            qrCode.Add(new byte[] { 0x1d, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x51, 0x30 });
            //qrCode.Add(new byte[] { 0x1d, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x52, 0x30 });
            return qrCode;
        }
        public static byte[] AddImage(byte[] image)
        {
            var bytes = new List<byte>(image);
            bytes.AddRange(_RESETPRINTER);
            return bytes.ToArray();
        }

        public static IEnumerable<byte[]> AddData(byte[] data, int maxLength)
        {
            var list = new List<byte[]>();
            if (data.Length < maxLength)
                list.Add(data);
            else
            {
                byte[] bytes = new byte[maxLength];
                var buffer = (int)Math.Floor(data.Length / (double)maxLength);
                for (int i = 0; i < buffer; i++)
                {
                    Array.Copy(data, i * maxLength, bytes, 0, bytes.Length);
                    list.Add(bytes);
                }

                bytes = new byte[data.Length - (buffer * maxLength)];
                Array.Copy(data, buffer * maxLength, bytes, 0, bytes.Length);
                list.Add(bytes);
            }
            return list;
        }

        public static byte[] Reset => _RESETPRINTER;
        public static byte[] ChangeLine => _LF;
        public static byte[] AlignLeft => _JUSTIFY_LEFT;
        public static byte[] AlignCenter => _JUSTIFY_CENTER;
        public static byte[] AlignRight => _JUSTIFY_RIGHT;
        public static byte[] FontWeightNormal => _BOLD_OFF;
        public static byte[] FontWeightBold => _BOLD_ON;
        public static byte[] FontSize8 => Encoding.ASCII.GetBytes("\x1B\x21\x01");
        public static byte[] FontSize10 => Encoding.ASCII.GetBytes("\x1B\x21\x00");
        public static byte[] FontSize12 => Encoding.ASCII.GetBytes("\x1B\x21\x10\x1B\x21\x20");
        public static byte[] PaperCut => _PAPERCUT;
        public static byte[] OpenDrawer => _OPENDRAWER;

        private static string ClearString(string text)
        {

            /** Troca os caracteres acentuados por não acentuados **/
            string[] acentos = new string[] {
                "ç",
                "Ç",
                "á",
                "é",
                "í",
                "ó",
                "ú",
                "ý",
                "Á",
                "É",
                "Í",
                "Ó",
                "Ú",
                "Ý",
                "à",
                "è",
                "ì",
                "ò",
                "ù",
                "À",
                "È",
                "Ì",
                "Ò",
                "Ù",
                "ã",
                "õ",
                "ñ",
                "ä",
                "ë",
                "ï",
                "ö",
                "ü",
                "ÿ",
                "Ä",
                "Ë",
                "Ï",
                "Ö",
                "Ü",
                "Ã",
                "Õ",
                "Ñ",
                "â",
                "ê",
                "î",
                "ô",
                "û",
                "Â",
                "Ê",
                "Î",
                "Ô",
                "Û"
            };
            string[] semAcento = new string[] {
                "c",
                "C",
                "a",
                "e",
                "i",
                "o",
                "u",
                "y",
                "A",
                "E",
                "I",
                "O",
                "U",
                "Y",
                "a",
                "e",
                "i",
                "o",
                "u",
                "A",
                "E",
                "I",
                "O",
                "U",
                "a",
                "o",
                "n",
                "a",
                "e",
                "i",
                "o",
                "u",
                "y",
                "A",
                "E",
                "I",
                "O",
                "U",
                "A",
                "O",
                "N",
                "a",
                "e",
                "i",
                "o",
                "u",
                "A",
                "E",
                "I",
                "O",
                "U"
            };

            for (int i = 0; i < acentos.Length; i++)
            {
                text = text.Replace(acentos[i], semAcento[i]);
            }

            /** Troca os caracteres especiais da string por "" **/
            string[] caracteresEspeciais = {
            //    ",",
            //    ":",
            //    "\\(",
            //    "\\)",
                "ª",
                "º",
            //    "\\|",
            //    "\\\\",
                "°",
            //    "!",
            //    "@",
            //    "#",
            //    "$",
            //    "%",
            //    "&",
            //    "*",
            //    "+",
            //    "=",
            //    "!",
            //    "#",
            //    "£",
            //    "§",
            //    "{",
            //    "[",
            //    "]",
            //    "}",
            //    "€"
            };

            for (int i = 0; i < caracteresEspeciais.Length; i++)
            {
                text = text.Replace(caracteresEspeciais[i], "");
            }

            /** Troca os espaços no início por "" **/
            //str = str.Replace("^\\s+", "");
            /** Troca os espaços no início por "" **/
            //str = str.Replace("\\s+$", "");
            /** Troca os espaços duplicados, tabulações e etc por  " " **/
            //str = str.Replace("\\s+", " ");

            //			// JIC
            //			str = str.Replace ("%","");
            //			str = str.Replace ("(","");
            //			str = str.Replace (")","");
            //			
            return text;
        }
    }
}
