using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TuAppXamarin;  // Asegúrate de tener la referencia correcta al espacio de nombres

namespace REPORTESDM
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());  // Envuelve MainPage en NavigationPage
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
