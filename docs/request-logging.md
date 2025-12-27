# Request Logging

MockHub provides comprehensive request logging capabilities to monitor API usage, debug issues, and analyze traffic patterns. This section covers real-time logging, persistent storage, log management, and analytics features.

## Overview

Request logging includes:
- Real-time request monitoring with SignalR
- Persistent log storage in database
- Detailed request/response information
- Role-based log access permissions
- Log filtering and search capabilities
- Automatic log cleanup options

## Enabling Request Logging

### Project-Level Configuration

Enable logging for individual projects:

1. **Open Project Settings**
   - Navigate to project detail page
   - Click **"Edit"** or settings icon

2. **Enable Logging**
   - Check **"Enable Request Logging"**
   - Save changes

3. **Verification**
   - Make test requests to endpoints
   - Check logs appear in real-time

### Global Project Settings

Logging settings apply to all endpoints in the project:
- **Enabled**: All requests are logged
- **Disabled**: No requests are logged for this project
- **Per-Endpoint Control**: Future feature for granular control

## Real-Time Monitoring

### SignalR Integration

MockHub uses SignalR for real-time log updates:

- **Live Updates**: New requests appear instantly in the UI
- **WebSocket Connection**: Persistent connection for real-time data
- **Automatic Reconnection**: Handles network interruptions
- **Performance Optimized**: Minimal overhead on server

### Live Log Viewer

Access real-time logs through the web interface:

1. **Navigate to Logs**
   - Click **"Request Logs"** in sidebar
   - Or access from project detail pages

2. **Real-Time Updates**
   - New requests appear automatically
   - No page refresh required
   - Color-coded status indicators

3. **Filter Options**
   - Filter by project
   - Filter by time range
   - Search by endpoint or status

## Log Data Structure

### Request Information

Each log entry contains comprehensive request data:

```json
{
  "id": "log_123456",
  "timestamp": "2024-12-24T18:30:00Z",
  "projectId": "proj_789",
  "projectName": "User API",
  "endpointId": "end_456",
  "endpointName": "Get User",
  "method": "GET",
  "path": "/api/users/123",
  "queryString": "?include=profile",
  "fullUrl": "/api/users/123?include=profile",
  "headers": {
    "User-Agent": "Mozilla/5.0...",
    "Authorization": "Bearer token...",
    "Content-Type": "application/json"
  },
  "body": null,
  "clientIp": "192.168.1.100",
  "userAgent": "Mozilla/5.0...",
  "responseStatusCode": 200,
  "responseHeaders": {
    "Content-Type": "application/json",
    "X-Powered-By": "MockHub"
  },
  "responseBody": "{\"id\":123,\"name\":\"John Doe\"}",
  "durationMs": 45,
  "createdAt": "2024-12-24T18:30:00Z"
}
```

### Log Fields Explained

| Field | Description | Example |
|-------|-------------|---------|
| `id` | Unique log identifier | `log_123456` |
| `timestamp` | Request timestamp (UTC) | `2024-12-24T18:30:00Z` |
| `projectId` | Associated project ID | `proj_789` |
| `endpointId` | Associated endpoint ID | `end_456` |
| `method` | HTTP method | `GET`, `POST`, etc. |
| `path` | Request path | `/api/users/123` |
| `queryString` | Query parameters | `?page=1&limit=10` |
| `headers` | Request headers | Object with header key-value pairs |
| `body` | Request body | JSON string or null |
| `clientIp` | Client IP address | `192.168.1.100` |
| `responseStatusCode` | HTTP response status | `200`, `404`, etc. |
| `responseBody` | Response content | JSON string |
| `durationMs` | Response time in milliseconds | `45` |

## Log Access Permissions

### Role-Based Access Control

Different user roles have different log access levels:

#### Administrators
- **Full Access**: View all logs across all projects and teams
- **Delete Permissions**: Delete any logs or clear entire log history
- **System Monitoring**: Access to system-wide analytics

#### Team Members
- **Team Project Logs**: View logs for projects within their teams
- **Owned Project Logs**: View logs for personally owned projects
- **Limited Deletion**: Delete logs for their own projects only

#### Access Matrix

| Permission | Admin | Team Owner | Team Admin | Team Member |
|------------|-------|------------|------------|-------------|
| View All Logs | ✓ | ✗ | ✗ | ✗ |
| View Team Logs | ✓ | ✓ | ✓ | ✓ |
| View Own Logs | ✓ | ✓ | ✓ | ✓ |
| Delete All Logs | ✓ | ✗ | ✗ | ✗ |
| Delete Team Logs | ✓ | ✓ | ✓ | ✗ |
| Delete Own Logs | ✓ | ✓ | ✓ | ✓ |

## Log Management

### Viewing Logs

#### Global Log View

Access all accessible logs:
1. Navigate to **"Request Logs"** in sidebar
2. View paginated list of all requests
3. Filter by project, time range, or search terms

#### Project-Specific Logs

View logs for specific projects:
1. Open project detail page
2. Navigate to **"Logs"** tab
3. View requests specific to that project

#### Endpoint-Specific Logs

View logs for individual endpoints:
1. Open endpoint editor
2. Navigate to **"Logs"** tab
3. See all requests made to that endpoint

