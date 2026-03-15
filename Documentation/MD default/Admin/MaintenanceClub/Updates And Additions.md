# MaintenanceClub Updates And Additions Documentation
## 0 Attachments Modal

## Overview
added Attachments Modal as like as in whole system .

### 0 VM,DTO,Entity Classes
```csharp
    // Attachments
    CreateMap<MaintenanceClubAttachmentsVM, MaintenanceClubAttachmentsDTO>().ReverseMap();
    CreateMap<MaintenanceClubAttachmentVM, MaintenanceClubAttachmentDTO>().ReverseMap();
    CreateMap<MaintenanceClubAttachment, MaintenanceClubAttachmentVM>().ReverseMap();
    CreateMap<MaintenanceClubAttachment, MaintenanceClubAttachmentDTO>().ReverseMap();
```

### 0 js in modal page _MaintenanceClubAttachmentsModal,index,_ListPartial
 ** to show new names on files added and show old in old files 
### 0 in services -->
 - Task<MaintenanceClubAttachmentsDTO> GetAttachmentsAsync(int MaintenanceClubId);
 **to get all his attachments from table `MaintenanceClubAttachments`
 - UploadAttachmentsAsync(MaintenanceClubAttachmentsDTO model);
  **to remove and add new updated attachments to table `MaintenanceClubAttachments`

### 0 Added in Controller -->

 - GetMaintenanceClubAttachments(int id)
 - UploadMaintenanceClubFiles(MaintenanceClubAttachmentsVM model)
 
*===================*
*===================*
