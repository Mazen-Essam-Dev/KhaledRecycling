using Application.Interfaces.Admin;
using Domain.Entities.Product;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class MainProductService : IMainProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MainProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MainProduct>> GetAllAsync(string? search = null)
        {
            var query = _unitOfWork.MainProducts.Table.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    (x.NameAr != null && x.NameAr.Contains(search)) ||
                    (x.NameEn != null && x.NameEn.Contains(search)));
            }

            return await query.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<MainProduct?> GetByIdAsync(int id)
        {
            return await _unitOfWork.MainProducts.GetByIdAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(MainProduct entity)
        {
            var created = await _unitOfWork.MainProducts.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return created.Id;
        }

        public async Task UpdateAsync(MainProduct entity)
        {
            var existing = await _unitOfWork.MainProducts.GetByIdAsync(entity.Id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.MainProducts.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.MainProducts.GetByIdAsync(id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.MainProducts.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> HasRelatedObjectsInDb(int id)
        {
            return await _unitOfWork.SubProducts.GetByColumnAsync(x => x.FKMainProductId == id) != null;
        }
    }
}
