# Expenses & Receipts – Add / Edit View

## Overview

This view handles **adding and editing Expenses and Receipts** within the Financial Management module. It dynamically adapts its UI and behavior based on:

* Item type (Expense / Receipt)
* Add vs Edit mode
* Signing state
* User interaction

Model used:

```
ExpenseAndReceiptVM
```

---

## Page Title & Breadcrumb Logic

* Titles are resolved dynamically using `ViewData` and localization resources.
* The displayed title depends on:

  * `ItemType` (Expenses / Receipts)
  * `Model.Id` (Add or Edit)

Breadcrumb adapts automatically to:

* Financial Management
* Expenses list or Receipts list
* Current Add/Edit action

---

## Form Configuration

```html
<form asp-action="AddEdit" method="post" enctype="multipart/form-data">
```

### Hidden Fields

* `Id` – record identifier
* `ItemTypeId` – selected type
* `Attachment_TempFilePath` – temporary uploaded file
* `Attachment_OldPath` – previous attachment

These ensure safe updates during edit operations.

---

## Item Type Selection (Expense / Receipt)

* Radio buttons generated from `ItemTypesList`.
* In **Edit mode**:

  * Selection is disabled
  * Value preserved using hidden input
* Client-side validation prevents submission without selection.

---

## Core Form Fields

### Common Fields

* Item Number *(required)*
* Date *(required)*
* Amount *(required)*
* Notes *(required)*

### Expense-only Fields

* Budget Item
* Expenses Section
* Expenses Source
* VAT

### Supplier

* Required for both Expenses and Receipts
* Dynamically filtered by supplier category

---

## Dynamic UI Rules (JavaScript)

### Item Type Change

* **Receipts**:

  * Hide Budget Item, Expenses Section, VAT
  * Supplier list filtered by category

* **Expenses**:

  * Show all financial fields
  * Load full supplier list

### Date Restriction Rule

* If Expense Source = `MiscellaneousExpenses`

  * Minimum allowed date enforced (`2025-12-01`)

---

## Supplier Population Logic

* All suppliers loaded from `ViewBag.AllSuppliers` (JSON)
* Filtered dynamically by category
* Supports Arabic / English names
* Safe refresh when Select2 is active

---

## File Upload (PDF)

* Accepts **PDF only**
* Maximum size: **5MB**
* Supports:

  * New upload
  * Temporary preview
  * Existing attachment preview

Preview button shown when a file exists.

---

## Save & Navigation Buttons

### Save Button

* Disabled when:

  * Record is already signed (`isSigned == true`)

### Back Button

* Redirects based on item type:

  * Expenses → Expenses List
  * Receipts → Receipts List

---

## Validation

* ASP.NET Core validation messages
* Custom client-side validation for Item Type selection
* Required field indicators via `.asteriskRequired`

---

## Select2 Integration

* Applied safely to dropdowns
* Loaded dynamically if not already present
* RTL supported

---

## Key Design Goals

* Single reusable Add/Edit view
* Strong UX rules based on business logic
* Safe edit behavior
* Clean separation between Expense and Receipt workflows

---

## Related Views

* Expenses List
* Receipts List
* Print Expenses
* Receipt View

---

*Documented for ASP.NET Core MVC – Financial Management Module*
