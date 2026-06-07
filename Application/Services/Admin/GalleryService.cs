using Application.Interfaces.Admin;
using Domain.Entities.Gallary;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class GalleryService : IGalleryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GalleryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Gallery>> GetAllAsync(string? search = null)
        {
            var query = _unitOfWork.Galleries.Table.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    (x.Name != null && x.Name.Contains(search)) ||
                    (x.Location != null && x.Location.Contains(search)) ||
                    (x.Description != null && x.Description.Contains(search)));
            }

            return await query.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<Gallery?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Galleries.GetByIdAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(Gallery entity)
        {
            var created = await _unitOfWork.Galleries.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return created.Id;
        }

        public async Task UpdateAsync(Gallery entity)
        {
            var existing = await _unitOfWork.Galleries.GetByIdAsync(entity.Id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.Galleries.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Galleries.GetByIdAsync(id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.Galleries.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> HasRelatedObjectsInDb(int id)
        {
            return await _unitOfWork.RoomGalleries.GetByColumnAsync(x => x.FkGallery == id) != null;
        }
    }
}
