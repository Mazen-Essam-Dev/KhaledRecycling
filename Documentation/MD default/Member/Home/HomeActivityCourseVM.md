# HomeActivityCourseVM — Full Documentation (Explanation Only)

This ViewModel is used to **combine data** from multiple modules — specifically **Courses** and **Activities** — along with basic member information.  
It serves as a **container model** for dashboards or pages that need to display both **enrolled courses** and **participated activities** for a specific member.

---

## Purpose

The main goal of `HomeActivityCourseVM` is to provide a **unified ViewModel** that merges:

- The **member’s name**
- The list of **courses** (`CourseVM`)
- The list of **activities** (`ActivityVM`)

It is typically used on **home pages or dashboards** in the **Member area** where both modules need to appear together.

---

## Properties

| Property | Type | Description |
|-----------|------|-------------|
| `MemberName` | `string?` | The full name of the logged-in member displayed on the dashboard. |
| `Courses` | `IEnumerable<CourseVM>?` | A list of courses that the member has subscribed to or can access. |
| `Activities` | `IEnumerable<ActivityVM>?` | A list of activities (such as events or workshops) related to the member. |

---

## Usage Example

### Scenario
When a member logs into their account, the system may need to show:
- Their **profile greeting** (via `MemberName`)
- Their **current or upcoming courses**
- Their **activities or events participation**

All of this information can be gathered and passed to the view through this **single composite ViewModel**.

### Example View Usage
```csharp
public async Task<IActionResult> Index()
{
    var username = User.Identity.Name;
    var member = await _memberService.GetByEmailAsync(username);

    var courses = await _courseService.GetAllAsync(username);
    var activities = await _activityService.GetAllAsync(username);

    var model = new HomeActivityCourseVM
    {
        MemberName = member?.FullName,
        Courses = courses.Courses.Select(c => new CourseVM { /* map data */ }),
        Activities = activities.Activities.Select(a => new ActivityVM { /* map data */ })
    };

    return View(model);
}
