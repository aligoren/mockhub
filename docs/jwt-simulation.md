# JWT Simulation

MockHub provides comprehensive JWT (JSON Web Token) simulation capabilities for testing authentication and authorization workflows. This feature allows you to validate tokens, simulate different user roles, and create realistic authentication scenarios without external identity providers.

## Overview

JWT simulation includes:
- Token validation and parsing
- Custom claim extraction
- Role-based response switching
- Token generation for testing
- HS256 algorithm support
- Configurable issuer and audience validation

## How JWT Simulation Works

MockHub can validate JWT tokens sent in Authorization headers and use the token claims to customize responses. This enables testing of:

- Authentication workflows
- Role-based access control
- Token expiration handling
- Custom claim validation
- User-specific responses

## Enabling JWT Simulation

### Project-Level Configuration

Enable JWT validation for a project:

1. **Open Project Settings**
   - Navigate to project detail page
   - Click **"Edit"** or access settings

2. **Enable JWT Validation**
   - Check **"Enable JWT Validation"**
   - Configure JWT settings:
     - **Secret**: The signing secret/key
     - **Issuer**: Expected token issuer
     - **Audience**: Expected token audience

3. **Save Configuration**
   - JWT validation is now active for all endpoints

### Configuration Parameters

| Setting | Description | Example |
|---------|-------------|---------|
| `Secret` | HMAC signing key | `my-secret-key-123` |
| `Issuer` | Token issuer claim | `https://auth.example.com` |
| `Audience` | Token audience claim | `https://api.example.com` |

## JWT Token Structure

MockHub expects standard JWT tokens with the following structure:

```
Header: {
  "alg": "HS256",
  "typ": "JWT"
}

Payload: {
  "iss": "https://auth.example.com",
  "aud": "https://api.example.com",
  "sub": "user123",
  "exp": 1640995200,
  "iat": 1640991600,
  "role": "admin",
  "permissions": ["read", "write"],
  "customClaim": "value"
}

Signature: HMAC-SHA256(base64(header) + "." + base64(payload), secret)
```

## Using JWT in Responses

### Accessing Token Claims

Use JWT claims in your response templates:

```json
{
  "user": {
    "id": "{{jwt.claims.sub}}",
    "role": "{{jwt.claims.role}}",
    "email": "{{jwt.claims.email}}"
  },
  "permissions": {{jwt.claims.permissions | json}},
  "authenticated": {{jwt.valid}}
}
```

### Token Validation Status

Check if the token is valid:

```json
{
  "authenticated": {{jwt.valid}},
  "message": "{{#if jwt.valid}}Access granted{{else}}Access denied{{/if}}",
  "user": "{{#if jwt.valid}}{{jwt.claims.sub}}{{else}}anonymous{{/if}}"
}
```

## Available JWT Variables

### Token Status

- `{{jwt.valid}}`: Boolean indicating if token is valid
- `{{jwt.expired}}`: Boolean indicating if token is expired
- `{{jwt.signatureValid}}`: Boolean for signature validation

### Standard Claims

- `{{jwt.claims.sub}}`: Subject (user ID)
- `{{jwt.claims.iss}}`: Issuer
- `{{jwt.claims.aud}}`: Audience
- `{{jwt.claims.exp}}`: Expiration timestamp
- `{{jwt.claims.iat}}`: Issued at timestamp
- `{{jwt.claims.nbf}}`: Not before timestamp

### Custom Claims

Access any custom claims in the token:

- `{{jwt.claims.role}}`: User role
- `{{jwt.claims.permissions}}`: User permissions array
- `{{jwt.claims.department}}`: Department information
- `{{jwt.claims.customClaim}}`: Any custom claim

## Response Examples

### Role-Based Responses

Create different responses based on user roles:

