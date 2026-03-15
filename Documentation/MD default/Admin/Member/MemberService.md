# MemberService Documentation

## Overview
C# service class implementing comprehensive member management with image handling, ID validation, automatic code generation, and membership suspension capabilities in a club management system.

## Service Configuration
- **Namespace**: Application.Services.Admin
- **Interface**: IMemberService
- **Dependencies**:
  - IUnitOfWork: Database operations and repository management
  - ICompareService: Similarity comparison for ID validation

## Core Member Operations

### Data Retrieval Methods
- **GetAllAsync()**: Retrieves all members with nationality information
- **GetByIdAsync(int id)**: Gets specific member by ID including nationality data
- **GetAllSpesificAsync()**: Lightweight query for specific member fields (ID, Nationality, Gender, Age)
- **MemberHasCourses(int memberId)**: Checks if member has any course subscriptions

### Member Management
- **AddAsync(MemberEntity entity)**: Creates new member with automatic image processing and code generation
- **UpdateAsync(MemberEntity entity)**: Updates existing member with conditional image management
- **DeleteAsync(int id)**: Removes member and associated image files
- **SuspendAsync(int id, bool suspend)**: Toggles member suspension status

## Image Management System

### Three-Tier Image Handling
- **Profile Image**: Member profile picture
- **ID Image**: Government ID document
- **Passport Image**: Passport document (optional)

### Add Operation
```csharp
entity.ProfileImagePath = entity.ProfileImage == null ? "" : await FileHelper.SaveImageAsync(entity.ProfileImage, FileName);