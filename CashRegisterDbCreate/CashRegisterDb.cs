namespace CashRegisterDbCreateSeed
{
    public static class CashRegisterDb
    {
        public static void Main(string[] args)
        {
            DbCreate.CreateTables();

            if (args != null && args.Length > 0 && args[0].Equals("-seed"))
                DbSeed.SeedTables();
        }
    }
}
