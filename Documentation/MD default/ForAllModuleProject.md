# For All Modules of this Project Documentation

>>>> This document covers all Project and show specific instructions: controller, service, view model, and views.

---
## 📂 Namespace & Dependencies
```
using Application.Interfaces;
using Domain.Entities; 
```
## 🧰 Required Libraries
- `Microsoft.AspNetCore.Mvc`: For routing and controller actions.

## Attributes
- All HttpPost,Delete,put will save her logs in logsRequest Table in Admin Area.
- All HttpGet `will not` save her logs in logsRequest Table in Admin Area.
- [YesGet] : is custom attribute used For make this Get method Saved in logsRequest Table.
- [NoLogging] : is custom attribute used For make this action not Saved in logsRequest Table.
- [AdminAuthorize] : is custom attribute To Sure That this Controller's Actions called only if user Have permission.
- [IgnoreAction] : is custom attribute To Sure That this Actions Not Stored in permissions Table (Claims).
- [ValidateAntiForgeryToken] : is attribute It ensures that every POST request to your server includes a special hidden token that proves the request came from your site — not another website [is used to protect your web application from Cross-Site Request Forgery (CSRF) attacks] , [If a user is logged into your banking app and visits a malicious site, that site could secretly submit a form to your app to transfer money — using the user’s valid session cookie].

## 🧭 Route
```
[Area("AreaName")]
[Route("[AreaName]/[ControllerName]/[ActionName]")]
```

## 🧩 Constructor Injection

- `IUnitOfWork`: Data abstraction layer.
- `IMapper`: AutoMapper for entity-VM conversion.


## 🖼️ Views:
>>>> Multi-language support via @inject ILanguageService.
>>>> Inputs and validation messages rendered with Tag Helpers.
>>>> Uses partial _ValidationScriptsPartial for client-side validation.

## ⚠️ Common Exceptions & Handling

- `404 NotFound`: If `id` is null or entity not found in Edit/Delete/Details.
- `500 Server Error`: If any code error or Exception if in deployment mode only.
- `503 Not Authorized`: If user try to call action not in his permission.
- `DbUpdateException`: When database fails to save changes or On failure during SaveChanges.
- `ModelState.IsValid == false`: Prevents invalid form submissions.
- `File upload path not found`: Directory is auto-created if missing.
- `Ensure all required fields are filled and valid`

## in 	
## Page Signature & Field Behavior Documentation

This document explains the behavior of signatures, buttons, printing, and input fields for pages with monetary or integer values. It is designed to ensure a smooth user experience (UX) and prevent accidental changes.

---

## 1. Printing Behavior

- **Condition:** All prints are **not shown** until **all required signatures** on the page are signed.  
- **Special Case:** If the page has **two signatures**, both must be signed to enable printing.

---

## 2. Button Behavior When One Signature is Signed

- If **only one signature** is signed:
  - Show **Fake Edit & Delete buttons**.
  - **Disable** these buttons to prevent editing or deletion.

---

## 3. Save/Submit Behavior

- In **Add/Edit mode**:
  - If the page has **at least one signature signed**, the **backend Save/Submit button** should be **hidden**.
  - Prevent saving to ensure integrity when partial signatures exist.

---

## 4. Printing Condition

- The **print Button** is only available if **all signatures** on the page have been signed.

---

## 5. Input Field Defaults

### Monetary / Decimal Fields
- If value is `null` and first is null in (DbEntity , VM) :
  - Set **placeholder**: `"0.00"`.
  - Ensures UX clarity and prevents double-click changes.

### Integer Fields
- If value is `null` and first is null in (DbEntity , VM) : 
  - Set **placeholder**: `"0"`.
  - Helps users understand the default value without accidental edits.
### Any Mobile or Phone Fields Remove 0 from First Digit and add 971 if not
9710 --> 971 , 088 --> 97188 , 258455 --> 971258455
---

## UX Notes

- Prevent users from unintentionally changing default values by:
  - Disabling editing until conditions are met.
  - Using clear placeholders for null values.
- Ensures smooth workflow and reduces errors in financial data entry.

