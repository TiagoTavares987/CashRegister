using Database;

namespace CashRegisterCore.Entities
{
    [IsDbTable(nameof(ImageResource))]
    public class ImageResource
    {
        [IsDbField(true)]
        public int Id { get; set; }
        [IsDbField]
        public string Name { get; set; }
        [IsDbField]
        public string Ext { get; set; }
        [IsDbField]
        public byte[] Image { get; set; }

        public string FilePath { get; set; }

        public ImageResource Clone() => (ImageResource)MemberwiseClone();
    }
}