# Car Updates And Additions Documentation
## 0 Attachments Modal

## Overview
added Attachments Modal as like as in whole system .

### 0 VM,DTO,Entity Classes
```csharp
    // Attachments
    CreateMap<CarAttachmentsVM, CarAttachmentsDTO>().ReverseMap();
    CreateMap<CarAttachmentVM, CarAttachmentDTO>().ReverseMap();
    CreateMap<CarAttachment, CarAttachmentVM>().ReverseMap();
    CreateMap<CarAttachment, CarAttachmentDTO>().ReverseMap();
```

### 0 js in modal page _CarAttachmentsModal,index,_ListPartial
 ** to show new names on files added and show old in old files 
### 0 in services -->
 - Task<CarAttachmentsDTO> GetAttachmentsAsync(int CarId);
 **to get all his attachments from table `CarAttachments`
 - UploadAttachmentsAsync(CarAttachmentsDTO model);
  **to remove and add new updated attachments to table `CarAttachments`

### 0 Added in Controller -->

 - GetCarAttachments(int id)
 - UploadCarFiles(CarAttachmentsVM model)
 
*===================*
*===================*