### Log Filtering

Filter logs by multiple criteria:

- **Project**: Select specific project
- **Time Range**: Last hour, day, week, or custom range
- **HTTP Method**: GET, POST, PUT, DELETE, etc.
- **Status Code**: Filter by response status (2xx, 4xx, 5xx)
- **Search**: Search in URLs, headers, or response bodies

### Log Details View

Click any log entry to view detailed information:

- **Request Details**: Full request information
- **Response Details**: Complete response data
- **Headers**: Both request and response headers
- **Body Content**: Formatted JSON/XML bodies
- **Timing Information**: Request duration and timestamps

## Log Storage and Retention

### Database Storage

- **Persistent Storage**: All logs stored in SQLite database
- **Indexed Fields**: Optimized queries on common fields
- **Compression**: Large response bodies may be compressed
- **Backup**: Logs included in database backups

### Retention Policies

*Future feature - configurable log retention periods*

### Storage Considerations

- **Database Size**: Logs can accumulate quickly
- **Performance**: Large log tables may impact query performance
- **Cleanup**: Regular log maintenance recommended
- **Archiving**: Export old logs for long-term storage

## Log Analytics

### Basic Analytics

View basic statistics:

- **Total Requests**: Overall request count
- **Success Rate**: Percentage of successful responses
- **Average Response Time**: Mean response duration
- **Popular Endpoints**: Most frequently called endpoints

### Advanced Analytics

*Future features:*
- Response time distributions
- Geographic request analysis
- Error pattern detection
- Usage trend analysis

## Log Export and Backup

### Export Options

*Future feature - export logs to various formats*

### Data Portability

- **JSON Export**: Export logs as JSON for analysis
- **CSV Export**: Tabular format for spreadsheets
- **API Access**: Programmatic access to log data

## Security Considerations

### Data Privacy

- **Request Bodies**: May contain sensitive information
- **Authentication Headers**: Tokens and credentials logged
- **PII Data**: Personal information in requests/responses

### Access Control

- **Role-Based Access**: Appropriate permissions for different roles
- **Audit Trail**: Log access itself is audited
- **Encryption**: Logs encrypted at rest

### Data Sanitization

- **Token Masking**: JWT tokens partially masked
- **Password Removal**: Password fields removed from logs
- **Sensitive Headers**: Certain headers excluded or masked

## Performance Impact

### Logging Overhead

- **Minimal Impact**: Designed for low overhead
- **Async Processing**: Logging doesn't block responses
- **Configurable**: Can be disabled per project
- **Resource Usage**: Monitor database size and query performance

### Optimization Strategies

- **Indexing**: Proper database indexing for fast queries
- **Pagination**: Large result sets paginated
- **Compression**: Response bodies compressed for storage
- **Cleanup**: Regular log maintenance and archiving

## API Endpoints

### Log Management APIs

```
GET    /api/logs                           # Get logs (with filtering)
GET    /api/logs/{id}                      # Get specific log entry
DELETE /api/logs/{id}                      # Delete specific log
DELETE /api/logs                           # Clear all accessible logs

GET    /api/projects/{id}/logs             # Get project logs
DELETE /api/projects/{id}/logs             # Clear project logs

GET    /api/projects/{id}/endpoints/{eid}/logs  # Get endpoint logs
```

### Real-Time APIs (SignalR)

```
Hub: /mockhub
Method: ReceiveLog
Data: LogEntry object
```

## Troubleshooting

### Logs Not Appearing

**Problem**: Requests not showing in logs

**Solutions**:
- Verify logging is enabled for the project
- Check project status (must be active)
- Ensure endpoint is configured correctly
- Verify network connectivity for real-time updates

### Missing Request Details

**Problem**: Some log fields are empty

**Solutions**:
- Check if request contains expected headers/body
- Verify endpoint configuration
- Ensure proper request format
- Check for request size limits

### Performance Issues

**Problem**: Log queries are slow

**Solutions**:
- Add more specific filters
- Use pagination for large result sets
- Consider log cleanup/archiving
- Check database performance

### Real-Time Updates Not Working

**Problem**: Logs not updating in real-time

**Solutions**:
- Check WebSocket connection
- Verify SignalR hub is running
- Check browser console for errors
- Ensure firewall allows WebSocket connections

### Permission Errors

**Problem**: Cannot access certain logs

**Solutions**:
- Verify user role and team membership
- Check project ownership
- Ensure user account is active
- Contact administrator for access issues

## Best Practices

### Log Management
- Enable logging only for necessary projects
- Regularly review and clean old logs
- Use appropriate filters for analysis
- Archive logs for long-term storage

### Security
- Be aware of sensitive data in logs
- Use appropriate access controls
- Regularly audit log access
- Implement data retention policies

### Performance
- Monitor database size and performance
- Use indexing for frequent queries
- Implement log rotation
- Consider log aggregation services

### Monitoring
- Set up alerts for unusual patterns
- Monitor error rates and response times
- Use logs for debugging and optimization
- Track API usage patterns

### Compliance
- Understand data retention requirements
- Implement appropriate access controls
- Be aware of PII in logged data
- Use encryption for sensitive logs
