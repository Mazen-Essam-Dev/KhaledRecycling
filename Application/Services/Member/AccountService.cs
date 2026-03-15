using Application.Helpers;
using Application.Interfaces.Member;
using DocumentFormat.OpenXml.Vml.Office;
using Domain.DTOs.Member.Account;
using Domain.Entities;
using Infrastructure.Repositories;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Member
{
    public class AccountService : IAccountService
    {
        private readonly IGenericRepository<MemberEntity> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _imagesFullPath = Path.Combine("wwwroot/uploads/Members");
        private readonly string _imagesPath = "uploads/Members/";
        private readonly Interfaces.Admin.IMemberService _memberService;

        public AccountService(IGenericRepository<MemberEntity> userRepository, IHttpContextAccessor httpContextAccessor, Interfaces.Admin.IMemberService memberService)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _memberService = memberService;
        }

        public async Task<(bool,string)> RegisterAsync(MemberEntity model)
        {
            var existingUser = await _userRepository.GetByIdAsync(u => u.Email == model.Email);
            var existingIdNumber = await _userRepository.GetByIdAsync(u => u.IdNumber == model.IdNumber);

            if (existingIdNumber != null && existingUser != null)
                return (false, "EmailUsedBefore_&&_IdNationalNumber_UsedBefore");

            if (existingUser != null)
                return (false, "EmailUsedBefore");

            if (existingIdNumber != null)
                return (false, "IdNationalNumber_UsedBefore"); //throw new Exception("رقم الهوية مسجل مسبقًا.");


            var codeExists = _userRepository.Table.Any(x => x.Code == model.Code);
            if (codeExists)
            {
                model.Code = await _memberService.GenerateNewCode();
            }

            await _userRepository.AddAsync(model);
            return (await _userRepository.SaveChangesAsync(),"done");
        }

        public async Task<(MemberEntity?,string?)> ValidateUserAsync(MemberEntity model)
        {
            var user = await _userRepository.GetByIdAsync(u => u.Email == model.Email);
            if (user == null) return (null,"NotExisting");

            var hash = HashHelper.ComputeSha256Hash(model.Password);
            return user.Password == hash ? (user,"Valid") : (null, "NotMatchingBassword");
        }

        public async Task<MemberEntity?> GetMemberByEmailAsync(string email)
        {
            return await _userRepository.GetByColumnAsync(m => m.Email == email);
        }
        public async Task<bool> UpdateMemberAsync(MemberEntity model)
        {
            var existingUser = await _userRepository.GetByIdAsync(u => u.Id == model.Id);
            if (existingUser == null)
                return false;

            _userRepository.Update(model);
            return await _userRepository.SaveChangesAsync();
        }
        public async Task<FilesDTO> SaveImagesInSession(FilesDTO model)
        {
            if (model.ProfileImage != null)
            {
                //delete old temp image
                var oldProfileImageFromSession = _httpContextAccessor.HttpContext?.Session.GetString("ProfileImagePath");
                if (oldProfileImageFromSession != null)
                    FileHelper.DeleteImageFile(oldProfileImageFromSession);


                Directory.CreateDirectory(_imagesFullPath);
                //var fileName = Guid.NewGuid() + Path.GetExtension(model.ProfileImage.FileName);
                var fileName = $"{model.MemberCode}_ProfileImage" + Path.GetExtension(model.ProfileImage.FileName);
                var filePath = Path.Combine(_imagesFullPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                model.ProfileImagePath = _imagesPath + fileName;
                // ✅ Safely set session value
                var httpContext = _httpContextAccessor.HttpContext;
                httpContext?.Session.SetString("ProfileImagePath", model.ProfileImagePath);
            }

            if (model.IdImage != null)
            {
                //delete old temp image
                var oldIdImageFromSession = _httpContextAccessor.HttpContext?.Session.GetString("IdImagePath");
                if (oldIdImageFromSession != null)
                    FileHelper.DeleteImageFile(oldIdImageFromSession);


                Directory.CreateDirectory(_imagesFullPath);
                //var fileName = Guid.NewGuid() + Path.GetExtension(model.IdImage.FileName);
                var fileName = $"{model.MemberCode}_IdImage" + Path.GetExtension(model.IdImage.FileName);
                var filePath = Path.Combine(_imagesFullPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.IdImage.CopyToAsync(stream);
                }

                model.IdImagePath = _imagesPath + fileName;
                // ✅ Safely set session value
                var httpContext = _httpContextAccessor.HttpContext;
                httpContext?.Session.SetString("IdImagePath", model.IdImagePath);
            }
            if (model.PassportImage != null)
            {
                //delete old temp image
                var oldPassportImageFromSession = _httpContextAccessor.HttpContext?.Session.GetString("PassportImagePath");
                if (oldPassportImageFromSession != null)
                    FileHelper.DeleteImageFile(oldPassportImageFromSession);


                Directory.CreateDirectory(_imagesFullPath);
                //var fileName = Guid.NewGuid() + Path.GetExtension(model.PassportImage.FileName);
                var fileName = $"{model.MemberCode}_PassportImage"+ Path.GetExtension(model.PassportImage.FileName);
                var filePath = Path.Combine(_imagesFullPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PassportImage.CopyToAsync(stream);
                }

                model.PassportImagePath = _imagesPath + fileName;
                // ✅ Safely set session value
                var httpContext = _httpContextAccessor.HttpContext;
                httpContext?.Session.SetString("PassportImagePath", model.PassportImagePath);
            }
            return model;

        }
        public FilesDTO MoveImagesFromSessionToModel(FilesDTO model)
        {
            if (model.ProfileImage == null)
            {
                var profileImageFromSession = _httpContextAccessor.HttpContext?.Session.GetString("ProfileImagePath");
                if (profileImageFromSession != null)
                {
                    model.ProfileImagePath = profileImageFromSession;
                    _httpContextAccessor.HttpContext?.Session.Remove("ProfileImagePath");
                }
            }
            if (model.IdImage == null)
            {
                var idImageFromSession = _httpContextAccessor.HttpContext?.Session.GetString("IdImagePath");
                if (idImageFromSession != null)
                {
                    model.IdImagePath = idImageFromSession;
                    _httpContextAccessor.HttpContext?.Session.Remove("IdImagePath");
                }
            }
            if (model.PassportImage == null)
            {
                var passportImageFromSession = _httpContextAccessor.HttpContext?.Session.GetString("PassportImagePath");
                if (passportImageFromSession != null)
                {
                    model.PassportImagePath = passportImageFromSession;
                    _httpContextAccessor.HttpContext?.Session.Remove("PassportImagePath");
                }
            }
            return model;
        }
        public bool DeleteImagesFromSession()
        {
            try
            {
                //delete old temp image
                var oldProfileImageFromSession = _httpContextAccessor.HttpContext?.Session.GetString("ProfileImagePath");
                if (oldProfileImageFromSession != null)
                {
                    FileHelper.DeleteImageFile(oldProfileImageFromSession);
                    _httpContextAccessor.HttpContext?.Session.Remove("ProfileImagePath");
                }

                var oldIdImageFromSession = _httpContextAccessor.HttpContext?.Session.GetString("IdImagePath");
                if (oldIdImageFromSession != null)
                {
                    FileHelper.DeleteImageFile(oldIdImageFromSession);
                    _httpContextAccessor.HttpContext?.Session.Remove("IdImagePath");
                }

                var oldPassportImageFromSession = _httpContextAccessor.HttpContext?.Session.GetString("PassportImagePath");
                if (oldPassportImageFromSession != null)
                {
                    FileHelper.DeleteImageFile(oldPassportImageFromSession);
                    _httpContextAccessor.HttpContext?.Session.Remove("PassportImagePath");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