```json
{
  {{#if jwt.claims.role == "admin"}}
  "data": "Admin data",
  "adminPanel": {
    "users": ["user1", "user2", "user3"],
    "systemStatus": "healthy"
  }
  {{else if jwt.claims.role == "user"}}
  "data": "User data",
  "profile": {
    "name": "{{jwt.claims.name}}",
    "email": "{{jwt.claims.email}}"
  }
  {{else}}
  "error": "Unauthorized",
  "message": "Invalid role"
  {{/if}}
}
```

### Permission-Based Access

Check specific permissions:

```json
{
  "canRead": {{jwt.claims.permissions | array.contains "read"}},
  "canWrite": {{jwt.claims.permissions | array.contains "write"}},
  "canDelete": {{jwt.claims.permissions | array.contains "delete"}},
  "actions": [
    {{#if jwt.claims.permissions | array.contains "read"}}
    "read"{{#unless jwt.claims.permissions | array.contains "write" or jwt.claims.permissions | array.contains "delete"}},{{/unless}}
    {{/if}}
    {{#if jwt.claims.permissions | array.contains "write"}}
    "write"{{#unless jwt.claims.permissions | array.contains "delete"}},{{/unless}}
    {{/if}}
    {{#if jwt.claims.permissions | array.contains "delete"}}
    "delete"
    {{/if}}
  ]
}
```

### User-Specific Data

Return user-specific information:

```json
{
  "userId": "{{jwt.claims.sub}}",
  "welcomeMessage": "Welcome back, {{jwt.claims.name}}!",
  "lastLogin": "{{jwt.claims.lastLogin}}",
  "preferences": {{jwt.claims.preferences | json}},
  "dashboard": {
    "notifications": {{jwt.claims.notificationCount | default: 0}},
    "tasks": [
      {{#repeat jwt.claims.taskCount | default: 3}}
      {
        "id": {{@@index + 1}},
        "title": "{{faker.lorem.sentence | string.rtruncate 30}}",
        "priority": "{{faker.random.arrayItem (array.create 'low' 'medium' 'high')}}",
        "assignedTo": "{{jwt.claims.sub}}"
      }{{#unless @@last}},{{/unless}}
      {{/repeat}}
    ]
  }
}
```

## Token Generation for Testing

### Creating Test Tokens

For testing, you can generate JWT tokens using online tools or libraries:

**Using jwt.io:**
1. Go to https://jwt.io
2. Select algorithm: HS256
3. Enter your secret key
4. Add payload claims
5. Copy the generated token

**Using JavaScript:**
```javascript
const jwt = require('jsonwebtoken');

const payload = {
  sub: 'user123',
  role: 'admin',
  name: 'John Doe',
  email: 'john@example.com',
  permissions: ['read', 'write']
};

const token = jwt.sign(payload, 'your-secret-key', {
  issuer: 'https://auth.example.com',
  audience: 'https://api.example.com',
  expiresIn: '1h'
});

console.log(token);
```

### Sample Test Tokens

**Admin User Token:**
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyMTIzIiwicm9sZSI6ImFkbWluIiwibmFtZSI6IkpvaG4gRG9lIiwiZW1haWwiOiJqb2huQGV4YW1wbGUuY29tIiwicGVybWlzc2lvbnMiOlsicmVhZCIsIndyaXRlIiwiZGVsZXRlIl0sImlhdCI6MTY0MDk5MTYwMCwiZXhwIjoxNjQwOTk1MjAwfQ.signature
```

**Regular User Token:**
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyNDU2Iiwicm9sZSI6InVzZXIiLCJuYW1lIjoiSmFuZSBTbWl0aCIsImVtYWlsIjoiamFuZUBleGFtcGxlLmNvbSIsInBlcm1pc3Npb25zIjpbInJlYWQiXSwiaWF0IjoxNjQwOTkxNjAwLCJleHAiOjE2NDA5OTUyMDB9.signature
```

## Error Handling

### Invalid Tokens

When JWT validation fails, MockHub provides error information:

