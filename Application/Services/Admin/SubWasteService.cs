using Application.Interfaces.Admin;
using Domain.Entities.Waste;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class SubWasteService : ISubWasteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubWasteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubWaste>> GetAllAsync(string? search = null, int? mainWasteId = null,bool? isAdd = null)
        {
            var query = _unitOfWork.SubWastes.Table.Include(x => x.MainWaste).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    (x.NameAr != null && x.NameAr.Contains(search)) ||
                    (x.NameEn != null && x.NameEn.Contains(search)));
            }

            if (mainWasteId.HasValue && mainWasteId.Value > 0)
            {
                if(isAdd.HasValue && isAdd == true)
                    query = query.Where(x => x.FKMainWasteId == mainWasteId.Value && x.StatusChar==null); // worked Now only
                else 
                    query = query.Where(x => x.FKMainWasteId == mainWasteId.Value); // All
            }

            return await query.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<SubWaste?> GetByIdAsync(int id)
        {
            return await _unitOfWork.SubWastes.GetByIdAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(SubWaste entity)
        {
            var created = await _unitOfWork.SubWastes.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return created.Id;
        }

        public async Task UpdateAsync(SubWaste entity)
        {
            var existing = await _unitOfWork.SubWastes.GetByIdAsync(entity.Id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.SubWastes.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.SubWastes.GetByIdAsync(id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.SubWastes.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> HasRelatedObjectsInDb(int id)
        {
            return await _unitOfWork.ContractSubWastes.GetByColumnAsync(x => x.FKSubWaste == id) != null;
        }
    }
}
