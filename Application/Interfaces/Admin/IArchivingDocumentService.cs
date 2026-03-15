using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface IArchivingDocumentService
    {
        Task<IEnumerable<ArchivingDocument>> GetAllAsync();
        Task<ArchivingDocument?> GetByIdAsync(int id);
        Task<int> AddAsync(ArchivingDocument entity);
        Task UpdateAsync(ArchivingDocument entity);
        Task DeleteAsync(int id);
    }

}
