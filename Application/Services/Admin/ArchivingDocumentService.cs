using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.Entities;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Admin
{
    public class ArchivingDocumentService : IArchivingDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly string FileName = "ArchivingDocuments";

        public ArchivingDocumentService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task<IEnumerable<ArchivingDocument>> GetAllAsync()
        {
            return await _unitOfWork.ArchivingDocuments.GetAllAsync(x => x.DocumentCategory);
        }

        public async Task<ArchivingDocument?> GetByIdAsync(int id)
        {
            return await _unitOfWork.ArchivingDocuments.GetByIdAsync(e => e.Id == id, x => x.DocumentCategory);
        }

        public async Task<int> AddAsync(ArchivingDocument entity)
        {
            //entity.PdfFilePath = entity.PdfFile == null ? "" : await FileHelper.SaveImageAsync(entity.PdfFile, FileName);

            var entit = await _unitOfWork.ArchivingDocuments.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(ArchivingDocument entity)
        {
            var existing = await _unitOfWork.ArchivingDocuments.GetByIdAsync(entity.Id);
            if (existing == null) return;

            //entity.PdfFilePath = existing.PdfFilePath;
            //if (entity.PdfFile != null && entity.PdfFile.Length >0)
            //{
            //    FileHelper.DeleteImageFile(existing.PdfFilePath);
            //    entity.PdfFilePath = await FileHelper.SaveImageAsync(entity.PdfFile, FileName);
            //}
            _unitOfWork.ArchivingDocuments.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ArchivingDocuments.GetByIdAsync(id);
            if (entity != null)
            {
                FileHelper.DeleteImageFile(entity.PdfFilePath);
                _unitOfWork.ArchivingDocuments.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

    }
}
