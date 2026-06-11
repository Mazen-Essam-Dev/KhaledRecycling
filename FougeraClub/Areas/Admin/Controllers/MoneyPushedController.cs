using Application.Helpers;
using Application.Interfaces.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using KhaledTeamRecycling.Areas.Admin.ViewModels.MoneyPushed;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Services.Admin;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class MoneyPushedController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MoneyPushedController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private static string? ShortenNotes(string? notes)
        {
            return notes?.Length > 100 ? notes.Substring(0, 100) + "....." : notes;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 50)
        {
            var query = from mp in _unitOfWork.MoneyPusheds.Table
                        join u in _unitOfWork.Users.Table on mp.FKUserId equals u.Id into userGroup
                        from u in userGroup.DefaultIfEmpty()
                        select new MoneyPushedItemDTO
                        {
                            Id = mp.Id,
                            FKUserId = mp.FKUserId,
                            UserName = u != null ? u.FullNameAr : "مستخدم غير معروف",
                            Money = mp.Money,
                            TypeTransaction = mp.TypeTransaction,
                            ItemName = mp.ItemName,
                            Notes = ShortenNotes(mp.Notes),
                            CreatedDate = mp.CreatedDate,
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x => x.UserName != null && x.UserName.Contains(searchTerm));
            }

            var totalRecords = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new MoneyPushedVM
            {
                Items = items,
                SearchString = searchTerm,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                HasNextPage = totalRecords > pageSize * page,
                HasPreviousPage = page > 1
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", vm);
            }

            return View(vm);
        }

        [YesGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            if (!id.HasValue || id.Value == 0)
            {
                return View(new MoneyPushedVM());
            }

            var entity = await _unitOfWork.MoneyPusheds.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }

            var vm = new MoneyPushedVM
            {
                Id = entity.Id,
                FKUserId = entity.FKUserId,
                Money = entity.Money,
                TypeTransaction = entity.TypeTransaction,
                ItemName = entity.ItemName,
                Notes = entity.Notes,
                CreatedDate = entity.CreatedDate
            };
            
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(MoneyPushedVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (model.Id == 0)
            {
                var entity = new MoneyPushed
                {
                    FKUserId = loggedInUserId,
                    Money = model.Money,
                    TypeTransaction = model.TypeTransaction,
                    ItemName = model.ItemName,
                    Notes = model.Notes,
                    CreatedDate = DateTime.Now
                };
                await _unitOfWork.MoneyPusheds.AddAsync(entity);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var entity = await _unitOfWork.MoneyPusheds.GetByIdAsync(model.Id);
                if (entity != null)
                {
                    entity.Money = model.Money;
                    entity.TypeTransaction = model.TypeTransaction;
                    entity.ItemName = model.ItemName;
                    entity.Notes = model.Notes;
                    // CreatedDate and FKUserId should remain unchanged for existing records
                    
                    _unitOfWork.MoneyPusheds.Update(entity);
                    await _unitOfWork.CompleteAsync();
                }
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var entity = await _unitOfWork.MoneyPusheds.GetByIdAsync(id);
            if (entity != null)
            {
                 _unitOfWork.MoneyPusheds.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm)
        {
            var query = from mp in _unitOfWork.MoneyPusheds.Table
                        join u in _unitOfWork.Users.Table on mp.FKUserId equals u.Id into userGroup
                        from u in userGroup.DefaultIfEmpty()
                        select new MoneyPushedItemDTO
                        {
                            Id = mp.Id,
                            FKUserId = mp.FKUserId,
                            UserName = u != null ? u.FullNameAr : "مستخدم غير معروف",
                            Money = mp.Money,
                            TypeTransaction = mp.TypeTransaction,
                            ItemName = mp.ItemName,
                            Notes = ShortenNotes(mp.Notes),
                            CreatedDate = mp.CreatedDate
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x => x.UserName != null && x.UserName.Contains(searchTerm));
            }
            var items = await query.ToListAsync();

            var vm = new MoneyPushedVM
            {
                Items = items,
                SearchString = searchTerm,
                TotalCount = items.Count
            };

            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm)
        {
            var query = from mp in _unitOfWork.MoneyPusheds.Table
                        join u in _unitOfWork.Users.Table on mp.FKUserId equals u.Id into userGroup
                        from u in userGroup.DefaultIfEmpty()
                        select new MoneyPushedItemDTO
                        {
                            Id = mp.Id,
                            FKUserId = mp.FKUserId,
                            UserName = u != null ? u.FullNameAr : "مستخدم غير معروف",
                            Money = mp.Money,
                            TypeTransaction = mp.TypeTransaction,
                            ItemName = mp.ItemName,
                            Notes = ShortenNotes(mp.Notes),
                            CreatedDate = mp.CreatedDate
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x => x.UserName != null && x.UserName.Contains(searchTerm));
            }
            var items = await query.ToListAsync();
            
            var list = items.ToList();

            if (!list.Any())
            {
                return RedirectToAction(nameof(Index), new { searchTerm });
            }

            var titles = new List<string> { "اسم المستخدم", "المبلغ", "نوع المعاملة", "اسم الصنف", "ملاحظات", "تاريخ الإنشاء" };
            var excelData = list.Select(x => new ExcelDataDTO
            {
                t1 = x.UserName ?? string.Empty,
                t2 = x.Money?.ToString("0.00") ?? string.Empty,
                t3 = (x.TypeTransaction == '+' ? "إيداع" : (x.TypeTransaction == '-' ? "سحب" : x.TypeTransaction?.ToString())) ?? string.Empty,
                t4 = x.ItemName ?? string.Empty,
                t5 = x.Notes ?? string.Empty,
                t6 = x.CreatedDate?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty
            }).ToList();


            var (ok, bytes) = ExcelStaticReport.ExcelReportArEn_(excelData, titles, 0, "ar");
            if (!ok || bytes == null || bytes.Length == 0)
            {
                return RedirectToAction(nameof(Index), new { searchTerm });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"MoneyPushed_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
