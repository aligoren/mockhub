# Path-Based Routing

MockHub uses a sophisticated path-based routing system that allows multiple projects to run simultaneously on the same server instance. Unlike traditional port-based approaches, path-based routing provides isolated API environments through URL paths, making it ideal for complex development scenarios.

## Overview

Path-based routing provides:
- Multiple projects on single server instance
- URL path isolation for different environments
- Team and personal project segregation
- RESTful URL structures
- Automatic route resolution

## URL Structure

MockHub organizes APIs using hierarchical URL paths:

### Personal Projects
```
http://localhost:5268/{project-slug}/api/{endpoint-path}
```

**Examples:**
- `http://localhost:5268/user-api/api/users`
- `http://localhost:5268/product-service/api/products/123`

### Team Projects
```
http://localhost:5268/{team-slug}/{project-slug}/api/{endpoint-path}
```

**Examples:**
- `http://localhost:5268/backend-team/user-api/api/users`
- `http://localhost:5268/mobile-team/product-api/api/categories`

## Route Components

### Project Slug

- **Auto-generated**: Created from project name
- **Unique within scope**: Personal projects unique globally, team projects unique within team
- **URL-friendly**: Lowercase, hyphens for spaces, special chars removed
- **Immutable**: Cannot be manually changed (updates automatically with name changes)

**Generation Rules:**
- Convert to lowercase
- Replace spaces with hyphens
- Remove special characters
- Maximum 50 characters
- Append numbers for uniqueness: `user-api-2`

### Team Slug

- **Team identifier**: Unique across the system
- **Auto-generated**: Based on team name
- **Hierarchical routing**: Creates team namespaces

### API Prefix

- **Standard prefix**: `/api/` separates MockHub paths from API endpoints
- **Consistent structure**: All endpoints use this prefix
- **Route isolation**: Prevents conflicts with MockHub UI routes

### Endpoint Paths

- **Flexible routing**: Support for parameters, wildcards, and complex patterns
- **RESTful design**: Standard REST conventions
- **Dynamic matching**: Regex and pattern-based matching

## Route Resolution

### Routing Algorithm

MockHub resolves routes through a multi-step process:

1. **Path Parsing**: Extract team slug, project slug, and endpoint path
2. **Scope Determination**: Identify if personal or team project
3. **Project Lookup**: Find project by slug within appropriate scope
4. **Endpoint Matching**: Match endpoint patterns within the project
5. **Parameter Extraction**: Parse route parameters and query strings

### Resolution Priority

Routes are resolved in hierarchical order:

1. **Team Routes**: `/team-slug/project-slug/api/endpoint`
2. **Personal Routes**: `/project-slug/api/endpoint`
3. **UI Routes**: `/teams`, `/projects`, `/logs`, etc.

### Example Resolution

**Request:** `GET /backend-team/user-api/api/users/123`

**Resolution Steps:**
1. Extract: `team-slug=backend-team`, `project-slug=user-api`, `endpoint=api/users/123`
2. Find team: Lookup "backend-team" in teams table
3. Find project: Lookup "user-api" within backend-team projects
4. Find endpoint: Match "api/users/{id}" pattern
5. Extract parameters: `id = 123`
6. Process response: Generate response using endpoint configuration

## Route Patterns

### Static Routes

Fixed path segments:
```
/api/users
/api/products
/api/health
/api/status
```

### Parameter Routes

Dynamic path parameters:
```
/api/users/{id}
/api/users/{userId}/posts/{postId}
/api/categories/{category}/products/{product}
```

**Parameter Types:**
- `{param}`: Required parameter
- `:param`: Alternative syntax (equivalent)

### Wildcard Routes

Match multiple path segments:
```
/api/**          # Matches any sub-path
/api/v*/users    # Matches version prefixes
/api/*/users     # Single segment wildcards
```

### Complex Patterns

Advanced routing patterns:
```
/api/v{version}/users     # Version parameters
/api/users/{id}.{format}  # File extensions
/api/search/{query}       # Search queries
```

## Route Matching

### Exact Matching

Routes match exactly as defined:
- `/api/users` matches `/api/users`
- `/api/users/123` matches `/api/users/{id}`

### Parameter Extraction

Route parameters are extracted and made available:
- `/api/users/123` with pattern `/api/users/{id}` → `id = 123`
- `/api/posts/456/comments/789` with pattern `/api/posts/{postId}/comments/{commentId}` → `postId = 456, commentId = 789`

### Query Parameters

URL query strings are parsed separately:
- `/api/users?page=1&limit=10` → `page = 1, limit = 10`
- Available in templates as `{{request.query.param}}`

## Route Conflicts

### Conflict Resolution

MockHub handles route conflicts through prioritization:

1. **Specificity**: More specific routes take precedence
2. **Parameter Count**: Routes with more parameters prioritized
3. **Definition Order**: Earlier defined routes (by creation time)
4. **Scope Isolation**: Team and personal projects don't conflict

### Conflict Examples

**No Conflicts:**
- `/api/users` (personal project A)
- `/backend/api/users` (personal project B)
- `/mobile-team/api/users` (team project)

**Potential Conflicts (within same project):**
- `/api/users/{id}` and `/api/users/profile` → Specific route wins
- `/api/**` and `/api/users` → Specific route wins

## Route Debugging

### Route Inspection

Debug routing issues:

