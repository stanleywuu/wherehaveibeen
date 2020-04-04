using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using WhereHaveIBeen.Mobile.Views;

namespace WhereHaveIBeen.Mobile.Droid.Services
{
    public class GeolocationServiceBinder : Binder
    {
        public GeolocationServiceBinder(GeolocationService service)
        {
            Service = service;
        }

        public GeolocationService Service { get; }

        public bool IsBound { get; set; }
    }

    [Service]
    public class GeolocationService : Service
    {
        IBinder binder;

        public override IBinder OnBind(Intent intent)
        {
            binder = new GeolocationServiceBinder(this);
            return binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var builder = new NotificationCompat.Builder(this, "location");

            var newIntent = new Intent(this, typeof(MainActivity));
            newIntent.PutExtra("tracking", true);
            newIntent.AddFlags(ActivityFlags.ClearTop);
            newIntent.AddFlags(ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity(this, 0, newIntent, 0);
            var notification = builder.SetContentIntent(pendingIntent)
                .SetSmallIcon(Resource.Drawable.ic_mtrl_chip_checked_black)
                .SetAutoCancel(false)
                .SetTicker("Tracking Location")
                .SetContentTitle("My Location")
                .Build();


            StartForeground((int)NotificationFlags.ForegroundService, notification);

            return StartCommandResult.Sticky;
        }

        public void StartLocationUpdates()
        {
            CrossGeolocator.Current.DesiredAccuracy = 25;
            CrossGeolocator.Current.PositionChanged += Geolocator_PositionChanged;

            CrossGeolocator.Current.StartListeningAsync(
                TimeSpan.FromSeconds(3), 5).ConfigureAwait(false);
        }

        private void Geolocator_PositionChanged(object sender, PositionEventArgs e)
        {
            // if current location is more than 50 meters away
            // Check out and submit
            //App.CurrentVisit;
        }

        public void StopLocationUpdates()
        {
            CrossGeolocator.Current.StopListeningAsync().ConfigureAwait(false);
        }
    }

    public class GeolocationServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public GeolocationServiceConnection(GeolocationServiceBinder binder)
        {
            if (binder != null)
            {
                Binder = binder;
            }
        }

        public GeolocationServiceBinder Binder { get; set; }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var serviceBinder = service as GeolocationServiceBinder;

            if (serviceBinder == null)
                return;


            Binder = serviceBinder;
            Binder.IsBound = true;

            // raise the service bound event
            ServiceConnected?.Invoke(this, new ServiceConnectedEventArgs { Binder = service });

            // begin updating the location in the Service
            serviceBinder.Service.StartLocationUpdates();
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Binder.IsBound = false;
        }

        public event EventHandler<ServiceConnectedEventArgs> ServiceConnected;
    }

    public class ServiceConnectedEventArgs : EventArgs
    {
        public IBinder Binder { get; set; }
    }
}