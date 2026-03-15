# External Work Mission Entity

## ExternalWorkMission

**Namespace**: `Domain.Entities`

**Purpose**: Manages external work missions with comprehensive candidate tracking and financial details

### Properties

#### Identification & Approval
- `Id`: Primary key identifier (int)
- `EmployeeId`: Foreign key to assigned employee (int?)
- `SignatureIdApproved`: Foreign key for approval signature (int?)
- `Signature`: Navigation property to Signature entity

#### Mission Details
- `MissionDate`: Date of the external mission (DateOnly?)
- `MissionLocation`: Location where mission takes place (string, max 200)
- `MissionCountry`: Country of the mission (string, max 200)
- `MissionWorkDescription`: Detailed description of mission work (string, max 300)

#### Financial Information
- `PetroleumFees`: Fuel and transportation costs (decimal?)
- `FoodFees`: Meal and sustenance expenses (decimal?)
- `MissionAllowance`: Additional allowance for the mission (decimal?)

#### Candidate Information
**Primary Candidates**:
- `CandidateName1`: Name of first candidate (string, max 200)
- `CandidateName2`: Name of second candidate (string, max 200)
- `CandidateName3`: Name of third candidate (string, max 200)
- `CandidateName4`: Name of fourth candidate (string, max 200)

**Candidate Designations**:
- `CandidateAdj1`: Designation/title for first candidate (string, max 200)
- `CandidateAdj2`: Designation/title for second candidate (string, max 200)
- `CandidateAdj3`: Designation/title for third candidate (string, max 200)
- `CandidateAdj4`: Designation/title for fourth candidate (string, max 200)

#### Relationships
- `Employee`: Navigation to assigned Employee entity
- `Missions`: Collection of related Mission entities

### Key Features

#### Mission Management
- Comprehensive external mission tracking
- Geographic information (location and country)
- Detailed work description for mission scope

#### Financial Tracking
- Multiple expense categories (petroleum, food, allowances)
- Flexible decimal amounts for accurate cost tracking
- Support for various mission-related expenses

#### Team Composition
- Support for up to four mission candidates
- Individual candidate designations/roles
- Structured candidate information storage

#### Workflow Integration
- Signature-based approval system
- Employee assignment tracking
- Relationship to broader mission framework

### Data Annotations
- `[Key]`: Primary key identification
- `[ForeignKey]`: Explicit foreign key relationships
- `[MaxLength]`: String length constraints for database optimization

### Use Cases
- External work mission planning and approval
- Candidate selection and team formation
- Mission budget estimation and tracking
- Employee assignment and accountability
- Audit trail through signature approval


# Mission Entity

## Mission

**Namespace**: `Domain.Entities`

**Purpose**: Represents individual mission records linked to external work missions

### Properties

#### Identification
- `Id`: Primary key identifier (int)
- `ExternalWorkMissionId`: Foreign key to parent external work mission (int?)
- `MessionKey`: Unique mission key or identifier (int?)

#### Relationships
- `ExternalWorkMission`: Navigation property to parent ExternalWorkMission entity

### Key Features

#### Mission Tracking
- **Individual Mission Records**: Separate entity for each mission instance
- **External Mission Linkage**: Connects to comprehensive external work missions
- **Unique Identification**: Mission key for external referencing

#### Relationship Architecture
- **One-to-Many**: One ExternalWorkMission can have multiple Mission records
- **Optional Parent**: Missions can exist independently of external work missions
- **Navigation Support**: Easy access to parent mission details

### Data Annotations
- `[Key]`: Primary key identification
- `[ForeignKey]`: Explicit foreign key relationship to ExternalWorkMission

### Use Cases
- Detailed mission instance tracking
- Relationship management between missions and external work
- Mission key-based referencing systems
- Hierarchical mission organization