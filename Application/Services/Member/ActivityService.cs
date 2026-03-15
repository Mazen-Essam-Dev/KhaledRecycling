using Application.Interfaces.Member;
using Infrastructure.Repositories.InterfacesDB;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Member
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IEnumerable<Activity> Activities, IEnumerable<Subscription> Subscriptions)> GetAllAsync(string username)
        {
            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == username);

            IEnumerable<Subscription> subscriptions = Enumerable.Empty<Subscription>();

            if (user != null)
            {
                subscriptions = await _unitOfWork.Subscriptions.GetAllAsync(s =>
                    s.MemberId == user.Id && s.SubscribedInType == SubscriptionType.Activity);
            }

            var activities = await _unitOfWork.Activities.GetAllAsync();

            return (activities, subscriptions);
        }

        public async Task<Activity?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Activities.GetByIdAsync(id);
        }
        public async Task AddAsync(Activity activity)
        {
            await _unitOfWork.Activities.AddAsync(activity);
            await _unitOfWork.CompleteAsync();
        }
        public async Task UpdateAsync(Activity activity)
        {
            _unitOfWork.Activities.Update(activity);
            await _unitOfWork.CompleteAsync();
        }
        public async Task SubscribeAsync(int activityId, string email)
        {
            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == email);

            var subscription = new Subscription
            {
                MemberId = user.Id,
                SubscribedInId = activityId,
                SubscribedInType = SubscriptionType.Activity,
            };
            await _unitOfWork.Subscriptions.AddAsync(subscription);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<bool> CheckAgeAsync(int activityId, string email)
        {
            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == email);
            var activity = await _unitOfWork.Activities.GetByIdAsync(activityId);

            var ageParts = activity?.MinimumAge?.Replace(" ","").Split('-');

            if (ageParts?.Count()==2)
            {
                int activityAge1 = int.TryParse(ageParts[0], out int age1) ? age1 : -1;
                int activityAge2 = int.TryParse(ageParts[1], out int age2) ? age2 : -1;
                if (activityAge1 != -1 && activityAge2 != -1 && (user?.Age < activityAge1 || user?.Age > activityAge2))
                    return false;
            }
            else if (ageParts?.Count() >2) 
                return false;

            var minimumAge = activity?.MinimumAge?.Replace(" ", "").Replace("-", "");
            int activityAgeOnlyOne = int.TryParse(minimumAge, out int age3) ? age3 : -1;

            if (user?.Age < activityAgeOnlyOne && activityAgeOnlyOne!=-1)
                return false;
            return true;
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Activities.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Activities.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }
        public bool Exists(int id)
        {
            return _unitOfWork.Activities.Table.Any(a => a.Id == id);
        }
    }
}
