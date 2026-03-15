# Purchase Order Entities

## File Structure
- **PurchaseOrder**: PurchaseOrder.cs
- **PurchaseOrderItem**: PurchaseOrderItem.cs

## Core Entities

### PurchaseOrder
**Purpose**: Main entity for purchase order management with VAT support and supplier integration

**Properties**:

#### Basic Information
- `Id`: Primary key identifier (long)
- `PurchaseOrderCode`: Unique order code (string, max 50) - required
- `SupplierId`: Supplier identifier (int?) - required
- `Date`: Order date (DateOnly?) - required

#### Financial Details
- `HasVAT`: Flag indicating if VAT is applicable (bool) - defaults to false
- `VATValue`: VAT percentage as decimal (0.05 = 5%) with range validation (decimal?) - defaults to 0.05m
- `OrderTotal`: Sum of (Quantity * SinglePrice) for all items (decimal?) - defaults to 0.00m
- `OrderTotalWithVAT`: Final total including VAT (decimal?) - defaults to 0.00m

#### Relationships & Navigation
- `Supplier`: Navigation to Supplier entity
- `Items`: Collection of purchase order items (ICollection<PurchaseOrderItem>)
- `SignatureId`: Manager Approval signature identifier (int?)
- `Signature`: Navigation to Signature entity for Manager approval

### PurchaseOrderItem
**Purpose**: Entity for individual items within a purchase order with quantity and pricing

**Properties**:

#### Identification
- `PurchaseOrderItemId`: Primary key identifier for the order item (int)
- `PurchaseOrderId`: Foreign key to parent purchase order (long) - required

#### Item Details
- `Quantity`: Number of units ordered (int?) - defaults to 0
- `SinglePrice`: Unit price with range validation (decimal?) - defaults to 0.00m
- `ItemName`: Name/description of the item (string, max 200) - required

#### Navigation
- `PurchaseOrder`: Navigation to parent purchase order entity

## Validation Features

### Data Annotations
- `[Required]`: Required field validation
- `[MaxLength]`: String length constraints
- `[Range]`: Numeric value range validation with Arabic error messages
- `[ForeignKey]`: Explicit foreign key relationships

### Financial Validation
- **VAT Range**: Minimum 0.05 (5%)
- **Price Validation**: Positive decimal values (0.00 to 9,999,999,999)
- **Arabic Messages**: Localized error messages for Arabic users

## Key Features

### Purchase Order Management
- **Unique Order Coding**: Purchase order code for tracking and reference
- **Supplier Integration**: Complete supplier relationship management
- **Date Tracking**: Order date with specific database date type

### Financial Processing
- **VAT Support**: Configurable VAT with percentage values
- **Automatic Totals**: Calculated order totals with and without VAT
- **Itemized Pricing**: Individual item quantity and price tracking

### Approval Workflow
- **Signature Integration**: Digital signature support for order approval
- **Multi-item Support**: Collection-based item management
- **Relationship Integrity**: Strong foreign key enforcement

### Data Integrity
- **Default Values**: Safe initialization for numeric fields
- **Collection Initialization**: Pre-initialized collections to prevent null issues
- **Database Optimization**: Proper data types and constraints

## Use Cases

### Purchase Order Processing
- Order creation and supplier assignment
- Item management with quantity and pricing
- VAT configuration and financial calculations
- Order approval and signature tracking

### Financial Management
- Total order value computation
- VAT application and reporting
- Supplier payment processing
- Purchase analytics and reporting

### Inventory & Procurement
- Item procurement tracking
- Supplier performance monitoring
- Purchase history maintenance
- Budget compliance verification