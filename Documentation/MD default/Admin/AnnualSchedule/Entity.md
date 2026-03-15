# AnnualSchedule Entity — Explanation (Domain Layer)

The `AnnualSchedule` entity represents an item in the **annual operational schedule/report**.  
It is used to record yearly planned activities, achievements, or performance indicators along with comparisons between previous and current data.

---

## Purpose
This model helps the organization **track yearly progress** by comparing:
- What was recorded **last year**
- What is happening **this year**
- The **final recorded value** or outcome

It is usually displayed in:
- Administrative annual reports
- Board summaries
- Strategic planning documents

---

## Field Breakdown

| Field Name | Type | Meaning | Notes |
|-----------|------|---------|------|
| `Id` | int | Unique identifier | Primary key. |
| `Item` | string (max 200) | The title or name of the schedule entry | Example: "Training Programs", "Community Events". |
| `Statement` | string (max 1000) | Description or explanation of the item | Used to clarify purpose or context. |
| `Previous` | decimal? | Value from the **previous year** | Used to compare changes over time. |
| `Current` | decimal? | Value from the **current year** | Represents present performance. |
| `Record` | decimal? | Actual recorded / achieved value | Could be the result after execution or evaluation. |
| `Status` | string (max 200) | General state or evaluation of the item | Example: "Completed", "In Progress", "Pending". |
| `Day` | DateTime? | The date associated with the record | Used when the schedule entry is tied to a specific event date. |
| `year` | string (max 200) | The year the schedule applies to | Example: "2024", or sometimes a range such as "2024/2025". |

---

## How It Is Used
- To **compare** performance across years
- To **report** progress in annual meetings
- To **document** what has been achieved and what remains pending
- To **support** visualization in charts or tables

---

## Common Interpretation Example

| Item | Previous | Current | Record | Status |
|------|---------|---------|--------|--------|
| Training Courses | 25 | 30 | 32 | Completed |
| Community Volunteer Events | 12 | 14 | 10 | In Progress |

This gives clear insight into growth or decline in organizational efforts.

---

## Summary
`AnnualSchedule` is a **reporting-focused entity** that supports administrative tracking and analysis of yearly performance, allowing clear comparison between past and present results and supporting strategic decision-making.

