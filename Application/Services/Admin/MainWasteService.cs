using Application.Interfaces.Admin;
using Domain.Entities.Waste;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class MainWasteService : IMainWasteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MainWasteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MainWaste>> GetAllAsync(string? search = null)
        {
            var query = _unitOfWork.MainWastes.Table.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    (x.NameAr != null && x.NameAr.Contains(search)) ||
                    (x.NameEn != null && x.NameEn.Contains(search)));
            }

            return await query.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<MainWaste?> GetByIdAsync(int id)
        {
            return await _unitOfWork.MainWastes.GetByIdAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(MainWaste entity)
        {
            var created = await _unitOfWork.MainWastes.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return created.Id;
        }

        public async Task UpdateAsync(MainWaste entity)
        {
            var existing = await _unitOfWork.MainWastes.GetByIdAsync(entity.Id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.MainWastes.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.MainWastes.GetByIdAsync(id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.MainWastes.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> HasRelatedObjectsInDb(int id)
        {
            return await _unitOfWork.SubWastes.GetByColumnAsync(x => x.FKMainWasteId == id) != null;
        }
    }
}
