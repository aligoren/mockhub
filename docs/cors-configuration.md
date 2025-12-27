# CORS Configuration

MockHub provides flexible Cross-Origin Resource Sharing (CORS) configuration to enable secure cross-origin requests from web browsers. This feature allows you to control which origins, methods, and headers are permitted for cross-origin requests to your mock APIs.

## Overview

CORS configuration in MockHub includes:
- Origin control for allowed domains
- HTTP method restrictions
- Custom header support
- Preflight request handling
- Automatic header injection

## What is CORS?

Cross-Origin Resource Sharing (CORS) is a security feature implemented by web browsers that controls how web pages in one domain can request resources from another domain. Without proper CORS configuration, browsers block cross-origin requests for security reasons.

## Enabling CORS in MockHub

### Project-Level Configuration

Configure CORS for individual projects:

1. **Open Project Settings**
   - Navigate to project detail page
   - Click **"Edit"** or access settings

2. **Enable CORS**
   - Check **"Enable CORS"**
   - Save changes

3. **Automatic Headers**
   - MockHub automatically adds CORS headers to all responses
   - No additional configuration required

### Global vs. Per-Project CORS

- **Project-Level**: Each project can have its own CORS settings
- **Automatic Application**: When enabled, applies to all endpoints in the project
- **Header Injection**: Headers added automatically to all responses

## CORS Headers

MockHub automatically adds the following CORS headers when enabled:

### Standard CORS Headers

```
Access-Control-Allow-Origin: *
Access-Control-Allow-Methods: GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS
Access-Control-Allow-Headers: Content-Type, Authorization, X-Requested-With
Access-Control-Max-Age: 86400
```

### Preflight Request Handling

For OPTIONS requests (preflight), MockHub returns:

```http
HTTP/1.1 200 OK
Access-Control-Allow-Origin: *
Access-Control-Allow-Methods: GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS
Access-Control-Allow-Headers: Content-Type, Authorization, X-Requested-With
Access-Control-Max-Age: 86400
Content-Length: 0
```

## CORS Use Cases

### Web Application Development

Enable CORS when your mock API needs to be called from a web application:

```javascript
// Frontend application at http://localhost:3000
// Mock API at http://localhost:5268

fetch('http://localhost:5268/my-project/api/users', {
  method: 'GET',
  headers: {
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => console.log(data));
```

### API Testing Tools

Allow CORS for API testing tools like Postman, Insomnia, or custom web-based testers.

### Development Environments

Different environments may require different CORS configurations:

- **Development**: Allow all origins (`*`)
- **Staging**: Allow specific domains
- **Production**: Restrict to production domains only

## CORS in Action

### Without CORS

Browser blocks the request:

```
Cross-Origin Request Blocked: The Same Origin Policy disallows reading the remote resource at http://localhost:5268/api/users. (Reason: CORS header 'Access-Control-Allow-Origin' missing).
```

### With CORS Enabled

Request succeeds with proper headers:

```http
HTTP/1.1 200 OK
Access-Control-Allow-Origin: *
Content-Type: application/json
X-Powered-By: MockHub

{"users": [...]}
```

## Advanced CORS Scenarios

### Custom Headers

If your API requires custom headers, ensure they're included:

```javascript
fetch('/api/data', {
  headers: {
    'X-Custom-Header': 'value',
    'X-API-Key': 'key123'
  }
});
```

MockHub's default configuration includes common headers. For additional headers, the current implementation allows all specified headers.

### Authentication Headers

CORS is particularly important for authenticated requests:

```javascript
fetch('/api/protected', {
  headers: {
    'Authorization': 'Bearer token123',
    'Content-Type': 'application/json'
  }
});
```

## Testing CORS Configuration

### Browser Developer Tools

Check CORS headers in browser Network tab:

1. Open browser Developer Tools
2. Make a cross-origin request
3. Check Response Headers for CORS headers
4. Verify `Access-Control-Allow-Origin` is present

### Command Line Testing

Test CORS with curl:

```bash
# Test preflight request
curl -X OPTIONS \
  -H "Origin: http://localhost:3000" \
  -H "Access-Control-Request-Method: POST" \
  -H "Access-Control-Request-Headers: Content-Type" \
  -v \
  http://localhost:5268/project-slug/api/endpoint

# Check for CORS headers in response
```

