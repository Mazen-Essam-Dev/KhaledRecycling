# 🏢 Employee Model Documentation

The Employee class represents a staff Trainers And employees within the system. It holds personal, identification, contact, employment, and financial information, as well as document paths and relationships to other entities.

---

## 🧩 Class: Employee

Used to store detailed information about an employee or trainer, including foreign key references to Department and Nationality.

**🔧 Properties **

| Property                      | Type        | Description                                                   | Constraints / Notes                          |
| ----------------------------- | ----------- | ------------------------------------------------------------- | -------------------------------------------- |
| `Id`                          | `int`       | Unique identifier for the employee.                           | Primary key 🔑                               |
| `Code`                        | `string`    | Unique employee code.                                         | Required (non-nullable)                      |
| `FullName`                    | `string?`   | Full name of the employee.                                    | `MaxLength(100)`                             |
| `PhoneNumber`                 | `string?`   | Contact phone number.                                         | `MaxLength(20)`                              |
| `NationalityId`               | `int?`      | Foreign key referencing `Nationality`.                        | Optional                                     |
| `DepartmentId`                | `int?`      | Foreign key referencing `Department`.                         | Optional                                     |
| `Email`                       | `string?`   | Email address.                                                | `MaxLength(100)`                             |
| `JobTitle`                    | `string?`   | Job or role title.                                            | `MaxLength(100)`                             |
| `Address`                     | `string?`   | Home or work address.                                         | `MaxLength(200)`                             |
| `PassportNumber`              | `string?`   | Passport number.                                              | `MaxLength(50)`                              |
| `PassportExpiryDate`          | `DateOnly?` | Passport expiration date.                                     | Optional 📅                                  |
| `NationalIdNumber`            | `string?`   | National ID number.                                           | `MaxLength(50)`                              |
| `NationalIdExpiryDate`        | `DateOnly?` | National ID expiration date.                                  | Optional 📅                                  |
| `BankAccountNumber`           | `string?`   | Bank account number.                                          | `MaxLength(50)`                              |
| `BankName`                    | `string?`   | Bank name.                                                    | `MaxLength(100)`                             |
| `PhotoPath`                   | `string?`   | Path to employee's profile photo.                             | `MaxLength(200)`                             |
| `IdImagePath`                 | `string?`   | Path to ID image.                                             | `MaxLength(200)`                             |
| `PassportImagePath`           | `string?`   | Path to passport image.                                       | `MaxLength(200)`                             |
| `SocialCommunityApprovalPath` | `string?`   | Path to social community approval document.                   | `MaxLength(200)`                             |
| `VisitingCardPath`            | `string?`   | Path to visiting/business card.                               | `MaxLength(200)`                             |
| `DirectWorkPoemsPath`         | `string?`   | Path to direct work permit or related document.               | `MaxLength(200)`                             |
| `Salary`                      | `double?`   | Salary amount.                                                | Nullable 💰                                  |
| `Notes`                       | `string?`   | Additional notes about the employee.                          | No length constraint ✍️                      |
| `Type`                        | `int`       | Type identifier (e.g., permanent, trainer, contractor, etc.). | Required; could be used for role-based logic |

**🔗 Relationships **

| Navigation Property | Type                  | Description                                    |
| ------------------- | --------------------- | ---------------------------------------------- |
| `Courses`           | `ICollection<Course>` | Courses conducted or attended by the employee. |
| `Department`        | `Department?`         | Department to which the employee belongs.      |
| `Nationality`       | `Nationality?`        | Nationality of the employee.                   |

