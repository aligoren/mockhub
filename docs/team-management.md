# Team Management

MockHub supports collaborative development through team workspaces. Teams allow multiple developers to work together on shared mock projects with role-based permissions. This section covers team creation, member management, roles, and collaboration features.

## Overview

Team management provides:
- Team workspace creation and configuration
- Role-based access control (Owner, Administrator, Member)
- Member invitation and management
- Project sharing within teams
- Team-specific URL routing

## Creating Teams

### Prerequisites

Only administrators can create teams in MockHub. Regular users cannot create teams - they can only be added to existing teams by administrators or team owners.

### Team Creation Process

1. **Access Team Management**
   - Navigate to **Administration > All Teams** (admin only)
   - Or **My Teams** page for team members

2. **Create New Team**
   - Click **"Create Team"**
   - Enter team details:
     - **Team Name**: Descriptive name for the team
     - **Description**: Optional description of team's purpose
   - Click **"Create"**

### Team Slug

When a team is created, a unique slug is automatically generated:
- Based on the team name (lowercase, hyphens for spaces)
- Automatically updated if team name changes
- Used in team URLs: `/team-slug/project-slug/api/endpoint`

Example: Team "Backend API Team" → slug "backend-api-team"

## Team Roles and Permissions

### Role Hierarchy

MockHub implements a three-tier role system for teams:

#### 1. Team Owner
**Permissions:**
- Full control over team settings
- Add/remove team members
- Change member roles (including promoting to Owner)
- Delete the team
- Create, edit, delete team projects
- Configure team-level settings

**Limitations:**
- Only one owner per team
- Cannot be removed by other team members

#### 2. Team Administrator
**Permissions:**
- Add/remove team members (except Owner)
- Change member roles (except Owner role)
- Create, edit, delete team projects
- Configure project settings
- View all team projects and logs

**Limitations:**
- Cannot delete the team
- Cannot change Owner role
- Cannot remove the Owner

#### 3. Team Member
**Permissions:**
- View team projects
- Create new projects in the team
- Edit projects they created or have been assigned to
- View request logs for accessible projects

**Limitations:**
- Cannot manage team members
- Cannot delete team projects (unless they own them)
- Cannot change team settings

## Member Management

### Adding Team Members

Team owners and administrators can add members through two methods:

#### Method 1: From Team Page
1. Navigate to the team detail page
2. Click **"Add"** in the Members section
3. Search for users by name, surname, or email
4. Select a user from the search results
5. Choose a role (Member or Administrator)
6. Click **"Add"**

#### Method 2: From User Management (Admin Only)
1. Go to **Administration > Users**
2. Find the user and click **"Edit"**
3. In the Teams section, add the user to teams
4. Assign appropriate roles

### Member Search and Selection

The member addition interface provides:
- **Real-time search** by name, surname, or email
- **Filtered results** excluding current team members
- **User preview** with avatar, name, and email
- **Role selection** before adding

### Changing Member Roles

Team owners and administrators can modify member roles:

1. Go to team member list
2. Find the member whose role needs changing
3. Use the role dropdown or edit button
4. Select new role (Member/Administrator)
5. Save changes

**Note**: Only team owners can assign Administrator roles and change existing Administrator roles.

### Removing Team Members

#### Voluntary Removal
Team members can leave teams if the feature is enabled (currently not implemented - requires team owner/admin intervention).

#### Administrative Removal
Team owners and administrators can remove members:

1. Go to team member list
2. Find the member to remove
3. Click **"Remove"** or **"Delete"**
4. Confirm the removal

**Restrictions:**
- Team owners cannot be removed by administrators
- Only team owners can remove other administrators
- Administrators can remove regular members

## Team Projects

### Project Creation in Teams

Any team member can create projects within the team:

1. Navigate to the team detail page
2. Click **"Add Project"**
3. Enter project details:
   - **Project Name**: Name for the mock project
   - **Description**: Optional project description
   - **CORS Settings**: Enable/disable cross-origin requests
   - **Logging**: Enable/disable request logging
4. Click **"Create"**

### Project Ownership

- **Creator**: The user who creates the project has ownership
- **Team Access**: All team members can view and work on projects
- **Permission Levels**: Based on team role and project-specific permissions

### Project URLs

Team projects are accessible at:
```
http://localhost:5268/{team-slug}/{project-slug}/api/{endpoint}
```

