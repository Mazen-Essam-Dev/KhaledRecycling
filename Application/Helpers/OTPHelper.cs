using Application.Interfaces.Admin;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Domain.Entities.SMS;
using Domain.Resources;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Http;

namespace Application.Helpers
{

    public static class OTPHelper
    {
        public static async Task<(bool,string)> SaveOtpAsync(this IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
                var code = new Random().Next(1000, 9999).ToString();
                //var code = "1111";
                var otpCodeHashed = HashHelper.ComputeSha256Hash(code);

                var entity = new OTP
                {
                    Code = otpCodeHashed,
                    CreateAt = AppDubaiTime.Now,
                    ExpireAt = AppDubaiTime.Now.AddMinutes(5),
                    UserId = userId
                };

                await _unitOfWork.OTPs.AddAsync(entity);
                await _unitOfWork.CompleteAsync();
                Console.WriteLine("OTP Code: " + code);
                return (true, code);
            }
            catch (Exception ex)
            {
                return (false, " ");
            }
        }

        public static async Task<bool> ValidateOtpAsync(this IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork, string code)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.GetUserId();

                var correctCode = new string(code.Reverse().ToArray());
                var otpCodeHashed = HashHelper.ComputeSha256Hash(correctCode);

                var otps = await _unitOfWork.OTPs.GetAllAsync();
                var otpData = otps.Where(o => o.Code == otpCodeHashed && !o.IsUsed).OrderByDescending(o => o.Id).FirstOrDefault();
                if (otpData == null)
                    return false;

                if (otpData != null && userId == otpData.UserId && AppDubaiTime.Now < otpData.ExpireAt && !otpData.IsUsed)
                {
                    var entity = new OTP
                    {
                        Id = otpData.Id,
                        Code = otpData.Code,
                        CreateAt = otpData.CreateAt,
                        ExpireAt = otpData.ExpireAt,
                        UserId = otpData.UserId,
                        IsUsed = true,
                    };


                    _unitOfWork.OTPs.UpdateValues(otpData, entity);
                    await _unitOfWork.CompleteAsync();

                    return true;
                }


                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
