using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WhereHaveIBeen.Mobile.Views;
using WhereHaveIBeen.Mobile.Data;
using Xamarin.Essentials;

namespace WhereHaveIBeen.Mobile
{
    public partial class App : Application
    {
        public static LoginResponse LoggedInUser { get; private set; }
        public static CheckInInfo CurrentVisit { get; set; }

        public static void PersistUser(LoginResponse userInfo)
        {
            Preferences.Set("token", userInfo.Token);
            Preferences.Set("user", userInfo.UserId);
        }

        public static LoginResponse LoadUser()
        {
            try
            {
                var token = Preferences.Get("token", string.Empty);
                var userId = Preferences.Get("user", -1);

                if (userId > -1)
                {
                    return new LoginResponse()
                    {
                        Token = token,
                        UserId = userId
                    };
                }
            }
            catch
            { }

            return null;
        }

        public App()
        {
            InitializeComponent();

            LoggedInUser = LoadUser();

            var page = new NavigationPage(LoggedInUser == null ? (Page)new LoginPage() : new MapPage());
            MainPage = page;
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
