using Application.Interfaces.Admin;
using Domain.Entities.Gallary;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class RoomGalleryService : IRoomGalleryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomGalleryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoomGallery>> GetAllAsync(string? search = null, int? galleryId = null, int? subProductId = null)
        {
            var query = _unitOfWork.RoomGalleries.Table
                .Include(x => x.Gallery)
                .Include(x => x.SubProduct)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    (x.GenCode != null && x.GenCode.Contains(search)) ||
                    (x.Description != null && x.Description.Contains(search)));
            }

            if (galleryId.HasValue && galleryId.Value > 0)
            {
                query = query.Where(x => x.FkGallery == galleryId.Value);
            }

            if (subProductId.HasValue && subProductId.Value > 0)
            {
                query = query.Where(x => x.FkSubProduct == subProductId.Value);
            }

            return await query.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<RoomGallery?> GetByIdAsync(int id)
        {
            return await _unitOfWork.RoomGalleries.GetByIdAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(RoomGallery entity)
        {
            var created = await _unitOfWork.RoomGalleries.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return created.Id;
        }

        public async Task UpdateAsync(RoomGallery entity)
        {
            var existing = await _unitOfWork.RoomGalleries.GetByIdAsync(entity.Id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.RoomGalleries.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.RoomGalleries.GetByIdAsync(id);
            if (existing == null)
            {
                return;
            }

            _unitOfWork.RoomGalleries.Delete(existing);
            await _unitOfWork.CompleteAsync();
        }

        public Task<bool> HasRelatedObjectsInDb(int id)
        {
            return Task.FromResult(false);
        }
    }
}
