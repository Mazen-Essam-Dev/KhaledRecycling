using Application.Interfaces.Admin;
using Domain.Entities.Inventory;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class RoomInventoryService : IRoomInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomInventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoomInventory>> GetAllAsync(string? search = null, int? inventoryId = null, int? subWasteId = null)
        {
            var query = _unitOfWork.RoomInventories.Table
                .Include(x => x.Inventory)
                .Include(x => x.SubWaste)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    (x.GenCode != null && x.GenCode.Contains(search)) ||
                    (x.Description != null && x.Description.Contains(search)));
            }

            if (inventoryId.HasValue && inventoryId.Value > 0)
            {
                query = query.Where(x => x.FkInventory == inventoryId.Value);
            }

            if (subWasteId.HasValue && subWasteId.Value > 0)
            {
                query = query.Where(x => x.FKSubWaste == subWasteId.Value);
            }

            return await query.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<RoomInventory?> GetByIdAsync(int id)
        {
            return await _unitOfWork.RoomInventories.GetByIdAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(RoomInventory entity)
        {
            var created = await _unitOfWork.RoomInventories.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return created.Id;
        }

        public async Task UpdateAsync(RoomInventory entity)
        {
            var existing = await _unitOfWork.RoomInventories.GetByIdAsync(entity.Id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.RoomInventories.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.RoomInventories.GetByIdAsync(id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.RoomInventories.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }

        public Task<bool> HasRelatedObjectsInDb(int id)
        {
            return Task.FromResult(false);
        }
    }
}
