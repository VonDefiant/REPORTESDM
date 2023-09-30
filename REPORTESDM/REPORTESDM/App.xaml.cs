using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace REPORTESDM
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new TuAppXamarin.MainPage();
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
