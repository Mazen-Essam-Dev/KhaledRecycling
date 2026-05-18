using Application.Interfaces.Admin;
using Domain.Entities.Waste;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class OrderBuyFromClientService : IOrderBuyFromClientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderBuyFromClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderBuyFromClient>> GetAllAsync(string? search = null, int? mainWasteId = null, int? subWasteId = null)
        {
            var query = _unitOfWork.OrderBuyFromClients.Table
                .Include(x => x.SubWaste)
                .ThenInclude(x => x.MainWaste)
                .Include(x => x.Status)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    (x.SubWaste != null && x.SubWaste.NameAr != null && x.SubWaste.NameAr.Contains(search)) ||
                    (x.SubWaste != null && x.SubWaste.NameEn != null && x.SubWaste.NameEn.Contains(search)));
            }

            if (mainWasteId.HasValue && mainWasteId.Value > 0)
            {
                query = query.Where(x => x.SubWaste != null && x.SubWaste.FKMainWasteId == mainWasteId.Value);
            }

            if (subWasteId.HasValue && subWasteId.Value > 0)
            {
                query = query.Where(x => x.FKSubWasteId == subWasteId.Value);
            }

            return await query.OrderByDescending(x => x.OrderDate).ToListAsync();
        }

        public async Task<OrderBuyFromClient?> GetByIdAsync(int id)
        {
            return await _unitOfWork.OrderBuyFromClients.GetByIdAsync(x => x.Id == id,c=>c.SubWaste!);
        }

        public async Task<int> AddAsync(OrderBuyFromClient entity)
        {
            var created = await _unitOfWork.OrderBuyFromClients.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return created.Id;
        }

        public async Task UpdateAsync(OrderBuyFromClient entity)
        {
            var existing = await _unitOfWork.OrderBuyFromClients.GetByIdAsync(entity.Id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.OrderBuyFromClients.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.OrderBuyFromClients.GetByIdAsync(id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.OrderBuyFromClients.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> HasRelatedObjectsInDb(int id)
        {
            // Check if this order has any related records
            return false;
        }
    }
}
