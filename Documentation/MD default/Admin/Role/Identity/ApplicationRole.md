## 📂 Namespace & Dependencies

```csharp
using Microsoft.AspNetCore.Identity;
```

## 🏷️ Class: ApplicationRole

**Namespace:** `Infrastructure.Identity`

`ApplicationRole` extends the built-in `IdentityRole` to include custom application-specific properties.

### 🔹 Properties

| Property     | Type  | Description                                                                                                           |
| ------------ | ----- | --------------------------------------------------------------------------------------------------------------------- |
| `RoleNumber` | `int` | A custom numeric identifier for the role, useful for application-specific role ordering, grouping, or identification. |

### 📝 Notes

* Inherits all default properties and behavior of `IdentityRole`, including `Id`, `Name`, `NormalizedName`, and `ConcurrencyStamp`.
* `RoleNumber` can be used to assign a numeric code to roles for additional logic in your application, such as permission checks or UI ordering.