1. **Check Project Slug**: Verify correct project slug in URL
2. **Verify Team Context**: Ensure team slug for team projects
3. **Endpoint Matching**: Check endpoint patterns match request
4. **Parameter Extraction**: Verify parameters are correctly parsed

### Common Issues

**404 Errors:**
- Incorrect project slug
- Wrong team slug
- Endpoint not configured
- Project deactivated

**Parameter Issues:**
- Route pattern doesn't match request
- Parameter names incorrect
- Query string parsing failures

**Conflict Issues:**
- Multiple endpoints match same path
- Wildcard routes interfering with specific routes

## Performance Considerations

### Route Resolution Performance

- **Optimized Matching**: Fast route resolution algorithm
- **Caching**: Route patterns cached for performance
- **Database Queries**: Minimal database lookups
- **Concurrent Access**: Thread-safe route resolution

### Scaling Considerations

- **Project Count**: Performance scales with project numbers
- **Endpoint Complexity**: Simple patterns perform better
- **Wildcard Usage**: Wildcards may impact resolution speed
- **Cache Invalidation**: Route cache updates on configuration changes

## Security Implications

### Route Isolation

- **Project Separation**: Complete isolation between projects
- **Team Boundaries**: Team projects only accessible to team members
- **Access Control**: Route-level permissions through project ownership

### URL Security

- **Predictable URLs**: Slugs prevent enumeration attacks
- **Access Validation**: All requests validated for permissions
- **Logging**: All route access logged for audit trails

## API Design Best Practices

### RESTful Design

Follow REST conventions:
```
/api/users          # GET: List users, POST: Create user
/api/users/{id}     # GET: Get user, PUT: Update user, DELETE: Delete user
/api/users/{id}/posts  # User's posts
```

### Resource Hierarchy

Organize resources logically:
```
/api/organizations/{orgId}/users/{userId}
/api/projects/{projectId}/tasks/{taskId}
/api/categories/{categoryId}/products/{productId}
```

### Versioning Strategies

Handle API versioning:
```
/api/v1/users       # Version in path
/api/users?v=1      # Version in query (less preferred)
/api/v1/users/{id}  # Versioned individual resources
```

## Migration from Port-Based

### Port to Path Migration

Converting from port-based to path-based routing:

**Old (Port-based):**
- Project A: `http://localhost:5001/api/users`
- Project B: `http://localhost:5002/api/users`

**New (Path-based):**
- Project A: `http://localhost:5268/project-a/api/users`
- Project B: `http://localhost:5268/project-b/api/users`

### Migration Steps

1. **Update Client Code**: Change base URLs
2. **Update Documentation**: Reflect new URL structure
3. **Test Endpoints**: Verify all endpoints work with new paths
4. **Update Bookmarks**: Any saved URLs need updating

## Advanced Routing

### Custom Route Patterns

*Future features:*
- Regex-based route matching
- Custom route constraints
- Route prioritization rules
- Dynamic route generation

### Route Middleware

*Future features:*
- Custom route processing
- Request transformation
- Response modification
- Route-specific middleware

## Troubleshooting

### Route Not Found

**Problem**: 404 errors for valid endpoints

**Solutions**:
- Verify project slug is correct
- Check team slug for team projects
- Ensure project is active
- Confirm endpoint is configured

### Parameter Issues

**Problem**: Route parameters not working

**Solutions**:
- Check route pattern syntax
- Verify parameter names match
- Test with simple parameter extraction
- Check for conflicting routes

### Performance Issues

**Problem**: Slow route resolution

**Solutions**:
- Simplify route patterns
- Reduce wildcard usage
- Check for route conflicts
- Monitor database performance

### Conflict Resolution

**Problem**: Wrong endpoint responding

**Solutions**:
- Review route specificity
- Check endpoint creation order
- Use more specific patterns
- Test route matching manually

## URL Examples

### Personal Project URLs

```bash
# User management API
GET    http://localhost:5268/user-mgmt/api/users
POST   http://localhost:5268/user-mgmt/api/users
GET    http://localhost:5268/user-mgmt/api/users/123
PUT    http://localhost:5268/user-mgmt/api/users/123
DELETE http://localhost:5268/user-mgmt/api/users/123

# Product catalog API
GET    http://localhost:5268/catalog/api/products
GET    http://localhost:5268/catalog/api/categories
GET    http://localhost:5268/catalog/api/products/456/reviews
```

### Team Project URLs

```bash
# Backend team APIs
GET    http://localhost:5268/backend-team/user-api/api/users
POST   http://localhost:5268/backend-team/user-api/api/auth/login
GET    http://localhost:5268/backend-team/product-api/api/products
PUT    http://localhost:5268/backend-team/order-api/api/orders/789

# Mobile team APIs
GET    http://localhost:5268/mobile-team/content-api/api/articles
POST   http://localhost:5268/mobile-team/push-api/api/notifications
```

## Best Practices

### URL Design
- Use descriptive project slugs
- Follow RESTful conventions
- Keep URLs readable and predictable
- Use consistent parameter naming

### Route Organization
- Group related endpoints
- Use hierarchical resource paths
- Plan for future expansion
- Document API structures

### Performance
- Minimize route complexity
- Use specific patterns over wildcards
- Monitor route resolution performance
- Cache frequently used routes

### Maintenance
- Regularly review route structures
- Update documentation with changes
- Test route changes thoroughly
- Use versioned APIs for breaking changes

### Security
- Validate all route parameters
- Implement proper access controls
- Log route access for auditing
- Use HTTPS for production deployments
