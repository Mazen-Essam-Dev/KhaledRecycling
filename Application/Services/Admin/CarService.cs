using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.Entities;
using Domain.DTOs.Admin.Car;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Admin
{
    public class CarService : ICarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string FileName = "Cars";

        public CarService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _unitOfWork.Cars.GetAllAsync();
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Cars
                .GetByIdAsync(e => e.Id == id);
        }

        public async Task<int> AddAsync(Car entity, IFormFile? file)
        {
            if (file != null)
            {
                entity.AttachmentPath = await FileHelper.SaveImageAsync(file, FileName); // Save new
            }

            var entit = await _unitOfWork.Cars.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(Car entity, IFormFile? file)
        {
            var existing = await _unitOfWork.Cars.GetByIdAsync(entity.Id);
            if (existing == null) return;

            //if (file != null)
            //{
            //    FileHelper.DeleteImageFile(existing.AttachmentPath); // Delete old
            //    entity.AttachmentPath = await FileHelper.SaveImageAsync(file, FileName); // Save new
            //}
            //else
            //{
            //    // Keep the old image if none uploaded
            //    entity.AttachmentPath = existing.AttachmentPath;
            //}

            _unitOfWork.Cars.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Cars.GetByIdAsync(id);
            if (entity != null)
            {
                // Delete image from disk
                FileHelper.DeleteImageFile(entity.AttachmentPath); // Delete old
                
                _unitOfWork.Cars.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task UploadAttachmentsAsync(CarAttachmentsDTO model)
        {
            // Get existing attachments
            var existingAttachments = await _unitOfWork.CarAttachments.GetAllAsync(e => e.CarId == model.CarId);

            // Delete removed attachments
            foreach (var existingAttachment in existingAttachments)
            {
                var stillExists = model.Attachments.Any(a => a.Id == existingAttachment.Id);
                if (!stillExists)
                {
                    FileHelper.DeleteImageFile(existingAttachment.Path);
                    _unitOfWork.CarAttachments.Delete(existingAttachment);
                }
            }

            // Add or update attachments
            foreach (var attachmentDto in model.Attachments)
            {
                if (attachmentDto.File != null)
                {
                    var path = await FileHelper.SaveImageAsync(attachmentDto.File, "CarAttachments");

                    var attachment = new CarAttachment
                    {
                        Name = attachmentDto.Name,
                        Path = path,
                        CarId = model.CarId
                    };

                    await _unitOfWork.CarAttachments.AddAsync(attachment);
                }
            }

            await _unitOfWork.CompleteAsync();

        }

        public async Task<CarAttachmentsDTO> GetAttachmentsAsync(int carId)
        {
            var attachments = await _unitOfWork.CarAttachments.GetAllAsync(e => e.CarId == carId);
            return new CarAttachmentsDTO
            {
                CarId = carId,
                Attachments = attachments.Select(a => new CarAttachmentDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Path = a.Path
                }).ToList()
            };
        }


    }

}
