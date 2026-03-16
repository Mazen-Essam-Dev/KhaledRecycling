using Domain.Resources;
using KhaledTeamRecycling.Helpers;
using Google.Protobuf.Collections;
using Infrastructure.Repositories.InterfacesDB;
using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace KhaledTeamRecycling.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UniqueAttribute : ValidationAttribute
    {
        private readonly Type _entityType;
        private readonly string _propertyName;

        public UniqueAttribute(Type entityType, string propertyName)
        {
            _entityType = entityType;
            _propertyName = propertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            // Get services
            var unitOfWork = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork))!;
            var remoteHelper = new RemoteAttributes(unitOfWork);

            // Get the Id of the current record (if exists)
            object? id = null;
            var idProp = validationContext.ObjectType.GetProperty("Id");
            if (idProp != null)
            {
                var idValue = idProp.GetValue(validationContext.ObjectInstance);
                if (idValue != null) id = idValue;
            }

            // Reference to IsUnique<T>() method --> Call your existing helper
            var method = typeof(RemoteAttributes)
                .GetMethod(nameof(RemoteAttributes.IsUnique))
                ?.MakeGenericMethod(_entityType);

            // 1️⃣ First check → original value
            bool isUniqueOriginal = (bool)method!.Invoke(remoteHelper,
                new object?[] { _propertyName, value, id })!;

            // ❌ if the original is NOT unique → return error immediately
            if (!isUniqueOriginal)
                return new ValidationResult(ErrorMessage);

            // 2️⃣ If original is unique → prepend 971 and check again
            string newValue = "971" + value.ToString()?.Trim();

            bool isUniqueWith971 = (bool)method!.Invoke(remoteHelper,
                new object?[] { _propertyName, newValue, id })!;

            // ❌ if 971 + value is NOT unique → also error
            if (!isUniqueWith971)
                return new ValidationResult(ErrorMessage);

            // ✔ Both checks passed
            return ValidationResult.Success;
        }
    }
}
