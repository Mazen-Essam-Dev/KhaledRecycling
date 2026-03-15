# ActivityService Documentation

## File Structure
- **ActivityService**: ActivityService.cs

## Core Service

### ActivityService
**Purpose**: Business logic service for activity management, subscription handling, and age validation in the member area

## Dependencies

### Injected Services
- `IUnitOfWork`: Database access interface for repository pattern

## Methods

### GetAllAsync
**Purpose**: Retrieves all activities with member's subscription status

**Parameters**:
- `username`: Member's email for subscription lookup

**Returns**: Tuple containing activities and member's subscriptions

**Implementation**:
- Finds member by email
- Retrieves member's activity subscriptions
- Gets all activities from database
- Returns combined dataset

### GetByIdAsync
**Purpose**: Fetches single activity by ID

**Parameters**:
- `id`: Activity identifier

**Returns**: Activity entity or null

### AddAsync
**Purpose**: Creates new activity

**Parameters**:
- `activity`: Activity entity to create

**Implementation**:
- Adds activity to repository
- Commits changes to database

### UpdateAsync
**Purpose**: Updates existing activity

**Parameters**:
- `activity`: Activity entity with updates

**Implementation**:
- Updates activity in repository
- Commits changes to database

### SubscribeAsync
**Purpose**: Subscribes member to activity

**Parameters**:
- `activityId`: Activity to subscribe to
- `email`: Member's email

**Implementation**:
- Finds member by email
- Creates subscription with Activity type
- Saves subscription to database

### CheckAgeAsync
**Purpose**: Validates if member meets activity age requirements

**Parameters**:
- `activityId`: Activity to check
- `email`: Member's email

**Returns**: Boolean indicating age requirement satisfaction

**Implementation**:
- Retrieves member and activity
- Compares member age with activity minimum age
- Returns false if member too young

### DeleteAsync
**Purpose**: Removes activity from system

**Parameters**:
- `id`: Activity identifier

**Implementation**:
- Finds activity entity
- Deletes from repository
- Commits changes

### Exists
**Purpose**: Checks if activity exists

**Parameters**:
- `id`: Activity identifier

**Returns**: Boolean indicating existence

## Technical Implementation

### Subscription Management
- **Type Safety**: Uses SubscriptionType.Activity enum
- **Member Resolution**: Email-based member identification
- **Relationship Handling**: Proper foreign key assignment

### Age Validation
- **Comparison Logic**: Member age vs activity minimum age
- **Null Safety**: Proper null checking for entities
- **Business Rules**: Enforces age restrictions

### Data Access Patterns
- **Repository Pattern**: Consistent data access
- **Async Operations**: Non-blocking database calls
- **Transaction Management**: Unit of Work commit pattern

## Business Logic

### Activity Lifecycle
1. **Creation**: AddAsync for new activities
2. **Management**: UpdateAsync for modifications
3. **Participation**: SubscribeAsync for member enrollment
4. **Validation**: CheckAgeAsync for requirement checking
5. **Removal**: DeleteAsync for activity deletion

### Subscription Workflow
- Member identification via email
- Activity type specification
- Database persistence
- Age requirement enforcement

## Usage Examples

### Get Activities with Subscriptions
```csharp
var (activities, subscriptions) = await activityService.GetAllAsync("member@email.com");