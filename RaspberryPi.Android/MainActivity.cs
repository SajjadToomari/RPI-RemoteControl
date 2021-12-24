using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaspberryPi.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        HubConnection connection;

        ImageView ImageView1;
        ImageView ImageView2;
        ImageView ImageView3;

        Button Button1;
        Button Button2;
        Button Button3;

        bool isOff1;
        bool isOff2;
        bool isOff3;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            isOff1 = true;
            isOff2 = true;
            isOff3 = true;

            connection = new HubConnectionBuilder()
              .WithUrl("https://toomari.ir/RelayBoardHub")
              .WithAutomaticReconnect()
              .AddMessagePackProtocol()
              .Build();

            connection.On<Dictionary<int, bool>>("ReceiveSwitchValues", (data) =>
            {
                foreach (var item in data)
                {
                    switch (item.Key)
                    {
                        case 26:
                            if (item.Value)
                            {
                                isOff1 = true;
                                RunOnUiThread(() =>
                                {
                                    ImageView1.SetImageResource(Resource.Drawable.off_lamp);
                                });
                            }
                            else
                            {
                                isOff1 = false;
                                RunOnUiThread(() =>
                                {
                                    ImageView1.SetImageResource(Resource.Drawable.on_lamp);
                                });
                            }
                            break;
                        case 21:
                            if (item.Value)
                            {
                                isOff3 = true;
                                RunOnUiThread(() =>
                                {
                                    ImageView3.SetImageResource(Resource.Drawable.off_lamp);
                                });
                            }
                            else
                            {
                                isOff3 = false;
                                RunOnUiThread(() =>
                                {
                                    ImageView3.SetImageResource(Resource.Drawable.on_lamp);
                                });
                            }
                            break;
                        case 20:
                            if (item.Value)
                            {
                                isOff2 = true;
                                RunOnUiThread(() =>
                                {
                                    ImageView2.SetImageResource(Resource.Drawable.off_lamp);
                                });
                            }
                            else
                            {
                                isOff2 = false;
                                RunOnUiThread(() =>
                                {
                                    ImageView2.SetImageResource(Resource.Drawable.on_lamp);
                                });
                            }
                            break;                            
                        default:
                            break;
                    }
                }
            });

            Task.Run(async () =>
            {
                try
                {
                    await connection.StartAsync();

                    await connection.SendAsync("SendGetSwitchValuesCommand");

                    Toast.MakeText(this, "Conncted", ToastLength.Long);
                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, $"Error : {ex.Message}", ToastLength.Long);
                }

            });

            ImageView1 = FindViewById<ImageView>(Resource.Id.imageView1);
            ImageView2 = FindViewById<ImageView>(Resource.Id.imageView2);
            ImageView3 = FindViewById<ImageView>(Resource.Id.imageView3);

            Button1 = FindViewById<Button>(Resource.Id.button1);
            Button2 = FindViewById<Button>(Resource.Id.button2);
            Button3 = FindViewById<Button>(Resource.Id.button3);

            Button1.Click += (sender, e) =>
            {
                if (isOff1)
                {
                    connection.SendAsync("SendSwitchCommand", 26, false);
                }
                else
                {
                    connection.SendAsync("SendSwitchCommand", 26, true);
                }
            };

            Button2.Click += (sender, e) =>
            {
                if (isOff2)
                {
                    connection.SendAsync("SendSwitchCommand", 20, false);
                }
                else
                {
                    connection.SendAsync("SendSwitchCommand", 20, true);
                }
            };

            Button3.Click += (sender, e) =>
            {
                if (isOff3)
                {
                    connection.SendAsync("SendSwitchCommand", 21, false);
                }
                else
                {
                    connection.SendAsync("SendSwitchCommand", 21, true);
                }
            };
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (connection != null && connection.State == HubConnectionState.Disconnected)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await connection.StartAsync();

                        await connection.SendAsync("SendGetSwitchValuesCommand");

                        Toast.MakeText(this, "Conncted", ToastLength.Long);
                    }
                    catch (System.Exception ex)
                    {
                        Toast.MakeText(this, $"Error : {ex.Message}", ToastLength.Long);
                    }

                });
            }
        }
    }
}