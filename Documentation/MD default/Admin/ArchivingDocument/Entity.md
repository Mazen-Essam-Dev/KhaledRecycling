# ArchivingDocument — Entity Documentation

The `ArchivingDocument` class represents a **document that is stored in the organization's archive system**.  
This model is typically used in modules that manage official documents, letters, certificates, reports, or administrative records.

---

## Purpose

This entity is designed to support:

- Uploading a PDF file.
- Storing document details such as issuing authority, reference number, and date.
- Categorizing documents using an enum (`DocumentType`).
- Displaying stored records in lists, search pages, and filter reports.

---

## Property Breakdown

| Property | Type | Meaning / Usage | Notes |
|---------|------|----------------|------|
| `Id` | `int` | Unique identifier for the document | Used internally by the database |
| `Title` | `string` | Title or subject of the document | Max length 200 |
| `Date` | `DateTime?` | The date associated with the document (issue or record date) | Nullable |
| `Authority` | `string` | The issuing authority (e.g., Government Dept., Internal Committee) | Max length 200 |
| `DocumentReferenceNumber` | `string` | Official reference / tracking number | Max length 200 |
| `Type` | `DocumentType?` | Document classification (e.g., Internal, Official, Confidential) | Enum-based for consistent grouping |
| `PdfFilePath` | `string` | Path where the uploaded PDF is saved on the server | Only stores *path*, not file itself |
| `PdfFile` | `IFormFile` (Not Mapped) | Used during file upload to receive the PDF from the form | Does not get stored in DB |

---

## File Handling Logic

- `PdfFile` is a **temporary field** used for uploading.
- When the document is saved:
  1. The uploaded file is stored in a folder such as `/uploads/Archive/`
  2. The full file path or relative path is saved into `PdfFilePath`
- `PdfFilePath` is the string displayed when retrieving or downloading the file.

This ensures clean separation between:
- File *storage* (file system)
- File *metadata* (database)

---

## Example Real Usage Scenario

| Title | Authority | Reference No. | Date | Type | PDF |
|------|-----------|---------------|------|------|-----|
| قرار تشكيل اللجنة | وزارة الشباب | 2023/45 | 2023-09-15 | رسمي | `uploads/Archive/123.pdf` |
| تقرير سنوي | النادي الثقافي | N/A | 2024-01-10 | داخلي | `uploads/Archive/456.pdf` |

This allows:
- Searching by title or authority
- Filtering documents by date or document type
- Opening PDF files directly for review

---

## Summary

The `ArchivingDocument` entity provides a structured model for storing and managing archived documents.  
It enables:

- Document classification
- PDF upload and secure storage
- Metadata tracking (authority, reference number, date)
- Clean database design via `NotMapped` upload property

This model forms the foundation for building a full **digital archiving system**.

