using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Maps.Map;
using Map = Xamarin.Forms.Maps.Map;

namespace WhereHaveIBeen.Mobile.Views
{
    public class CheckInInfo
    {
        public int UserId => App.LoggedInUser.UserId;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        Map map;

        public MapPage()
        {
            InitializeComponent();
            var checkinItem = new ToolbarItem
            {
                Text = "Check In"
            };
            var checkOutItem = new ToolbarItem
            {
                Text = "Check Out"
            };

            checkOutItem.Clicked += OnCheckOut;

            ToolbarItems.Add(checkinItem);
            ToolbarItems.Add(checkOutItem);

            map = new Map();

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                Children = {
                    map
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        private async Task ShowCurrentLocation()
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                map.Pins.Add(
                    new Pin()
                    {
                        Position = new Position(location.Latitude, location.Longitude)
                    });
            });
        }

        private void OnCheckIn(object sender, EventArgs e)
        {

        }

        private void OnCheckOut(object sender, EventArgs e)
        {

        }
    }
}