# Purchase Order ViewModels

## File Structure
- **PurchaseOrderItemVM**: PurchaseOrderItemVM.cs
- **PurchaseOrderVM**: PurchaseOrderVM.cs

## Core ViewModels

### PurchaseOrderItemVM
**Purpose**: ViewModel for individual items within a purchase order with quantity and pricing

**Properties**:

#### Identification
- `PurchaseOrderItemId`: Primary key identifier for the order item (int)
- `PurchaseOrderId`: Foreign key to parent purchase order (long)

#### Item Details
- `Quantity`: Number of units ordered (int) - defaults to 0
- `SinglePrice`: Unit price with decimal validation (decimal?) - defaults to null
- `ItemName`: Name/description of the item with length validation (string) - required

#### Navigation
- `PurchaseOrder`: Navigation to parent purchase order (PurchaseOrderVM)

### PurchaseOrderVM
**Purpose**: Main ViewModel for purchase order management with VAT support and supplier integration

**Properties**:

#### Basic Information
- `Id`: Primary key identifier (long)
- `PurchaseOrderCode`: Unique order code with validation (string, max 50) - required
- `SupplierId`: Supplier identifier with validation (int?) - required
- `Date`: Order date with validation (DateOnly?) - required

#### Financial Details
- `HasVAT`: Flag indicating if VAT is applicable (bool) - defaults to false
- `VATValue`: VAT percentage as decimal (0.05 = 5%) with range validation (decimal?) - defaults to 0.05m
- `OrderTotal`: Sum of (Quantity * SinglePrice) for all items with validation (decimal?) - defaults to 0.00m
- `OrderTotalWithVAT`: Final total including VAT with validation (decimal?) - defaults to 0.00m

#### Relationships & Navigation
- `Supplier`: Navigation to Supplier entity
- `Items`: Collection of purchase order items (ICollection<PurchaseOrderItem>)
- `suppliers`: Available suppliers for dropdown selection (IEnumerable<Supplier>)
- `SignatureId`: Manager Approval signature identifier (int?)
- `Signature`: Navigation to Signature entity for Manager approval

## Validation Features

### Custom Validation Attributes
- `[LocalizedRequired]`: Localized required field validation
- `[LocalizedMaxLength]`: Localized maximum length validation
- `[Range]`: Numeric value range validation with Arabic error messages
- `[Required]`: Standard required field validation

### Financial Validation
- **VAT Range**: 0.05 to 1 (5% to 100%)
- **Price Validation**: Positive decimal values for pricing
- **Total Calculations**: Automatic total and VAT calculations

## Key Features

### Purchase Order Management
- **Unique Order Coding**: Purchase order code for tracking and reference
- **Supplier Integration**: Complete supplier management and selection
- **Date Tracking**: Order date with proper data type handling

### Financial Processing
- **VAT Support**: Flexible VAT handling with percentage values
- **Automatic Totals**: Calculated order totals with and without VAT
- **Itemized Pricing**: Individual item quantity and price tracking

### Approval Workflow
- **Signature Integration**: Digital signature support for order approval
- **Supplier Selection**: Dropdown integration for supplier management
- **Multi-item Support**: Collection-based item management

### Data Integrity
- **Foreign Key Relationships**: Strong relationship enforcement
- **Default Values**: Safe default initialization for numeric fields
- **Collection Initialization**: Pre-initialized collections to prevent null issues

## Use Cases

### Purchase Order Creation
- Order header creation with supplier selection
- Item addition with quantity and pricing
- VAT configuration and calculation
- Order code generation and management

### Financial Processing
- Total order value calculation
- VAT application and computation
- Supplier payment processing
- Order approval workflows

### Reporting & Tracking
- Order history and tracking
- Supplier performance analysis
- VAT reporting and compliance
- Purchase analytics and reporting