# Mission Type Enumeration

## MissionType

**Namespace**: `Domain.Enums`

**Purpose**: Defines the different types of missions with localized display names

### Enum Values

#### 1. Forum
- **Value**: `1`
- **Display**: "Forum" (Localized)
- **Description**: Mission type for forum participation or organization

#### 2. Conference
- **Value**: `2`
- **Display**: "Conference" (Localized)
- **Description**: Mission type for conference attendance or hosting

#### 3. Exhibition
- **Value**: `3`
- **Display**: "Exhibition" (Localized)
- **Description**: Mission type for exhibition participation or setup

#### 4. Competition
- **Value**: `4`
- **Display**: "Competition" (Localized)
- **Description**: Mission type for competition involvement or organization

#### 5. ClubEquipment
- **Value**: `5`
- **Display**: "ClubEquipment" (Localized)
- **Description**: Mission type related to club equipment management or acquisition

#### 6. ExchangeOrDelivery
- **Value**: `6`
- **Display**: "ExchangeOrDelivery" (Localized)
- **Description**: Mission type for exchange programs or delivery operations

### Key Features

#### Localization Support
- **Resource-Based**: Uses `Resources.Resource1` for display names
- **Multi-language**: Supports internationalization through resource files
- **Display Attributes**: Proper UI representation with `[Display]` attributes

#### Mission Categorization
- **Event Types**: Forum, Conference, Exhibition, Competition
- **Operational Types**: ClubEquipment, ExchangeOrDelivery
- **Clear Distinction**: Well-defined mission categories for proper classification

#### Integration Benefits
- **Type Safety**: Compile-time checking of mission types
- **Database Storage**: Integer values for efficient storage
- **UI Consistency**: Standardized display names across application
- **Extensibility**: Easy to add new mission types as needed

### Use Cases
- Mission type classification in mission management systems
- Filtering and reporting based on mission categories
- UI dropdowns and selection interfaces
- Business logic branching based on mission type
- Localized display in user interfaces