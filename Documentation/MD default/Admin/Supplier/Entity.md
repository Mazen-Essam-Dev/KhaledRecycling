# Supplier Entity

## Supplier

**Namespace**: `Domain.Entities`

**Purpose**: Core entity for managing supplier/vendor information with comprehensive contact and business details

### Properties

#### Basic Information
- `Id`: Primary key identifier (int)

#### Supplier Identification
- `SupplierNameAr`: Arabic supplier name (string, max 200)
  - **Display**: "اسم المورد" (Supplier Name)
- `SupplierNameEn`: English supplier name (string, max 200)
  - **Display**: "اسم الموردE" (Supplier Name English)

#### Business Details
- `VATNumber`: Value Added Tax registration number (string, max 50)
  - **Display**: "رقم VAT" (VAT Number)

#### Contact Information
- `Phone`: Landline phone number with validation (string, max 20)
  - **Display**: "رقم الهاتف" (Phone Number)
  - **Validation**: Phone format validation with Arabic error message
- `Mobile`: Mobile phone number with validation (string, max 20)
  - **Display**: "رقم الموبايل" (Mobile Number)
  - **Validation**: Phone format validation with Arabic error message
- `Email`: Email address with validation (string, max 200)
  - **Display**: "البريد الإلكتروني" (Email)
  - **Validation**: Email format validation with Arabic error message

#### Location & Description
- `Address`: Physical business address (string, max 500)
  - **Display**: "العنوان" (Address)
- `Description`: Additional supplier information (string, max 1000)
  - **Display**: "الوصف" (Description)

#### Business Logic
- `HasRelatedData`: Flag indicating if supplier has related records (bool - NotMapped)

## Data Annotations

### Validation Attributes
- `[StringLength]`: Maximum length constraints for database optimization
- `[Phone]`: Phone number format validation with localized error messages
- `[EmailAddress]`: Email format validation with Arabic error messages
- `[Display]`: Arabic display names for UI localization

### Database Optimization
- `[NotMapped]`: Excludes business logic flag from database mapping
- Appropriate string lengths for efficient storage
- Clear property naming for database schema

## Key Features

### Multi-language Support
- **Arabic Interface**: All display names in Arabic for user-friendly interface
- **Bilingual Names**: Support for both Arabic and English supplier names
- **Localized Validation**: Arabic error messages for validation failures

### Comprehensive Contact Management
- **Multiple Contact Methods**: Phone, mobile, and email support
- **Format Validation**: Proper phone and email format enforcement
- **Business Information**: VAT number for tax compliance

### Business Logic Integration
- **Related Data Tracking**: HasRelatedData flag for dependency management
- **Data Integrity**: Proper validation to maintain clean supplier data
- **Audit Ready**: Complete supplier information for compliance

## Use Cases

### Procurement Management
- Supplier registration and information management
- Purchase order processing and vendor selection
- Supplier performance tracking and evaluation
- Vendor relationship management

### Financial Operations
- VAT-compliant invoicing and transactions
- Supplier payment processing
- Tax reporting and compliance
- Accounts payable management

### Business Intelligence
- Supplier analytics and performance metrics
- Vendor diversity and sourcing analysis
- Supply chain optimization
- Procurement strategy development

## Integration Benefits

### Data Integrity
- **Validation Enforcement**: Ensures clean, validated supplier data
- **Length Constraints**: Prevents database overflow and maintains performance
- **Format Compliance**: Standardized contact information storage

### User Experience
- **Arabic Localization**: Native Arabic interface for users
- **Clear Labels**: Descriptive Arabic display names
- **Error Guidance**: Helpful Arabic validation messages

### System Integration
- **Purchase Order Linking**: Foundation for procurement workflows
- **Financial System Integration**: VAT and payment processing
- **Reporting Compatibility**: Structured data for analytics

## Comparison with SupplierVM

### Entity vs ViewModel Differences

**Supplier Entity**:
- Database-focused with basic validation
- Optional fields for flexibility
- No uniqueness constraints
- Business logic flag (NotMapped)

**SupplierVM**:
- UI-focused with comprehensive validation
- Required fields for data quality
- Uniqueness constraints across critical fields
- Localized required field validation

### Validation Approach
- **Entity**: Basic format validation (phone, email)
- **ViewModel**: Comprehensive business rule validation including uniqueness

### Use Case Alignment
- **Entity**: Data persistence and storage
- **ViewModel**: User input handling and form validation