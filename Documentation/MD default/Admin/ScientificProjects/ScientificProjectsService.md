# 📄 ScientificProjectsService.cs

## 📦 Namespace
`Application.Services.Admin`

---

## 🧩 Class: ScientificProjectsService
Handles **business logic for Scientific Projects** in the Admin module, including:

- Sending OTPs for approval/signature
- Validating OTPs from multiple roles
- Updating signatures in the database

### 🔹 Attributes
- None specific, standard service class

---

## 🔹 Dependencies
| Dependency | Purpose |
|------------|---------|
| `IUnitOfWork _unitOfWork` | Repository & database unit of work |
| `IWebHostEnvironment _env` | Access web host environment (for file paths if needed) |
| `IHttpContextAccessor _httpContextAccessor` | Access session & HTTP context for OTP management |
| `ISMSService _SMSService` | Send SMS (currently not used) |
| `ISMSForSendingOTPService _SMSForSendingOTPService` | Send OTP SMS |
| `string FileName = "ScientificProjects"` | Default folder name for file attachments |

---

## 🔹 Core Responsibilities
- **OTP management** for signature approval
- **Signature assignment** for multiple roles:
  - Manager
  - Activity Monitor
  - Trainer
- **Database persistence** of signatures using UnitOfWork

---

## 🔹 Methods

### 1. SendOtpAsync

```csharp
public async Task<bool> SendOtpAsync(int id, string role)
