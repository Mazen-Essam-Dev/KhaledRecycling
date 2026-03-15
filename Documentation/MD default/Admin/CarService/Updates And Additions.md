# CarServices Updates And Additions Documentation
## 0 Attachments Modal

## Overview
added Attachments Modal as like as in whole system .

### 0 VM,DTO,Entity Classes
```csharp
    // Attachments
    CreateMap<CarServicesAttachmentsVM, CarServicesAttachmentsDTO>().ReverseMap();
    CreateMap<CarServicesAttachmentVM, CarServicesAttachmentDTO>().ReverseMap();
    CreateMap<CarServicesAttachment, CarServicesAttachmentVM>().ReverseMap();
    CreateMap<CarServicesAttachment, CarServicesAttachmentDTO>().ReverseMap();
```

### 0 js in modal page _CarServicesAttachmentsModal,index,_ListPartial
 ** to show new names on files added and show old in old files 
### 0 in services -->
 - Task<CarServicesAttachmentsDTO> GetAttachmentsAsync(int CarServicesId);
 **to get all his attachments from table `CarServicesAttachments`
 - UploadAttachmentsAsync(CarServicesAttachmentsDTO model);
  **to remove and add new updated attachments to table `CarServicesAttachments`

### 0 Added in Controller -->

 - GetCarServicesAttachments(int id)
 - UploadCarServicesFiles(CarServicesAttachmentsVM model)
 
*===================*
*===================*
