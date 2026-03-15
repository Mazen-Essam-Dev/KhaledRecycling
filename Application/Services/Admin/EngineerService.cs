using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.Entities;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class EngineerService : IEngineerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public EngineerService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task<IEnumerable<Engineer>> GetAllAsync()
        {
            return await _unitOfWork.Engineers.GetAllAsync(e => e.Nationality!);
        }
        public async Task<List<int>> GetAllGraduationYears()
        {
            var years = new List<int>();

            int currentYear = AppDubaiTime.Now.Year;

            for (int year = currentYear; year >= currentYear - 100; year--)
            {
                years.Add(year);
            }
            return years;
        }
        public async Task<List<int?>?> GetAllGraduationYears_index()
        {
            var allYears = await _unitOfWork.Engineers.Table.Select(x => x.GraduationYear).Distinct().Where(x => x != null).OrderByDescending(x => x).ToListAsync();
            return allYears;
        }
        public async Task<Engineer?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Engineers
                .GetByIdAsync(e => e.Id == id, e => e.Nationality!);
        }

        public async Task<int> AddAsync(Engineer entity, IFormFile? file)
        {
            if (file != null)
            {
                entity.ProfileImagePath = await SaveImageAsync(file);
            }

            var codeExists = _unitOfWork.Engineers.Table.Any(x => x.Code == entity.Code);
            if (codeExists)
            {
                entity.Code = await GenerateNewCode();
            }

            var entit = await _unitOfWork.Engineers.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(Engineer entity, IFormFile? file)
        {
            var existing = await _unitOfWork.Engineers.GetByIdAsync(entity.Id);
            if (existing == null) return;

            //if (file != null)
            //{
            //    // Delete the old image
            //    DeleteImageFile(existing.ProfileImagePath);

            //    // Save new image
            //    entity.ProfileImagePath = await SaveImageAsync(file);
            //}
            //else
            //{
            //    // Keep the old image if none uploaded
            //    entity.ProfileImagePath = existing.ProfileImagePath;
            //}

            _unitOfWork.Engineers.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Engineers.GetByIdAsync(id);
            if (entity != null)
            {
                // Delete image from disk
                DeleteImageFile(entity.ProfileImagePath);

                _unitOfWork.Engineers.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }


        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads/engineers");
            Directory.CreateDirectory(folder);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"uploads/engineers/{fileName}";
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

        public async Task<int> GenerateNewCode()
        {
            var engineers = await _unitOfWork.Engineers.GetAllAsync();

            if (engineers.Any())
            {
                var maxCode = engineers
                    .Where(e => e.Code.HasValue)
                    .Max(e => e.Code.Value);

                return maxCode + 1;
            }

            return 1000;
        }


    }

}
