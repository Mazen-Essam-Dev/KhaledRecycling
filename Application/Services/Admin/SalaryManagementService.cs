using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.DTOs.Admin;
using Domain.DTOs.Admin.SalaryManagement;
using Domain.Entities.SalaryManage;
using Domain.Enums;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Application.Services.Admin
{
    public class SalaryManagementService : ISalaryManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SalaryManagementService(IUnitOfWork unitOfWork, IWebHostEnvironment env ,IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        //// Signature/OTP for Trainer and Manager
        //public async Task<bool> SendOtpAsync()
        //{
        //    var (status, code) = await OTPHelper.SaveOtpAsync(_httpContextAccessor, _unitOfWork);

        //    if (status == false) return false;

        //    var resultStatus = await _SMSForSendingOTPService.SendOtpSMSAsync(code);

        //    return resultStatus.Item1;
        //}

        //public async Task<(bool success, string? message)> ValidateOtp_OpenDetails_payrollReportAsync(int year, int month, string code, string role, System.Security.Claims.ClaimsPrincipal user)
        //{
        //    var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);
        //    if (!success)
        //        return (false, "Invalid OTP");

        //    var userId = user.GetUserId();
        //    var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
        //    var latestSignature = allSignatures
        //        .Where(s => s.UserId == userId)
        //        .OrderByDescending(s => s.CreatedAt)
        //        .FirstOrDefault();
        //    if (latestSignature == null)
        //        return (false, "Signature not found");

        //    // Look for existing sign record by year, month, and ReportTypeId
        //    var report = await _unitOfWork.SalaryReportSigns.GetByColumnAsync(
        //        e => e.Year == year && e.Month == month && e.ReportSalaryTypeId == (int)ReportSalaryTypeEnum.SalaryReport);

        //    if (role == "Acountant")
        //    {
        //        // Accountant creates new record if none exists
        //        if (report == null)
        //        {
        //            report = new SalaryReportSign
        //            {
        //                ReportSalaryTypeId = (int)ReportSalaryTypeEnum.SalaryReport,
        //                Year = year,
        //                Month = month,
        //                AcountantSignatureId = latestSignature.Id
        //            };
        //            await _unitOfWork.SalaryReportSigns.AddAsync(report);
        //        }
        //        else
        //        {
        //            report.AcountantSignatureId = latestSignature.Id;
        //            _unitOfWork.SalaryReportSigns.Update(report);
        //        }
        //    }
        //    else if (role == "manager")
        //    {
        //        // Manager can only sign if record exists
        //        if (report == null)
        //            return (false, "Report must be signed by accountant first");
        //        report.ManagerSignitureId = latestSignature.Id;
        //        _unitOfWork.SalaryReportSigns.Update(report);
        //    }
        //    else
        //        return (false, "Invalid role");

        //    await _unitOfWork.CompleteAsync();
        //    return (true, null);
        //}

        //public async Task<(bool success, string? message)> ValidateOtp_OpenDetails_DiscountsAndBonusesReportAsync(int year, int month, string code, string role, System.Security.Claims.ClaimsPrincipal user)
        //{
        //    var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);
        //    if (!success)
        //        return (false, "Invalid OTP");

        //    var userId = user.GetUserId();
        //    var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
        //    var latestSignature = allSignatures
        //        .Where(s => s.UserId == userId)
        //        .OrderByDescending(s => s.CreatedAt)
        //        .FirstOrDefault();
        //    if (latestSignature == null)
        //        return (false, "Signature not found");

        //    // Look for existing sign record by year, month, and ReportTypeId
        //    var report = await _unitOfWork.SalaryReportSigns.GetByColumnAsync(
        //        e => e.Year == year && e.Month == month && e.ReportSalaryTypeId == (int)ReportSalaryTypeEnum.DiscountsAndBonusesReport);

        //    if (role == "Acountant")
        //    {
        //        // Accountant creates new record if none exists
        //        if (report == null)
        //        {
        //            report = new SalaryReportSign
        //            {
        //                ReportSalaryTypeId = (int)ReportSalaryTypeEnum.DiscountsAndBonusesReport,
        //                Year = year,
        //                Month = month,
        //                AcountantSignatureId = latestSignature.Id
        //            };
        //            await _unitOfWork.SalaryReportSigns.AddAsync(report);
        //        }
        //        else
        //        {
        //            report.AcountantSignatureId = latestSignature.Id;
        //            _unitOfWork.SalaryReportSigns.Update(report);
        //        }
        //    }
        //    else if (role == "manager")
        //    {
        //        // Manager can only sign if record exists
        //        if (report == null)
        //            return (false, "Report must be signed by accountant first");
        //        report.ManagerSignitureId = latestSignature.Id;
        //        _unitOfWork.SalaryReportSigns.Update(report);
        //    }
        //    else
        //        return (false, "Invalid role");

        //    await _unitOfWork.CompleteAsync();
        //    return (true, null);
        //}

        public async Task<IEnumerable<SalaryManagement>> GetAllAsync()
        {
            return await _unitOfWork.SalaryManagements.GetAllAsync();
        }

        public async Task<SalaryManagement?> GetByIdAsync(int id)
        {
            return await _unitOfWork.SalaryManagements
                .GetByIdAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<EmployeesNameDTO>> GetAllEmplyeeNames()
        {
            return await _unitOfWork.Employees.Table.Select(x => new EmployeesNameDTO
            {
                Id = x.Id,
                FullNameAr = x.FullNameAr,
                FullNameEn = x.FullNameEn,
                JobTitle = x.JobTitle,
                //JobId = x.JobId,
                //Job = x.Job,
                BasicSalary = x.Salary,
            }
            ).ToListAsync();
        }
		
		public async Task<int> AddAsync(SalaryManagement entity)
        {
            var entit = await _unitOfWork.SalaryManagements.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(SalaryManagement entity)
        {
            var existing = await _unitOfWork.SalaryManagements.GetByIdAsync(entity.Id);
            if (existing == null) return;

            _unitOfWork.SalaryManagements.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SalaryManagements.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.SalaryManagements.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }


        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads/SalaryManagements");
            Directory.CreateDirectory(folder);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"uploads/SalaryManagements/{fileName}";
        }
        public void DeleteImageFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        public async Task<IEnumerable<AbsenceDTO>> GetAllAbsencesAsync()
        {
            var allSalaries = await _unitOfWork.SalaryManagements.GetAllAsync(s => s.Employee /*, x=> x.Job*/);
            var absences = allSalaries.Select(x => new AbsenceDTO
            {
                Id = x.Id,
                EmpId = x.EmployeeId,
                FullNameAr = x.Employee?.FullNameAr,
                FullNameEn = x.Employee?.FullNameEn,
                //JobTitle = x.Employee?.Job?.NameAr,
                JobTitle = x.Employee?.JobTitle,
                Month = x.Month,
                Year = x.Year,
                NoOfDays = x.AbsentDays
            });

            return absences;
        }
        public async Task<IEnumerable<int>> GetAllYearsInDb()
        {
            var allRecords = await _unitOfWork.SalaryManagements.GetAllAsync();
            var allYears = allRecords
                .Where(e => e.Year != 0)
                .Select(e => e.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            return allYears;
        }

        public async Task UploadAttachmentsAsync(SalaryManagementAttachmentsDTO model)
        {
            // Get existing attachments
            var existingAttachments = await _unitOfWork.SalaryManagementAttachments.GetAllAsync(e => e.SalaryManagementId == model.SalaryManagementId);

            // Delete removed attachments
            foreach (var existingAttachment in existingAttachments)
            {
                var stillExists = model.Attachments.Any(a => a.Id == existingAttachment.Id);
                if (!stillExists)
                {
                    FileHelper.DeleteImageFile(existingAttachment.Path);
                    _unitOfWork.SalaryManagementAttachments.Delete(existingAttachment);
                }
            }

            // Add or update attachments
            foreach (var attachmentDto in model.Attachments)
            {
                if (attachmentDto.File != null)
                {
                    var path = await FileHelper.SaveImageAsync(attachmentDto.File, "SalaryManagementAttachments");

                    var attachment = new SalaryManagementAttachment
                    {
                        Name = attachmentDto.Name,
                        Path = path,
                        SalaryManagementId = model.SalaryManagementId
                    };

                    await _unitOfWork.SalaryManagementAttachments.AddAsync(attachment);
                }
            }

            await _unitOfWork.CompleteAsync();

        }

        public async Task<SalaryManagementAttachmentsDTO> GetAttachmentsAsync(int SalaryManagementId)
        {
            var attachments = await _unitOfWork.SalaryManagementAttachments.GetAllAsync(e => e.SalaryManagementId == SalaryManagementId);
            return new SalaryManagementAttachmentsDTO
            {
                SalaryManagementId = SalaryManagementId,
                Attachments = attachments.Select(a => new SalaryManagementAttachmentDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Path = a.Path
                }).ToList()
            };
        }

    }

}
