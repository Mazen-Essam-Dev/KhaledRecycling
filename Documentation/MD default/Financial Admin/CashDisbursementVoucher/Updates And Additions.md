# CashDisbursementVoucher Updates And Additions Documentation
## 0 Attachments Modal

## Overview
added Attachments Modal as like as in whole system .

### 0 VM,DTO,Entity Classes
```csharp
    // Attachments
    CreateMap<CashDisbursementVoucherAttachmentsVM, CashDisbursementVoucherAttachmentsDTO>().ReverseMap();
    CreateMap<CashDisbursementVoucherAttachmentVM, CashDisbursementVoucherAttachmentDTO>().ReverseMap();
    CreateMap<CashDisbursementVoucherAttachment, CashDisbursementVoucherAttachmentVM>().ReverseMap();
    CreateMap<CashDisbursementVoucherAttachment, CashDisbursementVoucherAttachmentDTO>().ReverseMap();
```

### 0 js in modal page _CashDisbursementVoucherAttachmentsModal,index,_ListPartial
 ** to show new names on files added and show old in old files 
### 0 in services -->
 - Task<CashDisbursementVoucherAttachmentsDTO> GetAttachmentsAsync(int CashDisbursementVoucherId);
 **to get all his attachments from table `CashDisbursementVoucherAttachments`
 - UploadAttachmentsAsync(CashDisbursementVoucherAttachmentsDTO model);
  **to remove and add new updated attachments to table `CashDisbursementVoucherAttachments`

### 0 Added in Controller -->

 - GetCashDisbursementVoucherAttachments(int id)
 - UploadCashDisbursementVoucherFiles(CashDisbursementVoucherAttachmentsVM model)
 
*===================*
*===================*
