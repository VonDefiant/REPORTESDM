using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REPORTESDM.Droid
{
    [Activity(Theme = "@style/MainTheme", MainLauncher =true, NoHistory =true, Label = "REPORTESDM")]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Drawable.splash_screen);

            VideoView videoView = FindViewById<VideoView>(Resource.Id.videoSplash);

            // Ruta del video en la carpeta "Resources/drawable" sin extensión
            string videoPath = "android.resource://" + PackageName + "/" + Resource.Raw.intro;


            Android.Net.Uri videoUri = Android.Net.Uri.Parse(videoPath);
            videoView.SetVideoURI(videoUri);

            // Configura eventos para manejar la finalización de la reproducción
            videoView.Completion += (sender, e) =>
            {
                StartActivity(typeof(MainActivity));
                Finish(); // Cierre la actividad del SplashScreen
            };

            videoView.Start(); // Comienza la reproducción del video
        }

    }
}