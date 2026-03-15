# SalaryManagement Updates And Additions Documentation
## 0 Attachments Modal

## Overview
added Attachments Modal as like as in whole system .

### 0 VM,DTO,Entity Classes
```csharp
    // Attachments
    CreateMap<SalaryManagementAttachmentsVM, SalaryManagementAttachmentsDTO>().ReverseMap();
    CreateMap<SalaryManagementAttachmentVM, SalaryManagementAttachmentDTO>().ReverseMap();
    CreateMap<SalaryManagementAttachment, SalaryManagementAttachmentVM>().ReverseMap();
    CreateMap<SalaryManagementAttachment, SalaryManagementAttachmentDTO>().ReverseMap();
```

### 0 js in modal page _SalaryManagementAttachmentsModal,index,_ListPartial
 ** to show new names on files added and show old in old files 
### 0 in services -->
 - Task<SalaryManagementAttachmentsDTO> GetAttachmentsAsync(int SalaryManagementId);
 **to get all his attachments from table `SalaryManagementAttachments`
 - UploadAttachmentsAsync(SalaryManagementAttachmentsDTO model);
  **to remove and add new updated attachments to table `SalaryManagementAttachments`

### 0 Added in Controller -->

 - GetSalaryManagementAttachments(int id)
 - UploadSalaryManagementFiles(SalaryManagementAttachmentsVM model)
 
*===================*
*===================*
