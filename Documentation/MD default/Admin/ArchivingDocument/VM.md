# ArchivingDocument & ArchivingDocumentVM Explanation (Easy Copy)

## 1. Entity (Domain Layer) — `ArchivingDocument`

This class represents the **database model** for storing archived documents.  
It maps directly to the **database table** and contains fields stored in the DB.

### Key Points:
- `Id`: Primary key.
- `Title`: Name/title of the document. Max length 200.
- `Date`: The date related to the document (could be issue date or archival date).
- `Authority`: Issuing or relevant authority/organization.
- `DocumentReferenceNumber`: The reference number of the document.
- `Type`: Enum representing the category/type of document (`DocumentType` enum).
- `PdfFilePath`: String path to the uploaded PDF stored on disk/server.
- `PdfFile`: Not mapped to the database. Used **only for upload** in forms.

### Why `[NotMapped]`?
Because `PdfFile` is used to upload the file but is **not stored in DB**, only its **file path** is stored.

---

## 2. ViewModel (Presentation Layer) — `ArchivingDocumentVM`

This is used only in **Forms / Views / UI**.  
It does **not represent the database**.  
It helps validation, texts, dropdowns, and user interaction.

### Key Points:
- Contains similar fields as the Entity, but includes:
  - **Validation attributes with localization** (`LocalizedRequired`, `LocalizedMaxLength`).
  - `TypeEnumList`: List of document types for a **select dropdown**.
  - `TypeText`: Used for **displaying the selected enum value in UI**.
- `PdfFile` is still used for file upload (not stored in DB).
- `PdfFilePath` still holds the uploaded file storage location.

---

## 3. Difference Between Entity and ViewModel

| Feature | Entity (`ArchivingDocument`) | ViewModel (`ArchivingDocumentVM`) |
|--------|------------------------------|-----------------------------------|
| Stored in Database | Yes | No |
| Used in UI / Views | Sometimes | Always |
| Has Validation Attributes for UI | Minimal | Yes (with localization) |
| File Upload Handling | `PdfFile` (NotMapped) | `PdfFile` (NotMapped) |
| Dropdown List Support | No | Yes (`TypeEnumList`) |
| Contains Display Text for Enum | No | Yes (`TypeText`) |

---

## 4. Summary (Very Simple)

- **Entity** = Represents DB table.  
- **ViewModel** = Represents form the user interacts with.  
- **File upload logic**: `PdfFile` is uploaded, saved as file → path stored in `PdfFilePath`.

---
