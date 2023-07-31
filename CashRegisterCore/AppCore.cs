using CashRegisterCore.Configs;
using CashRegisterCore.Managers;
using CashRegisterCore.Services;
using System;

namespace CashRegisterCore
{
    public static class AppCore
    {
        private static readonly int _terminalId = AppConfig.TerminalId;

        private static readonly Database.MySql _db = new Database.MySql(new MySqlConfig());
        private static readonly UserManager _userManager = new UserManager();
        private static readonly ClientManager _clientManager = new ClientManager();
        private static readonly DocumentManager _documentManager = new DocumentManager();
        private static readonly FamilyManager _familyManager = new FamilyManager();
        private static readonly ItemManager _itemManager = new ItemManager();
        private static readonly PrintingManager _printingManager = new PrintingManager();
        private static readonly ImageResourceManager _imageResourceManager = new ImageResourceManager();

        private static PrintingService _printingService;

        public static void Init() => _printingService = new PrintingService();

        public static int TerminalId { get => _terminalId; }
        public static int UserId { get; internal set; }
        internal static int LastDocId { get => _documentManager.GetLastDocumentId(); }

        public static UserManager UserManager => _userManager;
        public static ClientManager ClientManager => _clientManager;
        public static DocumentManager DocumentManager => _documentManager;
        public static FamilyManager FamilyManager => _familyManager;
        public static ItemManager ItemManager => _itemManager;
        public static PrintingManager PritingManager => _printingManager;
        public static ImageResourceManager ImageResourceManager => _imageResourceManager;

        internal static PrintingService PrintingService => _printingService;
        internal static Database.MySql Db => _db;
    }
}
