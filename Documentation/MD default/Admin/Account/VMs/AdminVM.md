# Admin User Management

## ViewModel Information
**Model Name:** `AdminVM`  
**Namespace:** `FougeraClub.Areas.Admin.ViewModels.Account`

Represents an Admin user for system management, identity, and role assignment.

---

## Fields & Validation

### Personal Information
| Property | Description | Validation Rules |
|---------|-------------|------------------|
| **FullNameAr** | Full Arabic name | Required, Max 200 chars, Arabic letters & numbers only |
| **FullNameEn** | Full English name | Required, Max 200 chars, English letters & numbers only |
| **PhoneNumber** | Phone number (optional) | Unique among all users |

---

### Account Credentials
| Property | Description | Validation Rules |
|---------|-------------|------------------|
| **Username** | Login username | Required, Unique, Remote server validation for existing check |
| **Email** | Email address | Required, Valid email format, Unique |
| **PasswordHash** | Password input | Must include uppercase, lowercase, number, special char, Min length: 8 |
| **ConfirmPassword** | Password confirmation | Must match PasswordHash, Same password rules applied |

---

### Role Management
| Property | Description |
|---------|-------------|
| **RoleId** | Selected role (Required) |
| **Role** | Role name (Display) |
| **RolesList** | Dropdown choices (`IEnumerable<SelectListItem>`) |

---

### Digital Signature
| Property | Description |
|---------|-------------|
| **Signature** | Signature model used for digital sign workflow |

---

### Additional Flags
| Property | Type | Default | Purpose |
|---------|------|---------|---------|
| **IsTrainer** | `bool` | `false` | Indicates if the Admin is also a Trainer |

---

## Behavior & Logic

### Username & Email Protection
- Checked using `Unique` attribute
- `Remote` validation prevents duplicates in real-time

### Password Security Enforcement
- Strong password format required (uppercase + lowercase + digit + special character)

### Multilingual Support
- All validation messages sourced from resource files
- Compatible with Arabic/English UI culture

---

## Usage Context
- Admin Dashboard → User Management
- Create / Edit Admin accounts
- Assign system roles
- Assign trainer access where applicable
