using Database;

namespace CashRegisterCore.Configs
{
    internal class MySqlConfig : IMySqlConfig
    {
        public string Address
        {
            get => AppConfig.ServerAddress;
            set => AppConfig.ServerAddress = value;
        }

        public string Port
        {
            get => AppConfig.ServerPort;
            set => AppConfig.ServerPort = value;
        }

        public string Database
        {
            get => AppConfig.ServerDatabase;
            set => AppConfig.ServerDatabase = value;
        }

        public string Username
        {
            get => AppConfig.ServerUsername;
            set => AppConfig.ServerUsername = value;
        }

        public string Password
        {
            get => AppConfig.ServerPassword;
            set => AppConfig.ServerPassword = value;
        }

        public void Save() => AppConfig.Save();
    }
}
