﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using System;
using Android.Content;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;

namespace ContosoInsurance.Droid
{
    [Activity (Label = "Contoso Insurance", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        public static MainActivity instance;
        public const string HOCKEYAPP_APPID = "8e7c354ae6d34dc7bacfc5033f4a88d1";

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            App.UIContext = this;
            LoadApplication(new ContosoInsurance.App());

            instance = this;

            // Register the crash manager before Initializing the trace writer
            CrashManager.Register(this, HOCKEYAPP_APPID);

            //Register to with the Update Manager
            UpdateManager.Register(this, HOCKEYAPP_APPID);

            MetricsManager.Register(Application, HOCKEYAPP_APPID);
            MetricsManager.EnableUserMetrics();

#if PUSH // need to use a Google image on an Android emulator
            try {
                // Check to ensure everything's setup right
                GcmClient.CheckDevice(this);
                GcmClient.CheckManifest(this);

                // Register for push notifications
                System.Diagnostics.Debug.WriteLine("Registering...");
                GcmClient.Register(this, PushHandlerBroadcastReceiver.SENDER_IDS);
            }
            catch (Java.Net.MalformedURLException) {

                CreateAndShowDialog(new Exception("There was an error creating the Mobile Service. Verify the URL"), "Error");
            }
            catch (Exception e) {
                CreateAndShowDialog(e, "Error");
            }
#endif 
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public static MainActivity DefaultService
        {
            get { return instance; }
        }

        private void CreateAndShowDialog(Exception e, string title)
        {
            //set alert for executing the task
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle(title);
            alert.SetMessage(e.Message);

            alert.SetPositiveButton("OK", (senderAlert, args) => { });

            //run the alert in UI thread to display in the screen
            RunOnUiThread(() => {
                alert.Show();
            });
        }
    }
}

