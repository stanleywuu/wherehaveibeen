using Application.Data;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Geocoding.Common.Enums;
using GoogleApi.Entities.Maps.Geocoding.Location.Request;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Requests
{
    public static class VisitRequestExtensions
    {
        public static async Task<Visit> ToPersistedData(this VisitRequest request)
        {
            var geoCode = GoogleApi.GoogleMaps.LocationGeocode;
            var places = await geoCode.QueryAsync(new LocationGeocodeRequest()
            {
                Key = ApplicationConfig.GoogleKey,
                Location = new GoogleApi.Entities.Common.Location()
                {
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                },
                LocationTypes = new GeometryLocationType[]
                {
                     GeometryLocationType.Approximate
                }
            });

            var place = places.Results.FirstOrDefault();
            string address = "";
            string placeId = null;

            // Place Id is misleading

            if (place != null)
            {
                address = place.FormattedAddress;
                placeId = place.PlaceId;
            }
            return new Visit()
            {
                UserId = request.UserId,
                CheckIn = request.CheckIn,
                // if there is no checkout time, assume 2 hours
                CheckOut = request.CheckOut ?? request.CheckIn.AddHours(2.0f),
                Longitude = request.Longitude,
                Latitude = request.Latitude,
                LatitudeRounded = (double)Math.Round((decimal)request.Latitude, 3),
                LongitudeRounded = (double)Math.Round((decimal)request.Longitude, 3),
                PlaceId = placeId,
                Address = address,
            };
        }
    }
}
