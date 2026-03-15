# BudgetItem & BudgetItemVM Explanation (Easy Copy)

## 1. Entity (Domain Layer) — `BudgetItem`

This class represents the **database model** for budget items.  
It maps directly to a **database table**.

### Purpose
Used to store basic information about a budget item that will likely be used in financial or expense records.

### Fields
| Property | Description |
|---------|-------------|
| `Id` | Primary key of the budget item. |
| `ItemNumber` | An optional number/identifier for organizational or accounting purposes. |
| `ItemTitle` | Name or title of the budget item. Limited to 200 characters. |

### Notes
- The model contains **no UI validation** logic.
- It is **purely a data storage structure**.

---

## 2. ViewModel (Presentation Layer) — `BudgetItemVM`

This class is designed for **UI and form interaction** in the Admin panel.

### Purpose
Allows:
- Validation in forms.
- Extra UI properties that are not stored in the database.
- Logic related to showing or disabling actions in the UI.

### Fields
| Property | Description |
|---------|-------------|
| `Id` | Same as the entity. |
| `ItemNumber` | Shown in UI but optional. |
| `ItemTitle` | Required in UI (`LocalizedRequired`) to ensure user inputs a name. |
| `HasRelatedExpenseOrReceipt` | **Not stored in DB.** Used to determine if the item is already used in another table (e.g., Expenses/Receipts). If true, editing or deleting may be disabled. |

### Key Differences
- `ItemTitle` in the ViewModel has **localized validation**, meaning the error message is automatically translated.
- `HasRelatedExpenseOrReceipt` is a **view-only helper property**, not stored in DB.

---

## 3. Difference Between Entity and ViewModel

| Feature | Entity (`BudgetItem`) | ViewModel (`BudgetItemVM`) |
|--------|------------------------|----------------------------|
| Stored in Database | Yes | No |
| Used in UI / Forms | Possibly | Always |
| Validation | No localized validation | Has localized validation (`LocalizedRequired`) |
| Extra UI Logic | No | Yes (`HasRelatedExpenseOrReceipt`) |

---

## 4. Summary

- **Entity** = Data that is stored in the database.
- **ViewModel** = Data that is shown to the user in forms/views.
- `HasRelatedExpenseOrReceipt` is used to **prevent deletion or editing** if the budget item is already referenced somewhere else.

---
