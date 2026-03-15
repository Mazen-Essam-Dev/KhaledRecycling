using Domain.Entities;


namespace Application.Interfaces.Admin
{
    public interface IAccountService
    {
        public Task<Signature> GetSignatureAsync(string userId);
        public Task<IEnumerable<Signature>> GetAllSignaturesAsync(string userId);
        public Task<bool> SaveSignatureAsync(Signature model);
        public Task<bool> CheckUsernameIfExistsAsync(string userName, string id);
    }
}
