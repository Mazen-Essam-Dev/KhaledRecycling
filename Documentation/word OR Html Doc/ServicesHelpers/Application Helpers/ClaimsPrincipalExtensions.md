# 📄 ClaimsPrincipalExtensions

This document explains the **ClaimsPrincipalExtensions** helper class used to simplify access to authenticated user data in an ASP.NET Core application.

---

## 🧩 Purpose

`ClaimsPrincipalExtensions` provides **extension methods** for `ClaimsPrincipal` to:

- Retrieve the current logged-in user ID
- Retrieve the user phone number from the database asynchronously

This helps keep controllers, services, and middleware **clean and readable** by centralizing common identity-related logic.

---

## 📦 Namespace

```
Application.Helpers
```

---

## 📚 Dependencies

```csharp
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
```

| Dependency | Description |
|---------|-------------|
| Infrastructure.Repositories.InterfacesDB | Access to `IUnitOfWork` and repositories |
| Microsoft.EntityFrameworkCore | Async LINQ database queries |
| System.Security.Claims | Access to user claims |

---

## 🧠 Static Helper Class

```csharp
public static class ClaimsPrincipalExtensions
```

- Declared as `static`
- Contains **extension methods** for `ClaimsPrincipal`
- Can be used anywhere once the namespace is imported

---

## 🔑 GetUserId()

```csharp
public static string? GetUserId(this ClaimsPrincipal user)
```

### ✅ What It Does

- Extracts the **User ID** from the authenticated user claims
- Uses the standard claim type:

```csharp
ClaimTypes.NameIdentifier
```

### 🔄 Behavior

- Returns `string?`
- Returns `null` if:
  - User is not authenticated
  - Claim does not exist

### 🧪 Example Usage

```csharp
var userId = User.GetUserId();
```

---

## 📱 GetUserPhoneNumberAsync()

```csharp
public static async Task<string?> GetUserPhoneNumberAsync(
    this ClaimsPrincipal user,
    IUnitOfWork unitOfWork)
```

### ✅ What It Does

- Retrieves the logged-in user's **phone number** from the database
- Uses the User ID stored in claims
- Queries the `Users` table via `IUnitOfWork`

---

### 🔍 Internal Flow

1. Extract User ID from claims
2. If User ID is missing → return `null`
3. Query database:

```csharp
unitOfWork.Users.Table
```

4. Select only the `PhoneNumber`
5. Return the value asynchronously

---

### 🧪 Example Usage

```csharp
var phoneNumber = await User.GetUserPhoneNumberAsync(_unitOfWork);
```

---

## ⚠️ Notes & Best Practices

- Designed for **read-only identity helpers**
- Avoid heavy logic inside extension methods
- Safe for Controllers, Services, and Middleware
- Keeps Identity-related queries centralized

---

## ✅ Summary

This helper class:

- Improves code readability
- Reduces duplication
- Encapsulates claim + database logic
- Aligns with clean architecture principles

📌 *Recommended for projects using ASP.NET Core Identity with Unit of Work pattern.*

