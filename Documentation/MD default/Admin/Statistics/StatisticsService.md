# StatisticsService Documentation

## Overview
Service layer implementation for comprehensive system statistics and analytics with multi-dimensional data analysis across members, courses, activities, and departments.

## Dependencies
- **IMemberService**: Member data access and processing
- **IUnitOfWork**: Database repository access
- **IHttpContextAccessor**: HTTP context access for language preferences

## Core Statistics Operations

### GetStatisticsAsync()
- **Purpose**: Generates comprehensive system statistics across multiple dimensions
- **Data Sources**:
  - Members with subscriptions
  - Courses and department associations
  - Activity participation
  - Demographic information
- **Returns**: `StatisticsDTO` with structured statistical data

## Statistical Analysis Dimensions

### Subscription Analytics
- **Activity Subscriptions**: Distinct member participation in activities
- **Course Subscriptions**: Distinct member enrollment in courses
- **Department Analysis**: Course distribution across departments
- **Participation Rates**: Subscription percentages per department

### Demographic Analysis
- **Age Group Distribution**: Member age segmentation (9-12, 13-15, 16-19, 20+)
- **Gender Distribution**: Male/Female participation ratios
- **Nationality Analysis**: Citizens vs Residents participation

### Department Performance
- **Course Distribution**: Number of courses per department
- **Participation Rates**: Subscription percentages per department
- **Bilingual Labels**: Arabic/English department names based on user preference

## Data Processing Logic

### Division Safety Handling
```csharp
// Prevent division by zero
if (allMembersCount == 0) { allMembersCount = 1; TotalInCourses = 1; TotalInActivities = 1; valid1 = false; }
if (TotalInCourses == 0) { TotalInCourses = 1; valid_Courses = false; }
if (TotalInActivities == 0) { TotalInActivities = 1; valid_Activities = false; }