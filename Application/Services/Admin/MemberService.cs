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
        private readonly ICompareService _iCompareService;
        private readonly string FileName = "Members";


        public MemberService(IUnitOfWork unitOfWork, ICompareService iCompareService)
        {
            _unitOfWork = unitOfWork;
            _iCompareService = iCompareService;
        }

        public async Task<IEnumerable<MemberEntity>> GetAllAsync()
        {
            return await _unitOfWork.Members.GetAllAsync(t => t.Nationality, b => b.MemberType);
        }
        public async Task<bool> MemberHasCourses(int memberId)
        {
            var result = await _unitOfWork.Subscriptions.GetByColumnAsync(s => s.MemberId == memberId && s.SubscribedInType == SubscriptionType.Course);
            return result != null;
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
        public async Task<(double Compaire_percentage_FullEnName, double Compaire_percentage_FullArName, double Compaire_percentage_IDNumber, double Compaire_percentage_BirthDate, double Compaire_percentage_ExpiryDate, DateTime? VM_ExpiryDate_DT)> ValidationCompareAllInputsToExtractedAsync(MemberDTO memberDTO, IDCardExtractedDataDTO iDCardExtractedDataDTO)
        {
            // For Testing
            //var Compaire_percentage_FullEnName = await _iCompareService.SimilarityPercentage(iDCardExtractedDataVM.matchFullEnName, "Muhammad Sajawal Khan Chaudhary Muhammad Iqbal");
            //65%
            var Compaire_percentage_FullEnName = await _iCompareService.SimilarityPercentage(iDCardExtractedDataDTO.matchFullEnName, memberDTO.FullNameEn);
            //65%
            var Compaire_percentage_FullArName = await _iCompareService.SimilarityPercentage(iDCardExtractedDataDTO.matchFullArName, memberDTO.FullNameAr);
            //100%
            var Compaire_percentage_IDNumber = await _iCompareService.SimilarityPercentage(iDCardExtractedDataDTO.matchIDNumber, memberDTO.IdNumber);

            string MM_yyyy_BirthDate = memberDTO.DateOfBirth?.ToString("MM/yyyy", CultureInfo.InvariantCulture) ?? " / / ";
            string MM_yyyy_ExpiryDate = memberDTO.IdExpiryDate?.ToString("MM/yyyy", CultureInfo.InvariantCulture) ?? " / / ";
            string iDCardExtracted_BirthDate = iDCardExtractedDataDTO.matchBirth.Substring(iDCardExtractedDataDTO.matchBirth.IndexOf("/") + 1);
            string iDCardExtracted_ExpiryDate = iDCardExtractedDataDTO.matchExpiryDate.Substring(iDCardExtractedDataDTO.matchExpiryDate.IndexOf("/") + 1);
            DateTime VM_BirthDate_DT = DateTime.ParseExact(
            "01/" + iDCardExtracted_BirthDate,      // => "01/10/1990"
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture);
            DateTime VM_ExpiryDate_DT = DateTime.ParseExact(
            "01/" + iDCardExtracted_ExpiryDate,      // => "01/10/1990"
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture);
            DateTime Page_BirthDate_DT = DateTime.ParseExact(
            "01/" + MM_yyyy_BirthDate,      // => "01/10/1990"
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture);
            DateTime Page_ExpiryDate_DT = DateTime.ParseExact(
            "01/" + MM_yyyy_ExpiryDate,      // => "01/10/1990"
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture);
            double Compaire_percentage_BirthDate = 0;
            double Compaire_percentage_ExpiryDate = 0;
            if (Page_BirthDate_DT.Date == VM_BirthDate_DT.Date) Compaire_percentage_BirthDate = 100;
            if (Page_ExpiryDate_DT.Date == VM_ExpiryDate_DT.Date) Compaire_percentage_ExpiryDate = 100;

            return (Compaire_percentage_FullEnName, Compaire_percentage_FullArName, Compaire_percentage_IDNumber, Compaire_percentage_BirthDate, Compaire_percentage_ExpiryDate, VM_ExpiryDate_DT);
        }
    }

}
