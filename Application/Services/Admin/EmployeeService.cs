using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.DTOs.Admin.Employees;
using Domain.Entities.Employees;
using Infrastructure.Repositories.InterfacesDB;

namespace Application.Services.Admin
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string FileName = "EmployeesAttachments";


        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> HasRelatedObjectsInDb(int empId)
        {
            var condition2 = await _unitOfWork.SalaryManagements.GetByColumnAsync(e => e.EmployeeId == empId) != null;
            var condition3 = await _unitOfWork.EmployeeAttachments.GetByColumnAsync(e => e.EmployeeId == empId) != null;
            return condition2 || condition3;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _unitOfWork.Employees.GetAllAsync(t => t.Nationality);
        }
        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Employees
                .GetByIdAsync(e => e.Id == id);
        }
        public async Task<int> AddAsync(Employee entity)
        {
            #region add image
            //entity.PhotoPath = entity.Photo == null ? null : await FileHelper.SaveImageAsync(entity.Photo, FileName);
            #endregion

            var entit = await _unitOfWork.Employees.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }
        public async Task UpdateAsync(Employee entity)
        {
            var existing = await _unitOfWork.Employees.GetByIdAsync(entity.Id);
            if (existing == null) return;

            #region update image
            //if (entity.Photo != null)
            //{
            //    FileHelper.DeleteImageFile(existing.PhotoPath); // Delete old
            //    entity.PhotoPath = await FileHelper.SaveImageAsync(entity.Photo, FileName); // Save new
            //}
            //else
            //{
            //    entity.PhotoPath = existing.PhotoPath; // Keep old
            //}
            #endregion

            _unitOfWork.Employees.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();

        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Employees.GetByIdAsync(id);
            if (entity != null)
            {
                #region delete image
                // Delete image from disk
                FileHelper.DeleteImageFile(entity.PhotoPath);
                #endregion

                _unitOfWork.Employees.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }
        public async Task UploadAttachmentsAsync(EmployeeAttachmentsDTO model)
        {
            // remove old files
            var oldFiles = await _unitOfWork.EmployeeAttachments.GetAllAsync(e => e.EmployeeId == model.EmployeeId);
            foreach (var oldFile in oldFiles)
            {
                if (FileHelper.IsFileExist(oldFile.Path) && model.Attachments != null && !model.Attachments.Any(a => a.Path == oldFile.Path))
                {
                    FileHelper.DeleteImageFile(oldFile.Path);
                    _unitOfWork.EmployeeAttachments.Delete(oldFile);
                }
            }

            //save new files
            var existing = new EmployeeAttachment();
            foreach (var attachment in model.Attachments ?? [])
            {
                if (!oldFiles.Any() || !oldFiles.Any(a => a.Path == attachment.Path))
                {
                    if (attachment.File != null)
                    {
                        existing.Path = await FileHelper.SaveImageAsync(attachment.File, FileName);
                    }
                    existing.Name = attachment.Name;
                    existing.EmployeeId = model.EmployeeId;
                    await _unitOfWork.EmployeeAttachments.AddAsync(existing);
                    existing = new EmployeeAttachment();
                }
            }

            await _unitOfWork.CompleteAsync();

        }
        public async Task<IEnumerable<EmployeeAttachment>> GetAttachmentsAsync(int empId)
        {
            var attachments = await _unitOfWork.EmployeeAttachments.GetAllAsync(e => e.EmployeeId == empId);
            return attachments;
        }
    }
}
