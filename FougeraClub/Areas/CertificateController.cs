using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Interfaces.Member;
using AutoMapper;
using DocumentFormat.OpenXml.Presentation;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Member.ViewModels;
using FougeraClub.Helpers;
using FougeraClub.Hub;
using FougeraClub.Middelware;
using Infrastructure.Attributes;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace FougeraClub.Areas
{
    [Route("[controller]/[action]")]
    public class CertificateController : Controller
    {
        #region properties
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Application.Interfaces.Member.ICourseService _courseService;
        private readonly Application.Interfaces.Admin.ICourseService _courseAdminService;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;


        #endregion

        #region constructor
        public CertificateController(IUnitOfWork UnitOfWork, IHttpContextAccessor httpContextAccessor,IServiceProvider serviceProvider, Application.Interfaces.Member.ICourseService courseService, IMapper mapper
            , Application.Interfaces.Admin.ICourseService courseAdminService
            )
        {
            _unitOfWork = UnitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _courseService = courseService;
            _mapper = mapper;
            _courseAdminService = courseAdminService;
            _serviceProvider = serviceProvider;
        }
        #endregion


        #region actions
       
        [YesGet]
        public async Task<IActionResult> PrintCertificate(int subscriptionId)
        {
            var config = _serviceProvider.GetRequiredService<IConfiguration>();
            var CertificateURLWebsite = config["CertificateURL:BaseUrl"]; // must be set in appsettings.json or secrets

            var data = await _courseAdminService.GetCertificateData(subscriptionId);
            var model = _mapper.Map<Admin.ViewModels.Course.CertificateVM>(data);
            var certificateSerialHashed = HashHelper.Encrypt(model.CertificateSerial??"0");
            string encodedCertificateSerialHashed = Uri.EscapeDataString(certificateSerialHashed); // save + , % وهكذا 
            model.CertificateSerialHashed = encodedCertificateSerialHashed;
            var url = CertificateURLWebsite + "/Certificate/CertificateVerified?serialHashed=" + encodedCertificateSerialHashed;
            // Change to Remote URL
            var qrCode = QrCodeHelper.GenerateQrBase64(url);
            model.QrCodeBase64 = qrCode;
            return View(model);
        }


        [YesGet]
        public async Task<IActionResult> CertificateVerified(string? serialHashed)
        {
            var CertificateModel = new Admin.ViewModels.Course.CertificateVM();
            if (string.IsNullOrEmpty(serialHashed))
            {
                CertificateModel.IsValid = false;
                return View(CertificateModel);
            }
            string originalSerialHashed = Uri.UnescapeDataString(serialHashed); // retrive + , % وهكذا 
            var serialDecrypted = HashHelper.Decrypt(originalSerialHashed);
            var dataCertificateExisted = await _unitOfWork.Subscriptions.Table.Where(x=>x.SubscribedInType==SubscriptionType.Course && x.CertificateSerial== serialDecrypted).FirstOrDefaultAsync();
            if(dataCertificateExisted == null)
            {
                CertificateModel.IsValid = false;
                return View(CertificateModel);
            }
            CertificateModel.IsValid = true;
            CertificateModel.Id = dataCertificateExisted.Id;

            return View(CertificateModel);
        }

        #endregion
    }
}
