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
    "expires": null
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
POST /visit/delete/{userId} HTTP/1.1
Host: https://wherehaveibeen.azurewebsites.net
Authorization: Bearer token
Content-Type: application/json

