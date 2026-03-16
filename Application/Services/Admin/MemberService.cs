using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Interfaces.Member;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Application.Services.Admin
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string FileName = "Members";


        public MemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MemberEntity>> GetAllAsync()
        {
            return await _unitOfWork.Members.GetAllAsync(t => t.Nationality, b => b.MemberType);
        }

        public async Task<IEnumerable<MemberEntity>> GetAllSpesificAsync()
        {
            return await _unitOfWork.Members
            .Table // It must be IQueryable for EF to Selection in SQL
            .Select(m => new MemberEntity
            {
                Id = m.Id,
                NationalityId = m.NationalityId,
                GenderId = m.GenderId,
                Age = m.Age
            })
            .ToListAsync();
        }

        public async Task<MemberEntity?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Members
                .GetByIdAsync(e => e.Id == id, e => e.Nationality);
        }

        public async Task<int> AddAsync(MemberEntity entity)
        {
            #region add images
            //entity.ProfileImagePath = entity.ProfileImage == null ? "" : await FileHelper.SaveImageAsync(entity.ProfileImage, FileName);
            //entity.IdImagePath = entity.IdImage == null ? "" : await FileHelper.SaveImageAsync(entity.IdImage, FileName);
            //entity.PassportImagePath = entity.PassportImage == null ? "" : await FileHelper.SaveImageAsync(entity.PassportImage, FileName);
            #endregion

            var entit = await _unitOfWork.Members.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(MemberEntity entity)
        {
            var existing = await _unitOfWork.Members.GetByIdAsync(entity.Id);
            if (existing == null) return;

            #region update images
            // PROFILE IMAGE
            if (entity.ProfileImage != null)
            {
                FileHelper.DeleteImageFile(existing.ProfileImagePath); // Delete old
                entity.ProfileImagePath = await FileHelper.SaveImageAsync(entity.ProfileImage, FileName); // Save new
            }
            else
            {
                entity.ProfileImagePath = existing.ProfileImagePath; // Keep old
            }

            // ID IMAGE
            if (entity.IdImage != null)
            {
                FileHelper.DeleteImageFile(existing.IdImagePath);
                entity.IdImagePath = await FileHelper.SaveImageAsync(entity.IdImage, FileName);
            }
            else
            {
                entity.IdImagePath = existing.IdImagePath;
            }

            // PASSPORT IMAGE
            if (entity.PassportImage != null)
            {
                FileHelper.DeleteImageFile(existing.PassportImagePath);
                entity.PassportImagePath = await FileHelper.SaveImageAsync(entity.PassportImage, FileName);
            }
            else
            {
                entity.PassportImagePath = existing.PassportImagePath;
            }
            #endregion
            if (entity.Password != null)
            {
                entity.Password = HashHelper.ComputeSha256Hash(entity.Password);
            }
            else
            {
                entity.Password = existing.Password;
            }
            _unitOfWork.Members.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Members.GetByIdAsync(id);
            if (entity != null)
            {
                #region delete images 
                // Delete image from disk
                FileHelper.DeleteImageFile(entity.ProfileImagePath);
                FileHelper.DeleteImageFile(entity.IdImagePath);
                FileHelper.DeleteImageFile(entity.PassportImagePath);
                #endregion

                _unitOfWork.Members.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<int> GenerateNewCode()
        {
            var members = await _unitOfWork.Members.GetAllAsync();

            try
            {
                if (members.Any())
                {
                    var maxCode = members
                        .Where(e => e.Code.HasValue)
                        .Max(e => e.Code.Value);

                    return maxCode + 1;
                }
            }
            catch (Exception ex)
            {
                return 1000;
            }
            return 1000;

        }

        public async Task<bool> SuspendAsync(int id, bool suspend)
        {
            var entity = await _unitOfWork.Members.GetByIdAsync(id);
            if (entity != null)
            {
                entity.Suspended = suspend;
                _unitOfWork.Members.Update(entity);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            return false;

        }
    }

}
