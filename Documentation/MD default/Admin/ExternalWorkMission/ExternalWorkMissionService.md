# ExternalWorkMissionService Documentation

## Overview
Service layer implementation for managing external work missions with employee assignment, mission type tracking, and OTP-based approval workflow.

## Dependencies
- **IUnitOfWork**: Database repository access and transaction management
- **IHttpContextAccessor**: HTTP context access for OTP operations and user identification
- **FileName Constant**: "ExternalWorkMissions" for organizational purposes

## Core Mission Operations

### GetAllAsync()
- **Purpose**: Retrieves all external work mission records
- **Includes**: Missions navigation property for mission type data
- **Returns**: `IEnumerable<ExternalWorkMission>` with mission types

### GetByIdAsync(int id)
- **Purpose**: Fetches specific mission with complete details
- **Includes**: Missions, Employee, Signature navigation properties
- **Returns**: Complete mission entity with all relationships

### AddAsync(ExternalWorkMission entity)
- **Purpose**: Creates new external work mission
- **Process**: Entity addition with transaction commit
- **Returns**: Integer ID of created mission

### UpdateAsync(ExternalWorkMission entity)
- **Purpose**: Modifies existing mission header information
- **Process**: Entity fetch → Value update → Transaction commit
- **Safety**: Null-check prevents errors on missing entities

### DeleteAsync(int id)
- **Purpose**: Removes mission and all associated mission types
- **Cascade**: Explicit removal of related Mission records
- **Process**: Entity and missions fetch → Range deletion → Transaction commit

## Employee Data Management

### GetAllEmplyeeNames()
- **Purpose**: Retrieves employee data for mission assignments
- **Returns**: `IEnumerable<EmployeesNameDTO>` with ID, names, and job titles
- **Data Structure**:
  - FullNameAr, FullNameEn for bilingual support
  - JobTitle for employee role information
  - Use Case: Dropdown population in UI

## Advanced Mission Type Management

### UpdateNewMissionsType(List<int>? MissionTypes, int? modelId)
- **Purpose**: Manages mission type assignments with complete replacement strategy
- **Process**:
  1. Removes all existing mission types for the mission
  2. Adds new mission types from provided list
  3. Commits transaction for each new mission type
- **Logic**: Complete refresh of mission type associations

```csharp
// Complete replacement of mission types
_unitOfWork.Missions.RemoveRange(allpreviousMissions);
foreach (var missType in MissionTypes)
{
    var missionType = new Mission
    {
        ExternalWorkMissionId = modelId,
        MessionKey = missType
    };
    await _unitOfWork.Missions.AddAsync(missionType);
    await _unitOfWork.CompleteAsync();
}