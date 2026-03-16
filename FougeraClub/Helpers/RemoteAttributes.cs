using Infrastructure.Repositories.InterfacesDB;
using System.Linq.Expressions;

namespace KhaledTeamRecycling.Helpers
{
    public class RemoteAttributes
    {
        private readonly IUnitOfWork _unitOfWork;
        public RemoteAttributes(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool IsUnique<T>(string propertyName, object value, object? id = null) where T : class
        {
            var repository = _unitOfWork.GetRepository<T>();
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);

            Expression finalExpression = equality;

            // ✅ Check if id is provided and valid
            if (id != null)
            {
                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty != null)
                {
                    var idPropExpression = Expression.Property(parameter, idProperty);
                    var idConstant = Expression.Constant(id);
                    var notEqualId = Expression.NotEqual(idPropExpression, idConstant);

                    finalExpression = Expression.AndAlso(equality, notEqualId);
                }
            }


            var lambda = Expression.Lambda<Func<T, bool>>(finalExpression, parameter);
            return !repository.Any(lambda);
        }

    }
}
