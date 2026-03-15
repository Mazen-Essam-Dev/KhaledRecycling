using Application.Interfaces.Admin;
using Domain.Entities.BudgetItem;
using Infrastructure.Repositories.InterfacesDB;

namespace Application.Services.Admin
{
    public class BudgetItemService : IBudgetItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BudgetItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BudgetItem>> GetAllAsync()
        {
            return await _unitOfWork.BudgetItems.GetAllAsync();
        }

        public async Task<BudgetItem?> GetByIdAsync(int id)
        {
            return await _unitOfWork.BudgetItems
                .GetByIdAsync(e => e.Id == id);
        }

        public async Task<int> AddAsync(BudgetItem entity)
        {
            var itemNumberExists = _unitOfWork.BudgetItems.Table.Any(x => x.ItemNumber == entity.ItemNumber);
            if (itemNumberExists)
            {
                entity.ItemNumber = await GenerateNewCode();
            }
            
            var result = await _unitOfWork.BudgetItems.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return result.Id;
        }

        public async Task UpdateAsync(BudgetItem entity)
        {
            var existing = await _unitOfWork.BudgetItems.GetByIdAsync(entity.Id);
            if (existing == null) return;

            _unitOfWork.BudgetItems.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.BudgetItems.GetByIdAsync(id);
                if (entity != null)
                {
                    _unitOfWork.BudgetItems.Delete(entity);
                    await _unitOfWork.CompleteAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int> GenerateNewCode()
        {
            var Employees = await _unitOfWork.BudgetItems.GetAllAsync();

            try
            {
                if (Employees.Any())
                {
                    var maxCode = Employees.Max(e => e.ItemNumber);

                    return maxCode.Value + 1;
                }
            }
            catch (Exception ex)
            {
                return 1000;
            }
            return 1000;
        }

    }

}
