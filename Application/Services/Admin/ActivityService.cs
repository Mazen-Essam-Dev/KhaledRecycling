using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Admin
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string FileName = "Activities";

        public ActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Activity>> GetAllAsync()
        {
            return await _unitOfWork.Activities.GetAllAsync();
        }

        public async Task<Activity?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Activities
                .GetByIdAsync(e => e.Id == id);
        }

        public async Task<int> AddAsync(Activity entity, IFormFile? file)
        {
            if (file != null)
            {
                entity.AttachmentPath = await FileHelper.SaveImageAsync(file, FileName); // Save new
            }

            var NewRecord = await _unitOfWork.Activities.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return NewRecord.Id;
        }

        public async Task UpdateAsync(Activity entity, IFormFile? file)
        {
            var existing = await _unitOfWork.Activities.GetByIdAsync(entity.Id);
            if (existing == null) return;

            //if (file != null)
            //{
            //    FileHelper.DeleteImageFile(existing.AttachmentPath); // Delete old
            //    entity.AttachmentPath = await FileHelper.SaveImageAsync(file, FileName); // Save new
            //}
            //else
            //{
            //    // Keep the old image if none uploaded
            //    entity.AttachmentPath = existing.AttachmentPath;
            //}

            _unitOfWork.Activities.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Activities.GetByIdAsync(id);
            if (entity != null)
            {
                FileHelper.DeleteImageFile(entity.AttachmentPath); // Delete old
                _unitOfWork.Activities.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<(IEnumerable<MemberEntity>?, IEnumerable<Subscription>?)> GetAllMembersOfActivityAsync(int ActivityId)
        {
            var allMembers = await _unitOfWork.Members.GetAllAsync(x => x.Nationality);
            var membersSubscriptionsActivity = await _unitOfWork.Subscriptions
                .GetAllAsync(s => s.SubscribedInId == ActivityId && s.SubscribedInType == SubscriptionType.Activity);

            var activityMembers = allMembers
                .Where(member => membersSubscriptionsActivity.Any(s => s.MemberId == member.Id));

            return (activityMembers, membersSubscriptionsActivity);
        }

    }

}
