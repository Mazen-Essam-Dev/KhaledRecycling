# Role Management Module Documentation

This file documents the `Roles` module in your ASP.NET Core 9 MVC application. It covers the main views: `Index`, `Manage`, and `_CreateForm`. The focus is on functionality, ViewModel usage, permissions, and interactions.

---

## 1. Manage.cshtml

**Purpose:** Assign and manage permissions for a specific role.

**Key Features:**
- Display role name and allow editing (except for SuperAdmin, if needed).
- Permissions are grouped by controller for easy navigation.
- `Select All` checkbox per group for bulk selection.
- Save button to persist changes via a POST form.
- Responsive grid layout to support different screen sizes.
- Alert messages for errors (`TempData["ErrorMessage"]`).

**Special Logic:**
- Permission names are localized using `Resource1` and `Resource2`.
- Group checkboxes dynamically maintain an **indeterminate state** when only some permissions are selected.
- JavaScript ensures proper model binding for checkboxes by creating hidden inputs as necessary.
- Optional visual hover feedback highlights permissions when hovering over `Select All`.

**Scripts:**
- Handles `Select All` functionality per controller group.
- Updates indeterminate state for partially selected groups.
- Ensures all checkboxes bind correctly for ASP.NET Core model binding.
- Provides visual cues on hover over select-all labels.

---

## 2. Index.cshtml

**Purpose:** Display a list of all roles and provide actions to manage, edit, or delete them.

**Key Features:**
- Breadcrumb navigation.
- Records count display.
- Permission-based action buttons:
  - Create new role
  - Manage role permissions
  - Edit role name
  - Delete role (only if multiple roles exist)
- Responsive table layout with icons for actions.
- Edit Role and Delete Role implemented via **Bootstrap modals**.

**Special Logic:**
- Buttons and actions only appear if the current user has appropriate permissions (`PermissionScanner.ValidatePermission()`).
- Delete confirmation modal displays role name dynamically.
- Edit role modal pre-populates the role name and updates hidden inputs for proper binding.
- Uses session storage or state management as needed for modal interactions.

**Modals:**
- **Edit Role Modal:** Allows updating role name and submits via POST to `Manage` action.
- **Delete Confirmation Modal:** Confirms deletion before submitting form to `Delete` action.

**Scripts:**
- Populate modals dynamically with role name.
- Handle modal show events to update inputs.
- Optional dynamic display for delete info messages.

---

## 3. _CreateForm.cshtml

**Purpose:** Provide a form to create a new role.

**Key Features:**
- Role name input with validation.
- Submit button to create a role.
- Integrated with server-side anti-forgery token.
- Responsive layout for small and large screens.
- Uses `RoleFormVM` ViewModel.

**Special Logic:**
- Placeholder and label are localized.
- Button includes icon for visual clarity.
- Form POSTs to `Create` action in Roles controller.

---

## 4. General Notes

- **ViewModels:**  
  - `RoleFormVM` for creating roles.  
  - `PermissionVM` for managing role permissions.
- **Localization:**  
  - Display names and labels use `Resource1` and `Resource2`.
- **Client-side validation:**  
  - Required fields and checkbox binding handled via JavaScript.
- **Permissions:**  
  - Actions (Create, Edit, Delete, Manage Permissions) are conditionally displayed based on `PermissionScanner.ValidatePermission()`.
- **Responsive Design:**  
  - Uses CSS grid and media queries to support mobile and desktop layouts.
- **Security:**  
  - Anti-forgery tokens used for all forms.
  - Only users with appropriate permissions can see or interact with action buttons.

---

### Summary

This module handles **full role management** functionality:

1. `Manage` – assign or revoke permissions per role, with group and select-all support.
2. `Index` – list roles, manage, edit, or delete them with modal confirmations.
3. `_CreateForm` – create new roles with validation and localized labels.

All operations respect **user permissions**, and modals ensure secure and user-friendly interactions.
