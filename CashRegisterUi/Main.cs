using CashRegisterCore;
using CashRegisterUi.Languages;
using CashRegisterUi.Windows;

using Gtk;
using System.Threading;
using System.Threading.Tasks;

namespace CashRegisterUi
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            new SplashWindow(StartLoading);
            Application.Run();
        }

        private static void StartLoading(SplashWindow splash) => Task.Run(() => Loading(splash));

        private static void Loading(object parameter)
        {
            var splash = (SplashWindow)parameter;
            splash.Loading = Translate.Language.DbConnecting;
            AppCore.Init();
            //Task.Delay(1000).Wait();
            splash.Loading = "A criar a janela principal";
            //Task.Delay(1000).Wait();

            Application.Invoke((o, e) => new LoginWindow(splash.Destroy));
        }
    }
}

