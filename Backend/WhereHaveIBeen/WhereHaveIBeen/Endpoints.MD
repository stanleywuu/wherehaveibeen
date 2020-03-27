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

{
	"userId":1,
	"latitude":43.563917,
	"longitude":-79.769994,
	"checkin":"2012-03-19T07:22Z"
}
```