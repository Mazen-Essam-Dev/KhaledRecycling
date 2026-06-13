using Domain.Entities.Gallery;

namespace Application.Interfaces.Admin
{
    public interface IRoomGalleryService
    {
        Task<IEnumerable<RoomGallery>> GetAllAsync(string? search = null, int? galleryId = null, int? subProductId = null);
        Task<RoomGallery?> GetByIdAsync(int id);
        Task<int> AddAsync(RoomGallery entity);
        Task UpdateAsync(RoomGallery entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
