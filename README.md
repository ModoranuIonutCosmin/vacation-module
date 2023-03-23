# Vacation module solution

## Checklist:
1. [X] [Goals](#main-requirements)
2. [X] [Testing](#testing)
~ 50% line coverage
3. [x] [Authorization with RBAC & JWT](#authorization-with-rbac--jwt)
3. [X] [Deploy](#deploy)
Deployed on Azure (App Service + Azure SQL) using GitHub Actions CI/CD.
Link: [link_remote](https://vacationmodule.azurewebsites.net/swagger)
4. [X] [Documentation](#documentation)
Checkout swagger at [link_remote](https://vacationmodule.azurewebsites.net/swagger)
Alternatively,  [link_local]()

## Quickstart

### Docker

```powershell
cd <root-folder-containing-src-folder>
docker compose -f .\docker-compose.yml -f .\docker-compose.override.yml up --build --force-recreate -d
```

### Local

```powershell
cd <root-folder-containing-src-folder>\src\VacationsModule.WebApi
# Set sql server connection string in appsettings.json
dotnet run
```

### Recommended flow

A default user with manager role is already seeded. (username: manageruser01, password: string123)

A manager can create an account for an employee.

The API uses versioning default version is 1.0.
1. Login as manager
POST /api/v1.0/Account/login

Request body
```json
{
  "userNameOrEmail": "manageruser01",
  "password": "string123"
}
```
Fetch the JWT token from the response
and add it to authorization (in swagger perhaps).
Authorization: Bearer {jwt}
Or just input it in swagger UI.

2. Create an employee account

POST /api/v1.0/Employee/register/employee

```json
{
    "firstName": "string",
    "lastName": "string",
    "userName": "string",
    "email": "newempl01@m.ro",
    "password": "string123",
    "employmentDate": "2023-02-22T11:28:25.729Z",
    "department": "string",
    "position": "string"
}
```

3. Login as employee

POST /api/v1.0/Account/login

Request body
```json
{
  "userNameOrEmail": "newempl01@m.ro",
  "password": "string123"
}
```
and add JWT token to authorization header.

4. Get current vacation requests

(Use default params in swagger UI, specify version 1.0)

GET /api/v1.0/VacationRequests/vacation-requests

5. Create a vacation request

POST /api/v1.0/VacationRequests/create-vacation-request

You can test multiple scenarios by changing the dates.



```json
{
  "requestedDateIntervals": [
    {
      "startDate": "2021-03-23T11:33:53.777Z",
      "endDate": "2021-04-23T11:33:53.777Z"
    }
  ],
  "description": "string"
}

```
(should fail, dates are in the past)

```json
{
  "requestedDateIntervals": [
    {
      "startDate": "2023-03-24T11:33:53.777Z",
      "endDate": "2023-03-29T11:33:53.777Z"
    },
    {
      "startDate": "2023-03-25T11:33:53.777Z",
      "endDate": "2023-04-01T11:33:53.777Z"
    }],
  "description": "string"
}

```
(should fail, dates overlap)


```json
{
  "requestedDateIntervals": [
    {
      "startDate": "2023-03-28T11:33:53.777Z",
      "endDate": "2023-03-29T11:33:53.777Z"
    },
    {
      "startDate": "2023-04-02T11:33:53.777Z",
      "endDate": "2023-04-04T11:33:53.777Z"
    }],
  "description": "string"
}
```
(works, record id field from response body)


6. Get current vacation requests

(Use default params in swagger UI, specify version 1.0)

GET /api/v1.0/VacationRequests/vacation-requests

7. Update vacation request

PUT /api/v1.0/VacationRequests/update-vacation-request

```json
{
  "vacationRequestId": "<vacation_request_id>",
  "requestedDateIntervals": [
    {
      "startDate": "2023-03-24T11:40:58.716Z",
      "endDate": "2023-03-25T11:40:58.716Z"
    }
  ],
  "extraComment": {
    "message": "string"
  },
  "description": "string"
}
```

8. Get current vacation requests

(Use default params in swagger UI, specify version 1.0)

GET /api/v1.0/VacationRequests/vacation-requests

9. Get national holidays

GET /api/v1.0/VacationRequests/national-holidays

You can specify query params in swagger UI. (will get the holidays within that interval)
Or not (will get the holidays for the current year).
<br>
StartDate: 2023-01-22T23:14:44.080Z 
<br>
EndDate: 2023-03-22T23:14:44.080Z

10. Get vacation requests by ID

GET /api/v1.0/VacationRequests/vacation-requests/{vacationRequestId}

Version is 1.0 in swagger UI.
Use the previously created vacation request id.

11. Get current user vacation days.
GET /api/v1.0/Employee/available-vacation-days






