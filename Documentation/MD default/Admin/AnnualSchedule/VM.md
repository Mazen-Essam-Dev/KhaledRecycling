# AnnualScheduleVM — ViewModel Documentation

The `AnnualScheduleVM` is the **ViewModel** used for creating, editing, and displaying annual schedule records in the Admin panel.  
It mirrors the `AnnualSchedule` entity but adds **validation attributes** and **display labels**, especially for multilingual support.

---

## Purpose of the ViewModel

This ViewModel ensures:

- **Input Validation** (required fields, numeric checks, text length limits).
- **User-Friendly Field Names** for UI forms.
- **Binding** between form inputs and backend logic.

It is used in forms where the administrator fills out yearly performance or progress schedule items.

---

## Fields Explanation

| Property | Meaning | User Input | Validation Applied |
|---------|---------|------------|-------------------|
| `Id` | Unique identifier (used for edit/update) | No | None |
| `Item` | Name/title of the schedule entry | Yes | Required, Max length = 200 |
| `Statement` | Description or clarification of the item | Yes | Required, Max length = 1000 |
| `Previous` | Recorded value from the previous year | Yes | Required, Must be numeric |
| `Current` | Recorded value from the current year | Yes | Required, Must be numeric |
| `Record` | Final recorded outcome or evaluated value | Yes | Required, Must be numeric |
| `Status` | The overall state of the item (e.g., Completed, Under Review) | Yes | Required, Max length = 200 |
| `Day` | Date referring to the entry record | Yes | Required, Must match date format |
| `year` | The year associated with this annual schedule | Optional | Max length = 200 |

---

## Usage Context

This ViewModel is typically used in:

- **Annual Administrative Reports**
- **Organizational Performance Evaluations**
- **Strategic Planning and Review Documents**

It helps structure yearly comparison and analysis by providing:
- Item description
- Comparison values (previous vs. current)
- Final evaluated record
- Status of completion or progress

---

## Example Interpretation (Conceptual)

| البند (Item) | السابق (Previous) | الحالي (Current) | السجل (Record) | الحالة (Status) |
|--------------|------------------|------------------|----------------|-----------------|
| الأنشطة التطوعية | 12 | 14 | 16 | مكتمل |
| الدورات التدريبية | 20 | 22 | 19 | قيد التنفيذ |

This demonstrates how administrators analyze growth, reduction, and progress.

---

## Summary

`AnnualScheduleVM` ensures structured, validated, and clear data input for annual performance scheduling.  
It supports:

- Strong validation rules
- Clean form display
- Easy comparison of yearly progress
- Multilingual-ready labels and messaging

This structure helps maintain consistency when preparing annual administrative reports.

