using Application.Interfaces.Admin;
using Domain.Entities.Product;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class SubProductService : ISubProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubProduct>> GetAllAsync(string? search = null, int? mainProductId = null)
        {
            var query = _unitOfWork.SubProducts.Table.Include(x => x.MainProduct).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    (x.NameAr != null && x.NameAr.Contains(search)) ||
                    (x.NameEn != null && x.NameEn.Contains(search)));
            }

            if (mainProductId.HasValue && mainProductId.Value > 0)
            {
                query = query.Where(x => x.FKMainProductId == mainProductId.Value);
            }

            return await query.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<SubProduct?> GetByIdAsync(int id)
        {
            return await _unitOfWork.SubProducts.GetByIdAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(SubProduct entity)
        {
            var created = await _unitOfWork.SubProducts.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return created.Id;
        }

        public async Task UpdateAsync(SubProduct entity)
        {
            var existing = await _unitOfWork.SubProducts.GetByIdAsync(entity.Id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.SubProducts.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.SubProducts.GetByIdAsync(id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.SubProducts.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> HasRelatedObjectsInDb(int id)
        {
            return await _unitOfWork.ContractSubProducts.GetByColumnAsync(x => x.FKSubProduct == id) != null;
        }
    }
}
