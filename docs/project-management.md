# Project Management

Projects are the core of MockHub - they contain your mock API endpoints and configurations. This section covers creating, configuring, and managing mock projects, both personal and team-based.

## Overview

MockHub projects provide:
- Isolated mock API environments
- Path-based URL routing
- Configurable CORS and logging settings
- Endpoint organization and management
- Dynamic response generation

## Project Types

MockHub supports two types of projects:

### Personal Projects
- **Owned by**: Individual users
- **Access**: Private to the owner
- **URL Pattern**: `/{project-slug}/api/{endpoint}`
- **Use Case**: Personal development, testing, prototyping

### Team Projects
- **Owned by**: Teams (shared workspace)
- **Access**: All team members based on roles
- **URL Pattern**: `/{team-slug}/{project-slug}/api/{endpoint}`
- **Use Case**: Collaborative development, shared APIs

## Creating Projects

### Prerequisites

- **Personal Projects**: Any authenticated user can create
- **Team Projects**: Must be a member of the team

### Project Creation Process

#### Creating Personal Projects

1. **Navigate to Projects**
   - Click **"Projects"** in the sidebar
   - Or click **"New Project"** from the dashboard

2. **Fill Project Details**
   - **Project Name**: Descriptive name for your project
   - **Description**: Optional project description
   - **Team**: Leave empty for personal project
   - **CORS**: Enable/disable cross-origin requests
   - **Request Logging**: Enable/disable request logging

3. **Save Project**
   - Click **"Create"**
   - Project slug is automatically generated

#### Creating Team Projects

1. **Navigate to Team**
   - Go to **"My Teams"**
   - Select the desired team

2. **Add Project to Team**
   - Click **"Add Project"** in the team detail page
   - Fill in project details (same as personal projects)
   - The project will be associated with the team

### Project Slug Generation

Project slugs are automatically generated:
- Based on project name (lowercase, hyphens for spaces)
- Special characters removed, spaces become hyphens
- Maximum 50 characters
- Automatically updated when project name changes
- Uniqueness ensured within scope (personal or team)

Examples:
- "User Management API" → "user-management-api"
- "Product Service v2" → "product-service-v2"

## Project Configuration

### Basic Settings

#### Project Information
- **Name**: Display name (editable)
- **Description**: Project purpose and notes
- **Slug**: URL identifier (auto-generated, read-only)
- **Status**: Active/Inactive toggle

#### Network Settings
- **CORS**: Cross-Origin Resource Sharing
- **Request Logging**: Enable/disable request logging
- **Default Delay**: Global response delay in milliseconds

### Advanced Settings

#### Latency Simulation
- **Enable Latency Simulation**: Add random delays to responses
- **Global Latency Min/Max**: Minimum and maximum delay range

#### JWT Validation
- **Enable JWT Validation**: Require JWT tokens for requests
- **JWT Secret**: Token signing secret
- **JWT Issuer**: Token issuer claim
- **JWT Audience**: Token audience claim

## Project Management

### Viewing Projects

#### Personal Projects
- Access via **"Projects"** in the sidebar
- Shows all projects you own
- Displays project status, endpoint count, and last modified

#### Team Projects
- Access via **"My Teams"** > Select Team
- Shows all projects in the team
- Role-based permissions apply

### Editing Projects

1. **Open Project**
   - Click on project name from list
   - Or navigate directly to project URL

2. **Modify Settings**
   - Update name, description, settings
   - Changes apply immediately to all endpoints

3. **Save Changes**
   - Click **"Save"**
   - Project slug updates automatically if name changed

### Project Status

#### Active Projects
- All endpoints are accessible
- Requests are logged (if enabled)
- Project appears in lists and searches

#### Inactive Projects
- All endpoints return 404
- No request logging
- Project still visible but marked as inactive

### Deleting Projects

#### Personal Projects
- Owners can delete their own projects
- Complete removal of project and all endpoints
- All data is permanently deleted

#### Team Projects
- **Owner**: Can delete their own projects
- **Administrators**: Can delete any project in the team
- **Members**: Cannot delete projects

## Project URLs and Routing

### URL Structure

#### Personal Projects
```
http://localhost:5268/{project-slug}/api/{endpoint-path}
```

Example:
- Project: "user-api" (slug)
- Endpoint: "/users"
- URL: `http://localhost:5268/user-api/api/users`

#### Team Projects
```
http://localhost:5268/{team-slug}/{project-slug}/api/{endpoint-path}
```

Example:
- Team: "backend-team" (slug)
- Project: "user-service" (slug)
- Endpoint: "/users/profile"
- URL: `http://localhost:5268/backend-team/user-service/api/users/profile`

### Routing Features

- **Path-based Isolation**: Each project has its own URL namespace
- **Wildcard Support**: Routes can include wildcards and parameters
- **Automatic Routing**: No manual server configuration needed
- **Concurrent Access**: Multiple projects run simultaneously