Example:
- Team: "backend-team" (slug)
- Project: "user-service" (slug)
- Endpoint: "/api/users"
- URL: `http://localhost:5268/backend-team/user-service/api/users`

### Project Management Permissions

| Action | Owner | Administrator | Member |
|--------|-------|---------------|---------|
| Create Project | ✓ | ✓ | ✓ |
| Edit Own Projects | ✓ | ✓ | ✓ |
| Edit Others' Projects | ✓ | ✓ | ✗ |
| Delete Own Projects | ✓ | ✓ | ✗ |
| Delete Others' Projects | ✓ | ✓ | ✗ |
| View All Projects | ✓ | ✓ | ✓ |
| Configure Project Settings | ✓ | ✓ | ✓ |

## Team Settings

### Editable Settings

Team owners can modify:
- **Team Name**: Automatically updates the slug
- **Description**: Team purpose and information
- **Logo/Icon**: Team visual identity (future feature)

### View-Only Information

All team members can view:
- **Member Count**: Total number of active members
- **Project Count**: Number of active projects
- **Creation Date**: When the team was created
- **Team Slug**: URL identifier

## Team Administration

### Team Deletion

Only team owners can delete teams:

1. Go to team settings
2. Click **"Delete Team"**
3. Confirm deletion (requires typing team name)

**Warning**: Deleting a team will:
- Remove all team projects
- Remove all member associations
- Delete all related data
- This action cannot be undone

### Team Statistics

Team pages show statistics:
- **Total Members**: Active team members
- **Total Projects**: Active projects in the team
- **Recent Activity**: Latest project updates and member changes

## Collaboration Features

### Shared Workspaces

Teams provide:
- **Centralized Project Management**: All team projects in one place
- **Role-Based Collaboration**: Appropriate permissions for each role
- **Shared Resources**: Common mock data and configurations
- **Team Communication**: Through project comments and documentation

### Access Control

- **Team-Level Access**: Must be a team member to access team projects
- **Project-Level Permissions**: Based on ownership and team role
- **Audit Trail**: All changes are logged and attributable

## API Endpoints

### Team Management APIs

```
GET    /api/teams                    # Get user's teams
POST   /api/teams                    # Create new team (admin only)
GET    /api/teams/{id}               # Get team details
PUT    /api/teams/{id}               # Update team (owner/admin only)
DELETE /api/teams/{id}               # Delete team (owner only)

GET    /api/teams/{id}/members       # Get team members
POST   /api/teams/{id}/members       # Add team member
DELETE /api/teams/{id}/members/{userId}  # Remove team member
PUT    /api/teams/{id}/members/{userId}  # Update member role

GET    /api/teams/{id}/projects      # Get team projects
POST   /api/teams/{id}/projects      # Create team project
```

### Administrative APIs (Admin Only)

```
GET    /api/admin/teams              # Get all teams
POST   /api/admin/teams              # Create team
PUT    /api/admin/teams/{id}         # Update any team
DELETE /api/admin/teams/{id}         # Delete any team
```

## Troubleshooting

### Cannot Create Team

**Problem**: "Create Team" button not visible

**Solutions**:
- Verify you have administrator privileges
- Check if you're on the correct page (Administration > Teams)

### Cannot Add Team Members

**Problem**: User not found in search or cannot add

**Solutions**:
- Ensure the user exists in the system
- Check if the user is already a team member
- Verify your permission level (Owner/Administrator required)

### Permission Errors

**Problem**: Cannot access team features

**Solutions**:
- Verify your team membership
- Check your team role
- Ensure the team is active
- Contact team owner or administrator

### Team Slug Conflicts

**Problem**: Team creation fails due to slug conflicts

**Solution**:
- Choose a different team name
- The system automatically handles uniqueness by appending numbers if needed

## Best Practices

### Team Organization
- Use descriptive team names that reflect purpose
- Keep team sizes manageable (5-15 members ideal)
- Clearly define roles and responsibilities
- Regularly review team membership

### Project Management
- Use consistent naming conventions for projects
- Document project purposes and APIs
- Regularly archive completed projects
- Use team projects for collaborative work

### Security
- Assign appropriate roles to team members
- Regularly review member permissions
- Use strong team names (avoid sensitive information)
- Monitor team activity through logs

### Collaboration
- Establish team coding standards
- Document API conventions
- Use team projects for shared mock services
- Regularly communicate changes and updates
