# Activity Entity Documentation

## File Structure
- **Activity**: Activity.cs

## Core Entity

### Activity
**Purpose**: Represents club activities/events with comprehensive event management capabilities including multi-language support, date ranges, age restrictions, and file attachments

## Properties

### Identification
- `Id`: Primary key identifier (int)

### Basic Information
- `TitleAr`: Arabic activity title, max 200 characters (string?)
- `TitleEn`: English activity title, max 200 characters (string?)

### Event Scheduling
- `StartDate`: Activity start date (DateOnly?)
- `EndDate`: Activity end date (DateOnly?)

### Participation Rules
- `MinimumAge`: Minimum age requirement for participation (int?)
- `Location`: Event venue/location, max 200 characters (string?)

### Content & Documentation
- `Description`: Detailed activity description (string?)
- `Achievement`: Expected outcomes or achievements (string?)
- `AttachmentPath`: File attachment path for additional documents, max 300 characters (string?)

### Relationships
- `Questions`: Collection of related questions for the activity (ICollection<Question>)

## Technical Implementation

### Data Annotations
- `[Key]`: Primary key identification
- `[MaxLength]`: String field length validation
- Nullable reference types for optional fields

### Collection Initialization
- `Questions`: Auto-initialized as empty list to prevent null references

### Date Handling
- `DateOnly`: Date-specific without time component
- Nullable for flexible scheduling

## Business Context

### Activity Lifecycle
1. **Planning**: Title, description, scheduling
2. **Configuration**: Age limits, location, attachments
3. **Engagement**: Question collection for participant interaction
4. **Execution**: Date-based activity occurrence

### Multi-language Support
- Bilingual titles for Arabic/English interfaces
- Consistent with UAE localization requirements

### Participant Management
- Age-based access control
- Location-based event hosting
- Document attachment support

## Integration Patterns

### Question Relationship
- One-to-Many with Question entity
- Supports activity-specific questionnaires
- Enables participant feedback collection

### File Management
- Attachment path storage
- Document association with activities
- Support for brochures, schedules, guidelines

## Usage Examples

### Entity Creation
```csharp
var activity = new Activity
{
    TitleAr = "نشاط العلوم الصيفي",
    TitleEn = "Summer Science Activity",
    StartDate = new DateOnly(2024, 7, 1),
    EndDate = new DateOnly(2024, 7, 15),
    MinimumAge = 12,
    Location = "Fujairah Science Club",
    Description = "Interactive science workshops for youth",
    AttachmentPath = "uploads/activities/science-summer.pdf"
};