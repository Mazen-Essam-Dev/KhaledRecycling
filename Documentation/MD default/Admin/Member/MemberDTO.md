# Member Data Transfer Object

## MemberDTO

**Namespace**: `Domain.DTOs`

**Purpose**: Simplified data transfer object for member information with core identity details

### Properties

#### Basic Identification
- `Code`: Member identification code/number (int?)
- `FullNameAr`: Arabic full name of the member (string)
- `FullNameEn`: English full name of the member (string)

#### Nationality & Identity
- `NationalityId`: Identifier for member's nationality (int?)
- `IdNumber`: National identification number (string)

#### Date Information
- `IdExpiryDate`: Expiration date of identification document (DateOnly?)
  - **DataType**: Date format optimized for storage and display
- `DateOfBirth`: Member's birth date (DateOnly?)
  - **DataType**: Date format for consistent handling

#### Demographic Information
- `Age`: Calculated age of the member (int?)

### Key Features

#### Data Optimization
- **Minimal Data Set**: Contains only essential member information
- **Efficient Transfer**: Lightweight object for API responses and data exchange
- **Serialization Friendly**: Simple structure for JSON/XML serialization

#### Use Cases
- **API Responses**: Clean data structure for frontend applications
- **Data Export**: Simplified format for external systems
- **Quick Lookups**: Essential member information for search results
- **Reporting**: Core member data for reports and analytics

#### Integration Benefits
- **Performance**: Reduced payload size compared to full entity
- **Security**: Limited exposure of sensitive member data
- **Maintainability**: Clear data contract between layers
- **Flexibility**: Easy to extend with additional properties as needed

### Data Annotations
- `[DataType(DataType.Date)]`: Ensures proper date handling and formatting
- Consistent with domain entity but with simplified structure