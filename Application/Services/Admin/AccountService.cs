using Application.Helpers;
using Application.Interfaces.Admin;
using DocumentFormat.OpenXml.Vml.Office;
using Domain.Entities;
using Infrastructure.Repositories;
using Infrastructure.Repositories.InterfacesDB;

namespace Application.Services.Admin
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string FileName = "Signatures";

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Signature> GetSignatureAsync(string userId)
        {
            return await _unitOfWork.Signatures.GetByColumnAsync(s => s.UserId == userId) ?? new Signature();
        }

        public async Task<IEnumerable<Signature>> GetAllSignaturesAsync(string userId)
        {
            var all = await _unitOfWork.Signatures.GetAllAsync();
            return all.Where(s => s.UserId == userId);
        }

        public async Task<bool> SaveSignatureAsync(Signature model)
        {
            try
            {
                // Always add a new signature, do not overwrite old ones
                if (model.SignatureFile != null)
                {
                    model.ImagePath = await FileHelper.SaveImageAsync(model.SignatureFile, FileName);
                }
                // Set CreatedAt to now (if not set)
                if (model.CreatedAt == default)
                    model.CreatedAt = DateOnly.FromDateTime(AppDubaiTime.Now);

                await _unitOfWork.Signatures.AddAsync(model);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> CheckUsernameIfExistsAsync(string userName, string id)
        {
            var user = await _unitOfWork.Users.GetByColumnAsync(u => u.UserName == userName);
            return user != null && user.Id != id;
        }


    }
}
