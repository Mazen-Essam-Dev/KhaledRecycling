# External Work Mission ViewModel

## ExternalWorkMissionVM

**Namespace**: `FougeraClub.Areas.Admin.ViewModels.ExternalWorkMission`

**Purpose**: ViewModel for managing external work missions with comprehensive validation and candidate tracking

### Properties

#### Identification & Approval
- `Id`: Primary key identifier (int)
- `EmployeeId`: Foreign key to assigned employee with validation (int?)
- `SignatureIdApproved`: Foreign key for approval signature (int?)
- `Signature`: Navigation property to Signature entity

#### Mission Details
- `MissionDate`: Date of the external mission with validation (DateOnly?)
- `MissionLocation`: Mission location with validation (string, max 200)
- `MissionCountry`: Mission country with validation (string, max 200)
- `MissionWorkDescription`: Mission description with validation (string, max 300)

#### Financial Information
- `PetroleumFees`: Fuel and transportation costs (decimal?)
- `FoodFees`: Meal and sustenance expenses (decimal?)
- `MissionAllowance`: Additional mission allowance (decimal?)

#### Candidate Information
**Primary Candidates**:
- `CandidateName1`: Name of first candidate (string, max 200)
- `CandidateName2`: Name of second candidate (string, max 200)
- `CandidateName3`: Name of third candidate (string, max 200)
- `CandidateName4`: Name of fourth candidate (string, max 200)

**Candidate Designations**:
- `CandidateAdj1`: Designation for first candidate (string, max 200)
- `CandidateAdj2`: Designation for second candidate (string, max 200)
- `CandidateAdj3`: Designation for third candidate (string, max 200)
- `CandidateAdj4`: Designation for fourth candidate (string, max 200)

#### Relationships
- `Employee`: Navigation to assigned Employee entity
- `Missions`: Collection of related Mission entities

### Validation Features

#### Custom Validation Attributes
- `[LocalizedRequired]`: Localized required field validation
- `[LocalizedMaxLength]`: Localized maximum length validation

#### Data Annotations
- `[Key]`: Primary key identification
- `[ForeignKey]`: Explicit foreign key relationships
- `[MaxLength]`: String length constraints

### Key Features

#### Mission Management
- Comprehensive external mission tracking with validation
- Geographic information with required field validation
- Detailed work description for mission scope

#### Financial Tracking
- Multiple expense categories (petroleum, food, allowances)
- Flexible decimal amounts for accurate cost tracking
- No validation on financial fields for flexibility

#### Team Composition
- Support for up to four mission candidates
- Individual candidate designations/roles
- Structured candidate information storage

#### Workflow Integration
- Signature-based approval system
- Employee assignment with validation
- Relationship to broader mission framework

### Use Cases
- External work mission planning and approval workflows
- Candidate selection and team formation interfaces
- Mission budget estimation and tracking forms
- Employee assignment and accountability management
- Audit trail through signature approval processes