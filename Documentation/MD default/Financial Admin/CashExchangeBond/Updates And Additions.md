# CashExchangeBond Updates And Additions Documentation
## 0 Attachments Modal

## Overview
added Attachments Modal as like as in whole system .

### 0 VM,DTO,Entity Classes
```csharp
    // Attachments
    CreateMap<CashExchangeBondAttachmentsVM, CashExchangeBondAttachmentsDTO>().ReverseMap();
    CreateMap<CashExchangeBondAttachmentVM, CashExchangeBondAttachmentDTO>().ReverseMap();
    CreateMap<CashExchangeBondAttachment, CashExchangeBondAttachmentVM>().ReverseMap();
    CreateMap<CashExchangeBondAttachment, CashExchangeBondAttachmentDTO>().ReverseMap();
```

### 0 js in modal page _CashExchangeBondAttachmentsModal,index,_ListPartial
 ** to show new names on files added and show old in old files 
### 0 in services -->
 - Task<CashExchangeBondAttachmentsDTO> GetAttachmentsAsync(int CashExchangeBondId);
 **to get all his attachments from table `CashExchangeBondAttachments`
 - UploadAttachmentsAsync(CashExchangeBondAttachmentsDTO model);
  **to remove and add new updated attachments to table `CashExchangeBondAttachments`

### 0 Added in Controller -->

 - GetCashExchangeBondAttachments(int id)
 - UploadCashExchangeBondFiles(CashExchangeBondAttachmentsVM model)
 
*===================*
*===================*
