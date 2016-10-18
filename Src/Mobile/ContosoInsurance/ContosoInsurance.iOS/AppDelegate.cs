﻿using Foundation;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using UIKit;
using ContosoInsurance.Helpers;
using HockeyApp.iOS;

namespace ContosoInsurance.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public static NSData DeviceToken { get; private set; }
        public static bool IsAfterLogin = false;
        const string HOCKEYAPP_APPID = "173bb49bbecf4b61873990c2aed13abf";

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            app.StatusBarHidden = true;

            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(246,246,247);
            UINavigationBar.Appearance.TintColor = UIColor.Black; 
            UIBarButtonItem.Appearance.TintColor = UIColor.Black;


            global::Xamarin.Forms.Forms.Init();

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            //SQLitePCL.CurrentPlatform.Init();
            //http://motzcod.es/post/150988588867/updating-azure-mobile-sqlitestore-to-30
            //SQLitePCL.Batteries.Init();

            var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert 
                                                                          | UIUserNotificationType.Badge
                                                                          | UIUserNotificationType.Sound,
                                                                          new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            var formsApp = new ContosoInsurance.App();
            LoadApplication(formsApp);

            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure(HOCKEYAPP_APPID);
            manager.StartManager();
            manager.Authenticator.AuthenticateInstallation(); // This line is obsolete in crash only builds

       
            return base.FinishedLaunching(app, options);
        }


        public static async Task RegisterWithMobilePushNotifications()
        {
            if (DeviceToken != null && IsAfterLogin) {

                var apnsBody = new JObject {
                    {
                        "message", "$(Message)"
                    }
                };

                var template = new JObject {
                    {
                        "genericMessage",
                        new JObject {
                            {"body", apnsBody}
                        }
                    }
                };

                try {
                    var push = MobileServiceHelper.msInstance.Client.GetPush();
                    await push.RegisterAsync(DeviceToken, template);
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine("Exception in RegisterWithMobilePushNotifications: " + ex.Message);

                    UIAlertView avAlert1 = new UIAlertView("Notification", "Exception in RegisterWithMobilePushNotifications", null, "OK", null);
                    avAlert1.Show();
                }
            }
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            string alert = (userInfo.ObjectForKey(new NSString("message")) as NSString).ToString();
            //show alert
            if (!string.IsNullOrEmpty(alert))
            {
                UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
                avAlert.Show();
            }

            UIAlertView avAlert1 = new UIAlertView("Notification", "DidReceiveRemoteNotification", null, "OK", null);
            avAlert1.Show();
        }

        public override async void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            DeviceToken = deviceToken;

            if (IsAfterLogin)
                await RegisterWithMobilePushNotifications();
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);
        }
    }
}
