# ActivityVM Documentation

## File Structure
- **ActivityVM**: ActivityVM.cs

## Core ViewModel

### ActivityVM
**Purpose**: ViewModel for activity management with comprehensive validation, multi-language support, and date consistency checking

## Properties

### Identification
- `Id`: Activity identifier (int)

### Basic Information
- `TitleAr`: Arabic title with regex validation and localization (string?)
- `TitleEn`: English title with regex validation and localization (string?)

### Event Scheduling
- `StartDate`: Activity start date with required validation (DateOnly?)
- `EndDate`: Activity end date with required validation (DateOnly?)

### Participation Rules
- `MinimumAge`: Minimum age requirement with required validation (int?)
- `Location`: Event venue with length and required validation (string?)

### Content & Status
- `Description`: Activity description (string?)
- `AttachmentPath`: File attachment storage path (string?)
- `Subscriptions`: Collection of activity subscriptions (ICollection<Subscription>)
- `Subscribed`: Boolean indicating current user's subscription status (bool)

## Validation Implementation

### Data Annotations
- `[LocalizedRequired]`: Culture-aware required field validation
- `[LocalizedMaxLength]`: Localized maximum length validation
- `[RegularExpression]`: Language-specific character validation

### Custom Validation
**Implements**: `IValidatableObject` for business rule validation

**Validation Rule**: End date must be after start date

**Implementation**:
- Checks both dates have values
- Validates EndDate > StartDate
- Returns language-specific error messages
- Targets EndDate field for error display

**Error Messages**:
- Arabic: "تاريخ النهاية يجب أن يكون بعد تاريخ البداية"
- English: "The End Date must be after the Start Date"

## Technical Features

### Multi-language Support
- **Arabic Validation**: Arabic characters and numbers only
- **English Validation**: English letters and numbers only
- **Localized Messages**: Session-based language detection
- **Culture Awareness**: Respects current UI culture

### Collection Management
- **Subscriptions**: Auto-initialized empty collection
- **Subscription Tracking**: Separate flag for current user status
- **Relationship Support**: Maintains activity-subscription relationships

### Date Validation
- **Business Logic**: Ensures logical date ranges
- **User Experience**: Field-specific error targeting
- **Consistency**: Prevents invalid date combinations

## Usage Context

### Form Integration
```html
<!-- Arabic Title -->
<input asp-for="TitleAr" class="form-control" />
<span asp-validation-for="TitleAr" class="text-danger"></span>

<!-- Date Validation -->
<input asp-for="StartDate" type="date" class="form-control" />
<input asp-for="EndDate" type="date" class="form-control" />
<span asp-validation-for="EndDate" class="text-danger"></span>