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

            map = new Map() { VerticalOptions = LayoutOptions.FillAndExpand };

            Content = new StackLayout
            {
                BackgroundColor = Color.Red,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    
                    map
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ShowCurrentLocation().ConfigureAwait(false);
        }

        private async Task ShowCurrentLocation()
        {
            var location = await Geolocation.GetLocationAsync();
            
            if (location == null && App.CurrentVisit == null)
            {
                await DisplayAlert("Missing Location Data", "Please Ensure you have enabled location on your device", null);
            }


            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                map.MoveToRegion(new MapSpan(new Position(location.Latitude, location.Longitude), 6.0, 6.0));
                map.Pins.Add(
                    new Pin()
                    {
                        Position = new Position(location.Latitude, location.Longitude),
                        Label = "Current Location",
                        Type = PinType.SearchResult
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