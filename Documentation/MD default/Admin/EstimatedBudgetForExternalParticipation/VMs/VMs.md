# Estimated Budget For External Participation ViewModels

## File Structure
- **EstimatedBudgetForExternalParticipationVM**: EstimatedBudgetForExternalParticipationVM.cs
- **EstimatedBudgetForExternalParticipationDetailVM**: EstimatedBudgetForExternalParticipationDetailVM.cs

## Core ViewModels

### EstimatedBudgetForExternalParticipationVM
**Purpose**: Main ViewModel for external participation budget estimation with comprehensive validation

**Properties**:

#### Identification & Approval
- `Id`: Primary key identifier (int)
- `SignatureIdApproved`: Foreign key for approval signature (int?)
- `Signature`: Navigation property to Signature entity

#### Participation Details
- `ParticipatingTitle`: Title of participation event with validation (string, max 100)
- `ParticipatingDate`: Participation date with validation (DateOnly?)
- `Regulator`: Regulatory body with validation (string, max 100)
- `ParticipatingCountry`: Host country with validation (string, max 100)
- `ParticipatingType`: Type of participation with integer validation (int?)
- `RequiredToparticipate`: Participation requirement with integer validation (int?)

#### Team Composition
- `ManagersCount`: Number of managers with validation (int?)
- `TechnicalSupervisorsCount`: Technical supervisors count with validation (int?)
- `ActivitiesSupervisorsCount`: Activities supervisors count with validation (int?)
- `MembersCount`: Regular members count with validation (int?)

#### Financial Breakdown - Participation Fees
- `ParticipationFeesHeadOfDelegation`: Head of delegation fees with decimal validation (decimal?)
- `ParticipationFeesForEntireTeam`: Total team fees with decimal validation (decimal?)
- `Subsidies`: Financial subsidies with decimal validation (decimal?)
- `FeeStatement`: Fee statement amount with decimal validation (decimal?)
- `TotalFeesForParticipation`: Total participation fees with decimal validation (decimal?)
- `ParticipationFeesForAdministrators`: Administrative fees with decimal validation (decimal?)

#### Financial Breakdown - Travel & Accommodation
- `ReserveAmountForDelegation`: Delegation reserve funds with decimal validation (decimal?)
- `AllowanceTravelForHeadOfDelegation`: Head travel allowance with decimal validation (decimal?)
- `TravelTicketValue`: Travel tickets cost with decimal validation (decimal?)
- `HotelAccommodationFees`: Accommodation costs with decimal validation (decimal?)
- `SupervisorsTravelAllowance`: Supervisors travel allowance with decimal validation (decimal?)
- `MembersAllowance`: Members allowance with decimal validation (decimal?)

#### Budget Totals
- `TotalBudgetRequired1`: First budget total with decimal validation (decimal?)
- `TotalBudgetRequired2`: Second budget total with decimal validation (decimal?)

#### Additional Information
- `Notes`: Additional notes and comments (string, max 500)
- `CreationDate`: Record creation date with validation (DateOnly?)

#### Relationships
- `EstimatedBudgetForExternalParticipationDetailVM`: Collection of detailed participant ViewModels
- `EstimatedBudgetForExternalParticipationDetails`: Collection of entity details

### EstimatedBudgetForExternalParticipationDetailVM
**Purpose**: ViewModel for individual participant details in external events

**Properties**:

#### Identification
- `Id`: Primary key identifier (int)
- `EstimatedBudgetForExternalParticipationId`: Foreign key to main budget (int?)

#### Participant Information
- `Name`: Participant's full name (string)
- `Adj`: Additional designation/title (string)
- `Profession`: Participant's profession/role (string)

## Validation Features

### Custom Validation Attributes
- `[LocalizedRequired]`: Localized required field validation
- `[LocalizedMaxLength]`: Localized maximum length validation
- `[IntAttribute]`: Integer value validation
- `[DecimalAttribute]`: Decimal value validation
- `[DataType(DataType.Date)]`: Date type specification

### Business Rules
- Comprehensive financial tracking for external events
- Team composition validation with role-based counts
- Multi-currency decimal handling for financial amounts
- Date validation for participation and creation dates

### Data Integrity
- Foreign key relationships maintained
- Nullable decimal fields with default null values
- Collection initialization for related entities
- Consistent naming conventions between entities and ViewModels