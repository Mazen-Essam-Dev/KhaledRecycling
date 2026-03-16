using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Interfaces.Member;
using AutoMapper;
using DocumentFormat.OpenXml.Vml.Office;
using Domain.DTOs.Member.Account;
using Domain.Entities;
using Domain.Enums;
using Domain.Resources;
using KhaledTeamRecycling.Areas.Member.ViewModels;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Net.Http.Headers;

namespace KhaledTeamRecycling.Areas.Member.Controllers
{
    [Area("Member")]
    [Route("Member/[controller]/[action]")]
    public class AccountController : Controller
    {
        #region Fields
        private readonly Application.Interfaces.Member.IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _imageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "members");
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IMemberService _memberService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion

        #region Constructor
        public AccountController(Application.Interfaces.Member.IAccountService accountService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper, IMemberService memberService, IWebHostEnvironment webHostEnvironment)
        {
            _accountService = accountService;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _memberService = memberService;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion

    

        #region Register
        public async Task<IActionResult> Register()
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var model = new MemberRegisterVM();
            model.GenderEnumList = SelectListHelper.GetEnumSelectList<Gender>();
            model.ProfessionEnumList = SelectListHelper.GetEnumSelectList<Profession>();
            model.HeardByEnumList = SelectListHelper.GetEnumSelectList<HeardBySources>();
            
            // Set default member type to Member
            model.MemberTypeId = (int)MemberTypeEnum.Member;

            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            //Select Only United Arab Emirates 
            var nationalitiesOne = nationalities.Where(n=> (!string.IsNullOrEmpty(n.NameAr) && n.NameAr.Contains("مارات", StringComparison.OrdinalIgnoreCase)));
            model.NationalityId = nationalities.FirstOrDefault()?.Id;

            model.Nationalities = SelectListHelper.BindSelectList(nationalities.ToList(), model.NationalityId).ToList();
            model.NationalitiesOne = SelectListHelper.BindSelectList(nationalitiesOne.ToList(), model.NationalityId).ToList();

            var cities = await _unitOfWork.Cities.GetAllAsync();
            if (cities != null && cities.Count() > 0) cities = cities.OrderBy(x => x.Id);
            var citySharqa = await _unitOfWork.Cities.GetAllAsync(x => x.NameAr != null && x.NameAr.Contains("فجير"));
            var sharqaId = citySharqa.FirstOrDefault()?.Id;
            model.Cities = SelectListHelper.BindSelectList(cities.ToList(), model.CityId ?? sharqaId).ToList();
            model.Code = await _memberService.GenerateNewCode();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MemberRegisterVM model)
        {
            #region binding Data
            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            //Select Only United Arab Emirates 
            var nationalitiesOne = nationalities.Where(n => (!string.IsNullOrEmpty(n.NameAr) && n.NameAr.Contains("مارات", StringComparison.OrdinalIgnoreCase)));
            //model.NationalityId = nationalities.FirstOrDefault()?.Id;

            model.Nationalities = SelectListHelper.BindSelectList(nationalities.ToList(), model.NationalityId).ToList();
            model.NationalitiesOne = SelectListHelper.BindSelectList(nationalitiesOne.ToList(), model.NationalityId).ToList();

            var cities = await _unitOfWork.Cities.GetAllAsync();
            if (cities != null && cities.Count() > 0) cities = cities.OrderBy(x => x.Id);
            var citySharqa = await _unitOfWork.Cities.GetAllAsync(x => x.NameAr != null && x.NameAr.Contains("فجير"));
            var sharqaId = citySharqa.FirstOrDefault()?.Id;
            model.Cities = SelectListHelper.BindSelectList(cities.ToList(), model.CityId ?? sharqaId).ToList();
            #endregion binding Data

            #region Save And Get 3 Images From Sesssion and Temp

            var filesDto = _mapper.Map<FilesDTO>(model);
            filesDto = await _accountService.SaveImagesInSession(filesDto);
            _mapper.Map(filesDto, model);

            filesDto = _mapper.Map<FilesDTO>(model);
            filesDto = _accountService.MoveImagesFromSessionToModel(filesDto);
            _mapper.Map(filesDto, model);

            if (model.PhoneNumber != null && model.PhoneNumber.StartsWith("971")) // Is PhoneNumber StartsWith 971 Remove it
                model.PhoneNumber = model.PhoneNumber.Substring(3);
            if (model.FatherPhone != null && model.FatherPhone.StartsWith("971")) // Is FatherPhone StartsWith 971 Remove it
                model.FatherPhone = model.FatherPhone.Substring(3);
            if (model.MotherPhone != null && model.MotherPhone.StartsWith("971")) // Is MotherPhone StartsWith 971 Remove it
                model.MotherPhone = model.MotherPhone.Substring(3);

            IFormFile? IdImage = model.IdImage;
            if (IdImage == null && !string.IsNullOrEmpty(model.IdImagePath))
            {
                // Get the full physical path
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, model.IdImagePath.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                {
                    var fileName = Path.GetFileName(fullPath);
                    var memoryStream = new MemoryStream(await System.IO.File.ReadAllBytesAsync(fullPath));

                    // Create header with content-disposition info
                    var header = new HeaderDictionary
                    {
                        [HeaderNames.ContentDisposition] = $"form-data; name=\"IdImage\"; filename=\"{fileName}\"",
                        [HeaderNames.ContentType] = "image/png" // you can detect dynamically if needed
                    };

                    IdImage = new FormFile(memoryStream, 0, memoryStream.Length, "IdImage", fileName)
                    {
                        Headers = header,
                        ContentType = "image/png"
                    };

                    model.IdImage = IdImage;
                }
            }

            IFormFile? PassportImage = model.PassportImage;
            if (PassportImage == null && !string.IsNullOrEmpty(model.PassportImagePath))
            {
                // Get the full physical path
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, model.PassportImagePath.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                {
                    var fileName = Path.GetFileName(fullPath);
                    var memoryStream = new MemoryStream(await System.IO.File.ReadAllBytesAsync(fullPath));

                    // Create header with content-disposition info
                    var header = new HeaderDictionary
                    {
                        [HeaderNames.ContentDisposition] = $"form-data; name=\"PassportImage\"; filename=\"{fileName}\"",
                        [HeaderNames.ContentType] = "image/png" // you can detect dynamically if needed
                    };

                    PassportImage = new FormFile(memoryStream, 0, memoryStream.Length, "PassportImage", fileName)
                    {
                        Headers = header,
                        ContentType = "image/png"
                    };

                    model.PassportImage = PassportImage;
                }
            }

            IFormFile? ProfileImage = model.ProfileImage;
            if (ProfileImage == null && !string.IsNullOrEmpty(model.ProfileImagePath))
            {
                // Get the full physical path
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, model.ProfileImagePath.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                {
                    var fileName = Path.GetFileName(fullPath);
                    var memoryStream = new MemoryStream(await System.IO.File.ReadAllBytesAsync(fullPath));

                    // Create header with content-disposition info
                    var header = new HeaderDictionary
                    {
                        [HeaderNames.ContentDisposition] = $"form-data; name=\"ProfileImage\"; filename=\"{fileName}\"",
                        [HeaderNames.ContentType] = "image/png" // you can detect dynamically if needed
                    };

                    ProfileImage = new FormFile(memoryStream, 0, memoryStream.Length, "ProfileImage", fileName)
                    {
                        Headers = header,
                        ContentType = "image/png"
                    };

                    model.ProfileImage = ProfileImage;
                }
            }
            #endregion Save And Get 3 Images From Sesssion and Temp

            #region Validation 3 Images And Size
            if (model.PassportImage?.Length == model.IdImage?.Length && model.IdImage?.Length > 1) { ModelState.AddModelError("PassportImage", Resource1.UploadPassportImageDiffrent); ModelState.AddModelError("IdImage", Resource1.UploadIDCardDiffrent); }

            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            var ProfileImage_Text = await FileHelper.CheckFileIsImage_3Mg_Async(model.ProfileImage);
            if (ProfileImage_Text != "OK" && ProfileImage_Text != "null") ModelState.AddModelError("ProfileImage", ProfileImage_Text);
            var IdImage_Text = await FileHelper.CheckFileIsImage_3Mg_Async(model.IdImage);
            if (IdImage_Text != "OK" && IdImage_Text != "null") ModelState.AddModelError("IdImage", IdImage_Text);
            var PassportImage_Text = await FileHelper.CheckFileIsImage_3Mg_Async(model.PassportImage);
            if (PassportImage_Text != "OK" && PassportImage_Text != "null") ModelState.AddModelError("PassportImage", PassportImage_Text);


            if (model.ProfileImage==null || model.ProfileImage.Length <2 ) ModelState.AddModelError("ProfileImage", Resource1.Required);
            if (model.IdImage == null || model.IdImage.Length < 2) ModelState.AddModelError("IdImage", Resource1.Required);
            if (model.PassportImage == null || model.PassportImage.Length < 2) ModelState.AddModelError("PassportImage", Resource1.Required);

            #endregion Validation 3 Images And Size

            // Ensure folder exists
            Directory.CreateDirectory(_imageSavePath);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            filesDto = _mapper.Map<FilesDTO>(model);
            filesDto = _accountService.MoveImagesFromSessionToModel(filesDto);
            _mapper.Map(filesDto, model);

            var member = _mapper.Map<MemberEntity>(model);

            //#region Validation If Extracted Data == Input Data
            //if (model.IdImagePath == null && model.PassportImagePath == null)
            //{
            //    ModelState.AddModelError("PassportImage", Resource1.UploadPassportImage);
            //    ModelState.AddModelError("IdImage", Resource1.UploadIDCard);
            //    return View(model);
            //}
            //if (model.IdImagePath == null)
            //{
            //    ModelState.AddModelError("IdImage", Resource1.UploadIDCard);
            //    return View(model);
            //}
            //if (model.PassportImagePath == null)
            //{
            //    ModelState.AddModelError("PassportImage", Resource1.UploadPassportImage);
            //    return View(model);
            //}
            //var result = await IDClassification_Json(model.IdImage) as JsonResult;
            //var iDCardExtractedDataVM = result.Value as IDCardExtractedDataVM;

            //// You can use vm here
            //if (iDCardExtractedDataVM?.doneOCR_bool == true)
            //{
            //    MemberDTO memberDTO = _mapper.Map<MemberDTO>(model);
            //    IDCardExtractedDataDTO iDCardExtractedDataDTO = _mapper.Map<IDCardExtractedDataDTO>(iDCardExtractedDataVM);
            //    var (Compaire_percentage_FullEnName, Compaire_percentage_FullArName, Compaire_percentage_IDNumber, Compaire_percentage_BirthDate, Compaire_percentage_ExpiryDate, VM_ExpiryDate_DT) = await _memberService.ValidationCompareAllInputsToExtractedAsync(memberDTO, iDCardExtractedDataDTO);

            //    if (Compaire_percentage_FullEnName > 65 && Compaire_percentage_FullArName > 55 && Compaire_percentage_IDNumber > 99 && Compaire_percentage_BirthDate > 99 && Compaire_percentage_ExpiryDate > 99)
            //    {
            //        iDCardExtractedDataVM.doneValidation_bool = true;
            //    }
            //    else
            //    {
            //        if (Compaire_percentage_FullEnName <= 65) ModelState.AddModelError("FullNameEn", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
            //        if (Compaire_percentage_FullArName <= 55) ModelState.AddModelError("FullNameAr", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
            //        if (Compaire_percentage_IDNumber <= 99) ModelState.AddModelError("IdNumber", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
            //        if (Compaire_percentage_BirthDate <= 99) ModelState.AddModelError("DateOfBirth", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
            //        if (Compaire_percentage_ExpiryDate <= 99) ModelState.AddModelError("IdExpiryDate", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
            //        return View(model);
            //    }
            //}
            //else
            //{
            //    ModelState.AddModelError("IdImage", iDCardExtractedDataVM?.DoneTextExtracted_Error_Str ?? " ");
            //    return View(model);
            //}
            //#endregion

            // //PhoneNumber Dubai
            member.PhoneNumber = await PhoneHelper.CheckAndDoPhoneStart971(member.PhoneNumber);

            // //FatherPhone Dubai
            member.FatherPhone = await PhoneHelper.CheckAndDoPhoneStart971(member.FatherPhone);

            // //MotherPhone Dubai
            member.MotherPhone = await PhoneHelper.CheckAndDoPhoneStart971(member.MotherPhone);


            var (success, textValue) = await _accountService.RegisterAsync(member);

            if (!success)
            {
                if (textValue == "EmailUsedBefore")
                    ModelState.AddModelError("Email", Resource1.EmailAlreadyExists);
                else if (textValue == "IdNationalNumber_UsedBefore")
                    ModelState.AddModelError("IdNumber", Resource1.IdNationalNumber_UsedBefore);
                else if (textValue == "EmailUsedBefore_&&_IdNationalNumber_UsedBefore")
                { ModelState.AddModelError("IdNumber", Resource1.IdNationalNumber_UsedBefore); ModelState.AddModelError("Email", Resource1.EmailAlreadyExists); }
                else
                    ModelState.AddModelError("", Resource1.UsernameAlreadyExists);

                return View(model);
            }
            else
            {
                _httpContextAccessor.HttpContext?.Session.Remove("ProfileImagePath");
                _httpContextAccessor.HttpContext?.Session.Remove("IdImagePath");
                _httpContextAccessor.HttpContext?.Session.Remove("PassportImagePath");

                #region From Resgister Success To Logged in
                var member2 = new MemberEntity
                {
                    Email = model.Email,
                    Password = model.Password
                };
                var (user, stringStatus) = await _accountService.ValidateUserAsync(member2);
                if (user == null)
                {
                    if (stringStatus == "NotExisting")
                    {
                        ModelState.AddModelError("", Resource1.EmailNotExisting);
                        return View();
                    }
                    else if (stringStatus == "NotMatchingBassword")
                    {
                        ModelState.AddModelError("", Resource1.NotMatchingBassword);
                        return View();
                    }
                    ModelState.AddModelError("", Resource1.InvalidCredentials);
                    return View();
                }
                if (user.Suspended)
                {
                    ModelState.AddModelError("", Resource1.AccountSuspended);
                    return View();
                }
                HttpContext.Session.SetString("Email", user.Email);

                // 🟢 Generate token and store it
                var token = Guid.NewGuid().ToString(); // or use SHA256
                HttpContext.Session.SetString("AuthToken", token);
                return RedirectToAction("Index", "Home");

                #endregion From Resgister Success To Logged in

                //return RedirectToAction("Login");
            }

        }

