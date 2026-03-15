# Supplier ViewModel

## SupplierVM

**Namespace**: `FougeraClub.Areas.Admin.ViewModels.Suppliers`

**Purpose**: Comprehensive ViewModel for supplier management with advanced validation and business rules

### Properties

#### Basic Information
- `Id`: Primary identifier (int)

#### Supplier Identification
- `SupplierNameAr`: Arabic supplier name with validation (string, max 200)
  - **Validation**: Required field with localized messages
  - **Display**: "اسم المورد" (Supplier Name)
- `SupplierNameEn`: English supplier name (string)

#### Business Details
- `VATNumber`: Value Added Tax registration number with validation (string, max 50)
  - **Validation**: Required field
  - **Display**: "رقم VAT" (VAT Number)

#### Contact Information
- `Phone`: Landline phone number with comprehensive validation (string, max 20)
  - **Validation**: Required, phone format, unique across suppliers
  - **Display**: "رقم الهاتف" (Phone Number)
  - **Error Messages**: Arabic format validation and uniqueness
- `Mobile`: Mobile phone number with comprehensive validation (string, max 20)
  - **Validation**: Required, phone format, unique across suppliers
  - **Display**: "رقم الموبايل" (Mobile Number)
  - **Error Messages**: Arabic format validation and uniqueness
- `Email`: Email address with comprehensive validation (string, max 200)
  - **Validation**: Required, email format, unique across suppliers
  - **Error Messages**: Arabic format validation and uniqueness

#### Location & Description
- `Address`: Physical business address with validation (string, max 500)
  - **Validation**: Required field
- `Description`: Additional supplier information (string, max 1000)

#### Business Logic
- `HasRelatedData`: Flag indicating if supplier has related purchase records (bool)

## Validation Features

### Custom Validation Attributes
- `[LocalizedRequired]`: Localized required field validation for multi-language support
- `[Unique]`: Database uniqueness validation across supplier entities
- `[StringLength]`: Maximum length constraints for database optimization
- `[Phone]`: Phone number format validation with Arabic error messages
- `[EmailAddress]`: Email format validation with Arabic error messages
- `[Display]`: Arabic display names for UI localization

### Business Rule Validation
- **Phone Uniqueness**: Prevents duplicate phone numbers across suppliers
- **Mobile Uniqueness**: Prevents duplicate mobile numbers across suppliers
- **Email Uniqueness**: Prevents duplicate email addresses across suppliers
- **Required Field Enforcement**: Ensures essential supplier information

### Resource Integration
- **Localized Messages**: Uses Resource2 for validation error messages
- **Arabic Error Text**: Culture-appropriate validation feedback
- **Consistent Terminology**: Standardized error messages across application

## Key Features

### Comprehensive Contact Management
- **Multiple Contact Points**: Phone, mobile, and email with individual validation
- **Unique Contact Enforcement**: Prevents supplier contact duplication
- **Format Validation**: Ensures proper phone and email formatting
- **Arabic Interface**: Full Arabic localization for Middle Eastern market

### Business Compliance
- **VAT Support**: Tax compliance information tracking
- **Required Business Information**: Essential supplier details enforcement
- **Data Integrity**: Comprehensive validation for clean supplier data
- **Audit Ready**: Complete supplier information for compliance

### Data Integrity
- **Uniqueness Constraints**: Prevents data duplication across critical fields
- **Length Limitations**: Database optimization and data quality
- **Business Logic**: Related data tracking for dependency management
- **Validation Pipeline**: Multi-layer validation approach

## Use Cases

### Supplier Registration
- New supplier onboarding with comprehensive validation
- Contact information management with uniqueness checks
- Business compliance data collection (VAT numbers)
- Address and description information storage

### Supplier Management
- Existing supplier information updates
- Contact detail modifications with validation
- Business information maintenance
- Supplier relationship tracking

### Procurement Integration
- Purchase order processing vendor selection
- Supplier performance evaluation
- Vendor relationship management
- Supply chain optimization

## Technical Implementation

### Validation Strategy
- **Client-side Validation**: Immediate user feedback
- **Server-side Validation**: Data integrity enforcement
- **Database Constraints**: Uniqueness at data layer
- **Business Rule Validation**: Application logic enforcement

### Localization Support
- **Arabic Interface**: Native Arabic labels and error messages
- **Culture-aware Validation**: Appropriate format validation for region
- **Resource Files**: Centralized message management
- **User Experience**: Arabic-speaking user optimization

### Integration Benefits
- **Form Binding**: Optimized for MVC form submission
- **API Compatibility**: Clean structure for web API consumption
- **Database Mapping**: Efficient entity framework integration
- **UI Component Support**: Compatible with modern frontend frameworks