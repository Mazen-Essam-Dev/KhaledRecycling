# EngineerProfile.cs

## Description
The `EngineerProfile` class is an AutoMapper configuration profile used to define the mapping rules between the `Engineer` entity in the **Domain layer** and the `EngineerVM` ViewModel in the **Admin area** of the application.  
This profile simplifies object transformations between data models and view models, ensuring consistency and reducing manual mapping code across the application.

---

## Purpose
This file enables automatic conversion between:
- **`EngineerVM` → `Engineer`**: Used when saving or updating engineer information from the Admin interface.
- **`Engineer` → `EngineerVM`**: Used when displaying engineer data in Admin views.

By defining these mappings, developers can easily move data between layers without repetitive, manual property assignments.

---

## Implementation Details
- Inherits from `AutoMapper.Profile`.
- Configures two-way mapping between `Engineer` and `EngineerVM`.
- Ignores the `ProfileImagePath` property when mapping from the ViewModel to the Entity to allow manual handling of image uploads.
