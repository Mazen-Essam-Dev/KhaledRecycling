# Activity.md

# Activity.cs

**Path:** `Domain/Entities/Activity.cs`  
**Description:**  
Represents an **Activity** entity in the system with properties for title, date range, location, description, attachments, and associated questions.

---

## Properties

- `Id` (`int`): Primary key. `[Key]` attribute.  
- `TitleAr` (`string?`): Arabic title. Max length 200. `[MaxLength(200)]` attribute.  
- `TitleEn` (`string?`): English title. Max length 200. `[MaxLength(200)]` attribute.  
- `StartDate` (`DateOnly?`): Start date of the activity.  
- `EndDate` (`DateOnly?`): End date of the activity.  
- `MinimumAge` (`int?`): Minimum age for participants.  
- `Location` (`string?`): Location of the activity. Max length 200. `[MaxLength(200)]` attribute.  
- `Description` (`string?`): Description of the activity.  
- `Achievement` (`string?`): Achievements of the activity.  
- `AttachmentPath` (`string?`): Path to attachment file. Max length 300. `[MaxLength(300)]` attribute.  
- `Questions` (`ICollection<Question>`): Collection of related questions. Initialized as empty list.

---

## Notes

- Uses Data Annotations for validation (`Key`, `MaxLength`).  
- Nullable properties are used for optional fields.  
- `Questions` collection supports a one-to-many relationship with `Question` entity.  
