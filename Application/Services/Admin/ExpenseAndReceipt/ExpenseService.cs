using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Interfaces.Admin.ExpenseAndReceipt;
using Azure.Core;
using DocumentFormat.OpenXml.Bibliography;
using Domain.DTOs.Admin.ExpenseAndReceipt;
using Domain.Entities.ExpenseAndReceipt;
using Domain.Enums;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin.ExpenseAndReceipt
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string FileName = "Expenses";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISMSService _SMSService;
        private readonly ISMSForSendingOTPService _SMSForSendingOTPService;
        private readonly decimal _beginningBalanceExpensesReportSeed;
        private readonly decimal _beginningBalanceExpenseAndReceiptReportSeed;


        public ExpenseService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ISMSService sMSService, ISMSForSendingOTPService sMSForSendingOTPService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _SMSService = sMSService;
            _SMSForSendingOTPService = sMSForSendingOTPService;
            _beginningBalanceExpensesReportSeed = configuration.GetValue<decimal?>("Finance:BeginningBalanceExpensesReport") ?? 50000m;
            _beginningBalanceExpenseAndReceiptReportSeed = configuration.GetValue<decimal?>("Finance:BeginningBalanceExpenseAndReceiptReport") ?? 100000m;
        }

        public async Task<string> GetLastSerialCode()
        {
            var lastSerial = await _unitOfWork.ReceivingReceipts.Table
               .Where(s => s.itemType == (int)ItemType.Expenses)
               .Select(x => x.CodeSerial)
               .Where(x => !string.IsNullOrEmpty(x))
               .Distinct()
               .OrderByDescending(x => x).LastOrDefaultAsync();

            int newCodeSerialInt;

            if (!int.TryParse(lastSerial, out newCodeSerialInt))
            {
                newCodeSerialInt = 0; // null or any string
            }

            newCodeSerialInt++;

            var newCodeSerialStr = newCodeSerialInt.ToString("0000");
            return newCodeSerialStr;
        }

        // Signature/OTP for Trainer and Manager
        public async Task<bool> SendOtpAsync()
        {
            var (status, code) = await OTPHelper.SaveOtpAsync(_httpContextAccessor, _unitOfWork);

            if (status == false) return false;

            var resultStatus = await _SMSForSendingOTPService.SendOtpSMSAsync(code);

            return resultStatus.Item1;
        }

        public async Task<(bool success, string? message)> ValidateOtp_OpenDetails_ExpensesReportAsync(int year, int month, string code, string role, System.Security.Claims.ClaimsPrincipal user)
        {
            var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);
            if (!success)
                return (false, "Invalid OTP");

            var userId = user.GetUserId();
            var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
            var latestSignature = allSignatures
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefault();
            if (latestSignature == null)
                return (false, "Signature not found");

            // Look for existing sign record by year, month, and ReportTypeId
            var report = await _unitOfWork.ExpensesAndReciptReportSigns.GetByColumnAsync(
                e => e.Year == year && e.Month == month && e.ReportTypeId == (int)ReportTypeEnum.ExpensesReport);

            if (role == "Acountant")
            {
                // Accountant creates new record if none exists
                if (report == null)
                {
                    report = new ExpensesAndReciptReportSign
                    {
                        ReportTypeId = (int)ReportTypeEnum.ExpensesReport,
                        Year = year,
                        Month = month,
                        AcountantSignatureId = latestSignature.Id
                    };
                    await _unitOfWork.ExpensesAndReciptReportSigns.AddAsync(report);
                }
                else
                {
                    report.AcountantSignatureId = latestSignature.Id;
                    _unitOfWork.ExpensesAndReciptReportSigns.Update(report);
                }
            }
            else if (role == "manager")
            {
                // Manager can only sign if record exists
                if (report == null)
                    return (false, "Report must be signed by accountant first");
                report.ManagerSignitureId = latestSignature.Id;
                _unitOfWork.ExpensesAndReciptReportSigns.Update(report);
            }
            else
                return (false, "Invalid role");

            await _unitOfWork.CompleteAsync();
            return (true, null);
        }
        public async Task<(bool success, string? message)> ValidateOtp_OpenDetails_ExpensesAndReciptReportAsync(int year, int month, string code, string role, System.Security.Claims.ClaimsPrincipal user)
        {
            var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);
            if (!success)
                return (false, "Invalid OTP");

            var userId = user.GetUserId();
            var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
            var latestSignature = allSignatures
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefault();
            if (latestSignature == null)
                return (false, "Signature not found");

            // Look for existing sign record by year, month, and ReportTypeId
            var report = await _unitOfWork.ExpensesAndReciptReportSigns.GetByColumnAsync(
                e => e.Year == year && e.Month == month && e.ReportTypeId == (int)ReportTypeEnum.ExpensesAndReceiptsReport);

            if (role == "Acountant")
            {
                // Accountant creates new record if none exists
                if (report == null)
                {
                    report = new ExpensesAndReciptReportSign
                    {
                        ReportTypeId = (int)ReportTypeEnum.ExpensesAndReceiptsReport,
                        Year = year,
                        Month = month,
                        AcountantSignatureId = latestSignature.Id
                    };
                    await _unitOfWork.ExpensesAndReciptReportSigns.AddAsync(report);
                }
                else
                {
                    report.AcountantSignatureId = latestSignature.Id;
                    _unitOfWork.ExpensesAndReciptReportSigns.Update(report);
                }
            }
            else if (role == "manager")
            {
                // Manager can only sign if record exists
                if (report == null)
                    return (false, "Report must be signed by accountant first");
                report.ManagerSignitureId = latestSignature.Id;
                _unitOfWork.ExpensesAndReciptReportSigns.Update(report);
            }
            else
                return (false, "Invalid role");

            await _unitOfWork.CompleteAsync();
            return (true, null);
        }

        public async Task<IEnumerable<ExpenseAndReceiptAndOther>> GetAllAsync()
        {
            return await _unitOfWork.ExpenseAndReceiptAndOther.GetAllAsync(ee => ee.ItemType == (int)ItemType.Expenses , e => e.BudgetItem, e => e.Supplier);
        }
        

        public async Task<ExpensesReportDTO> GetAllExpensesReportAsync(string lang, int year, int month)
        {
            // Dynamic calculation: compute beginning balance from seed + all prior months' chapter-one net, then compute running balances for the requested month
            var reportList = new List<ExpensesReportElementDTO>();

            // Seed beginning balance is a fixed fallback (do not read stored report rows). The opening balance is computed
            // purely from prior transactions plus this seed.
            decimal seed = _beginningBalanceExpensesReportSeed;

            // Sum net of all chapter-one (ExpensesSourceId == 2) transactions strictly before requested month
            var prevQuery = _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.Date.Year < year || (e.Date.Year == year && e.Date.Month < month));
            var prevTransactions = await prevQuery.ToListAsync();
            decimal netPrior = 0m;
            foreach (var t in prevTransactions.Where(t => t.ExpensesSourceId == 2))
            {
                if (t.ItemType == (int)ItemType.Expenses)
                    netPrior -= (t.Amount ?? 0) + (t.Vat ?? 0);
                else if (t.ItemType == (int)ItemType.Receipts)
                    netPrior += (t.Amount ?? 0) + (t.Vat ?? 0);
            }

            // Additionally include chapter-one receipts that belong to the requested month in the beginning balance
            // so their effect appears in the selected month's start and thus in its ending balance. These receipts
            // will not be listed in the month's expense rows (we only show expenses), so we avoid double-counting.
            var receiptsInMonthQuery = _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.Date.Year == year && e.Date.Month == month && e.ExpensesSourceId == 2 && e.ItemType == (int)ItemType.Receipts);
            var receiptsInMonth = await receiptsInMonthQuery.ToListAsync();
            decimal receiptsInMonthSum = receiptsInMonth.Sum(r => (r.Amount ?? 0) + (r.Vat ?? 0));

            decimal beginningBalance = seed + netPrior + receiptsInMonthSum;

            // Fetch month transactions (chapter-one only)
            var monthQuery = _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.Date.Year == year && e.Date.Month == month && e.ExpensesSourceId == 2);
            var monthTrans = await monthQuery.Include(e => e.BudgetItem).Include(e => e.Supplier).ToListAsync();

            var ordered = monthTrans.Where(e => e.ItemType == (int)ItemType.Expenses).OrderBy(e => e.Date).ThenBy(e => e.ItemNumber);
            decimal balance = beginningBalance;
            foreach (var expense in ordered)
            {
                var amountWithVat = (expense.Amount ?? 0) + (expense.Vat ?? 0);
                balance -= amountWithVat;
                reportList.Add(new ExpensesReportElementDTO
                {
                    ItemNumber = expense.ItemNumber ?? string.Empty,
                    Date = expense.Date,
                    Notes = expense.Notes,
                    SupplierName = expense.Supplier != null ? (lang == "ar" ? expense.Supplier.SupplierNameAr : expense.Supplier.SupplierNameEn) : "",
                    BudgetItem = expense.BudgetItem != null ? expense.BudgetItem.ItemTitle : string.Empty,
                    Amount = expense.Amount ?? 0,
                    Vat = expense.Vat ?? 0,
                    AmountWithVat = amountWithVat,
                    Balance = balance
                });
            }

            //New ExpensesReportDTO
            var getSign = await _unitOfWork.ExpensesAndReciptReportSigns.GetAllAsync(e => e.ReportTypeId == (int)ReportTypeEnum.ExpensesReport && e.Year == year && e.Month == month, x => x.AcountantSignature, x => x.ManagerSignature);
            var getSignSingle = getSign.FirstOrDefault();

            var report = new ExpensesReportDTO
            {
                BeginningBalance = beginningBalance,
                Month = month,
                Year = year,
                Expenses = reportList,
                EndingBalance = reportList.Any() ? reportList.Last().Balance : beginningBalance,
                AcountantSignatureId = getSignSingle?.AcountantSignatureId,
                AcountantSignature = getSignSingle?.AcountantSignature,
                ManagerSignitureId = getSignSingle?.ManagerSignitureId,
                ManagerSignature = getSignSingle?.ManagerSignature,
                Id = getSignSingle != null ? getSignSingle.Id : 0,
            };
            return report;
        }


        public async Task<ExpensesAndReceiptsReportDTO> GetAllExpenseAndReceiptReportAsync(string lang, int year, int month)
        {
            // Dynamic calculation: compute beginning balance from seed + net of all prior transactions (excluding chapter-one receipts),
            // then build running balance for the requested month.
            var reportList = new List<ExpensesAndReceiptsReportElementDTO>();

            // Seed beginning balance is a fixed fallback (do not read stored report rows). The opening balance is computed
            // purely from prior transactions plus this seed.
            decimal seed = _beginningBalanceExpenseAndReceiptReportSeed;

            // Sum net prior transactions (before selected month). For combined report,
            // include only expenses from sourceId == 1 and exclude chapter-one receipts (ExpensesSourceId == 2).
            var prevQuery = _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.Date.Year < year || (e.Date.Year == year && e.Date.Month < month));
            var prevTransactions = await prevQuery.ToListAsync();

            decimal netPrior = 0m;
            foreach (var t in prevTransactions)
            {
                if (t.ItemType == (int)ItemType.Expenses)
                {
                    if (t.ExpensesSourceId != 1) continue; // only sourceId == 1 expenses affect this combined report
                    netPrior -= (t.Amount ?? 0) + (t.Vat ?? 0);
                }
                else if (t.ItemType == (int)ItemType.Receipts)
                {
                    if (t.ExpensesSourceId == 2) continue; // chapter-one receipts excluded
                    netPrior += (t.Amount ?? 0);
                }
            }

            decimal beginningBalance = seed + netPrior;

            // Fetch current month transactions
            var monthQuery = _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.Date.Year == year && e.Date.Month == month);
            var monthTrans = await monthQuery.Include(e => e.Supplier).ToListAsync();

            // Build merged list
            var merged = new List<(DateOnly Date, int Type, int Seq, decimal Deposit, decimal Withdrawal, string? Notes, string? SupplierName)>();
            foreach (var exp in monthTrans)
            {
                if (exp.ItemType == (int)ItemType.Expenses)
                {
                    if (exp.ExpensesSourceId != 1) continue; // show only sourceId == 1 expenses in this combined report
                    var withdrawal = (exp.Amount ?? 0) + (exp.Vat ?? 0);
                    merged.Add((exp.Date, 0, exp.Id, 0m, withdrawal, exp.Notes,
                        exp.Supplier != null ? (lang == "ar" ? exp.Supplier.SupplierNameAr : exp.Supplier.SupplierNameEn) : ""));
                }
                else if (exp.ItemType == (int)ItemType.Receipts)
                {
                    if (exp.ExpensesSourceId == 2) continue; // skip chapter-one receipts
                    merged.Add((exp.Date, 1, exp.Id, exp.Amount ?? 0, 0m, exp.Notes,
                        exp.Supplier != null ? (lang == "ar" ? exp.Supplier.SupplierNameAr : exp.Supplier.SupplierNameEn) : ""));
                }
            }

            // Preserve original sequence (do not group by Type) — use merged order as retrieved from DB
            var ordered = merged.ToList();

            decimal balance = beginningBalance;
            foreach (var item in ordered)
            {
                if (item.Deposit > 0)
                    balance += item.Deposit;
                else if (item.Withdrawal > 0)
                    balance -= item.Withdrawal;

                reportList.Add(new ExpensesAndReceiptsReportElementDTO
                {
                    Date = item.Date,
                    Notes = item.Notes,
                    SupplierName = item.SupplierName,
                    Deposit = item.Deposit,
                    Withdrawal = item.Withdrawal,
                    Balance = balance
                });
            }

            //New ExpenseAndReceiptReportDTO
            var getSign = await _unitOfWork.ExpensesAndReciptReportSigns.GetAllAsync(e => e.ReportTypeId == (int)ReportTypeEnum.ExpensesAndReceiptsReport && e.Year == year && e.Month == month, x => x.AcountantSignature, x => x.ManagerSignature);
            var getSignSingle = getSign.FirstOrDefault();

            var report = new ExpensesAndReceiptsReportDTO
            {
                BeginningBalance = beginningBalance,
                Month = month,
                Year = year,
                ExpensesAndReceipts = reportList,
                EndingBalance = reportList.Any() ? reportList.Last().Balance : beginningBalance,
                AcountantSignatureId = getSignSingle?.AcountantSignatureId,
                AcountantSignature = getSignSingle?.AcountantSignature,
                ManagerSignitureId = getSignSingle?.ManagerSignitureId,
                ManagerSignature = getSignSingle?.ManagerSignature,
                Id = getSignSingle != null ? getSignSingle.Id : 0,
            };
            return report;
        }

        public async Task<ExpenseAndReceiptAndOther?> GetByIdAsync(int id)
        {
            return await _unitOfWork.ExpenseAndReceiptAndOther
                .GetByIdAsync(e => e.Id == id);
        }
        public async Task<int> AddAsync(ExpenseAndReceiptAndOther entity)
        {
            //entity.AttachmentPath = entity.Attachment == null ? "" : await FileHelper.SaveImageAsync(entity.Attachment, FileName);

            // 1. Prepare and/or create reports
            var emptyExpenseReportTable = await _unitOfWork.ExpensesReports.CheckTableIfEmpty();
            var emptyExpenseAndReceiptReportTable = await _unitOfWork.ExpenseAndReceiptReports.CheckTableIfEmpty();

            Domain.Entities.ExpenseAndReceipt.ExpensesReport? currentExpensesReport = null;
            
            // ExpensesReport - only for ChapterOne expenses
            if (entity.ExpensesSourceId == 2)
            {
                currentExpensesReport = await _unitOfWork.ExpensesReports.GetByColumnAsync(e => e.Year == entity.Date.Year && e.Month == entity.Date.Month);
                if (currentExpensesReport == null)
                {
                    // find the most recent report that is chronologically before the new expense's month
                    var prevReportQuery = _unitOfWork.ExpensesReports.GetAsync(r => r.Year < entity.Date.Year || (r.Year == entity.Date.Year && r.Month < entity.Date.Month));
                    var prevReport = await prevReportQuery
                        .OrderByDescending(r => r.Year)
                        .ThenByDescending(r => r.Month)
                        .FirstOrDefaultAsync();
                    var lastReportEndingBalance = prevReport?.EndingBalance;

                    decimal endingBalance = 0m;

                    if(entity.ItemType == (int)ItemType.Expenses)
                        endingBalance = (lastReportEndingBalance ?? _beginningBalanceExpensesReportSeed) - ((entity.Amount ?? 0) + (entity.Vat ?? 0));

                    if (entity.ItemType == (int)ItemType.Receipts)
                        endingBalance = (lastReportEndingBalance ?? _beginningBalanceExpensesReportSeed) + ((entity.Amount ?? 0) + (entity.Vat ?? 0));

                    var report = new ExpensesReport
                    {
                        BeginningBalance = lastReportEndingBalance ?? _beginningBalanceExpensesReportSeed,
                        Year = entity.Date.Year,
                        Month = entity.Date.Month,
                        EndingBalance = endingBalance,
                    };
                    if (emptyExpenseReportTable)
                    {
                        report.BeginningBalance = _beginningBalanceExpensesReportSeed;
                    }
                    else if (lastReportEndingBalance == null)
                    {
                        // if there's no previous ExpensesReport but there is a seeded ExpenseAndReceiptReport, use its beginning balance
                        var seededEar = await _unitOfWork.ExpenseAndReceiptReports.GetMaxRecordAsync(r => r.Id);
                        if (seededEar != null)
                        {
                            report.BeginningBalance = seededEar.BeginningBalance;
                        }
                    }
                    var newReportId = await _unitOfWork.ExpensesReports.AddAsyncThenGetLastId(report);
                    entity.ExpensesReportId = newReportId;
                }
                else
                {
                    entity.ExpensesReportId = currentExpensesReport.Id;
                }
            }
            else
            {
                entity.ExpensesReportId = null;
            }

            // ExpenseAndReceiptReport
            var currentReport = await _unitOfWork.ExpenseAndReceiptReports.GetByColumnAsync(e => e.Year == entity.Date.Year && e.Month == entity.Date.Month);
            // ChapterOne items (ExpensesSourceId == 2) should not create or be assigned to ExpenseAndReceiptReport
            if (entity.ExpensesSourceId == 2)
            {
                entity.ExpenseAndReceiptReportId = null;
            }
            else if (currentReport == null)
            {
                // find the most recent expense-and-receipt report before the new expense's month
                var prevEarQuery = _unitOfWork.ExpenseAndReceiptReports.GetAsync(r => r.Year < entity.Date.Year || (r.Year == entity.Date.Year && r.Month < entity.Date.Month));
                var prevEar = await prevEarQuery
                    .OrderByDescending(r => r.Year)
                    .ThenByDescending(r => r.Month)
                    .FirstOrDefaultAsync();
                var lastReportEndingBalance = prevEar?.EndingBalance;

                decimal endingBalance = 0m;
                if (entity.ItemType == (int)ItemType.Expenses)
                    endingBalance = (lastReportEndingBalance ?? 0) - ((entity.Amount ?? 0) + (entity.Vat ?? 0));

                if (entity.ItemType == (int)ItemType.Receipts)
                    endingBalance = (lastReportEndingBalance ?? 0) + ((entity.Amount ?? 0) + (entity.Vat ?? 0));

                var report = new ExpenseAndReceiptReport
                {
                    BeginningBalance = lastReportEndingBalance ?? 0,
                    Year = entity.Date.Year,
                    Month = entity.Date.Month,
                    EndingBalance = endingBalance,
                };
                if (emptyExpenseAndReceiptReportTable)
                {
                    report.BeginningBalance = _beginningBalanceExpenseAndReceiptReportSeed;
                }
                else if (lastReportEndingBalance == null)
                {
                    // if there's no previous ExpenseAndReceiptReport but there's a seeded ExpensesReport, use its beginning balance
                    var seededExp = await _unitOfWork.ExpensesReports.GetMaxRecordAsync(r => r.Id);
                    if (seededExp != null)
                    {
                        report.BeginningBalance = seededExp.BeginningBalance;
                    }
                }
                var newReportId = await _unitOfWork.ExpenseAndReceiptReports.AddAsyncThenGetLastId(report);
                entity.ExpenseAndReceiptReportId = newReportId;
            }
            else
            {
                entity.ExpenseAndReceiptReportId = currentReport.Id;
            }

            // 2. Add the expense ONCE
            // ensure CreatedAt is set so we can order by time when reporting
            var entit = await _unitOfWork.ExpenseAndReceiptAndOther.AddAsync(entity);
            await _unitOfWork.CompleteAsync();


            // 3. Update balances for reports if needed
            // ExpensesReport
            var updatedExpensesReport = currentExpensesReport ?? await _unitOfWork.ExpensesReports.GetByColumnWithIncludesAsync(
                e => e.Id == entity.ExpensesReportId,
                q => q.Include(r => r.ExpenseAndReceiptAndOthers).ThenInclude(e => e.BudgetItem).Include(r => r.ExpenseAndReceiptAndOthers).ThenInclude(e => e.Supplier));
                if (updatedExpensesReport != null)
                {
                    if (updatedExpensesReport.ExpenseAndReceiptAndOthers?.Count() > 0)
                    {
                            // Only chapter-one items (ExpensesSourceId == 2) affect ExpensesReport
                            decimal totalExpensesWithVat = 0;
                            decimal totalReceipts = 0;
                            foreach (var detail in updatedExpensesReport.ExpenseAndReceiptAndOthers)
                            {
                                if (detail.ExpensesSourceId != 2) continue;
                                if (detail.ItemType == (int)ItemType.Expenses)
                                    totalExpensesWithVat += (detail.Amount ?? 0) + (detail.Vat ?? 0);
                                else if (detail.ItemType == (int)ItemType.Receipts)
                                    totalReceipts += (detail.Amount ?? 0);
                            }
                            updatedExpensesReport.EndingBalance = updatedExpensesReport.BeginningBalance - totalExpensesWithVat + totalReceipts;
                    }
                    _unitOfWork.ExpensesReports.Update(updatedExpensesReport);
                    await _unitOfWork.CompleteAsync();
                //// cascade update following months so their BeginningBalance/EndingBalance remain consistent
                //await CascadeUpdateExpensesReportsAsync(updatedExpensesReport.EndingBalance, updatedExpensesReport.Year, updatedExpensesReport.Month);
                }

            // ExpenseAndReceiptReport - only update if this item is NOT ChapterOne (ExpensesSourceId != 2)
            if (entity.ExpensesSourceId != 2)
            {
                var updatedExpenseAndReceiptReport = currentReport ?? await _unitOfWork.ExpenseAndReceiptReports.GetByColumnWithIncludesAsync(
                    e => e.Id == entity.ExpenseAndReceiptReportId,
                    q => q.Include(r => r.ExpensesOrReceipts).ThenInclude(e => e.Supplier));
                if (updatedExpenseAndReceiptReport != null)
                {
                    // Sum expenses (including VAT) and receipts, and compute final ending balance
                    decimal totalExpensesWithVat = 0;
                    if (updatedExpenseAndReceiptReport.ExpensesOrReceipts?.Count() > 0)
                    {
                        foreach (var detail in updatedExpenseAndReceiptReport.ExpensesOrReceipts)
                        {
                            if(detail.ItemType != (int)ItemType.Expenses) continue;
                            totalExpensesWithVat += (detail.Amount ?? 0) + (detail.Vat ?? 0);
                        }
                    }

                    decimal totalReceipts = 0;
                    if (updatedExpenseAndReceiptReport.ExpensesOrReceipts?.Count() > 0)
                    {
                        foreach (var r in updatedExpenseAndReceiptReport.ExpensesOrReceipts)
                        {
                            // exclude chapter-one receipts from ExpenseAndReceiptReport totals
                            if (r.ItemType != (int)ItemType.Receipts || r.ExpensesSourceId == 2) continue;
                            totalReceipts += (r.Amount ?? 0);
                        }
                    }

                    updatedExpenseAndReceiptReport.EndingBalance = updatedExpenseAndReceiptReport.BeginningBalance - totalExpensesWithVat + totalReceipts;

                    _unitOfWork.ExpenseAndReceiptReports.Update(updatedExpenseAndReceiptReport);
                    await _unitOfWork.CompleteAsync();
                    //// cascade update following months so their BeginningBalance/EndingBalance remain consistent
                    //await CascadeUpdateExpenseAndReceiptReportsAsync(updatedExpenseAndReceiptReport.EndingBalance, updatedExpenseAndReceiptReport.Year, updatedExpenseAndReceiptReport.Month);
                }
            }
            await UpdateAsync(entit);
            return entit.Id;
        }
        public async Task UpdateAsync(ExpenseAndReceiptAndOther entity)
        {
            var existing = await _unitOfWork.ExpenseAndReceiptAndOther.GetByIdAsync(entity.Id);
            if (existing == null) return;

            var oldExpensesReportId = existing.ExpensesReportId;
            var oldExpenseAndReceiptReportId = existing.ExpenseAndReceiptReportId;
            var oldDate = existing.Date;

            _unitOfWork.ExpenseAndReceiptAndOther.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();

            var newDate = entity.Date;
            
            // First, ensure reports exist and get their IDs
            var expensesReport = await _unitOfWork.ExpensesReports.GetByColumnAsync(r => r.Year == newDate.Year && r.Month == newDate.Month);
            var expenseAndReceiptReport = await _unitOfWork.ExpenseAndReceiptReports.GetByColumnAsync(r => r.Year == newDate.Year && r.Month == newDate.Month);
            
            // Update foreign keys BEFORE refreshing so includes pick up the expense
            // ExpensesReport only for ChapterOne expenses
            existing.ExpensesReportId = (entity.ExpensesSourceId == 2) ? expensesReport?.Id : null;
            // Do NOT assign ExpenseAndReceiptReport for ChapterOne items
            existing.ExpenseAndReceiptReportId = (entity.ExpensesSourceId == 2) ? null : expenseAndReceiptReport?.Id;
            _unitOfWork.ExpenseAndReceiptAndOther.Update(existing);
            await _unitOfWork.CompleteAsync();

            // Now refresh reports for old and new months to recalculate totals
            var monthsToRefresh = new List<(int year, int month)>();
            if (oldDate != null)
                monthsToRefresh.Add((oldDate.Year, oldDate.Month));
            monthsToRefresh.Add((newDate.Year, newDate.Month));

            monthsToRefresh = monthsToRefresh.Distinct().ToList();

            foreach (var (year, month) in monthsToRefresh)
            {
                await RefreshExpensesReportForMonthAsync(year, month);
                await RefreshExpenseAndReceiptReportForMonthAsync(year, month);
            }
        }

        private async Task RefreshExpensesReportForMonthAsync(int year, int month)
        {
            var report = await _unitOfWork.ExpensesReports.GetByColumnWithIncludesAsync(
                r => r.Year == year && r.Month == month,
                q => q.Include(r => r.ExpenseAndReceiptAndOthers).ThenInclude(e => e.BudgetItem).Include(r => r.ExpenseAndReceiptAndOthers).ThenInclude(e => e.Supplier));

            if (report == null)
            {
                // create a report if any ChapterOne expenses exist for that month
                var anyExpenses = await _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.ItemType == (int)ItemType.Expenses && e.Date.Year == year && e.Date.Month == month && e.ExpensesSourceId == 2).ToListAsync();
                if (!anyExpenses.Any()) return;

                var prevQuery = _unitOfWork.ExpensesReports.GetAsync(r => r.Year < year || (r.Year == year && r.Month < month));
                var prev = await prevQuery.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                
                decimal beginningBalance;
                if (prev == null)
                {
                    // No earlier reports exist, this is the first chronological report
                    beginningBalance = _beginningBalanceExpensesReportSeed;
                }
                else
                {
                    // Use the previous month's ending balance
                    beginningBalance = prev.EndingBalance;
                }

                decimal totalAmountWithVat = 0;
                foreach (var exp in anyExpenses)
                    totalAmountWithVat += (exp.Amount ?? 0) + (exp.Vat ?? 0);

                var newReport = new Domain.Entities.ExpenseAndReceipt.ExpensesReport
                {
                    BeginningBalance = beginningBalance,
                    Year = year,
                    Month = month,
                    EndingBalance = beginningBalance - totalAmountWithVat
                };
                var newId = await _unitOfWork.ExpensesReports.AddAsyncThenGetLastId(newReport);
                await _unitOfWork.CompleteAsync();

                // Ensure existing expenses in this month point to the newly created report
                var expensesToUpdate = await _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.ItemType == (int)ItemType.Expenses && e.Date.Year == year && e.Date.Month == month).ToListAsync();
                if (expensesToUpdate != null && expensesToUpdate.Any())
                {
                    foreach (var exp in expensesToUpdate)
                    {
                        exp.ExpensesReportId = newId;
                        _unitOfWork.ExpenseAndReceiptAndOther.Update(exp);
                    }
                    await _unitOfWork.CompleteAsync();
                }

                await CascadeUpdateExpensesReportsAsync(newReport.EndingBalance, year, month);
                return;
            }

            // if exists, recalc or delete - only count ChapterOne expenses
            var chapterOneExpenses = report.ExpenseAndReceiptAndOthers?.Where(e => e.ExpensesSourceId == 2).ToList();
            if (chapterOneExpenses == null || !chapterOneExpenses.Any())
            {
                _unitOfWork.ExpensesReports.Delete(report);
                await _unitOfWork.CompleteAsync();
                // Cascade to future months since this month is now deleted
                var prevQuery = _unitOfWork.ExpensesReports.GetAsync(r => r.Year < year || (r.Year == year && r.Month < month));
                var prev = await prevQuery.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                await CascadeUpdateExpensesReportsAsync(prev == null ? _beginningBalanceExpensesReportSeed : prev.EndingBalance, year, month);
                return;
            }

            // Recalculate BeginningBalance from previous month
            var prevQuery2 = _unitOfWork.ExpensesReports.GetAsync(r => r.Year < year || (r.Year == year && r.Month < month));
            var prev2 = await prevQuery2.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
            report.BeginningBalance = prev2 == null ? _beginningBalanceExpensesReportSeed : prev2.EndingBalance;

            decimal totalAmountWithVat2 = 0;
            foreach (var detail in chapterOneExpenses)
            {
                totalAmountWithVat2 += (detail.Amount ?? 0) + (detail.Vat ?? 0);
            }
            report.EndingBalance = report.BeginningBalance - totalAmountWithVat2;
            _unitOfWork.ExpensesReports.Update(report);
            await _unitOfWork.CompleteAsync();
            await CascadeUpdateExpensesReportsAsync(report.EndingBalance, report.Year, report.Month);
        }

        private async Task RefreshExpenseAndReceiptReportForMonthAsync(int year, int month)
        {
            var report = await _unitOfWork.ExpenseAndReceiptReports.GetByColumnWithIncludesAsync(
                r => r.Year == year && r.Month == month,
                q => q.Include(r => r.ExpensesOrReceipts).ThenInclude(e => e.Supplier));

            if (report == null)
            {
                // create only if any expenses or receipts exist
                var anyExpenses = await _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.ItemType == (int)ItemType.Expenses && e.Date.Year == year && e.Date.Month == month).ToListAsync();
                // exclude ChapterOne receipts from ExpenseAndReceiptReport (they belong to ExpensesReport)
                var anyReceipts = await _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.ItemType == (int)ItemType.Receipts && e.Date.Year == year && e.Date.Month == month && e.ExpensesSourceId != 2).ToListAsync();
                if (!anyExpenses.Any() && !anyReceipts.Any()) return;

                var prevQuery = _unitOfWork.ExpenseAndReceiptReports.GetAsync(r => r.Year < year || (r.Year == year && r.Month < month));
                var prev = await prevQuery.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                
                decimal beginningBalance;
                if (prev == null)
                {
                    // No earlier reports exist, this is the first chronological report
                    beginningBalance = _beginningBalanceExpenseAndReceiptReportSeed;
                }
                else
                {
                    // Use the previous month's ending balance
                    beginningBalance = prev.EndingBalance;
                }

                decimal totalExpensesWithVat = 0;
                foreach (var e in anyExpenses) totalExpensesWithVat += (e.Amount ?? 0) + (e.Vat ?? 0);
                decimal totalReceipts = 0;
                foreach (var r in anyReceipts) totalReceipts += (r.Amount ?? 0);

                var newReport = new Domain.Entities.ExpenseAndReceipt.ExpenseAndReceiptReport
                {
                    BeginningBalance = beginningBalance,
                    Year = year,
                    Month = month,
                    EndingBalance = beginningBalance - totalExpensesWithVat + totalReceipts
                };
                var newId = await _unitOfWork.ExpenseAndReceiptReports.AddAsyncThenGetLastId(newReport);
                await _unitOfWork.CompleteAsync();

                // Ensure existing expenses/receipts in this month point to the newly created report
                var monthExpenses = await _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.ItemType == (int)ItemType.Expenses && e.Date.Year == year && e.Date.Month == month).ToListAsync();
                if (monthExpenses != null && monthExpenses.Any())
                {
                    foreach (var e in monthExpenses)
                    {
                        e.ExpenseAndReceiptReportId = newId;
                        _unitOfWork.ExpenseAndReceiptAndOther.Update(e);
                    }
                    await _unitOfWork.CompleteAsync();
                }

                // only assign EAR id to receipts that are not ChapterOne
                var monthReceipts = await _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(r => r.ItemType == (int)ItemType.Receipts && r.Date.Year == year && r.Date.Month == month && r.ExpensesSourceId != 2).ToListAsync();
                if (monthReceipts != null && monthReceipts.Any())
                {
                    foreach (var r in monthReceipts)
                    {
                        r.ExpenseAndReceiptReportId = newId;
                        _unitOfWork.ExpenseAndReceiptAndOther.Update(r);
                    }
                    await _unitOfWork.CompleteAsync();
                }

                await CascadeUpdateExpenseAndReceiptReportsAsync(newReport.EndingBalance, year, month);
                return;
            }

            var hasExpenses = report.ExpensesOrReceipts != null  && report.ExpensesOrReceipts.Any() && report.ExpensesOrReceipts.Any(x => x.ItemType == (int)ItemType.Expenses);
            // hasReceipts should ignore ChapterOne receipts (ExpensesSourceId == 2)
            var hasReceipts = report.ExpensesOrReceipts != null && report.ExpensesOrReceipts.Any() && report.ExpensesOrReceipts.Any(x => x.ItemType == (int)ItemType.Receipts && x.ExpensesSourceId != 2);

            if (!hasExpenses && !hasReceipts)
            {
                _unitOfWork.ExpenseAndReceiptReports.Delete(report);
                await _unitOfWork.CompleteAsync();
                // Cascade to future months since this month is now deleted
                var prevQuery3 = _unitOfWork.ExpenseAndReceiptReports.GetAsync(r => r.Year < year || (r.Year == year && r.Month < month));
                var prev3 = await prevQuery3.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                await CascadeUpdateExpenseAndReceiptReportsAsync(prev3 == null ? _beginningBalanceExpenseAndReceiptReportSeed : prev3.EndingBalance, year, month);
                return;
            }

            // Recalculate BeginningBalance from previous month
            var prevQuery4 = _unitOfWork.ExpenseAndReceiptReports.GetAsync(r => r.Year < year || (r.Year == year && r.Month < month));
            var prev4 = await prevQuery4.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
            report.BeginningBalance = prev4 == null ? _beginningBalanceExpenseAndReceiptReportSeed : prev4.EndingBalance;

            decimal totalExpensesWithVat2 = 0;
            if (hasExpenses)
            {
                foreach (var d in report.ExpensesOrReceipts)
                {
                    if(d.ItemType != (int)ItemType.Expenses) continue;
                    totalExpensesWithVat2 += (d.Amount ?? 0) + (d.Vat ?? 0);
                }
            }

            decimal totalReceipts2 = 0;
            if (hasReceipts)
            {
                foreach (var r in report.ExpensesOrReceipts)
                {
                    if (r.ItemType != (int)ItemType.Receipts || r.ExpensesSourceId == 2) continue;
                    totalReceipts2 += (r.Amount ?? 0);
                }
            }

            report.EndingBalance = report.BeginningBalance - totalExpensesWithVat2 + totalReceipts2;
            _unitOfWork.ExpenseAndReceiptReports.Update(report);
            await _unitOfWork.CompleteAsync();
            await CascadeUpdateExpenseAndReceiptReportsAsync(report.EndingBalance, report.Year, report.Month);
        }

        public async Task<ReceivingReceipt?> GetReceivingReceiptByIdAsync(int expenseId)
        {
            return await _unitOfWork.ReceivingReceipts.GetByIdAsync(e => e.ExpenseId == expenseId, e => e.ManagerSignature);
        }
        public async Task<int> AddAsync(ReceivingReceipt entity)
        {
            var id = await _unitOfWork.ReceivingReceipts.AddAsyncThenGetLastId(entity);
            await _unitOfWork.CompleteAsync();

            return id;
        }
        public async Task UpdateAsync(ReceivingReceipt entity)
        {
            var existing = await _unitOfWork.ReceivingReceipts.GetByIdAsync(entity.Id);
            if (existing == null) return;

            _unitOfWork.ReceivingReceipts.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> ValidateOtpAsync(int id, string code)
        {
            var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);

            if (success)
            {
                var receivingReceipt = await GetReceivingReceiptByIdAsync(id);
                var updatedReceivingReceipt = receivingReceipt;

                // Get all signatures for the user and pick the latest
                var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
                var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
                var latestSignature = allSignatures
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefault();
                if (latestSignature != null && updatedReceivingReceipt != null)
                    updatedReceivingReceipt.ManagerSignitureId = latestSignature.Id;

                _unitOfWork.ReceivingReceipts?.UpdateValues(receivingReceipt, updatedReceivingReceipt);
                await _unitOfWork.CompleteAsync();
                return true;
            }

            return false;
        }


        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ExpenseAndReceiptAndOther.GetByIdAsync(id);
            if (entity == null) return;

            FileHelper.DeleteImageFile(entity.AttachmentPath);

            // capture related report ids before delete
            var expensesReportId = entity.ExpensesReportId;
            var expenseAndReceiptReportId = entity.ExpenseAndReceiptReportId;

            _unitOfWork.ExpenseAndReceiptAndOther.Delete(entity);
            await _unitOfWork.CompleteAsync();

            // Update or remove ExpensesReport
            if (expensesReportId != 0)
            {
                var expensesReport = await _unitOfWork.ExpensesReports.GetByColumnWithIncludesAsync(
                    r => r.Id == expensesReportId,
                    q => q.Include(r => r.ExpenseAndReceiptAndOthers).ThenInclude(e => e.BudgetItem).Include(r => r.ExpenseAndReceiptAndOthers).ThenInclude(e => e.Supplier));

                if (expensesReport != null)
                {
                    if (expensesReport.ExpenseAndReceiptAndOthers == null || !expensesReport.ExpenseAndReceiptAndOthers.Any())
                    {
                        // Report will be deleted, cascade with previous month's ending balance
                        var prevQuery = _unitOfWork.ExpensesReports.GetAsync(r => r.Year < expensesReport.Year || (r.Year == expensesReport.Year && r.Month < expensesReport.Month));
                        var prev = await prevQuery.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                        
                        _unitOfWork.ExpensesReports.Delete(expensesReport);
                        await _unitOfWork.CompleteAsync();
                        await CascadeUpdateExpensesReportsAsync(prev == null ? _beginningBalanceExpensesReportSeed : prev.EndingBalance, expensesReport.Year, expensesReport.Month);
                    }
                    else
                    {
                        // Recalculate BeginningBalance from previous month
                        var prevQuery = _unitOfWork.ExpensesReports.GetAsync(r => r.Year < expensesReport.Year || (r.Year == expensesReport.Year && r.Month < expensesReport.Month));
                        var prev = await prevQuery.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                        expensesReport.BeginningBalance = prev == null ? _beginningBalanceExpensesReportSeed : prev.EndingBalance;
                        
                        decimal totalAmountWithVat = 0;
                        foreach (var detail in expensesReport.ExpenseAndReceiptAndOthers)
                        {
                            if (detail.ExpensesSourceId != (int)ExpensesSourceEnum.MiscellaneousExpenses) continue;
                            if(detail.ItemType == (int)ItemType.Expenses)
                                totalAmountWithVat += ((detail.Amount ?? 0) + (detail.Vat ?? 0));
                            if (detail.ItemType == (int)ItemType.Receipts)
                                totalAmountWithVat -= ((detail.Amount ?? 0) + (detail.Vat ?? 0));
                        }
                        expensesReport.EndingBalance = expensesReport.BeginningBalance - totalAmountWithVat;
                        _unitOfWork.ExpensesReports.Update(expensesReport);
                        await _unitOfWork.CompleteAsync();
                        await CascadeUpdateExpensesReportsAsync(expensesReport.EndingBalance, expensesReport.Year, expensesReport.Month);
                    }
                }
            }

            // Update or remove ExpenseAndReceiptReport (only for non-ChapterOne items)
            if (expenseAndReceiptReportId != 0 && entity.ExpensesSourceId != 2)
            {
                var earReport = await _unitOfWork.ExpenseAndReceiptReports.GetByColumnWithIncludesAsync(
                    r => r.Id == expenseAndReceiptReportId,
                    q => q.Include(r => r.ExpensesOrReceipts).ThenInclude(e => e.Supplier));

                if (earReport != null)
                {
                    var hasExpenses = earReport.ExpensesOrReceipts != null && earReport.ExpensesOrReceipts.Any() && earReport.ExpensesOrReceipts.Any(x => x.ItemType == (int)ItemType.Expenses);
                        // ignore ChapterOne receipts for ExpenseAndReceiptReport
                        var hasReceipts = earReport.ExpensesOrReceipts != null && earReport.ExpensesOrReceipts.Any() && earReport.ExpensesOrReceipts.Any(x => x.ItemType == (int)ItemType.Receipts && x.ExpensesSourceId != 2);


                    if (!hasExpenses && !hasReceipts)
                    {
                        // Report will be deleted, cascade with previous month's ending balance
                        var prevQuery = _unitOfWork.ExpenseAndReceiptReports.GetAsync(r => r.Year < earReport.Year || (r.Year == earReport.Year && r.Month < earReport.Month));
                        var prev = await prevQuery.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                        _unitOfWork.ExpenseAndReceiptReports.Delete(earReport);
                        await _unitOfWork.CompleteAsync();
                        await CascadeUpdateExpenseAndReceiptReportsAsync(prev == null ? _beginningBalanceExpenseAndReceiptReportSeed : prev.EndingBalance, earReport.Year, earReport.Month);
                    }
                    else
                    {
                        // Recalculate BeginningBalance from previous month
                        var prevQuery = _unitOfWork.ExpenseAndReceiptReports.GetAsync(r => r.Year < earReport.Year || (r.Year == earReport.Year && r.Month < earReport.Month));
                        var prev = await prevQuery.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                        earReport.BeginningBalance = prev == null ? _beginningBalanceExpenseAndReceiptReportSeed : prev.EndingBalance;
                        
                        decimal totalExpensesWithVat = 0;
                        if (hasExpenses)
                        {
                            foreach (var d in earReport.ExpensesOrReceipts)
                            {
                                if(d.ItemType != (int)ItemType.Expenses) continue;
                                totalExpensesWithVat += (d.Amount ?? 0) + (d.Vat ?? 0);
                            }
                        }

                        decimal totalReceipts = 0;
                        if (hasReceipts)
                        {
                            foreach (var r in earReport.ExpensesOrReceipts)
                            {
                                if (r.ItemType != (int)ItemType.Receipts || r.ExpensesSourceId == 2) continue;
                                totalReceipts += (r.Amount ?? 0);
                            }
                        }

                        earReport.EndingBalance = earReport.BeginningBalance - totalExpensesWithVat + totalReceipts;
                        _unitOfWork.ExpenseAndReceiptReports.Update(earReport);
                        await _unitOfWork.CompleteAsync();
                        await CascadeUpdateExpenseAndReceiptReportsAsync(earReport.EndingBalance, earReport.Year, earReport.Month);
                    }
                }
            }
        }
        public async Task<IEnumerable<int>> GetAllYearsInDb()
        {
            // Derive years from chapter-one (ExpensesSourceId == 2) transactions instead of ExpensesReports table
            var recordsQuery = _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.ExpensesSourceId == 2);
            var records = await recordsQuery.ToListAsync();
            var allYears = records
                .Where(e => e.Date != null)
                .Select(e => e.Date.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();
            return allYears;
        }
        public async Task<IEnumerable<int>> GetAllYearsOfExpenseAndReceiptReportInDb()
        {
            // Derive years from the ExpenseAndReceiptAndOther table for records with ExpensesSourceId == 1 only
            var allRecords = await _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.ExpensesSourceId == 1).ToListAsync();
            var allYears = allRecords
                .Where(e => e.Date != null)
                .Select(e => e.Date.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();
            return allYears;
        }

        private async Task CascadeUpdateExpenseAndReceiptReportsAsync(decimal startingEndingBalance, int year, int month)
        {
            // get all reports strictly after (year,month)
            var followingQuery = _unitOfWork.ExpenseAndReceiptReports.GetAsync(r => r.Year > year || (r.Year == year && r.Month > month), r => r.ExpensesOrReceipts);
            var following = await followingQuery.OrderBy(r => r.Year).ThenBy(r => r.Month).ToListAsync();

            decimal prevEnding = startingEndingBalance;
            foreach (var rep in following)
            {
                rep.BeginningBalance = prevEnding;

                decimal totalExpensesWithVat = 0;
                if (rep.ExpensesOrReceipts?.Count() > 0)
                {
                    foreach (var d in rep.ExpensesOrReceipts)
                    {
                        if(d.ItemType != (int)ItemType.Expenses) continue;
                        totalExpensesWithVat += (d.Amount ?? 0) + (d.Vat ?? 0);
                    }
                }

                decimal totalReceipts = 0;
                if (rep.ExpensesOrReceipts?.Count() > 0)
                {
                    foreach (var r in rep.ExpensesOrReceipts)
                    {
                        // skip chapter-one receipts when cascading ExpenseAndReceiptReports
                        if (r.ItemType != (int)ItemType.Receipts || r.ExpensesSourceId == 2) continue;
                        totalReceipts += (r.Amount ?? 0);
                    }
                }

                rep.EndingBalance = rep.BeginningBalance - totalExpensesWithVat + totalReceipts;
                _unitOfWork.ExpenseAndReceiptReports.Update(rep);
                await _unitOfWork.CompleteAsync();

                prevEnding = rep.EndingBalance;
            }
        }

        private async Task CascadeUpdateExpensesReportsAsync(decimal startingEndingBalance, int year, int month)
        {
            var followingQuery = _unitOfWork.ExpensesReports.GetAsync(r => r.Year > year || (r.Year == year && r.Month > month), r => r.ExpenseAndReceiptAndOthers);
            var following = await followingQuery.OrderBy(r => r.Year).ThenBy(r => r.Month).ToListAsync();

            decimal prevEnding = startingEndingBalance;
            foreach (var rep in following)
            {
                rep.BeginningBalance = prevEnding;

                // Only chapter-one items (ExpensesSourceId == 2) affect ExpensesReport
                decimal totalExpensesWithVat = 0;
                decimal totalReceipts = 0;
                if (rep.ExpenseAndReceiptAndOthers?.Count() > 0)
                {
                    foreach (var d in rep.ExpenseAndReceiptAndOthers)
                    {
                        if (d.ExpensesSourceId != 2) continue;
                        if (d.ItemType == (int)ItemType.Expenses)
                            totalExpensesWithVat += ((d.Amount ?? 0) + (d.Vat ?? 0));
                        else if (d.ItemType == (int)ItemType.Receipts)
                            totalReceipts += (d.Amount ?? 0);
                    }
                }

                rep.EndingBalance = rep.BeginningBalance - totalExpensesWithVat + totalReceipts;
                _unitOfWork.ExpensesReports.Update(rep);
                await _unitOfWork.CompleteAsync();

                prevEnding = rep.EndingBalance;
            }
        }


    }
}