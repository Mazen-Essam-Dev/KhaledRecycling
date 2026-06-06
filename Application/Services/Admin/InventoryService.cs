using Application.Interfaces.Admin;
using Domain.Entities.Inventory;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync(string? search = null)
        {
            var query = _unitOfWork.Inventories.Table.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    (x.Name != null && x.Name.Contains(search)) ||
                    (x.Location != null && x.Location.Contains(search)) ||
                    (x.Description != null && x.Description.Contains(search)));
            }

            return await query.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<Inventory?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Inventories.GetByIdAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(Inventory entity)
        {
            var created = await _unitOfWork.Inventories.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return created.Id;
        }

        public async Task UpdateAsync(Inventory entity)
        {
            var existing = await _unitOfWork.Inventories.GetByIdAsync(entity.Id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.Inventories.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Inventories.GetByIdAsync(id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.Inventories.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> HasRelatedObjectsInDb(int id)
        {
            return await _unitOfWork.RoomInventories.GetByColumnAsync(x => x.FkInventory == id) != null;
        }
    }
}
