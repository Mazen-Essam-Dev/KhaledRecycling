# 🎓 Course Entity — Full Documentation

This Markdown file explains the **Course** entity used in the domain layer of an **ASP.NET Core 9 MVC application**.  
It defines the database structure, relationships, and purpose of each property — without code.

---

## 📘 Overview

The `Course` entity represents a **training course** that belongs to a **department** and may be managed by a specific **trainer**.  
It includes multilingual support, date tracking, file attachment paths, and navigation properties for relationships.

---

## 🧱 Table Structure

| Property | Type | Attributes | Description |
|-----------|------|-------------|--------------|
| **Id** | `int` | `[Key]` | Primary key for the Course table. |
| **DepartmentId** | `int?` | `[ForeignKey(nameof(Department))]` | Optional foreign key referencing the `Department` entity. |
| **TrainerId** | `int?` | `[ForeignKey(nameof(Trainer))]` | Optional foreign key referencing the `Trainer` entity. |
| **TitleAr** | `string?` | `[MaxLength(200)]` | Arabic name/title of the course (for multilingual support). |
| **TitleEn** | `string?` | `[MaxLength(200)]` | English name/title of the course. |
| **StartDate** | `DateOnly?` | — | The date when the course begins. |
| **EndDate** | `DateOnly?` | — | The date when the course ends. |
| **Location** | `string?` | `[MaxLength(200)]` | The location or venue of the course. |
| **Description** | `string?` | — | Full textual description or course overview. |
| **AttachmentPath** | `string?` | `[MaxLength(300)]` | File path or URL for course materials or attachments. |
| **Department** | `Department?` | `[Virtual]` | Navigation property for the related department. |
| **Trainer** | `Trainer?` | `[Virtual]` | Navigation property for the related trainer. |

---

## 🧩 Relationships

| Relationship | Type | Description |
|---------------|------|-------------|
| **Department → Course** | One-to-Many | Each department can have multiple courses. |
| **Trainer → Course** | One-to-Many | Each trainer can manage multiple courses. |

The use of `virtual` enables **lazy loading** (if proxy creation is enabled).

---

## 🌍 Multilingual Support

- The entity includes both **Arabic (`TitleAr`)** and **English (`TitleEn`)** fields.
- Useful for applications supporting **multi-language UIs** or resource-based translation.

---

## 📂 Attachments

- The `AttachmentPath` property stores the relative or absolute path to course files such as:
  - Course outline
  - Presentation slides
  - Reading materials
- Stored in a `string` format (e.g., `/uploads/courses/file.pdf`).

---

## 🗓️ Date Fields

| Field | Meaning |
|--------|----------|
| **StartDate** | Marks when the course begins. |
| **EndDate** | Marks when the course ends. |
| Both are of type `DateOnly` to store only the **date part** (no time). |

---

## ⚙️ Usage Notes

- Used in the **Courses Management Module** (Admin & Member areas).
- When creating or editing a course, multilingual input is supported through `CourseVM` (ViewModel).
- Attachments are uploaded via form binding and stored in a designated folder.
- `DepartmentId` and `TrainerId` are **optional**, allowing a course to be temporarily unassigned.

---

## 🔒 Validation & Constraints

| Attribute | Purpose |
|------------|----------|
| `[Key]` | Identifies the primary key. |
| `[MaxLength(n)]` | Restricts text column length in the database. |
| `[ForeignKey]` | Explicitly defines the relationship to another entity. |

---

## 🧠 Summary

The `Course` entity forms the backbone of the **training management** system.  
It provides:
- Clear data structure  
- Language flexibility  
- Strong foreign key relationships  
- Secure and validated data entry  

It’s designed to integrate seamlessly with **Entity Framework Core**, **MVC ViewModels**, and multilingual UI components.
