using CashRegisterCore.Entities;

namespace CashRegisterCore.Providers
{
    internal class ImageResourceProvider
    {
        public ImageResource GetImageResource(int id) => AppCore.Db.GetEntity<ImageResource>(id);

        public int InsertImageResource(ImageResource imageResource) => AppCore.Db.Insert(imageResource);

        public int UpdateImageResource(ImageResource imageResource) => AppCore.Db.Update(imageResource);

        public int DeleteImageResource(ImageResource imageResource) => AppCore.Db.Delete(imageResource);
    }
}
