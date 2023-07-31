using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CashRegisterCore.Configs
{
    public static class AppConfig
    {
        private static readonly string _iniPath = Path.Combine(Directory.GetCurrentDirectory(), "config.ini");

        static AppConfig()
        {
            if (!File.Exists(_iniPath))
                return;

            Dictionary<string, string> key_value_pair = new Dictionary<string, string>();
            try
            {
                using (StreamReader iniFile = new StreamReader(_iniPath))
                {
                    string strLine = iniFile.ReadLine();
                    while (strLine != null)
                    {
                        if (!string.IsNullOrEmpty(strLine))
                        {
                            string[] keyvalue = strLine.Split(new char[] { '=' }, 2);
                            if (keyvalue.Length > 1)
                            {
                                string key = keyvalue[0];
                                string value = keyvalue[1];
                                if (!string.IsNullOrEmpty(key))
                                    key = key.Trim();
                                if (!string.IsNullOrEmpty(value))
                                    value = value.Trim();
                                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && !key_value_pair.ContainsKey(key))
                                    key_value_pair.Add(key, value);
                            }
                        }
                        strLine = iniFile.ReadLine();
                    }
                    iniFile.Close();
                }
            }
            catch { }

            if (key_value_pair.ContainsKey("Language"))
                Language = key_value_pair["Language"];

            if (key_value_pair.ContainsKey("TerminalId"))
            {
                int value;
                if (int.TryParse(key_value_pair["TerminalId"], out value))
                    TerminalId = value;
            }

            if (key_value_pair.ContainsKey("ServerAddress"))
                ServerAddress = key_value_pair["ServerAddress"];

            if (key_value_pair.ContainsKey("ServerPort"))
                ServerPort = key_value_pair["ServerPort"];

            if (key_value_pair.ContainsKey("ServerDatabase"))
                ServerDatabase = key_value_pair["ServerDatabase"];

            if (key_value_pair.ContainsKey("ServerUsername"))
                ServerUsername = key_value_pair["ServerUsername"];

            if (key_value_pair.ContainsKey("ServerPassword"))
                ServerPassword = key_value_pair["ServerPassword"];
        }

        public static string Language { get; set; } = "PT";
        public static int TerminalId { get; set; } = -1;
        public static string ServerAddress { get; set; } = "127.0.0.1";
        public static string ServerPort { get; set; } = "3306";
        public static string ServerDatabase { get; set; } = "CashReg";
        public static string ServerUsername { get; set; } = "CashReg";
        public static string ServerPassword { get; set; } = "cGFzc3dvcmRZM0k9";

        public static void Save()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Language = " + Language);
            sb.AppendLine("TerminalId = " + TerminalId);
            sb.AppendLine("ServerAddress = " + ServerAddress);
            sb.AppendLine("ServerPort = " + ServerPort);
            sb.AppendLine("ServerDatabase = " + ServerDatabase);
            sb.AppendLine("ServerUsername = " + ServerUsername);
            sb.AppendLine("ServerPassword = " + ServerPassword);

            using (TextWriter tw = new StreamWriter(_iniPath))
            {
                tw.Write(sb.ToString());
                tw.Close();
            }
        }
    }
}
