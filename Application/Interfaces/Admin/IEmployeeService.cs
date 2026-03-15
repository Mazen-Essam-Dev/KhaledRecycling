using Domain.DTOs.Admin.Employees;
using Domain.Entities.Employees;

namespace Application.Interfaces.Admin
{
    public interface IEmployeeService
    {
        Task<bool> HasRelatedObjectsInDb(int empId);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<int> AddAsync(Employee entity);
        Task UpdateAsync(Employee entity);
        Task DeleteAsync(int id);
        Task UploadAttachmentsAsync(EmployeeAttachmentsDTO model);
        Task<IEnumerable<EmployeeAttachment>> GetAttachmentsAsync(int empId);
    }
}
