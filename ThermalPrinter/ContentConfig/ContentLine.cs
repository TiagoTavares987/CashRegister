using System.Drawing;

namespace ThermalPrinter.ContentConfig
{
    public class ContentLine
    {
        public static ContentLine AddText(string text) => new ContentLine(ContentLineType.Text, text);

        public static ContentLine AddBarcode(string barcode, int size = 4) => new ContentLine(ContentLineType.Barcode, barcode, width: size);

        public static ContentLine AddQrCode(string qrCode, int size = 4) => new ContentLine(ContentLineType.QrCode, qrCode, width: size);

        public static ContentLine AddImage(Bitmap image, int width = 100) => new ContentLine(ContentLineType.Image, bitmap: image, width: width, height: int.MinValue);

        public static ContentLine AddImage(Bitmap image, int width, int height) => new ContentLine(ContentLineType.Image, bitmap: image, width: width, height: height);

        public static ContentLine AddData(byte[] data) => new ContentLine(ContentLineType.Data, data: data);

        public static ContentLine SetInstruction(ContentLineInstruction instruction)
        {
            if (instruction != ContentLineInstruction.None)
                return new ContentLine(ContentLineType.Instruction, instruction: instruction);

            return null;
        }

        private ContentLine(ContentLineType type, string text = null, Bitmap bitmap = null, byte[] data = null, ContentLineInstruction instruction = ContentLineInstruction.None, int width = 0, int height = 0)
        {
            Type = type;
            Text = text;
            Bitmap = bitmap;
            Data = data;
            Width = width;
            Height = height;
            Instruction = instruction;
        }

        internal ContentLineType Type { get; }
        internal string Text { get; }
        internal Bitmap Bitmap { get; }
        internal byte[] Data { get; }
        internal int Width { get; }
        internal int Height { get; }
        internal ContentLineInstruction Instruction { get; }
    }
}
