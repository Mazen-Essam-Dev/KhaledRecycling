# PurchaseOrder Updates And Additions Documentation
## 0 Attachments Modal

## Overview
added Attachments Modal as like as in whole system .

### 0 VM,DTO,Entity Classes
```csharp
    // Attachments
    CreateMap<PurchaseOrderAttachmentsVM, PurchaseOrderAttachmentsDTO>().ReverseMap();
    CreateMap<PurchaseOrderAttachmentVM, PurchaseOrderAttachmentDTO>().ReverseMap();
    CreateMap<PurchaseOrderAttachment, PurchaseOrderAttachmentVM>().ReverseMap();
    CreateMap<PurchaseOrderAttachment, PurchaseOrderAttachmentDTO>().ReverseMap();
```

### 0 js in modal page _PurchaseOrderAttachmentsModal,index,_ListPartial
 ** to show new names on files added and show old in old files 
### 0 in services -->
 - Task<PurchaseOrderAttachmentsDTO> GetAttachmentsAsync(int PurchaseOrderId);
 **to get all his attachments from table `PurchaseOrderAttachments`
 - UploadAttachmentsAsync(PurchaseOrderAttachmentsDTO model);
  **to remove and add new updated attachments to table `PurchaseOrderAttachments`

### 0 Added in Controller -->

 - GetPurchaseOrderAttachments(int id)
 - UploadPurchaseOrderFiles(PurchaseOrderAttachmentsVM model)
 
*===================*
*===================*
