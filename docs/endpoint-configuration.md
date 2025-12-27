# Endpoint Configuration

Endpoints are the core components of your mock APIs. This section covers creating, configuring, and managing HTTP endpoints with various features like route parameters, validation rules, custom responses, and dynamic content.

## Overview

MockHub endpoints support:
- All standard HTTP methods
- Flexible route patterns with parameters
- Custom response configuration
- Request validation and transformation
- Dynamic response generation
- Real-time request logging

## HTTP Methods

MockHub supports all standard HTTP methods:

- **GET**: Retrieve data
- **POST**: Create new resources
- **PUT**: Update existing resources
- **PATCH**: Partial resource updates
- **DELETE**: Remove resources
- **HEAD**: Retrieve headers only
- **OPTIONS**: CORS preflight requests

## Route Patterns

### Basic Routes

Simple static routes:
```
/api/users
/api/products
/health
```

### Route Parameters

Dynamic parameters in routes:
```
/api/users/{id}
/api/users/:userId
/api/posts/{postId}/comments/{commentId}
```

**Parameter Types:**
- `{param}`: Curly brace notation
- `:param`: Colon notation
- Both are equivalent and interchangeable

### Wildcard Routes

Match multiple path segments:
```
/api/**          # Matches /api/users, /api/users/123, /api/products/electronics
/api/v*/users    # Matches /api/v1/users, /api/v2/users, /api/v3/users
```

### Route Examples

| Route Pattern | Matches | Example URL |
|---------------|---------|-------------|
| `/api/users` | Exact match | `/api/users` |
| `/api/users/{id}` | Single parameter | `/api/users/123` |
| `/api/users/{id}/posts/{postId}` | Multiple parameters | `/api/users/123/posts/456` |
| `/api/v*/users` | Version wildcard | `/api/v1/users`, `/api/v2/users` |
| `/api/**` | Any sub-path | `/api/users/123/profile` |

## Creating Endpoints

### Basic Configuration

1. **Navigate to Project**
   - Open your project from the Projects page

2. **Add Endpoint**
   - Click **"Add Endpoint"**
   - Configure basic settings:
     - **Name**: Descriptive endpoint name
     - **HTTP Method**: Select from dropdown
     - **Route**: Define the URL path
     - **Description**: Optional endpoint description

3. **Configure Response**
   - **Status Code**: HTTP response status
   - **Content-Type**: Response content type
   - **Response Body**: The response content

4. **Save Endpoint**
   - Click **"Create"**
   - Endpoint becomes immediately active

## Response Configuration

### Status Codes

Common HTTP status codes are available:

**2xx Success:**
- 200 OK
- 201 Created
- 204 No Content

**3xx Redirection:**
- 301 Moved Permanently
- 302 Found
- 304 Not Modified

**4xx Client Error:**
- 400 Bad Request
- 401 Unauthorized
- 403 Forbidden
- 404 Not Found
- 422 Unprocessable Entity
- 429 Too Many Requests

**5xx Server Error:**
- 500 Internal Server Error
- 502 Bad Gateway
- 503 Service Unavailable

### Content Types

Supported response content types:
- `application/json` (default)
- `application/xml`
- `text/plain`
- `text/html`
- `text/javascript`
- `text/css`
- `application/octet-stream`

### Response Body

The response body supports:
- **Plain Text**: Simple string responses
- **JSON**: Structured data with dynamic variables
- **XML**: XML formatted responses
- **HTML**: Web page content
- **Custom Content**: Any text-based content

### Response Headers

Add custom response headers:
1. Navigate to **Headers** tab in endpoint editor
2. Click **"Add Header"**
3. Enter header name and value
4. Save

**Automatically Added Headers:**
- `Content-Type`: Based on selected content type
- `X-Powered-By`: MockHub
- `Access-Control-Allow-Origin`: * (if CORS enabled on project)

## Response Delays

### Static Delays

Set fixed response delays:
- **Min Delay**: Delay in milliseconds (0 = instant response)
- **Max Delay**: Same as min delay for consistent timing

### Random Delays

Create realistic response times:
- Set different min/max values
- Random delay applied to each request
- Useful for testing loading states and timeouts

## Validation Rules

Define validation rules for request parameters:

### Parameter Types

MockHub supports validation for different parameter locations:

- **Query String**: URL query parameters (`?page=1&limit=10`)
- **URL Path**: Route parameters (`/users/123`)
- **Headers**: HTTP headers (`Authorization: Bearer token`)
- **Body**: Request body fields (JSON/XML)

### Data Types

Available validation types:
- **String**: Text validation
- **Number**: Numeric validation
- **Boolean**: True/false validation
- **Email**: Email format validation
- **UUID**: UUID format validation

### Adding Validation Rules

1. Navigate to **Rules** tab in endpoint editor
2. Click **"Add Rule"**
3. Configure:
   - **Parameter Name**: Parameter to validate
   - **Location**: Where to find the parameter
   - **Data Type**: Expected data type
   - **Required**: Whether parameter is mandatory
4. Save

### Validation Examples

| Parameter | Location | Type | Required | Example |
|-----------|----------|------|----------|---------|
| `id` | Path | UUID | Yes | `/api/users/550e8400-e29b-41d4-a716-446655440000` |
| `page` | Query | Number | No | `/api/users?page=1` |
| `email` | Body | Email | Yes | `{"email": "user@example.com"}` |
| `active` | Query | Boolean | No | `/api/users?active=true` |

### Validation Behavior