        [NoLogging]
        [IgnoreAction]
        [HttpPost]
        public IActionResult DeleteTempImages()
        {
            var success = _accountService.DeleteImagesFromSession();
            return Json(new { success });
        }


        #endregion

        #region login & logout
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AuthToken") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", Resource1.InvalidCredentials);
                return View();
            }
            var member = new MemberEntity
            {
                Email = model.Email,
                Password = model.Password
            };
            var (user, stringStatus) = await _accountService.ValidateUserAsync(member);
            if (user == null)
            {
                if (stringStatus == "NotExisting")
                {
                    ModelState.AddModelError("", Resource1.EmailNotExisting);
                    return View();
                }
                else if (stringStatus == "NotMatchingBassword")
                {
                    ModelState.AddModelError("", Resource1.NotMatchingBassword);
                    return View();
                }
                ModelState.AddModelError("", Resource1.InvalidCredentials);
                return View();
            }
            if (user.Suspended)
            {
                ModelState.AddModelError("", Resource1.AccountSuspended);
                return View();
            }
            HttpContext.Session.SetString("Email", user.Email);

            // 🟢 Generate token and store it
            var token = Guid.NewGuid().ToString(); // or use SHA256
            HttpContext.Session.SetString("AuthToken", token);

            return RedirectToAction("Index", "Home");
        }
        [YesGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        #endregion

        #region edit profile
        public async Task<IActionResult> Edit()
        {
            var session = _httpContextAccessor?.HttpContext?.Session;
            var email = session?.GetString("Email");
            if (email == null)
                return NotFound();

            var member = await _accountService.GetMemberByEmailAsync(email);

            if (member == null)
                return NotFound();
            var vm = _mapper.Map<MemberVM>(member);
            vm.Password = string.Empty;
            vm.ConfirmPassword = string.Empty;


            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            ////Select Only United Arab Emirates 
            //nationalities = nationalities.Where(n => (!string.IsNullOrEmpty(n.NameAr) && n.NameAr.Contains("مارات", StringComparison.OrdinalIgnoreCase)));
            //vm.NationalityId = nationalities.FirstOrDefault()?.Id;

            var cities = await _unitOfWork.Cities.GetAllAsync();
            if (cities != null && cities.Count() > 0) cities = cities.OrderBy(x => x.Id);

            vm.NationalitiesList = SelectListHelper.BindSelectList(nationalities.ToList(), vm.NationalityId).ToList();
            var citySharqa = await _unitOfWork.Cities.GetAllAsync(x => x.NameAr != null && x.NameAr.Contains("فجير"));
            var sharqaId = citySharqa.FirstOrDefault()?.Id;
            vm.CitiesList = SelectListHelper.BindSelectList(cities.ToList(), vm.CityId ?? sharqaId).ToList();
            vm.GenderEnumList = SelectListHelper.GetEnumSelectList<Gender>();
            vm.ProfessionEnumList = SelectListHelper.GetEnumSelectList<Profession>();
            vm.HeardByEnumList = SelectListHelper.GetEnumSelectList<HeardBySources>();

            if (vm.PhoneNumber != null && vm.PhoneNumber.StartsWith("971")) // Is PhoneNumber StartsWith 971 Remove it
                vm.PhoneNumber = vm.PhoneNumber.Substring(3);
            if (vm.FatherPhone != null && vm.FatherPhone.StartsWith("971")) // Is FatherPhone StartsWith 971 Remove it
                vm.FatherPhone = vm.FatherPhone.Substring(3);
            if (vm.MotherPhone != null && vm.MotherPhone.StartsWith("971")) // Is MotherPhone StartsWith 971 Remove it
                vm.MotherPhone = vm.MotherPhone.Substring(3);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MemberVM model)
        {
            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            ////Select Only United Arab Emirates 
            //nationalities = nationalities.Where(n => (!string.IsNullOrEmpty(n.NameAr) && n.NameAr.Contains("مارات", StringComparison.OrdinalIgnoreCase)));
            //model.NationalityId = nationalities.FirstOrDefault()?.Id;

            var cities = await _unitOfWork.Cities.GetAllAsync();
            if (cities != null && cities.Count() > 0) cities = cities.OrderBy(x => x.Id);

            model.NationalitiesList = SelectListHelper.BindSelectList(nationalities.ToList(), model.NationalityId).ToList();
            var citySharqa = await _unitOfWork.Cities.GetAllAsync(x=> x.NameAr!=null && x.NameAr.Contains("فجير"));
            var sharqaId = citySharqa.FirstOrDefault()?.Id;
            model.CitiesList = SelectListHelper.BindSelectList(cities.ToList(), model.CityId??sharqaId).ToList();
            model.GenderEnumList = SelectListHelper.GetEnumSelectList<Gender>();
            model.ProfessionEnumList = SelectListHelper.GetEnumSelectList<Profession>();
            model.HeardByEnumList = SelectListHelper.GetEnumSelectList<HeardBySources>();

            if (model.PhoneNumber != null && model.PhoneNumber.StartsWith("971")) // Is PhoneNumber StartsWith 971 Remove it
                model.PhoneNumber = model.PhoneNumber.Substring(3);
            if (model.FatherPhone != null && model.FatherPhone.StartsWith("971")) // Is FatherPhone StartsWith 971 Remove it
                model.FatherPhone = model.FatherPhone.Substring(3);
            if (model.MotherPhone != null && model.MotherPhone.StartsWith("971")) // Is MotherPhone StartsWith 971 Remove it
                model.MotherPhone = model.MotherPhone.Substring(3);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ProfileImage_Text = await FileHelper.CheckFileIsImage_3Mg_Async(model.ProfileImage);
            if (ProfileImage_Text != "OK" && ProfileImage_Text != "null") ModelState.AddModelError("ProfileImage", ProfileImage_Text);
            var IdImage_Text = await FileHelper.CheckFileIsImage_3Mg_Async(model.IdImage);
            if (IdImage_Text != "OK" && IdImage_Text != "null") ModelState.AddModelError("IdImage", IdImage_Text);
            var PassportImage_Text = await FileHelper.CheckFileIsImage_3Mg_Async(model.PassportImage);
            if (PassportImage_Text != "OK" && PassportImage_Text != "null") ModelState.AddModelError("PassportImage", PassportImage_Text);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = _mapper.Map<MemberEntity>(model);

            // //PhoneNumber Dubai
            entity.PhoneNumber = await PhoneHelper.CheckAndDoPhoneStart971(entity.PhoneNumber);

            // //FatherPhone Dubai
            entity.FatherPhone = await PhoneHelper.CheckAndDoPhoneStart971(entity.FatherPhone);

            // //MotherPhone Dubai
            entity.MotherPhone = await PhoneHelper.CheckAndDoPhoneStart971(entity.MotherPhone);


            await _memberService.UpdateAsync(entity);

            return RedirectToAction("Edit");
        }
        #endregion
        [YesGet]
        public async Task<IActionResult> PrintDetails(int? id)
        {
            var vm = new MemberVM();

            if (id.HasValue && id.Value != 0)
            {
                var member = await _memberService.GetByIdAsync(id.Value);
                if (member == null) return NotFound();
                vm = _mapper.Map<MemberVM>(member);
            }

            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            var cities = await _unitOfWork.Cities.GetAllAsync();
            if (cities != null && cities.Count() > 0) cities = cities.OrderBy(x => x.Id);


            vm.NationalitiesList = SelectListHelper.BindSelectList(nationalities.ToList(), vm.NationalityId).ToList();
            vm.CitiesList = SelectListHelper.BindSelectList(cities.ToList(), vm.CityId).ToList();
            ViewBag.Professions = SelectListHelper.GetEnumSelectList<Profession>().Distinct();
            ViewBag.Genders = SelectListHelper.GetEnumSelectList<Gender>().Distinct();
            ViewBag.HeardBySources = SelectListHelper.GetEnumSelectList<HeardBySources>().Distinct();

            return View(vm);
        }

    }


}
