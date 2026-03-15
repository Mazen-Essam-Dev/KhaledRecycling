using Application.Interfaces.Member;
using Infrastructure.Repositories.InterfacesDB;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Member
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SubscribeAsync(int CourseId, string username)
        {
            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == username);

            var subscription = new Subscription
            {
                MemberId = user.Id,
                SubscribedInId = CourseId,
                SubscribedInType = SubscriptionType.Course,
            };
            await _unitOfWork.Subscriptions.AddAsync(subscription);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<bool> AddRateAsync(int CourseId, string email, int? rate)
        {
            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == email);

            var subscription = await _unitOfWork.Subscriptions.GetByColumnAsync(x => x.MemberId == user.Id && x.SubscribedInType == SubscriptionType.Course && x.SubscribedInId == CourseId);
            if (rate == null || subscription == null) return false;
            subscription.Rate = rate;
            _unitOfWork.Subscriptions.Update(subscription);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<(IEnumerable<Course> Courses, IEnumerable<Subscription> Subscriptions)> GetAllAsync(string username)
        {
            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == username);

            IEnumerable<Subscription> subscriptions = Enumerable.Empty<Subscription>();

            if (user != null)
            {
                subscriptions = await _unitOfWork.Subscriptions.GetAllAsync(s =>
                    s.MemberId == user.Id && s.SubscribedInType == SubscriptionType.Course);
            }

            var courses = await _unitOfWork.Courses.GetAllAsync(c => c.Department, c => c.Trainer);

            return (courses, subscriptions);
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Courses.GetByIdAsync(id);
        }

        public async Task AddAsync(Course Course)
        {
            await _unitOfWork.Courses.AddAsync(Course);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(Course Course)
        {
            _unitOfWork.Courses.Update(Course);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Courses.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Courses.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public bool Exists(int id)
        {
            return _unitOfWork.Courses.Table.Any(a => a.Id == id);
        }
    }
}
