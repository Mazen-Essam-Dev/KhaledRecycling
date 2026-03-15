## 📂 Namespace & Dependencies

```csharp
namespace Domain.Enums
```

## 🏷️ Enum: RoleNumber

Represents application-specific user role identifiers.

### 🔹 Enum Members

| Name                   | Value | Description                                               |
| ---------------------- | ----- | --------------------------------------------------------- |
| `NormalUser`           | 1     | Standard user with basic permissions.                     |
| `Manager`              | 2     | User with managerial permissions and elevated access.     |
| `ActivitiesSupervisor` | 3     | User responsible for supervising activities.              |
| `Accountant`           | 4     | User responsible for financial operations and accounting. |

### 📝 Notes

* The enum values can be used in the application to check roles programmatically.
* Typically used alongside `ApplicationRole.RoleNumber` for dynamic permission or authorization checks.
* Uses for determine if user has permission to put his signature. 