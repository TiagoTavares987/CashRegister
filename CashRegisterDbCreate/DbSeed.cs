using CashRegisterCore;
using CashRegisterCore.Entities;
using CashRegisterCore.Enumerators;
using CashRegisterCore.Utils;

namespace CashRegisterDbCreateSeed
{
    internal static class DbSeed
    {
        internal static void SeedTables()
        {
            var imageResource = new ImageResource() { FilePath = @"Images\User\tiago.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            AppCore.Db.Insert(new User() { IsAdmin = true, Username = "Tiago", Password = "81dc9bdb52d04dc20036dbd8313ed055", ImageId = imageResource.Id });
            AppCore.Db.Insert(new User() { IsAdmin = true, Username = "Diana", Password = "81dc9bdb52d04dc20036dbd8313ed055" });

            imageResource = new ImageResource() { FilePath = @"Images\User\user.png" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            AppCore.Db.Insert(new User() { IsAdmin = false, Username = "Empregado", Password = "81dc9bdb52d04dc20036dbd8313ed055", ImageId = imageResource.Id });

            AppCore.Db.Insert(new Client() { Name = "Cliente", Nif = "123456789", Email = "tmpqwertytmp123@gmail.com", Address = new FullAddress() { Address = "Rua", City = "C", PostalCode = "0000-000", Country = "Portugal", CountryShort = Country.PT } });

            imageResource = new ImageResource() { FilePath = @"Images\Family\entradas.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            var entradas = AppCore.Db.Insert(new Family() { Name = "Entradas", ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Item\pao.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            AppCore.Db.Insert(new Item() { Name = "Pão", ShortName = "Pão", BarCode = "5601125701602", Price = 1.20m, Cost = 0.25m, Tax = Constant.TaxRed, FamilyId = entradas, ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Item\azeitonas.jpeg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            AppCore.Db.Insert(new Item() { Name = "Azeitonas", ShortName = "Azeitonas", BarCode = "8456367354676", Price = 1.80m, Cost = 0.30m, Tax = Constant.TaxNor, FamilyId = entradas, ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Family\bebidas_quentes.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            var bebidaQ = AppCore.Db.Insert(new Family() { Name = "Bebidas quentes", ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Item\cafe.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            AppCore.Db.Insert(new Item() { Name = "Café", ShortName = "Café", BarCode = "8456367354676", Price = 0.80m, Cost = 0.12m, Tax = Constant.TaxInt, FamilyId = bebidaQ, ImageId = imageResource.Id });
            imageResource = new ImageResource() { FilePath = @"Images\Item\meia_de_leite.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            AppCore.Db.Insert(new Item() { Name = "Meia de leite", ShortName = "1/2 Leite", BarCode = "8456367354676", Price = 1m, Cost = 0.30m, Tax = Constant.TaxInt, FamilyId = bebidaQ, ImageId = imageResource.Id });
            imageResource = new ImageResource() { FilePath = @"Images\Item\galao.png" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            AppCore.Db.Insert(new Item() { Name = "Galão", ShortName = "Galão", BarCode = "8456367354676", Price = 1.20m, Cost = 0.35m, Tax = Constant.TaxInt, FamilyId = bebidaQ, ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Family\bebidas_frias.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            var bebidaF = AppCore.Db.Insert(new Family() { Name = "Bebidas frias", ImageId = imageResource.Id });
            imageResource = new ImageResource() { FilePath = @"Images\Item\cocacola.png" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            AppCore.Db.Insert(new Item() { Name = "Coca-Cola", ShortName = "Cola", BarCode = "8456367354676", Price = 1.50m, Cost = 0.60m, Tax = Constant.TaxNor, FamilyId = bebidaF, ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Family\bebidas_alcoolicas.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            var alcool = AppCore.Db.Insert(new Family() { Name = "Bebidas alcoolicas", ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Family\sandes.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            var sandes = AppCore.Db.Insert(new Family() { Name = "Sandes", ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Family\prato_dia.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            var pratos= AppCore.Db.Insert(new Family() { Name = "Pratos do dia", ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Family\salgados.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            var salgados = AppCore.Db.Insert(new Family() { Name = "Salgados", ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Family\doces.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            var doces = AppCore.Db.Insert(new Family() { Name = "Doces", ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Family\outros.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            var outros = AppCore.Db.Insert(new Family() { Name = "Outros", ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Family\sopas.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            var sopas = AppCore.Db.Insert(new Family() { Name = "Sopas", ImageId = imageResource.Id });

            imageResource = new ImageResource() { FilePath = @"Images\Family\saladas.jpg" };
            AppCore.ImageResourceManager.SaveImageResource(imageResource);
            var saladas = AppCore.Db.Insert(new Family() { Name = "Saladas", ImageId = imageResource.Id });

        }
    }
}
