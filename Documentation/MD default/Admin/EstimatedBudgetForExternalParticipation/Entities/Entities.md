# Estimated Budget For External Participation

## Entities Overview

### EstimatedBudgetForExternalParticipation
**Purpose**: Main entity for managing external participation budget estimations

**Properties**:

#### Identification & Approval
- `Id`: Primary key identifier (int)
- `SignatureIdApproved`: Foreign key for approval signature (int?)
- `Signature`: Navigation property to Signature entity

#### Participation Details
- `ParticipatingTitle`: Title of the participation event (string, max 100)
- `ParticipatingDate`: Date of participation (DateOnly)
- `Regulator`: Regulatory body/organizer (string, max 100)
- `ParticipatingCountry`: Host country (string, max 100)
- `ParticipatingType`: Type of participation (int?)
- `RequiredToparticipate`: Participation requirement flag (int?)

#### Team Composition
- `ManagersCount`: Number of managers participating (int?)
- `TechnicalSupervisorsCount`: Number of technical supervisors (int?)
- `ActivitiesSupervisorsCount`: Number of activities supervisors (int?)
- `MembersCount`: Number of regular members (int?)

#### Financial Breakdown - Participation Fees
- `ParticipationFeesHeadOfDelegation`: Fees for head of delegation (decimal?)
- `ParticipationFeesForEntireTeam`: Total team participation fees (decimal?)
- `Subsidies`: Financial subsidies received (decimal?)
- `FeeStatement`: Fee statement amount (decimal?)
- `TotalFeesForParticipation`: Sum of all participation fees (decimal?)
- `ParticipationFeesForAdministrators`: Administrative fees (decimal?)

#### Financial Breakdown - Travel & Accommodation
- `ReserveAmountForDelegation`: Reserve funds for delegation (decimal?)
- `AllowanceTravelForHeadOfDelegation`: Travel allowance for head (decimal?)
- `TravelTicketValue`: Cost of travel tickets (decimal?)
- `HotelAccommodationFees`: Accommodation costs (decimal?)
- `SupervisorsTravelAllowance`: Travel allowance for supervisors (decimal?)
- `MembersAllowance`: Allowance for regular members (decimal?)

#### Budget Totals
- `TotalBudgetRequired1`: First total budget calculation (decimal?)
- `TotalBudgetRequired2`: Second total budget calculation (decimal?)

#### Additional Information
- `Notes`: Additional notes and comments (string, max 500)
- `CreationDate`: Record creation date (DateOnly?)

#### Relationships
- `EstimatedBudgetForExternalParticipationDetails`: Collection of detailed participant information

### EstimatedBudgetForExternalParticipationDetail
**Purpose**: Detailed information about individual participants in external events

**Properties**:

#### Identification
- `Id`: Primary key identifier (int)
- `EstimatedBudgetForExternalParticipationId`: Foreign key to main entity (int?)

#### Participant Information
- `Name`: Participant's full name (string)
- `Adj`: Additional designation/title (string)
- `Profession`: Participant's profession/role (string)

#### Relationships
- `EstimatedBudgetForExternalParticipation`: Navigation to main budget entity

## Key Features

### Data Annotations
- `[Key]`: Primary key identification
- `[ForeignKey]`: Explicit foreign key relationships
- `[MaxLength]`: String length constraints
- `[DataType(DataType.Date)]`: Date type specification

### Financial Management
- Comprehensive budget tracking for external events
- Multiple cost categories (fees, travel, accommodation, allowances)
- Team composition tracking with role-based counts
- Dual total budget calculations for verification

### Relationship Structure
- One-to-Many: Main entity to details
- Optional approval signature integration
- Flexible participant detail tracking