# Device Manager API
## Configuration

### appsettings.json

The application requires a connection string to connect to a SQL Server database. Example appsettings.json:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<Host>,<Port>;Database=<DatabaseName>;User Id=<Username>;Password=<Password>;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": <256-BIT-SECRET>,
    "Issuer": <Issuer>,
    "Audience": <Audience>,
    "ExpireMinutes": <TimeInMinutes>
  },
}

```

## Why Not Multiple Projects?

*  We keep all code in one project folder. It is simple and doesn't need many references.
*  No need to open many project files, so it is easier to debug and work.
*  Building and starting app is faster because there is only one assembly.
*  If the code becomes large later, you can move any folder to separate project when you need.

## Endpoints

* **Devices**
    * `GET /api/devices`
    * `GET /api/devices/{id}`
    * `POST /api/devices`
    * `PUT /api/devices/{id}`
    * `DELETE /api/devices/{id}`
* **Employees**
    * `GET /api/employees`
    * `GET /api/employees/{id}`
* **Auth**
    * `POST /api/auth`
* **Accounts**
    * `POST /api/accounts`
    * `GET /api/accounts`
    * `GET /api/accounts/{id}`
    * `PUT /api/accounts/{id}`
    * `DELETE /api/accounts/{id}`
    * `GET /api/accounts/me`
    * `PUT /api/accounts/me`
