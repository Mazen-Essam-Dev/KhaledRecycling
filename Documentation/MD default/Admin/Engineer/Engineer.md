# 💼 Engineer Model Documentation

The Engineer entity represents a professional engineer’s personal, educational, and financial information. It includes validation for formats and ranges, and is connected to a nationality reference.

---

## 🧩 Class: Engineer

This model holds detailed profile data about an engineer, including contact info, specialization, documents, and salary-related information.

**🔧 Properties **

| Property             | Type           | Description                                  | Constraints / Notes                          |
| -------------------- | -------------- | -------------------------------------------- | -------------------------------------------- |
| `Id`                 | `int`          | Unique identifier for the engineer.          | Primary key 🔑                               |
| `FullName`           | `string?`      | Engineer's full name.                        | `MaxLength(100)`                             |
| `Code`               | `int?`         | Engineer-specific internal code.             | Optional                                     |
| `Specialization`     | `string?`      | Engineering field (e.g., Civil, Electrical). | `MaxLength(100)`                             |
| `GraduationYear`     | `int?`         | Year of graduation.                          | `Range(1900, 2100)`                          |
| `DateOfBirth`        | `DateTime?`    | Date of birth.                               | Stored as `date` in DB (not datetime) 📅     |
| `Position`           | `string?`      | Job position or title.                       | `MaxLength(100)`                             |
| `NationalityId`      | `int?`         | Foreign key referencing `Nationality`.       | Optional                                     |
| `Nationality`        | `Nationality?` | Navigation to nationality entity.            | Virtual navigation property                  |
| `WorkAddress`        | `string?`      | Address of current workplace.                | `MaxLength(200)`                             |
| `Address`            | `string?`      | Home or personal address.                    | `MaxLength(200)`                             |
| `PhoneNumber`        | `string?`      | Contact phone number.                        | `Phone` validation + `MaxLength(20)`         |
| `Email`              | `string?`      | Email address.                               | `EmailAddress` validation + `MaxLength(100)` |
| `Notes`              | `string?`      | Optional notes or comments.                  | No constraint                                |
| `ProfileImagePath`   | `string?`      | Path to profile image.                       | `MaxLength(255)`                             |
| `PassportNumber`     | `string?`      | Passport number.                             | `MaxLength(50)`                              |
| `PassportExpiryDate` | `DateOnly?`    | Passport expiration date.                    | Stored as `date` in DB 📅                    |
| `IdNumber`           | `string?`      | Government-issued ID number.                 | `MaxLength(50)`                              |
| `IdExpiryDate`       | `DateOnly?`    | National ID expiration date.                 | Stored as `date` in DB 📅                    |
| `IdReleaseLocation`  | `string?`      | Place where the ID was issued.               | `MaxLength(50)`                              |
| `Salary`             | `decimal?`     | Monthly or yearly salary.                    | `Range(0, 1,000,000)` 💰                     |
| `BankAccountNo`      | `string?`      | Bank account number.                         | `MaxLength(50)`                              |
| `BankName`           | `string?`      | Name of the bank.                            | `MaxLength(50)`                              |

**🔗 Relationships **

| Property      | Type           | Description                           |
| ------------- | -------------- | ------------------------------------- |
| `Nationality` | `Nationality?` | Linked nationality entity (optional). |
