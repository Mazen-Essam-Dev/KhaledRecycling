# Cash Disbursement Voucher – Views Documentation

## 6️⃣ Add / Edit Cash Disbursement Voucher View (`AddEdit.cshtml`)

### 📌 Purpose

This view is used for **creating and editing** a Cash Disbursement Voucher. It supports dynamic details rows, validation, and business rules that lock editing after approval.

---

### 🧩 Model

```csharp
CashDisbursementVoucherVM
```

The ViewModel contains:

* Voucher header data (Type, DocumentNo, Date, Month, Description)
* Collection of voucher **Details**
* Calculated **TotalAmount**
* Approval flags (e.g. `AcknowledgmentReceiptApprovalDone`)

---

### 🧭 Page Mode

```csharp
var isEdit = Model?.Id > 0;
```

* **Create mode** → New voucher
* **Edit mode** → Existing voucher

Titles and breadcrumbs change dynamically based on mode.

---

### 🔐 Business Rules (UI Level)

* If `AcknowledgmentReceiptApprovalDone == true`:

  * Voucher **Type** becomes read-only
  * **Save** button is disabled
  * Existing data is view-only

This prevents modifying approved vouchers.

---

### 🧾 Voucher Header Fields

| Field       | Description                         |
| ----------- | ----------------------------------- |
| Type        | Radio buttons loaded from enum list |
| DocumentNo  | Voucher number                      |
| Date        | Main voucher date                   |
| Month       | Auto-filled from Date (localized)   |
| Description | General voucher description         |

Month is automatically calculated using JavaScript when the date changes.

---

### 💰 Voucher Details (Dynamic Rows)

This section allows adding multiple **disbursement detail rows** dynamically.

#### Detail Inputs

* Document Number
* Document Date
* Amount
* Notes

Rows are added client-side and bound correctly to MVC using indexed names:

```text
Details[0].DocumentNo
Details[0].DocumentDate
Details[0].Notes
Details[0].Amount
```

---

### ➕ Add Detail Row (JavaScript)

Key behaviors:

* Client-side validation before adding
* Automatic row indexing
* Inputs become read-only after insertion

```js
let rowIndex = Model.Details.Count;
```

---

### 🗑 Delete Detail Row

* Clicking delete opens a **confirmation modal**
* After deletion:

  * Rows are re-indexed
  * Total amount is recalculated

This ensures **correct model binding** on submit.

---

### 🧮 Total Amount Calculation

```js
function updateTotalAmount() {
    let total = 0;
    $('.Amount').each(function () {
        total += parseFloat($(this).val()) || 0;
    });
    $('#totalAmount').val(total.toFixed(2));
}
```

The total is recalculated automatically whenever rows are added or removed.

---

### 📅 Month Auto-Fill Logic

* Listens to changes on the `Date` field
* Converts date to **localized month name**
* Supports Arabic / English based on UI culture

---

### 💾 Form Submission

* Uses standard POST to `AddEdit` action
* Protected with **Anti-Forgery Token**
* Fully compatible with ASP.NET Core model binding

---

### ✅ Summary

The **Add/Edit view** provides:

* Strong client-side validation
* Dynamic detail management
* Safe approval-based locking
* Clean MVC model binding

It complements the Index, List, Print, and Attachments views to form a **complete Cash Disbursement Voucher workflow**.

---

📌 *This section extends the Cash Disbursement Voucher Views Documentation.*
