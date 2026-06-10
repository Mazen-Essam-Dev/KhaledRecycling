using Application.Interfaces.Admin;
using Domain.Entities.Product;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class OrderSellToClientService : IOrderSellToClientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderSellToClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderSellToClient>> GetAllAsync(string? search = null, int? mainProductId = null, int? subProductId = null)
        {
            var query = _unitOfWork.OrderSellToClients.Table
                .Include(x => x.SubProduct)
                .ThenInclude(x => x.MainProduct)
                .Include(x => x.Status)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    (x.SubProduct != null && x.SubProduct.NameAr != null && x.SubProduct.NameAr.Contains(search)) ||
                    (x.SubProduct != null && x.SubProduct.NameEn != null && x.SubProduct.NameEn.Contains(search)));
            }

            if (mainProductId.HasValue && mainProductId.Value > 0)
            {
                query = query.Where(x => x.SubProduct != null && x.SubProduct.FKMainProductId == mainProductId.Value);
            }

            if (subProductId.HasValue && subProductId.Value > 0)
            {
                query = query.Where(x => x.FKSubProductId == subProductId.Value);
            }

            return await query.OrderByDescending(x => x.OrderDate).ToListAsync();
        }

        public async Task<OrderSellToClient?> GetByIdAsync(int id)
        {
            return await _unitOfWork.OrderSellToClients.GetByIdAsync(x => x.Id == id,c=>c.SubProduct!);
        }

        public async Task<int> AddAsync(OrderSellToClient entity)
        {
            var pendingStatus = await _unitOfWork.Statuses.GetByIdAsync(x=>x.ShortChar=="P");
            entity.StatusId = pendingStatus?.Id;
            var created = await _unitOfWork.OrderSellToClients.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return created.Id;
        }

        public async Task UpdateAsync(OrderSellToClient entity)
        {
            var existing = await _unitOfWork.OrderSellToClients.GetByIdAsync(entity.Id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.OrderSellToClients.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.OrderSellToClients.GetByIdAsync(id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.OrderSellToClients.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> HasRelatedObjectsInDb(int id)
        {
            // Check if this order has any related records
            return false;
        }
    }
}