## Endpoint Management

### Adding Endpoints

1. **Open Project**
   - Navigate to project detail page

2. **Create Endpoint**
   - Click **"Add Endpoint"**
   - Configure endpoint details (see Endpoint Configuration docs)

3. **Save Endpoint**
   - Endpoint becomes immediately active

### Endpoint Organization

- Endpoints are listed in the project detail page
- Grouped by HTTP method
- Show status codes and route patterns
- Quick access to edit/delete actions

### Endpoint Statistics

Project pages show:
- **Total Endpoints**: Number of configured endpoints
- **Active Endpoints**: Currently accessible endpoints
- **Request Count**: Total requests (if logging enabled)
- **Last Activity**: Most recent request timestamp

## Project Permissions

### Personal Project Permissions

| Action | Owner | Others |
|--------|-------|---------|
| View Project | ✓ | ✗ |
| Edit Settings | ✓ | ✗ |
| Add Endpoints | ✓ | ✗ |
| Edit Endpoints | ✓ | ✗ |
| Delete Endpoints | ✓ | ✗ |
| Delete Project | ✓ | ✗ |
| View Logs | ✓ | ✗ |

### Team Project Permissions

| Action | Owner | Team Admin | Team Member | Non-Member |
|--------|--------|------------|-------------|------------|
| View Project | ✓ | ✓ | ✓ | ✗ |
| Edit Settings | ✓ | ✓ | ✗ | ✗ |
| Add Endpoints | ✓ | ✓ | ✓ | ✗ |
| Edit Own Endpoints | ✓ | ✓ | ✓ | ✗ |
| Edit Others' Endpoints | ✓ | ✓ | ✗ | ✗ |
| Delete Own Endpoints | ✓ | ✓ | ✗ | ✗ |
| Delete Others' Endpoints | ✓ | ✓ | ✗ | ✗ |
| Delete Project | ✓ | ✓ | ✗ | ✗ |
| View Logs | ✓ | ✓ | ✓ | ✗ |

## Project Import/Export

*Future feature - project import/export capabilities for backup and migration*

## Project Analytics

### Request Statistics

If logging is enabled, projects show:
- **Total Requests**: All-time request count
- **Recent Activity**: Last 24 hours of requests
- **Popular Endpoints**: Most frequently called endpoints
- **Error Rates**: Failed request percentages

### Performance Metrics

- **Average Response Time**: Across all endpoints
- **Response Time Distribution**: By endpoint
- **Error Breakdown**: By HTTP status code
- **Geographic Distribution**: Request sources (future feature)

## API Endpoints

### Project Management APIs

```
GET    /api/projects                 # Get user's projects
POST   /api/projects                 # Create new project
GET    /api/projects/{id}            # Get project details
PUT    /api/projects/{id}            # Update project
DELETE /api/projects/{id}            # Delete project

GET    /api/projects/{id}/endpoints  # Get project endpoints
POST   /api/projects/{id}/endpoints  # Create endpoint
PUT    /api/projects/{id}/endpoints/{endpointId}  # Update endpoint
DELETE /api/projects/{id}/endpoints/{endpointId}  # Delete endpoint

GET    /api/projects/{id}/logs       # Get project logs
DELETE /api/projects/{id}/logs       # Clear project logs
```

### Team Project APIs

```
GET    /api/teams/{teamId}/projects  # Get team projects
POST   /api/teams/{teamId}/projects  # Create team project
```

## Troubleshooting

### Project Not Accessible

**Problem**: Project endpoints return 404

**Solutions**:
- Verify project is active
- Check project slug in URL
- For team projects, verify team membership
- Ensure correct URL pattern

### Cannot Create Project

**Problem**: Project creation fails

**Solutions**:
- Check for duplicate project names/slugs
- Verify team membership for team projects
- Ensure you have necessary permissions
- Check database connectivity

### Permission Errors

**Problem**: Cannot edit project or endpoints

**Solutions**:
- Verify ownership or team role
- For team projects, check team membership
- Ensure project is not locked
- Contact team administrator

### Slug Conflicts

**Problem**: Project creation fails due to slug conflicts

**Solution**:
- Choose a different project name
- System automatically handles uniqueness within scope

## Best Practices

### Project Organization
- Use descriptive project names
- Group related endpoints in single projects
- Use consistent naming conventions
- Document project purposes

### Configuration
- Enable logging for development projects
- Configure CORS based on usage scenarios
- Set appropriate default delays
- Use JWT validation for secured APIs

### Security
- Regularly review project permissions
- Use descriptive but non-sensitive project names
- Enable logging for monitoring
- Keep sensitive data out of mock responses

### Performance
- Minimize response delays for testing
- Use appropriate data sizes in responses
- Monitor request patterns
- Archive unused projects

### Collaboration
- Use team projects for shared APIs
- Document endpoint purposes and usage
- Establish naming conventions
- Regularly review and update projects
