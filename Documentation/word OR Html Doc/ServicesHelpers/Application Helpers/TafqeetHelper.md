# TafqeetHelper Documentation

## File Structure
- **TafqeetHelper**: TafqeetHelper.cs

## Core Helper

### TafqeetHelper
**Purpose**: Converts numerical amounts to written text in both Arabic and English languages with proper currency formatting and grammatical rules

## Methods

### Tafqeet
**Purpose**: Main method that converts decimal amount to written text based on currency language

**Parameters**:
- `amount`: Decimal value to convert
- `mainCurrency`: CurrencyInfoDTO for primary currency unit (Dirham)
- `subCurrency`: CurrencyInfoDTO for fractional currency unit (Fils)

**Returns**: String representation of amount in words with currency

**Language Routing**:
- Arabic: Routes to TafqeetArabic for Arabic text conversion
- English: Routes to TafqeetEnglish for English text conversion

## Arabic Implementation

### TafqeetArabic
**Purpose**: Converts amount to Arabic written text with proper grammatical rules

**Features**:
- Handles negative amounts with "سالب" prefix
- Separates integer and fractional parts
- Applies Arabic dual and plural rules
- Supports amounts up to 100 billion

**Grammatical Rules**:
- **Singular**: Used for 1 unit
- **Dual**: Used for exactly 2 units
- **Plural**: Used for 3-10 units
- **Singular**: Used for 11+ units

### TafqeetWithCurrencyArabic
**Purpose**: Applies Arabic currency grammatical rules to number words

**Currency Rules**:
- 1: "واحد درهم"
- 2: "درهمان" 
- 3-10: "ثلاثة دراهم"
- 11+: "أحد عشر درهم"

### TafqeetNumberArabic
**Purpose**: Converts numbers to Arabic words with gender consideration

**Number Systems**:
- **Ones**: Separate masculine/feminine forms (واحد/واحدة)
- **Tens**: عشرون, ثلاثون, etc.
- **Hundreds**: مائة, مئتان, ثلاثمائة, etc.
- **Scales**: ألف, مليون, مليار with proper pluralization

**Scale Pluralization**:
- 1: "ألف"
- 2: "ألفان"
- 3-10: "ثلاثة آلاف"
- 11+: "أحد عشر ألف"

## English Implementation

### TafqeetEnglish
**Purpose**: Converts amount to English written text

**Features**:
- Handles negative amounts with "Minus" prefix
- Separates integer and fractional parts
- Uses standard English pluralization
- Supports amounts up to 100 billion

### TafqeetWithCurrencyEnglish
**Purpose**: Applies English currency rules to number words

**Currency Rules**:
- 1: "One Dirham"
- 2+: "Two Dirhams"

### TafqeetNumberEnglish
**Purpose**: Converts numbers to English words

**Number Systems**:
- **Ones**: One through Nineteen
- **Tens**: Twenty through Ninety
- **Hundreds**: Hundred
- **Scales**: Thousand, Million, Billion

## Technical Implementation

### Number Processing
- **Maximum Value**: 100,000,000,000 (100 billion)
- **Decimal Handling**: Separates integer and fractional parts
- **Negative Support**: Handles negative amounts
- **Rounding**: Mathematical rounding for fractional parts

### Language Support
- **Arabic**: Full grammatical gender support
- **English**: Standard number formatting
- **Currency Integration**: Uses CurrencyInfoDTO for proper terms

### Currency Integration
- **Main Currency**: Primary unit (Dirham)
- **Sub Currency**: Fractional unit (Fils)
- **Suffix Support**: Country designation (إماراتي/UAE)
- **Language Detection**: Auto-routing based on currency language

## Usage Examples

### Arabic Usage
```csharp
var amount = 1234.56m;
var result = TafqeetHelper.Tafqeet(amount, CurrencyHelper.AED_Main_Ar, CurrencyHelper.AED_Sub_Ar);
// Result: "ألف ومائتان وأربعة وثلاثون درهم إماراتي وستة وخمسون فلس إماراتي"