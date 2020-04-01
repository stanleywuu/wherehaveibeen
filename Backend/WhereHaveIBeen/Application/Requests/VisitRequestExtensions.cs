using Application.Data;
using GoogleApi.Entities.Maps.Geocoding.Common.Enums;
using GoogleApi.Entities.Maps.Geocoding.Location.Request;
using GoogleApi.Entities.Places.Details.Request;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Requests
{
    public static class VisitRequestExtensions
    {
        public static async Task<Visit> ToPersistedData(this VisitRequest request)
        {
            var placeId = request.PlaceId;
            string placeName = null;
            string address = null;
            if (string.IsNullOrEmpty(placeId))
            {
                var geoCode = GoogleApi.GoogleMaps.LocationGeocode;
                var results = await geoCode.QueryAsync(new LocationGeocodeRequest()
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

                if (results != null && results.Results != null && results.Results.Count() > 0)
                {
                    var result = results.Results.FirstOrDefault();
                    placeId = result.PlaceId;
                    address = result.FormattedAddress;
                }
            }

            if (!string.IsNullOrEmpty(placeId))
            {
                var response = await GoogleApi.GooglePlaces.Details.QueryAsync(new PlacesDetailsRequest()
                {
                    Key = ApplicationConfig.GoogleKey,
                    PlaceId = placeId
                });

                if (response != null && response.Result != null)
                {
                    placeName = response.Result.Name;
                    address = response.Result.FormattedAddress;
                }
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
                Latitude2Decimal = (double)Math.Round((decimal)request.Latitude, 2),
                Longitude2Decimal = (double)Math.Round((decimal)request.Longitude, 2),
                PlaceId = placeId,
                PlaceName = placeName,
                Address = address
            };
        }
    }
}
