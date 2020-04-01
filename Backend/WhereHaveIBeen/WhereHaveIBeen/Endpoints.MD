## Sample requests and response for Endpoints
Because I'm an idiot and didn't think I needed to use swagger

### Users
- Create Users
```
POST /membership HTTP/1.1
Host: https://wherehaveibeen.azurewebsites.net
Content-Type: application/json

{"Username":"admin","Email":"swu@swu.com","Password":"blah","Token":null}
```
- Login

Request
```
POST /membership/login HTTP/1.1
Host: https://wherehaveibeen.azurewebsites.net
Content-Type: application/json

{"Username":"admin","Password":"blah"}
```
Response
```
{
    "userId": 1,
    "token": "YoWVr2vPxWCmJyvnvsO2aRAcG1nWyZwEFplUSTiDDwM=",
    "expires": null,
    "hasunseennotification":true
}
```

- I have Corona
Request
```
POST /membership/corona HTTP/1.1
Host: https://wherehaveibeen.azurewebsites.net
Content-Type: application/json

{
	"userId":2,
	"IsAtRisk":true
}

```
This will also mark all the places this person has visited within 14 days as Risk

### Visits (I do realize this is a bad name)
- Create a new visit
```
POST /visit HTTP/1.1
Host: https://wherehaveibeen.azurewebsites.net
Content-Type: application/json

{
	"userId":1,
	"latitude":43.563917,
	"longitude":-79.769994,
	"checkin":"2012-03-19T07:22Z"
}
```
- Get Information about visits
```
GET /visit?userId=1 HTTP/1.1
Host: https://wherehaveibeen.azurewebsites.net
Content-Type: application/json

```
Response
```
{"VisitId":1,"Longitude":-79.769994,"Latitude":43.563917,"LatitudeRounded":43.564,"LongitudeRounded":-79.77,"CheckIn":"2012-03-19T07:22:00","CheckOut":"2012-03-19T09:22:00","UserId":1,"Address":"Mississauga, ON L5N 7V9, Canada","PlaceId":"ChIJ4ySOJydqK4gRqIMWbEw8fEY","AtRisk":false}
{"VisitId":2,"Longitude":-79.769994,"Latitude":43.563917,"LatitudeRounded":43.564,"LongitudeRounded":-79.77,"CheckIn":"2012-03-19T07:22:00","CheckOut":"2012-03-19T09:22:00","UserId":1,"Address":"Mississauga, ON L5N 7V9, Canada","PlaceId":"ChIJ4ySOJydqK4gRqIMWbEw8fEY","AtRisk":false}
```

- Delete all visits from a user
```
POST /visit/delete/{userId} HTTP/1.1
Host: https://wherehaveibeen.azurewebsites.net
Authorization: Bearer token
Content-Type: application/json
```
### Risk
Get all risky locations within a given coordinate

```
GET /risk?lat={51.51}&lng={-0.09} HTTP/1.1
Host: https://wherehaveibeen.azurewebsites.net
```
Sample Response
```
 {
        "visitId": 5,
        "longitude": -0.0877321,
        "latitude": 51.5078788,
        "latitudeRounded": 51.508,
        "longitudeRounded": -0.088,
        "latitude2Decimal": 51.51,
        "longitude2Decimal": -0.09,
        "checkIn": "2020-03-29T17:38:00",
        "checkOut": "2020-03-29T19:38:00",
        "userId": 1,
        "address": "London SE1 9RA, UK",
        "placeId": "ChIJHefmLlcDdkgRVwOQ6omEsRM",
        "atRisk": true
    }
```
Get all visits from a user that could be considered "risky"
```
GET /risk/user?userId={userId}&lat={51.51}&lng={-0.09} HTTP/1.1
Host: https://wherehaveibeen.azurewebsites.net
Authorization: Bearer {token}
```
Sample Response
```
[
    {
        "visitId": 6,
        "checkIn": "2020-03-29T17:42:00",
        "checkOut": "2020-03-29T19:42:00",
        "address": "London SE1 9RA, UK",
        "latitude": 51.5078788,
        "longitude": -0.0877321,
        "distanceInKm": 0,
        "linkedVisits": [
            {
                "visitId": 0,
                "checkIn": "2020-03-29T17:42:00",
                "checkOut": "2020-03-29T19:42:00",
                "address": "London SE1 9RA, UK",
                "latitude": 51.5078788,
                "longitude": -0.0877321,
                "distanceInKm": 0,
                "linkedVisits": null
            }
        ]
    }
]
```
