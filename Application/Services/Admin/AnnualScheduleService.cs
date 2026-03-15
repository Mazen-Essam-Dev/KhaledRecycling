using Application.Interfaces.Admin;
using Infrastructure.Repositories.InterfacesDB;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Application.Services.Admin
{
    public class AnnualScheduleService : IAnnualScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public AnnualScheduleService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task<IEnumerable<AnnualSchedule>> GetAllAsync()
        {
            return await _unitOfWork.AnnualSchedules.GetAllAsync(a => a.AnnualScheduleCategory);
        }

        public async Task<AnnualSchedule?> GetByIdAsync(int id)
        {
            var schedules = await _unitOfWork.AnnualSchedules
                .GetAllAsync(e => e.Id == id, a => a.AnnualScheduleCategory);
            return schedules.FirstOrDefault();
        }

        public async Task<int> AddAsync(AnnualSchedule entity)
        {
            var entit= await _unitOfWork.AnnualSchedules.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(AnnualSchedule entity)
        {
            var existing = await _unitOfWork.AnnualSchedules.GetByIdAsync(entity.Id);
            if (existing == null) return;

            _unitOfWork.AnnualSchedules.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.AnnualSchedules.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.AnnualSchedules.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }


        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads/AnnualSchedules");
            Directory.CreateDirectory(folder);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"uploads/AnnualSchedules/{fileName}";
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

    }

}
