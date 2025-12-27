# MockHub Documentation

## Table of Contents

1. [Installation](#installation)
2. [Getting Started](#getting-started)
3. [User Management](#user-management)
4. [Project Management](#project-management)
5. [Team Management](#team-management)
6. [Endpoint Configuration](#endpoint-configuration)
7. [Dynamic Variables](#dynamic-variables)
8. [Request Logging](#request-logging)
9. [Advanced Features](#advanced-features)
10. [Troubleshooting](#troubleshooting)

## Installation

### System Requirements

- .NET 8 SDK or later
- Windows, Linux, or macOS
- Minimum 2GB RAM
- 500MB disk space for application and database

### Installation Methods

#### Method 1: Source Code Build

1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/MockHub.git
   cd MockHub
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run database migrations:
   ```bash
   cd src/MockHub.Web
   dotnet ef database update
   ```

5. Start the application:
   ```bash
   dotnet run
   ```

The application will be available at `http://localhost:5268`.

#### Method 2: Docker Deployment

1. Build the Docker image:
   ```bash
   docker build -t mockhub -f docker/Dockerfile .
   ```

2. Run the container:
   ```bash
   docker run -d -p 5268:5268 --name mockhub mockhub
   ```

3. Or use Docker Compose:
   ```bash
   cd docker
   docker-compose up -d
   ```

### Initial Setup

On first launch, you'll be redirected to the setup page where you need to:

1. Create the first administrator account
2. Provide email, password, first name, and last name
3. Complete the setup process

After setup, you can log in with the administrator credentials.

## Getting Started

### First Login

1. Navigate to the login page
2. Enter your administrator email and password
3. Click "Login"

### Dashboard Overview

The dashboard provides:
- Total project count
- Team count
- Total endpoint count
- Quick access to create new projects

### Creating Your First Project

1. Click "New Project" from the dashboard or Projects page
2. Enter project details:
   - **Name**: A descriptive name for your project
   - **Description**: Optional project description
   - **Enable CORS**: Allow cross-origin requests
   - **Enable Request Logging**: Enable request logging for this project
3. Click "Create"

The project slug is automatically generated from the project name and cannot be manually edited.

### Adding Your First Endpoint

1. Open your project from the Projects page
2. Click "Add Endpoint"
3. Configure the endpoint:
   - **Name**: Descriptive name for the endpoint
   - **HTTP Method**: Select GET, POST, PUT, etc.
   - **Route**: Define the route path (e.g., `/api/users` or `/api/users/:id`)
   - **Status Code**: HTTP response status code
   - **Content-Type**: Response content type (application/json, text/plain, etc.)
   - **Response Body**: The response body content
4. Click "Save"

Your endpoint is now active and accessible at:
```
http://localhost:5268/{project-slug}/api/{your-route}
```

## User Management

### Administrator Role

Administrators have full access to:
- User management (create, edit, deactivate users)
- Team management (create and manage all teams)
- View all request logs across all projects
- Delete logs for any project

### Creating Users

Only administrators can create new users:

1. Navigate to Administration > Users
2. Click "Add User"
3. Fill in user details:
   - First Name and Last Name
   - Email address
   - Password
   - Administrator privileges (optional)
4. Click "Create User"

### User Roles

- **Administrator**: Full system access
- **User**: Can create personal projects and work on team projects

### Managing Users

Administrators can:
- Edit user information
- Reset user passwords
- Activate or deactivate users
- Assign users to teams
- View user activity

## Project Management

### Project Types

MockHub supports two project types:

1. **Personal Projects**: Owned by individual users
   - Accessible at: `/{project-slug}/api/{endpoint}`
   - Only the owner can modify

2. **Team Projects**: Owned by teams
   - Accessible at: `/{team-slug}/{project-slug}/api/{endpoint}`
   - Team members with appropriate roles can modify

### Project Settings

Each project can be configured with:

- **CORS**: Enable or disable Cross-Origin Resource Sharing
- **Request Logging**: Enable or disable request logging
- **Default Delay**: Default response delay in milliseconds
- **JWT Validation**: Enable JWT token validation (if configured)

### Project Slug

The project slug is automatically generated from the project name:
- Converted to lowercase
- Special characters removed
- Spaces replaced with hyphens
- Automatically updated when the project name changes
- Ensured to be unique within its scope (personal or team)

## Team Management

### Creating Teams

Only administrators can create teams:

1. Navigate to Administration > All Teams
2. Click "Create Team"
3. Enter team name and description
4. Click "Create"

The team slug is automatically generated and cannot be manually edited.

### Team Roles

Team members can have one of three roles:

- **Owner**: Full control over the team and its projects
- **Administrator**: Can manage team projects and members
- **Member**: Can view and work on team projects

### Adding Team Members

Team owners and administrators can add members:

1. Navigate to the team detail page
2. Click "Add" in the Members section
3. Search for a user by name or email
4. Select the user and assign a role
5. Click "Add"

### Team Projects

Team projects are created from the team detail page:

1. Navigate to the team
2. Click "Add Project"
3. Enter project details
4. The project will be accessible under the team's slug

## Endpoint Configuration

### HTTP Methods

MockHub supports all standard HTTP methods:
- GET
- POST
- PUT
- PATCH
- DELETE
- HEAD
- OPTIONS

### Route Patterns

Routes support various patterns:

- **Static routes**: `/api/users`
- **Route parameters**: `/api/users/:id` or `/api/users/{id}`
- **Multiple parameters**: `/api/users/:userId/posts/:postId`

### Response Configuration

#### Status Codes

Select from common HTTP status codes:
- 2xx: Success (200, 201, 204)
- 3xx: Redirection (301, 302, 304)
- 4xx: Client Error (400, 401, 403, 404, 422, 429)
- 5xx: Server Error (500, 502, 503)

#### Content Types

Supported content types:
- application/json
- application/xml
- text/plain
- text/html
- text/javascript
- text/css
- application/octet-stream

#### Response Headers

Add custom response headers:
1. Navigate to the Headers tab in the endpoint editor
2. Click "Add Header"
3. Enter header name and value
4. Save

Standard headers are automatically added:
- Content-Type (based on selection)
- X-Powered-By: MockHub
- Access-Control-Allow-Origin: * (if CORS is enabled)

### Response Delays

Configure response delays:
- **Min Delay**: Minimum delay in milliseconds
- **Max Delay**: Maximum delay in milliseconds

A random delay between min and max will be applied to each request.

### Validation Rules

Define validation rules for request parameters:

1. Navigate to the Rules tab
2. Click "Add Rule"
3. Configure:
   - **Parameter**: Parameter name
   - **Location**: Where to look (Query String, URL Path, Header, Body)
   - **Type**: Data type (String, Number, Boolean, Email, UUID)
   - **Required**: Whether the parameter is required
4. Save

If validation fails, a 400 Bad Request response is returned with error details.

## Dynamic Variables

### Faker Variables

Generate random data using Faker variables:

**Personal Information:**
- `{{faker.name.fullName}}` - Full name
- `{{faker.name.firstName}}` - First name
- `{{faker.name.lastName}}` - Last name
- `{{faker.internet.email}}` - Email address
- `{{faker.phone.number}}` - Phone number

**Address:**
- `{{faker.address.city}}` - City name
- `{{faker.address.country}}` - Country name
- `{{faker.address.streetAddress}}` - Street address
- `{{faker.address.zipCode}}` - Zip code

**Commerce:**
- `{{faker.commerce.productName}}` - Product name
- `{{faker.commerce.price}}` - Price

**Date and Time:**
- `{{now}}` - Current time in ISO format
- `{{nowUnix}}` - Unix timestamp
- `{{faker.date.recent}}` - Recent date
- `{{faker.date.past}}` - Past date
- `{{faker.date.future}}` - Future date

**Random Values:**
- `{{faker.random.uuid}}` - UUID
- `{{faker.random.number}}` - Random number
- `{{faker.random.boolean}}` - Boolean value

### Request Variables

Access request data:

- `{{request.params.id}}` - URL parameter value
- `{{request.query.page}}` - Query string parameter
- `{{request.body.name}}` - Request body field
- `{{request.headers.authorization}}` - Header value
- `{{request.method}}` - HTTP method
- `{{request.path}}` - Request path

### Template Logic

Use Scriban template syntax for conditional logic:

```json
{
  "status": "{{#if request.query.success}}success{{else}}error{{/if}}",
  "items": [
    {{#repeat 5}}
    {
      "id": {{@@index}},
      "name": "{{faker.commerce.productName}}"
    }{{#unless @@last}},{{/unless}}
    {{/repeat}}
  ]
}
```

For a complete list of available variables, see the Variables page in the application.

## Request Logging

### Viewing Logs

1. Navigate to Request Logs from the sidebar
2. View all requests or filter by project
3. Click on a log entry to see detailed information

### Log Details

Each log entry contains:
- Request timestamp
- HTTP method and path
- Query string parameters
- Request headers
- Request body
- Response status code
- Response headers
- Response body
- Response duration
- Client IP address

### Log Management

- **Administrators**: Can delete all logs or project-specific logs
- **Users**: Can delete logs for their own projects or team projects they belong to

### Endpoint-Specific Logs

View logs for a specific endpoint:
1. Open the endpoint editor
2. Navigate to the Logs tab
3. View all requests made to that endpoint

## Advanced Features

### JWT Token Simulation

Configure JWT validation for a project:

1. Open project settings
2. Enable JWT Validation
3. Configure:
   - **Secret**: JWT signing secret
   - **Issuer**: Token issuer
   - **Audience**: Token audience

When enabled, endpoints will validate JWT tokens from the Authorization header.

### CORS Configuration

Enable CORS for a project to allow cross-origin requests:
1. Open project settings
2. Enable CORS
3. The `Access-Control-Allow-Origin: *` header will be added to all responses

### Latency Simulation

Configure global latency simulation:
1. Open project settings
2. Enable Latency Simulation
3. Set minimum and maximum latency values

All requests to the project will have a random delay applied.

### Monaco Editor

The response body editor uses Monaco Editor with:
- Syntax highlighting based on Content-Type
- JSON formatting
- Code completion
- Sample content insertion

## Troubleshooting

### Database Issues

If you encounter database errors:

1. Delete the existing database file (`mockhub.db`)
2. Restart the application
3. The database will be recreated automatically

### Port Conflicts

If the default port (5268) is in use:

1. Edit `appsettings.json` or `appsettings.Development.json`
2. Configure the port in the Kestrel settings:
   ```json
   {
     "Kestrel": {
       "Endpoints": {
         "Http": {
           "Url": "http://localhost:5000"
         }
       }
     }
   }
   ```

### Authentication Issues

If you cannot log in:

1. Ensure the database exists and is accessible
2. Check that the administrator account was created during setup
3. Try resetting the password (if you have admin access)
4. Delete the database and run setup again

### Endpoint Not Responding

If your endpoint is not responding:

1. Verify the project is active
2. Check the route pattern matches your request
3. Verify the HTTP method matches
4. Check request logs for error details
5. Ensure the endpoint is saved correctly

### Dynamic Variables Not Working

If dynamic variables are not being processed:

1. Verify the syntax is correct (double curly braces)
2. Check that the variable name is valid
3. For request variables, ensure the parameter exists in the request
4. Check the response body format matches the Content-Type

### Team Access Issues

If you cannot access team projects:

1. Verify you are a member of the team
2. Check your team role has appropriate permissions
3. Ensure the team and project are active
4. Contact a team administrator

## Support

For issues, questions, or contributions:
- Open an issue on GitHub
- Review existing documentation
- Check the application logs for detailed error messages

