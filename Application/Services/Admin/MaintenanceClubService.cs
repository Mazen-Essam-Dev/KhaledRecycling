using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.DTOs;
using Domain.Entities;
using Domain.Entities.PurchaseOrder;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class MaintenanceClubService : IMaintenanceClubService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly string FileName = "MaintenanceClubs";

        public MaintenanceClubService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task<IEnumerable<MaintenanceClub>> GetAllAsync()
        {
            return await _unitOfWork.MaintenanceClubs.GetAllAsync();
        }

        public async Task<MaintenanceClub?> GetByIdAsync(int id)
        {
            return await _unitOfWork.MaintenanceClubs.GetByIdAsync(e => e.Id == id);
        }

        public async Task<int> AddAsync(MaintenanceClub entity)
        {
            //entity.PdfFilePath = entity.PdfFile == null ? "" : await FileHelper.SaveImageAsync(entity.PdfFile, FileName);

            var entit =  await _unitOfWork.MaintenanceClubs.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(MaintenanceClub entity)
        {
            var existing = await _unitOfWork.MaintenanceClubs.GetByIdAsync(entity.Id);
            if (existing == null) return;

            //if (entity.PdfFile != null)
            //{
            //    FileHelper.DeleteImageFile(existing.PdfFilePath);
            //    entity.PdfFilePath = await FileHelper.SaveImageAsync(entity.PdfFile, FileName);
            //}
            _unitOfWork.MaintenanceClubs.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.MaintenanceClubs.GetByIdAsync(id);
            if (entity != null)
            {
                FileHelper.DeleteImageFile(entity.PdfFilePath);
                _unitOfWork.MaintenanceClubs.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<MaintenanceClubAttachmentsDTO> GetAttachmentsAsync(int maintenanceClubId)
        {
            var attachments = await _unitOfWork.Context.MaintenanceClubAttachments
                .Where(a => a.MaintenanceClubId == maintenanceClubId)
                .Select(a => new MaintenanceClubAttachmentDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Path = a.Path,
                    MaintenanceClubId = a.MaintenanceClubId
                })
                .ToListAsync();

            return new MaintenanceClubAttachmentsDTO
            {
                MaintenanceClubId = maintenanceClubId,
                Attachments = attachments
            };
        }

        public async Task UploadAttachmentsAsync(MaintenanceClubAttachmentsDTO dto)
        {
            // Get existing attachments
            var existingAttachments = await _unitOfWork.MaintenanceClubAttachments.GetAllAsync(e => e.MaintenanceClubId == dto.MaintenanceClubId);

            // Delete removed attachments
            foreach (var existingAttachment in existingAttachments)
            {
                var stillExists = dto.Attachments.Any(a => a.Id == existingAttachment.Id);
                if (!stillExists)
                {
                    FileHelper.DeleteImageFile(existingAttachment.Path);
                    _unitOfWork.MaintenanceClubAttachments.Delete(existingAttachment);
                }
            }

            // Add or update attachments
            foreach (var attachmentDto in dto.Attachments)
            {
                if (attachmentDto.File != null)
                {
                    var filePath = await FileHelper.SaveImageAsync(attachmentDto.File, "MaintenanceClubAttachments");

                    var attachment = new MaintenanceClubAttachment
                    {
                        Name = attachmentDto.Name,
                        Path = filePath,
                        MaintenanceClubId = dto.MaintenanceClubId
                    };

                    await _unitOfWork.MaintenanceClubAttachments.AddAsync(attachment);
                }
            }

            await _unitOfWork.CompleteAsync();
        }

    }
}