### Online CORS Testers

Use online tools to test CORS configuration:
- https://cors-test.appspot.com/
- https://www.test-cors.org/

## Common CORS Issues

### Missing CORS Headers

**Problem**: Browser blocks cross-origin requests

**Solutions**:
- Enable CORS in project settings
- Verify project is active
- Check browser console for specific error messages
- Ensure correct endpoint URL

### Preflight Request Failures

**Problem**: OPTIONS requests fail

**Solutions**:
- CORS automatically handles OPTIONS requests
- Check if endpoint exists
- Verify project CORS settings
- Test with simple GET request first

### Custom Header Issues

**Problem**: Custom headers not allowed

**Solutions**:
- MockHub allows common headers by default
- For additional headers, ensure they're specified in requests
- Check browser error messages for specific header issues

## Security Considerations

### Origin Restrictions

Current implementation allows all origins (`*`) for simplicity. In production:

- **Restrict Origins**: Limit to specific domains
- **HTTPS Only**: Require secure connections
- **Domain Validation**: Validate origin domains

### Header Limitations

- **Sensitive Headers**: Some headers cannot be set via CORS
- **Credential Headers**: Authorization headers require special handling
- **Safe Headers**: Only certain headers are considered "safe"

### Best Practices

- **Minimal Permissions**: Use restrictive CORS policies
- **HTTPS Enforcement**: Always use HTTPS for CORS requests
- **Origin Validation**: Validate request origins
- **Header Whitelisting**: Only allow necessary headers

## Performance Impact

### CORS Overhead

- **Minimal Impact**: CORS headers add negligible overhead
- **Preflight Caching**: Browsers cache preflight responses
- **Header Size**: Small increase in response size

### Optimization

- **Cache Preflight**: Browsers cache OPTIONS responses
- **Header Compression**: Use HTTP compression
- **CDN Considerations**: CORS headers may affect caching

## Troubleshooting

### CORS Errors in Browser

**Problem**: Browser shows CORS-related errors

**Solutions**:
1. Check browser console for specific error messages
2. Verify CORS is enabled in project settings
3. Test with simple requests first
4. Check network tab for missing headers

### Development vs. Production

**Different Behaviors**:
- Development: More permissive CORS settings
- Production: Stricter CORS policies
- Testing: Ensure test environments match production

### Framework-Specific Issues

**React/Vue/Angular**:
- Development servers may have different CORS handling
- Production builds may behave differently
- Check framework documentation for CORS handling

**API Testing Tools**:
- Postman/Insomnia may bypass CORS in development
- Browser-based tools respect CORS policies
- Use appropriate testing tools for each scenario

## API Integration

### CORS Headers in Responses

MockHub automatically includes CORS headers in all responses when enabled:

```http
HTTP/1.1 200 OK
Access-Control-Allow-Origin: *
Access-Control-Allow-Methods: GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS
Access-Control-Allow-Headers: Content-Type, Authorization, X-Requested-With
Content-Type: application/json
X-Powered-By: MockHub

{...response body...}
```

### Programmatic CORS Testing

```javascript
// Test CORS configuration
async function testCORS() {
  try {
    const response = await fetch('http://localhost:5268/project/api/test', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    });
    
    // Check CORS headers
    console.log('CORS Origin:', response.headers.get('Access-Control-Allow-Origin'));
    console.log('CORS Methods:', response.headers.get('Access-Control-Allow-Methods'));
    
    const data = await response.json();
    console.log('Response:', data);
  } catch (error) {
    console.error('CORS Error:', error);
  }
}
```

## Future Enhancements

*Planned CORS features:*
- Origin-specific allowlists
- Method-specific permissions
- Header-specific controls
- Credential support
- Custom CORS policies per endpoint

## Best Practices

### Development
- Enable CORS during development
- Test with various origins
- Use permissive settings for flexibility
- Document CORS requirements

### Production
- Restrict origins to necessary domains
- Use HTTPS for all CORS requests
- Implement proper error handling
- Monitor CORS-related issues

### Security
- Validate all origins in production
- Use restrictive header policies
- Implement rate limiting for CORS requests
- Log CORS-related security events

### Testing
- Test CORS with different browsers
- Verify preflight request handling
- Test with various header combinations
- Include CORS testing in CI/CD pipelines
