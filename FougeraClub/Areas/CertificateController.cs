using Application.Helpers;
using Application.Interfaces.Admin;
using AutoMapper;
using DocumentFormat.OpenXml.Presentation;
using Domain.Enums;
using Domain.Resources;
using KhaledTeamRecycling.Areas.Member.ViewModels;
using KhaledTeamRecycling.Helpers;
using KhaledTeamRecycling.Hub;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Attributes;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;
using KhaledTeamRecycling.Areas.Admin.ViewModels.Course;

namespace KhaledTeamRecycling.Areas
{
    [Route("[controller]/[action]")]
    public class CertificateController : Controller
    {
        #region properties
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;


        #endregion

        #region constructor
        public CertificateController(IUnitOfWork UnitOfWork, IHttpContextAccessor httpContextAccessor,IServiceProvider serviceProvider, IMapper mapper
            
            )
        {
            _unitOfWork = UnitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }
        #endregion


        #region actions
       
        [YesGet]
        public async Task<IActionResult> PrintCertificate(int subscriptionId)
        {
            var config = _serviceProvider.GetRequiredService<IConfiguration>();
            var CertificateURLWebsite = config["CertificateURL:BaseUrl"]; // must be set in appsettings.json or secrets

            var model = new Admin.ViewModels.Course.CertificateVM();
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
            var CertificateModel = new CertificateVM();
            if (string.IsNullOrEmpty(serialHashed))
            {
                CertificateModel.IsValid = false;
                return View(CertificateModel);
            }
            string originalSerialHashed = Uri.UnescapeDataString(serialHashed); // retrive + , % وهكذا 
            var serialDecrypted = HashHelper.Decrypt(originalSerialHashed);

            CertificateModel.IsValid = true;

            return View(CertificateModel);
        }

        #endregion
    }
}
