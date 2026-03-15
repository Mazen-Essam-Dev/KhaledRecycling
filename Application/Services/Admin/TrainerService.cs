using Application.Interfaces.Admin;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.DTOs.Admin;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Identity;
using Infrastructure.Repositories;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.Services.Admin
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRolesService _rolePermissionService;
        private readonly UserManager<ApplicationUser> _userManager;


        public TrainerService(IUnitOfWork unitOfWork, IWebHostEnvironment env, IRolesService rolePermissionService, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _roleManager = roleManager;
            _rolePermissionService = rolePermissionService;
            _userManager = userManager;
        }
        public async Task<IEnumerable<Trainer>> GetAllAsync()
        {
            var trainers = await _unitOfWork.Trainers
                .GetAllAsync();

            return trainers;
        }

        public async Task<int?> GetThisTrainerId_IfTrainer_else_0(string? UserEMail)
        {
            int ThisTrainerId = 0;
            if (UserEMail == null) return 0;
            var ThisUser = await _unitOfWork.Users.GetByIdAsync(x => x.UserName == UserEMail || x.Email == UserEMail);
            if (ThisUser != null)
            {
                var ThisTrainer = await _unitOfWork.Trainers.GetByIdAsync(x => x.UserId == ThisUser.Id);
                if (ThisTrainer != null) ThisTrainerId = ThisTrainer.Id;
            }
            return ThisTrainerId;
        }

        public async Task<List<TrainersNameDTO>?> GetAllTrainerNamesAr_En_only()
        {
            var trainersWithUsers = await _unitOfWork.Trainers.Table.Join(_unitOfWork.Users.Table,
                  trainer => trainer.UserId,
                  user => user.Id,
                  (trainer, user) => new TrainersNameDTO
                  {
                      Id = trainer.Id,
                      UserId = trainer.UserId,
                      FullNameAr = user.FullNameAr,
                      FullNameEn = user.FullNameEn,
                      Email = user.Email,
                      DepartmentId = trainer.DepartmentId
                  })
                  .ToListAsync();

            return trainersWithUsers;
        }
        public async Task<List<TrainersNameDTO>?> GetAllUsersNamesAr_En_only()
        {
            var Users = await _unitOfWork.Users.Table.Select(
                  user => new TrainersNameDTO
                  {
                      UserId = user.Id,
                      FullNameAr = user.FullNameAr,
                      FullNameEn = user.FullNameEn,
                      Email = user.Email,
                      PhoneNumber = user.PhoneNumber
                  })
                  .ToListAsync();

            return Users;
        }

        public async Task<List<TrainersNameDTO>?> GetAllUsersNotTrainers_NamesAr_En_only()
        {
            var allRoles = await _roleManager.Roles.ToListAsync();

            var rolesNot = allRoles.Where(r =>
                r.RoleNumber != (int)RoleNumber.Accountant &&
                r.RoleNumber != (int)RoleNumber.Manager &&
                r.RoleNumber != (int)RoleNumber.ActivitiesSupervisor &&
                r.Name != Role.Master.ToString() &&
                r.Name != Role.SuperAdmin.ToString()
            ).Select(r => r.Name) // role names only
             .ToList();

            // Get all users
            var allUsers = await _userManager.Users.ToListAsync();

            var userIdsInRolesNotWanted2 = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                // if user does NOT have ANY of the rolesNot
                if (userRoles != null && !userRoles.Any(r => rolesNot.Contains(r)))
                {
                    userIdsInRolesNotWanted2.Add(user);
                }
            }

            // → Users IDs
            var userIdsInRolesNotWanted = userIdsInRolesNotWanted2.Select(u => u.Id).ToList();

            // Bring all the UserIds of the Trainers
            var trainerUserIds = await _unitOfWork.Trainers.Table
                .Select(t => t.UserId)
                .ToListAsync();

            // Get all users that are not in the trainerUserIds
            var UsersNotTrainers = await _unitOfWork.Users.Table
                .Where(u => !trainerUserIds.Contains(u.Id))
                .Select(user => new TrainersNameDTO
                {
                    UserId = user.Id,
                    FullNameAr = user.FullNameAr,
                    FullNameEn = user.FullNameEn,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                })
                .ToListAsync();

            if (UsersNotTrainers.Any() && UsersNotTrainers.Count > 0)
            {
                UsersNotTrainers = UsersNotTrainers.Where(x => !string.IsNullOrEmpty(x.UserId) && !userIdsInRolesNotWanted.Contains(x.UserId) && x.Email != null && (x.Email?.ToLower().Contains("admin") == false && x.FullNameEn?.ToLower().Contains("admin") == false)).ToList();
            }

            return UsersNotTrainers;
        }

        public async Task<List<TrainersNameDTO>?> GetAllUsersNotTrainers_NamesAr_En_only(string currentUserId)
        {

            var allRoles = await _roleManager.Roles.ToListAsync();

            var rolesNot = allRoles.Where(r =>
                r.RoleNumber != (int)RoleNumber.Accountant &&
                r.RoleNumber != (int)RoleNumber.Manager &&
                r.RoleNumber != (int)RoleNumber.ActivitiesSupervisor &&
                r.Name != Role.Master.ToString() &&
                r.Name != Role.SuperAdmin.ToString()
            ).Select(r => r.Name) // role names only
             .ToList();

            // Get all users
            var allUsers = await _userManager.Users.ToListAsync();

            var userIdsInRolesNotWanted2 = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                // if user does NOT have ANY of the rolesNot
                if (userRoles != null && !userRoles.Any(r => rolesNot.Contains(r)))
                {
                    userIdsInRolesNotWanted2.Add(user);
                }
            }

            // → Users IDs
            var userIdsInRolesNotWanted = userIdsInRolesNotWanted2.Select(u => u.Id).ToList();



            // Bring all the UserIds of the Trainers
            var trainerUserIds = await _unitOfWork.Trainers.Table
                .Where(t => t.UserId != currentUserId)
                .Select(t => t.UserId)
                .ToListAsync();

            // Get all the users that are not in the trainerUserIds
            var UsersNotTrainers = await _unitOfWork.Users.Table
                .Where(u => !trainerUserIds.Contains(u.Id))
                .Select(user => new TrainersNameDTO
                {
                    UserId = user.Id,
                    FullNameAr = user.FullNameAr,
                    FullNameEn = user.FullNameEn,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber

                })
                .ToListAsync();

            if (UsersNotTrainers.Any() && UsersNotTrainers.Count > 0)
            {
                UsersNotTrainers = UsersNotTrainers.Where(x => !string.IsNullOrEmpty(x.UserId) && !userIdsInRolesNotWanted.Contains(x.UserId) &&  x.Email != null && (x.Email?.ToLower().Contains("admin") == false && x.FullNameEn?.ToLower().Contains("admin") == false)).ToList();
            }

            return UsersNotTrainers;
        }


        public async Task<Trainer?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Trainers
                .GetByIdAsync(e => e.Id == id);
        }
        public async Task<Trainer?> GetByIdAsync_byuserId(string id)
        {
            return await _unitOfWork.Trainers
                .GetByIdAsync(e =>e.UserId == id.Trim());
        }
        public async Task<int> AddAsync(Trainer entity, IFormFile? file)
        {
            if (file != null)
            {
                entity.AttachmentPath = await SaveImageAsync(file);
            }

            var entit = await _unitOfWork.Trainers.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(Trainer entity, IFormFile? file)
        {
            var existing = await _unitOfWork.Trainers.GetByIdAsync(entity.Id);
            if (existing == null) return;

            //if (file != null)
            //{
            //    // Delete the old image
            //    DeleteImageFile(existing.AttachmentPath);

            //    // Save new image
            //    entity.AttachmentPath = await SaveImageAsync(file);
            //}
            //else
            //{
            //    // Keep the old image if none uploaded
            //    entity.AttachmentPath = existing.AttachmentPath;
            //}

            _unitOfWork.Trainers.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Trainers.GetByIdAsync(id);
            if (entity != null)
            {
                // Delete only the trainer record and its attachment file.
                // Do NOT delete the associated User or Signatures —
                // keep the user account intact (only unassigning trainer role).
                DeleteImageFile(entity.AttachmentPath);
                _unitOfWork.Trainers.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads/Trainers");
            Directory.CreateDirectory(folder);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"uploads/Trainers/{fileName}";
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
