using Domain.DTOs;
using Domain.Entities;

namespace Application.Interfaces.Admin
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberEntity>> GetAllAsync();
        Task<bool> MemberHasCourses(int memberId);
        Task<IEnumerable<MemberEntity>> GetAllSpesificAsync();
        Task<MemberEntity?> GetByIdAsync(int id);
        Task<int> AddAsync(MemberEntity entity);
        Task UpdateAsync(MemberEntity entity);
        Task DeleteAsync(int id);
        Task<int> GenerateNewCode();
        Task<bool> SuspendAsync(int id, bool suspend);
        Task<(double Compaire_percentage_FullEnName, double Compaire_percentage_FullArName, double Compaire_percentage_IDNumber, double Compaire_percentage_BirthDate, double Compaire_percentage_ExpiryDate, DateTime? VM_ExpiryDate_DT)> ValidationCompareAllInputsToExtractedAsync(MemberDTO memberDTO, IDCardExtractedDataDTO iDCardExtractedDataDTO);

    }

}
