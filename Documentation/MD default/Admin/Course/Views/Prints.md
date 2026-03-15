# Print Views Module Documentation

This document explains the **Print views** in the FougeraClub ASP.NET Core MVC application. It covers `PrintCertificate`, `PrintIndex`, `PrintMembersCourse`, and `PrintSubscribedMembersInCourses`, including ViewModel usage, layout, localization, permissions, and printing behavior.

---

## 1. PrintCertificate

**ViewModel:** `CertificateVM`  
**Purpose:** Generate a certificate of attendance and participation for a course.

**Key Features:**
- Localizes member name, course title, and gender-based phrasing.
- Displays start and end dates in `dd-MMMM-yyyy` format.
- Shows top logos, certificate title, participant name, info text, congratulatory line.
- Bottom row includes date, seal, signature, and social icons.
- Supports print-friendly styling (`@media print`).
- Layout is null (`Layout = null`) for standalone printing.
- Automatically hides loading overlay and triggers print on page load.

**Notes:**
- Uses `fancy-arabic` font for Arabic script.
- Auto-print script:
```javascript
window.addEventListener('load', () => {
    setTimeout(() => document.getElementById('loadingOverlay').style.display = 'none', 500);
    setTimeout(() => window.print(), 800);
});

# Print Courses & Members Module Documentation

This document explains the **Print views** in the FougeraClub ASP.NET Core MVC application. It covers `PrintIndex`, `PrintMembersCourse`, and `PrintSubscribedMembersInCourses`, including ViewModel usage, layout, localization, permissions, and printing behavior.

---

## 1. PrintIndex

**ViewModel:** `IndexCoursesVM`  
**Purpose:** Print the list of courses with optional trainer and department columns.

**Key Features:**
- Conditional columns based on `UserIsTrainer`.
- Localizes course title, trainer name, and department.
- Displays current date/time and printed by user email.
- Handles empty datasets gracefully.
- Layout is null for print view.
- Includes official header with UAE and Fujairah Science Club branding.

**Notes:**
- Uses Bootstrap RTL/LTR dynamically based on session language.
- Auto-print and close script:
```javascript
window.addEventListener('load', () => {
    setTimeout(() => document.getElementById('loadingOverlay').style.display = 'none', 500);
    setTimeout(() => window.print(), 800);
});

window.onafterprint = function() {
    setTimeout(function() { window.close(); }, 500);
};
