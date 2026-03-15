using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.Entities;
using Domain.DTOs.Admin.CarService;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Admin
{
    public class CarServiceManager : ICarServiceManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string FileName = "CarServices";

        public CarServiceManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<CarServiceEntity>> GetAllAsync()
        {
            return await _unitOfWork.CarServices.GetAllAsync(c => c.Car);
        }

        public async Task<CarServiceEntity?> GetByIdAsync(int id)
        {
            return await _unitOfWork.CarServices.GetByIdAsync(e => e.Id == id);
        }

        public async Task<(int, int)> AddAsync(CarServiceEntity entity, IFormFile? file)
        {
            //if (file != null)
            //{
            //    entity.AttachmentPath = await FileHelper.SaveImageAsync(file, FileName); // Save new
            //}
            var carServ = await _unitOfWork.CarServices.AddAsync(entity);

            var car = await _unitOfWork.Cars.GetByIdAsync(entity.CarId);
            var carModified = car;


            var carServices = await _unitOfWork.CarServices.GetAllAsync();


            _unitOfWork.Cars.UpdateValues(car, carModified);
            await _unitOfWork.CompleteAsync();

            return (carServ.Id, carServ.CarId ?? 0);
        }

        public async Task UpdateAsync(CarServiceEntity entity, IFormFile? file)
        {
            var existing = await _unitOfWork.CarServices.GetByIdAsync(entity.Id);
            if (existing == null) return;

            //if (file != null)
            //{
            //    FileHelper.DeleteImageFile(existing.AttachmentPath); // Delete old
            //    entity.AttachmentPath = await FileHelper.SaveImageAsync(file, FileName); // Save new
            //}
            //else
            //{
            //    entity. AttachmentPath = existing.AttachmentPath;
            //}

            _unitOfWork.CarServices.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.CarServices.GetByIdAsync(id);
            if (entity != null)
            {
                FileHelper.DeleteImageFile(entity.AttachmentPath); // Delete old
                _unitOfWork.CarServices.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task UploadAttachmentsAsync(CarServiceAttachmentsDTO model)
        {
            // Get existing attachments
            var existingAttachments = await _unitOfWork.CarServiceAttachments.GetAllAsync(e => e.CarServiceId == model.CarServiceId);

            // Delete removed attachments
            foreach (var existingAttachment in existingAttachments)
            {
                var stillExists = model.Attachments.Any(a => a.Id == existingAttachment.Id);
                if (!stillExists)
                {
                    FileHelper.DeleteImageFile(existingAttachment.Path);
                    _unitOfWork.CarServiceAttachments.Delete(existingAttachment);
                }
            }

            // Add or update attachments
            foreach (var attachmentDto in model.Attachments)
            {
                if (attachmentDto.File != null)
                {
                    var path = await FileHelper.SaveImageAsync(attachmentDto.File, "CarServiceAttachments");

                    var attachment = new CarServiceAttachment
                    {
                        Name = attachmentDto.Name,
                        Path = path,
                        CarServiceId = model.CarServiceId
                    };

                    await _unitOfWork.CarServiceAttachments.AddAsync(attachment);
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<CarServiceAttachmentsDTO> GetAttachmentsAsync(int carServiceId)
        {
            var attachments = await _unitOfWork.CarServiceAttachments.GetAllAsync(e => e.CarServiceId == carServiceId);
            return new CarServiceAttachmentsDTO
            {
                CarServiceId = carServiceId,
                Attachments = attachments.Select(a => new CarServiceAttachmentDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Path = a.Path
                }).ToList()
            };
        }
    }
}
