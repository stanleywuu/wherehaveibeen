using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

namespace WhereHaveIBeen.Mobile.Droid
{
    public class LocationLogger : Java.Lang.Object, ILocationListener
    {
        public String DeviceID { get; set; }
        public int LogInterval { get; set; }
        public Location Loc { get; private set; }
        public DateTime LogTime { get; private set; }
        public DateTime OverrideLogTime { get; set; }
        public Int32? RouteCode { get; set; }
        public short? StopNumber { get; set; }
        private readonly LocationManager _locMngr;
        private readonly String _locProvider;
        private readonly Criteria _locCriteria;
        private HandlerThread _handlerThread;
        private HandlerThread _handlerThreadSingle;


        public LocationLogger()
        {
            _locMngr = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            Criteria criteriaForLocationService = new Criteria { Accuracy = Accuracy.Fine };
            _locCriteria = criteriaForLocationService;
            IList<String> acceptableLocationProviders = _locMngr.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
                _locProvider = acceptableLocationProviders.First();
            else
                _locProvider = String.Empty;
        }

        public void StartRequestionUpdates()
        {
            if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                Int64.TryParse(TimeSpan.FromMinutes(LogInterval).TotalMilliseconds.ToString(), out var interval);

                _locMngr.RemoveUpdates(this);
                if (_handlerThread != null) //ensure there is only one thread running
                {
                    _handlerThread.Quit();
                    _handlerThread = null;
                }

                _handlerThread = new HandlerThread("LocationLoggerThread", (Int32)ThreadPriority.Background);
                _handlerThread.Start();
                _locMngr.RequestLocationUpdates(interval, 0, _locCriteria, this, _handlerThread.Looper);
            }
        }

        public void StopUpdates()
        {
            _locMngr.RemoveUpdates(this);
            if (_handlerThread != null)
            {
                _handlerThread.Quit();
                _handlerThread.Interrupt();
                _handlerThread = null;
            }

            if (_handlerThreadSingle != null)
            {
                _handlerThreadSingle.Quit();
                _handlerThreadSingle.Interrupt();
                _handlerThreadSingle = null;
            }
        }

        public void OnLocationChanged(Location location)
        {
            if (location != null)
            {
                Loc = location;
                LogTime = DateTime.Now;
                if (OverrideLogTime != DateTime.MinValue)
                    LogTime = OverrideLogTime;
                else
                    LogTime = DateTime.Now;

                MessagingCenter.Send<LocationLogger>(this, LocationMessages.LocationLogged);
            }
        }

        public void RequestSingleLocationUpdate()
        {
            if (_handlerThreadSingle != null) //ensure there is only one thread running
            {
                _handlerThreadSingle.Quit();
                _handlerThreadSingle = null;
            }

            _handlerThreadSingle = new HandlerThread("SingleLocationThread", (Int32)ThreadPriority.Background);
            _handlerThreadSingle.Start();
            _locMngr.RequestSingleUpdate(_locCriteria, this, _handlerThreadSingle.Looper);
        }

        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {

        }
    }
}