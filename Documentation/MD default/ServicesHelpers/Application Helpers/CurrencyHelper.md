# Application Helpers Documentation

## CurrencyHelper

### Overview
A static utility class providing comprehensive currency information for UAE Dirham (AED) in both Arabic and English languages. This helper supports proper grammatical forms for singular, dual, and plural amounts in financial applications.

### Currency Information Structure

#### CurrencyInfoDTO Properties
- **Singular**: Term for single unit (1 Dirham)
- **Dual**: Term for two units (2 Dirhams) - Arabic specific
- **Plural**: Term for three or more units (3+ Dirhams)
- **IsMasculine**: Grammatical gender for proper sentence construction
- **Suffix**: Currency origin/country designation
- **Language**: Language identifier ("ar" for Arabic, "en" for English)

### Arabic Currency Definitions

#### AED_Main_Ar (UAE Dirham - Arabic)
- **Singular**: "درهم" - Single Dirham
- **Dual**: "درهمان" - Two Dirhams (Arabic dual form)
- **Plural**: "دراهم" - Three or more Dirhams
- **Gender**: Masculine (مذكر)
- **Suffix**: "إماراتي" - Emirati designation
- **Usage**: Main currency unit for Arabic interfaces

#### AED_Sub_Ar (UAE Fils - Arabic)
- **Singular**: "فلس" - Single Fils
- **Dual**: "فلسان" - Two Fils (Arabic dual form)
- **Plural**: "فلوس" - Three or more Fils
- **Gender**: Masculine (مذكر)
- **Suffix**: "إماراتي" - Emirati designation
- **Usage**: Sub-currency unit (1 Dirham = 100 Fils)

### English Currency Definitions

#### AED_Main_En (UAE Dirham - English)
- **Singular**: "Dirham" - Single Dirham
- **Dual**: "" - Empty (English doesn't use dual forms)
- **Plural**: "Dirhams" - Multiple Dirhams
- **Gender**: Masculine (for grammatical consistency)
- **Suffix**: "UAE" - Country designation
- **Usage**: Main currency unit for English interfaces

#### AED_Sub_En (UAE Fils - English)
- **Singular**: "Fils" - Single Fils
- **Dual**: "" - Empty (English doesn't use dual forms)
- **Plural**: "Fils" - Same as singular (English convention)
- **Gender**: Masculine (for grammatical consistency)
- **Suffix**: "UAE" - Country designation
- **Usage**: Sub-currency unit for English interfaces

### Language-Specific Features

#### Arabic Language Support
- **Dual Forms**: Proper grammatical handling for two units (مثنى)
- **Gender Agreement**: Masculine gender for currency terms
- **Plural Forms**: Correct Arabic plural patterns (جمع)
- **Cultural Context**: Emirati-specific currency designation

#### English Language Support
- **Simplified Forms**: No dual number distinction
- **Standard Pluralization**: Regular plural forms
- **International Standards**: UAE designation for clarity
- **Consistent Gender**: Maintained for cross-language compatibility

### Usage Examples

#### Arabic Usage
```csharp
// For 1 Dirham
var currency = CurrencyHelper.AED_Main_Ar;
string amountText = $"واحد {currency.Singular} {currency.Suffix}";

// For 2 Dirhams  
string amountText = $"اثنان {currency.Dual} {currency.Suffix}";

// For 5 Dirhams
string amountText = $"خمسة {currency.Plural} {currency.Suffix}";