```json
{
  "error": {
    "code": "INVALID_TOKEN",
    "message": "JWT token validation failed",
    "details": "{{jwt.error}}"
  },
  "authenticated": false
}
```

### Common JWT Errors

- **Invalid Signature**: Secret key doesn't match
- **Token Expired**: `exp` claim is in the past
- **Invalid Issuer**: `iss` claim doesn't match configured issuer
- **Invalid Audience**: `aud` claim doesn't match configured audience
- **Malformed Token**: Invalid JWT format

## Advanced Usage

### Multi-Role Scenarios

Handle complex role hierarchies:

```json
{
  "accessLevel": "{{#if jwt.claims.role == 'super-admin'}}full{{else if jwt.claims.role == 'admin'}}elevated{{else if jwt.claims.role == 'moderator'}}moderate{{else}}basic{{/if}}",
  "features": [
    {{#if jwt.claims.role == 'super-admin' or jwt.claims.role == 'admin'}}
    "user-management",
    "system-config",
    {{/if}}
    {{#if jwt.claims.role == 'super-admin' or jwt.claims.role == 'admin' or jwt.claims.role == 'moderator'}}
    "content-moderation",
    {{/if}}
    "basic-features"
  ]
}
```

### Token-Based Content Filtering

Filter content based on user permissions:

```json
{
  "articles": [
    {{#repeat 10}}
    {{#if jwt.claims.permissions | array.contains "read"}}
    {
      "id": {{@@index + 1}},
      "title": "{{faker.lorem.sentence}}",
      "content": "{{faker.lorem.paragraph}}",
      "author": "{{faker.name.fullName}}",
      "published": {{faker.random.boolean}},
      "restricted": {{#if jwt.claims.permissions | array.contains "admin"}}false{{else}}true{{/if}}
    }
    {{else}}
    {
      "id": {{@@index + 1}},
      "title": "{{faker.lorem.sentence}}",
      "content": "Content restricted - login required",
      "restricted": true
    }
    {{/if}}
    {{#unless @@last}},{{/unless}}
    {{/repeat}}
  ]
}
```

## Security Considerations

### Token Security

- **Secret Management**: Keep secrets secure and rotate regularly
- **Token Expiration**: Use appropriate expiration times
- **Claim Validation**: Validate all critical claims
- **HTTPS Only**: Always use HTTPS for token transmission

### Best Practices

- **Minimal Claims**: Include only necessary information in tokens
- **Short Expiration**: Use short-lived tokens for security
- **Refresh Tokens**: Implement token refresh mechanisms
- **Audit Logging**: Log token validation events

## API Integration

### Testing JWT Endpoints

Use tools like curl, Postman, or Insomnia:

```bash
curl -X GET \
  http://localhost:5268/project-slug/api/protected-endpoint \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Programmatic Testing

```javascript
const response = await fetch('/api/protected-endpoint', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

## Troubleshooting

### Token Validation Issues

**Problem**: Token validation fails

**Solutions**:
- Verify secret key matches
- Check token expiration
- Ensure correct issuer and audience
- Validate token format

### Claim Access Problems

**Problem**: Cannot access JWT claims in templates

**Solutions**:
- Ensure JWT validation is enabled
- Check token contains expected claims
- Verify claim names are correct
- Test with simple claim access first

### Configuration Errors

**Problem**: JWT settings not working

**Solutions**:
- Verify project settings are saved
- Check secret key format
- Ensure issuer/audience are correct
- Restart application if needed

## Performance Considerations

### Validation Overhead

- **Minimal Impact**: JWT validation is lightweight
- **Caching**: Consider caching validation results
- **Async Processing**: Validation doesn't block responses

### Optimization Tips

- Use short secrets for better performance
- Minimize claim payload size
- Cache frequently validated tokens
- Monitor validation performance

## Future Enhancements

*Planned features:*
- RSA token validation support
- Token refresh simulation
- OAuth2 flow simulation
- Multiple issuer support
- Token introspection endpoints
