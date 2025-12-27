# User Management

MockHub uses an admin-based user management system where the first registered user becomes the administrator. This section covers user creation, management, roles, and permissions.

## Overview

The user management system provides:
- Admin-controlled user registration
- Role-based access control
- User activation/deactivation
- Password management
- Team membership management

## Administrator Role

### First Administrator Setup

When MockHub is launched for the first time, you'll be redirected to the setup page where you can create the first administrator account:

1. **Email**: Administrator's email address
2. **Password**: Strong password (minimum 6 characters with uppercase, lowercase, and numbers)
3. **First Name & Last Name**: Administrator's full name
4. **Confirm Password**: Re-enter the password

This first user automatically receives administrator privileges and can create additional users.

### Administrator Permissions

Administrators have full access to:
- **User Management**: Create, edit, activate/deactivate users
- **Team Management**: Create and manage all teams in the system
- **System Settings**: Access to all administrative functions
- **Request Logs**: View all logs across all projects and teams
- **Project Access**: Access to any project in the system

## User Creation

Only administrators can create new users through the web interface.

### Creating a New User

1. Navigate to **Administration > Users**
2. Click **"Add User"**
3. Fill in the user details:
   - **First Name**: User's first name
   - **Last Name**: User's last name
   - **Email**: User's email address (used as username)
   - **Password**: Temporary password for the user
   - **Administrator**: Check to grant admin privileges
4. Click **"Create User"**

### User Roles

- **Administrator**: Full system access
- **Regular User**: Can create personal projects and work on team projects based on permissions

## User Management

### Viewing Users

Administrators can view all users in the system at **Administration > Users**. The user list shows:

- User avatar (initials)
- Full name and email
- Role (Administrator/User)
- Team memberships
- Account status (Active/Inactive)
- Last login date

### Editing Users

Click the **"Edit"** button next to any user to modify:

- **Personal Information**: First name, last name
- **Role**: Change between Administrator and User
- **Status**: Activate or deactivate the account
- **Team Memberships**: Add/remove user from teams

### User Activation/Deactivation

- **Active Users**: Can log in and use the system
- **Inactive Users**: Cannot log in but their data is preserved

**Important**: You cannot deactivate the last administrator. Ensure at least one administrator remains active.

### Password Management

#### Resetting User Passwords

Administrators can reset passwords for any user:

1. Go to user details (**Administration > Users > [User] > Edit**)
2. Enter a new password
3. The user will be required to change their password on next login

#### User Password Changes

Regular users can change their own passwords through their profile settings (if implemented).

## User Authentication

### Login Process

1. Users access the login page at `/Account/Login`
2. Enter email and password
3. Optionally check "Remember me" for persistent sessions
4. Click "Login"

### Authentication Flow

- **First Launch**: Redirected to setup for administrator creation
- **Subsequent Logins**: Direct login with existing credentials
- **Inactive Users**: Login blocked with error message
- **Locked Accounts**: Temporary lockout after failed attempts

### Session Management

- Sessions persist based on "Remember me" setting
- Automatic logout on browser close (unless remembered)
- Admin sessions have no special timeout differences

## User Profile

### Profile Information

Users can view their profile information including:
- Full name and email
- Account creation date
- Last login date
- Team memberships
- Project ownership

### Preferences

Users can set personal preferences:
- **Theme**: Light or dark mode
- **Sidebar**: Collapsed or expanded state
- **Language**: Interface language (if multiple languages supported)

These preferences are stored in the database and persist across sessions and devices.

## Team Membership

### User-Team Relationships

Users can be members of multiple teams with different roles in each team:

- **Team Owner**: Full control over the team and its projects
- **Team Administrator**: Can manage team projects and members
- **Team Member**: Can view and work on team projects

### Adding Users to Teams

Administrators can add users to teams through:
1. **Team Management** page: Add members to specific teams
2. **User Management** page: Assign teams to users

### Removing Users from Teams

Users can be removed from teams by:
- Team owners or administrators
- System administrators
- The user leaving voluntarily (if allowed)

## Security Considerations

### Password Policies

- Minimum 6 characters
- Must contain uppercase letters
- Must contain lowercase letters
- Must contain numbers
- Regular password change recommendations

### Account Security

- Failed login attempt tracking
- Account lockout after multiple failures
- Secure password hashing with ASP.NET Core Identity
- Session management with secure cookies

### Data Privacy

- User data is encrypted at rest
- Personal information is only accessible to administrators
- Audit logs for all administrative actions
- GDPR compliance considerations

## API Endpoints

### User Management APIs

```
GET    /api/admin/users              # Get all users (admin only)
POST   /api/admin/users              # Create new user (admin only)
GET    /api/admin/users/{id}         # Get user details (admin only)
PUT    /api/admin/users/{id}         # Update user (admin only)
DELETE /api/admin/users/{id}         # Delete user (admin only)
POST   /api/admin/users/{id}/reset-password  # Reset password (admin only)
```

### Authentication APIs

```
POST   /api/account/login            # User login
POST   /api/account/logout           # User logout
GET    /api/account/setup-required   # Check if setup is needed
POST   /api/account/setup            # Initial admin setup
POST   /api/account/update-preferences  # Update user preferences
```

## Troubleshooting

### Cannot Create Administrator

**Problem**: Setup page shows "System is already set up"

**Solution**:
1. Check if database file exists (`mockhub.db`)
2. Delete the database file to reset the system
3. Restart the application

### User Cannot Log In

**Possible Issues**:
- Account is deactivated
- Incorrect password
- Account is temporarily locked

**Solutions**:
- Check user status in admin panel
- Reset password if needed
- Wait for lockout period to expire

### Permission Errors

**Problem**: User cannot access certain features

**Solutions**:
- Verify user role and permissions
- Check team membership for team-based access
- Ensure user account is active

## Best Practices

### User Management
- Regularly review user accounts and permissions
- Use strong passwords for administrator accounts
- Keep at least one active administrator
- Regularly rotate administrator passwords

### Security
- Monitor login attempts and suspicious activity
- Use HTTPS in production
- Regularly backup user data
- Implement multi-factor authentication (future enhancement)

### Team Organization
- Clearly define team roles and responsibilities
- Regularly review team memberships
- Use descriptive team names
- Keep team sizes manageable
