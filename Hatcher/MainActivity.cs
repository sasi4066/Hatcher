using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Android.Net.Wifi;
using Android.Content;
using System.Threading.Tasks;
using System.Threading;
using OperationCanceledException = Android.OS.OperationCanceledException;

namespace Hatcher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button startButton;
        CancellationTokenSource cts;
        readonly WifiManager wm = Application.Context.GetSystemService(Context.WifiService) as WifiManager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            startButton = FindViewById<Button>(Resource.Id.HatchingButton);
            startButton.SetBackgroundColor(Android.Graphics.Color.Green);
            startButton.Click += HatchingButtonClick;            
        }

        private async void HatchingButtonClick(object sender, EventArgs e)
        {
            cts = new CancellationTokenSource();

            if (startButton.Text == GetString(Resource.String.start))
            {
                try
                {
                    Task myTask = StartHatchingAsync(cts.Token);
                    startButton.Text = GetString(Resource.String.stop);
                    startButton.SetBackgroundColor(Android.Graphics.Color.Red);
                    await myTask;
                }
                catch (OperationCanceledException)
                {
                    startButton.Text = GetString(Resource.String.start);
                    startButton.SetBackgroundColor(Android.Graphics.Color.Green);
                }                
            }
            else if (startButton.Text == GetString(Resource.String.stop) && cts != null)
            {
                cts.Cancel();
                wm.SetWifiEnabled(true);
                startButton.Text = GetString(Resource.String.start);
                startButton.SetBackgroundColor(Android.Graphics.Color.Green);
            }            
        }

        private async Task StartHatchingAsync(CancellationToken token)
        {
            while (true)
            {
                wm.SetWifiEnabled(false);
                await Task.Delay(15000, token);
                wm.SetWifiEnabled(true);
                await Task.Delay(5000, token);
            }
        }
    }
}