- **Valid Requests**: Process normally with configured response
- **Invalid Requests**: Return 400 Bad Request with error details
- **Missing Required Parameters**: Validation error with specific message
- **Type Mismatches**: Error indicating expected vs actual type

## Dynamic Responses

### Using Variables

Endpoints support dynamic content through variables:

```json
{
  "id": "{{faker.random.uuid}}",
  "name": "{{faker.name.fullName}}",
  "email": "{{faker.internet.email}}",
  "createdAt": "{{now}}",
  "userId": "{{request.params.id}}",
  "page": "{{request.query.page}}"
}
```

### Conditional Logic

Use Scriban template syntax for conditions:

```json
{
  "status": "{{#if request.query.success}}success{{else}}error{{/if}}",
  "data": "{{#if request.params.id}}{{faker.lorem.paragraph}}{{else}}No data{{/if}}"
}
```

### Arrays and Loops

Generate dynamic arrays:

```json
{
  "items": [
    {{#repeat request.query.count | default: 5}}
    {
      "id": {{@@index}},
      "name": "{{faker.commerce.productName}}",
      "price": {{faker.commerce.price}}
    }{{#unless @@last}},{{/unless}}
    {{/repeat}}
  ]
}
```

## Request Logging

### Endpoint-Specific Logs

View logs for individual endpoints:
1. Open endpoint editor
2. Navigate to **Logs** tab
3. View recent requests to this endpoint
4. See request details, response status, and timing

### Log Details

Each log entry includes:
- **Timestamp**: When request was made
- **HTTP Method**: Request method used
- **Path**: Full request path with query parameters
- **Status Code**: Response status returned
- **Duration**: Response time in milliseconds
- **Client IP**: Request source IP address

## Advanced Features

### Request Transformation

Access and transform request data in responses:

```json
{
  "originalPath": "{{request.path}}",
  "method": "{{request.method}}",
  "userAgent": "{{request.headers.User-Agent}}",
  "authToken": "{{request.headers.Authorization}}",
  "requestBody": {{request.body | json}}
}
```

### Error Simulation

Simulate different error conditions:

```json
{
  "error": "Resource not found",
  "code": "RESOURCE_NOT_FOUND",
  "timestamp": "{{now}}",
  "requestId": "{{request.id}}"
}
```

### Pagination Support

Mock pagination responses:

```json
{
  "data": [
    {{#repeat 10}}
    {
      "id": {{@@index + 1}},
      "name": "{{faker.name.fullName}}"
    }{{#unless @@last}},{{/unless}}
    {{/repeat}}
  ],
  "pagination": {
    "page": {{request.query.page | default: 1}},
    "limit": {{request.query.limit | default: 10}},
    "total": 100
  }
}
```

## Endpoint Management

### Editing Endpoints

Modify existing endpoints:
1. Click on endpoint in project view
2. Update any configuration
3. Changes apply immediately

### Endpoint Status

All endpoints are active by default. Inactive projects disable all endpoints.

### Deleting Endpoints

Remove endpoints permanently:
1. Click delete button on endpoint
2. Confirm deletion
3. Endpoint and logs are removed

### Endpoint Cloning

*Future feature - duplicate endpoints with similar configurations*

## Performance Considerations

### Response Size

- Keep response bodies reasonable for testing
- Use pagination for large datasets
- Consider compression for large responses

### Validation Overhead

- Minimize complex validation rules
- Use appropriate data types
- Avoid excessive regex patterns

### Dynamic Content

- Cache expensive operations if possible
- Use efficient template expressions
- Limit array sizes in responses

## API Endpoints

### Endpoint Management APIs

```
POST   /api/projects/{projectId}/endpoints           # Create endpoint
GET    /api/projects/{projectId}/endpoints           # List endpoints
GET    /api/projects/{projectId}/endpoints/{id}      # Get endpoint
PUT    /api/projects/{projectId}/endpoints/{id}      # Update endpoint
DELETE /api/projects/{projectId}/endpoints/{id}      # Delete endpoint

GET    /api/projects/{projectId}/endpoints/{id}/logs # Get endpoint logs
```

## Troubleshooting

### Endpoint Not Responding

**Problem**: Endpoint returns 404 or unexpected response

**Solutions**:
- Verify HTTP method matches
- Check route pattern matches request
- Ensure project is active
- Verify URL structure for team/personal projects

### Validation Errors

**Problem**: Requests fail validation

**Solutions**:
- Check parameter names and locations
- Verify data types match expectations
- Ensure required parameters are provided
- Review validation rule configuration

### Dynamic Variables Not Working

**Problem**: Variables not being replaced in responses

**Solutions**:
- Verify syntax (double curly braces)
- Check variable names are correct
- Ensure request parameters exist
- Test with simple variables first

### Performance Issues

**Problem**: Slow response times

**Solutions**:
- Reduce response body size
- Minimize validation rules
- Check for complex template expressions
- Review delay configurations

## Best Practices

### Route Design
- Use RESTful conventions
- Keep routes simple and intuitive
- Use consistent parameter naming
- Document route purposes

### Response Design
- Use appropriate HTTP status codes
- Include relevant headers
- Structure JSON responses consistently
- Use meaningful error messages

### Validation
- Validate only necessary parameters
- Use appropriate data types
- Provide clear error messages
- Test validation thoroughly

### Dynamic Content
- Use variables for realistic data
- Test edge cases in templates
- Document variable usage
- Keep templates maintainable

### Documentation
- Document endpoint purposes
- Include example requests/responses
- Note special behaviors
- Update documentation with changes
