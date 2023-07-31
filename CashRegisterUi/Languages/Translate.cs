using CashRegisterCore.Configs;
using System;
using System.Configuration;
using System.Linq;

namespace CashRegisterUi.Languages
{
    internal static class Translate
    {
        private const string _laguageConfigKey = nameof(Language);

        private static ILanguage _language = GetLanguage(GetConfig());
        private static Language _defaultLanguage = Languages.Language.PT;

        public static ILanguage Language => _language;

        public static void SetLanguage(Language language)
            => _language = GetLanguage(language);

        private static ILanguage GetLanguage(Language language)
        {
            AppConfig.Language = language.ToString();
            AppConfig.Save();
            switch (language)
            {
                case Languages.Language.PT:
                    return new PT();

                case Languages.Language.EN:
                    return new EN();

                default:
                    return new PT();
            }
        }

        private static Language GetConfig()
        {
            try
            {
                return (Language)Enum.Parse(typeof(Language), AppConfig.Language);
            }
            catch { return _defaultLanguage; }
        }
    }
}
