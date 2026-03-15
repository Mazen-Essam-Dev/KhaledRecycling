using Application.Helpers;
using Application.Interfaces.Admin.ExpenseAndReceipt;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Vml.Office;
using Domain.DTOs.Admin.ExpenseAndReceipt;
using Domain.Entities.ExpenseAndReceipt;
using Domain.Enums;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Services.Admin.ExpenseAndReceipt
{
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string FileName = "Receipts";
        private readonly decimal _beginningBalanceExpensesReportSeed;
        private readonly decimal _beginningBalanceExpenseAndReceiptReportSeed;

        public ReceiptService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _beginningBalanceExpensesReportSeed = configuration.GetValue<decimal?>("Finance:BeginningBalanceExpensesReport") ?? 50000m;
            _beginningBalanceExpenseAndReceiptReportSeed = configuration.GetValue<decimal?>("Finance:BeginningBalanceExpenseAndReceiptReport") ?? 100000m;
        }

        public async Task<string> GetLastSerialCode()
        {
                var lastSerial = await _unitOfWork.ReceivingReceipts.Table
                    .Where(s => s.itemType == (int)ItemType.Receipts)
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
        public async Task<IEnumerable<ExpenseAndReceiptAndOther>> GetAllAsync()
        {
            return await _unitOfWork.ExpenseAndReceiptAndOther.GetAllAsync(e=> e.ItemType == (int)ItemType.Receipts , r => r.Supplier, r => r.BudgetItem);
        }

        public async Task<ExpenseAndReceiptAndOther?> GetByIdAsync(int id)
        {
            return await _unitOfWork.ExpenseAndReceiptAndOther
                .GetByIdAsync(e => e.ItemType == (int)ItemType.Receipts && e.Id == id);
        }

        public async Task<int> AddAsync(ExpenseAndReceiptAndOther entity)
        {
            //entity.AttachmentPath = entity.Attachment == null ? "" : await FileHelper.SaveImageAsync(entity.Attachment, FileName);

            var currentReport = await _unitOfWork.ExpenseAndReceiptReports.GetByColumnAsync(e => e.Year == entity.Date.Year && e.Month == entity.Date.Month);

            // ChapterOne receipts (MiscellaneousExpenses) should NOT be included in ExpenseAndReceiptReport
            var isChapterOne = entity.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses;

            if (isChapterOne)
            {
                // Add the receipt, do not create/assign ExpenseAndReceiptReport; instead attach to ExpensesReport
                entity.ExpenseAndReceiptReportId = null;
                var entit = await _unitOfWork.ExpenseAndReceiptAndOther.AddAsync(entity);
                await _unitOfWork.CompleteAsync();

                // Update computed fields and then add to ExpensesReport
                await UpdateAsync(entit);
                await AddReceiptToExpensesReportAsync(entit);
                return entit.Id;
            }

            if (currentReport == null)
            {
                // find the most recent expense-and-receipt report before the new receipt's month
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
                var newReportId = await _unitOfWork.ExpenseAndReceiptReports.AddAsyncThenGetLastId(report);
                entity.ExpenseAndReceiptReportId = newReportId;

                // set creation timestamp so reports can sort by time-of-day
                var entit = await _unitOfWork.ExpenseAndReceiptAndOther.AddAsync(entity);
                await _unitOfWork.CompleteAsync();

                //// cascade update following months since we inserted/created this month
                //await CascadeUpdateExpenseAndReceiptReportsAsync(report.EndingBalance, report.Year, report.Month);

                await UpdateAsync(entit);
                if (entit.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses)
                {
                    await AddReceiptToExpensesReportAsync(entit);
                }
                return entit.Id;
            }
            else
            {
                entity.ExpenseAndReceiptReportId = currentReport.Id;
                // set creation timestamp
                var entit = await _unitOfWork.ExpenseAndReceiptAndOther.AddAsync(entity);
                await _unitOfWork.CompleteAsync();

                // Re-load the report with includes so we calculate totals from the saved data
                var refreshed = await _unitOfWork.ExpenseAndReceiptReports.GetByColumnWithIncludesAsync(
                    r => r.Year == currentReport.Year && r.Month == currentReport.Month,
                    q => q.Include(r => r.ExpensesOrReceipts).ThenInclude(e => e.Supplier));

                if (refreshed != null)
                {
                    decimal totalExpensesWithVat = 0;
                    if (refreshed.ExpensesOrReceipts?.Count() > 0)
                    {
                        foreach (var d in refreshed.ExpensesOrReceipts)
                        {
                            //  if(d.ItemType == ItemType.Expenses && d.ExpensesSourceId != (int)ExpensesSourceEnum.MiscellaneousExpenses) // Any Expenses Not MiscellaneousExpenses
                            if (d.ItemType == (int)ItemType.Expenses)
                                totalExpensesWithVat += (d.Amount ?? 0) + (d.Vat ?? 0);
                        }
                    }

                    decimal totalReceipts = 0;
                    if (refreshed.ExpensesOrReceipts?.Count() > 0)
                    {
                        foreach (var detail in refreshed.ExpensesOrReceipts)
                        {
                            // only include receipts that are NOT ChapterOne in EAR totals
                            if (detail.ItemType != (int)ItemType.Receipts || detail.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses) continue;
                            totalReceipts += (detail.Amount ?? 0);
                        }
                    }

                    refreshed.EndingBalance = refreshed.BeginningBalance - totalExpensesWithVat + totalReceipts;
                    _unitOfWork.ExpenseAndReceiptReports.Update(refreshed);
                    await _unitOfWork.CompleteAsync();
                    //await CascadeUpdateExpenseAndReceiptReportsAsync(refreshed.EndingBalance, refreshed.Year, refreshed.Month);
                }

                await UpdateAsync(entit);
                // If the saved receipt belongs to ChapterOne, move it to ExpensesReport
                if (entit.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses)
                {
                    await AddReceiptToExpensesReportAsync(entit);
                }
                return entit.Id;
            }
        }
        public async Task<int> AddReceiptToExpensesReportAsync(ExpenseAndReceiptAndOther entity)
        {
            //entity.AttachmentPath = entity.Attachment == null ? "" : await FileHelper.SaveImageAsync(entity.Attachment, FileName);

            var currentReport = await _unitOfWork.ExpensesReports.GetByColumnAsync(e => e.Year == entity.Date.Year && e.Month == entity.Date.Month);

            var existing = await _unitOfWork.ExpenseAndReceiptAndOther.GetByIdAsync(entity.Id);
            if (existing == null) return 0;

            if (currentReport == null)
            {
                // find the most recent expense-and-receipt report before the new receipt's month
                var prevEarQuery = _unitOfWork.ExpensesReports.GetAsync(r => r.Year < entity.Date.Year || (r.Year == entity.Date.Year && r.Month < entity.Date.Month));
                var prevEar = await prevEarQuery
                    .OrderByDescending(r => r.Year)
                    .ThenByDescending(r => r.Month)
                    .FirstOrDefaultAsync();
                var lastReportEndingBalance = prevEar?.EndingBalance;

                decimal endingBalance = 0m;

                    if (entity.ItemType == (int)ItemType.Expenses)
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
                var newReportId = await _unitOfWork.ExpensesReports.AddAsyncThenGetLastId(report);
                entity.ExpensesReportId = newReportId;

                // set creation timestamp so reports can sort by time-of-day
                _unitOfWork.ExpenseAndReceiptAndOther.UpdateValues(existing, entity);
                await _unitOfWork.CompleteAsync();

                //// cascade update following months since we inserted/created this month
                //await CascadeUpdateExpenseAndReceiptReportsAsync(report.EndingBalance, report.Year, report.Month);

                await UpdateReceiptToExpensesReportAsync(entity);
                return entity.Id;
            }
            else
            {
                entity.ExpensesReportId = currentReport.Id;

                // set creation timestamp
                _unitOfWork.ExpenseAndReceiptAndOther.UpdateValues(existing, entity);
                await _unitOfWork.CompleteAsync();

                // Re-load the report with includes so we calculate totals from the saved data
                var refreshed = await _unitOfWork.ExpensesReports.GetByColumnWithIncludesAsync(
                    r => r.Year == currentReport.Year && r.Month == currentReport.Month,
                    q => q.Include(r => r.ExpenseAndReceiptAndOthers).ThenInclude(e => e.Supplier));

                if (refreshed != null)
                {
                    decimal totalExpensesWithVat = 0;
                    if (refreshed.ExpenseAndReceiptAndOthers?.Count() > 0)
                    {
                        foreach (var d in refreshed.ExpenseAndReceiptAndOthers)
                        {
                            //  if(d.ItemType == ItemType.Expenses && d.ExpensesSourceId != (int)ExpensesSourceEnum.MiscellaneousExpenses) // Any Expenses Not MiscellaneousExpenses
                            if (d.ItemType == (int)ItemType.Expenses)
                                totalExpensesWithVat += (d.Amount ?? 0) + (d.Vat ?? 0);
                        }
                    }

                    decimal totalReceipts = 0;
                    if (refreshed.ExpenseAndReceiptAndOthers?.Count() > 0)
                    {
                        foreach (var detail in refreshed.ExpenseAndReceiptAndOthers)
                        {
                            // ExpensesReport should only include chapter-one receipts
                            if (detail.ItemType != (int)ItemType.Receipts || detail.ExpensesSourceId != (int)ExpensesSourceEnum.MiscellaneousExpenses) continue;
                            totalReceipts += (detail.Amount ?? 0);
                        }
                    }

                    refreshed.EndingBalance = refreshed.BeginningBalance - totalExpensesWithVat + totalReceipts;
                    _unitOfWork.ExpensesReports.Update(refreshed);
                    await _unitOfWork.CompleteAsync();
                    //await CascadeUpdateExpenseAndReceiptReportsAsync(refreshed.EndingBalance, refreshed.Year, refreshed.Month);
                }

                await UpdateReceiptToExpensesReportAsync(entity);
                return entity.Id;
            }
        }

        public async Task UpdateAsync(ExpenseAndReceiptAndOther entity)
        {
            var existing = await _unitOfWork.ExpenseAndReceiptAndOther.GetByIdAsync(entity.Id);
            if (existing == null) return;

            var oldDate = existing.Date;

            _unitOfWork.ExpenseAndReceiptAndOther.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();

            var newDate = entity.Date;

            if (entity.ExpensesSourceId != (int)ExpensesSourceEnum.MiscellaneousExpenses)
            {

                // First, get/ensure report exists for the new date
                var expenseAndReceiptReport = await _unitOfWork.ExpenseAndReceiptReports.GetByColumnAsync(r => r.Year == newDate.Year && r.Month == newDate.Month);

            // Update foreign key BEFORE refreshing so includes pick up the receipt
            // Do not assign ExpenseAndReceiptReportId for ChapterOne receipts
            existing.ExpenseAndReceiptReportId = (entity.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses) ? null : expenseAndReceiptReport?.Id;
            _unitOfWork.ExpenseAndReceiptAndOther.Update(existing);
            await _unitOfWork.CompleteAsync();

                // Now refresh affected months (old and new) to recalculate totals
                var monthsToRefresh = new List<(int year, int month)>();
                if (oldDate != null) monthsToRefresh.Add((oldDate.Year, oldDate.Month));
                monthsToRefresh.Add((newDate.Year, newDate.Month));
                monthsToRefresh = monthsToRefresh.Distinct().ToList();

                foreach (var (year, month) in monthsToRefresh)
                {
                    await RefreshExpenseAndReceiptReportForMonthAsync(year, month);
                }
            }
            if (entity.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses)
            {
                await UpdateReceiptToExpensesReportAsync(entity);
            }
        }
        public async Task UpdateReceiptToExpensesReportAsync(ExpenseAndReceiptAndOther entity)
        {
            var existing = await _unitOfWork.ExpenseAndReceiptAndOther.GetByIdAsync(entity.Id);
            if (existing == null) return;

            var oldDate = existing.Date;

            _unitOfWork.ExpenseAndReceiptAndOther.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();

            var newDate = entity.Date;

            // First, get/ensure report exists for the new date
            var expenseAndReceiptReport = await _unitOfWork.ExpensesReports.GetByColumnAsync(r => r.Year == newDate.Year && r.Month == newDate.Month);

            // Update foreign key BEFORE refreshing so includes pick up the receipt
            existing.ExpensesReportId = expenseAndReceiptReport?.Id;
            _unitOfWork.ExpenseAndReceiptAndOther.Update(existing);
            await _unitOfWork.CompleteAsync();

            // Now refresh affected months (old and new) to recalculate totals
            var monthsToRefresh = new List<(int year, int month)>();
            if (oldDate != null) monthsToRefresh.Add((oldDate.Year, oldDate.Month));
            monthsToRefresh.Add((newDate.Year, newDate.Month));
            monthsToRefresh = monthsToRefresh.Distinct().ToList();

            foreach (var (year, month) in monthsToRefresh)
            {
                await RefreshExpensesReportForMonthAsync(year, month);
            }
        }


        private async Task RefreshExpenseAndReceiptReportForMonthAsync(int year, int month)
        {
            var report = await _unitOfWork.ExpenseAndReceiptReports.GetByColumnWithIncludesAsync(
                r => r.Year == year && r.Month == month,
                q => q.Include(r => r.ExpensesOrReceipts).ThenInclude(e => e.Supplier));

            if (report == null)
            {
                var anyExpenses = await _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.ItemType == (int)ItemType.Expenses && e.Date.Year == year && e.Date.Month == month).ToListAsync();
                // exclude ChapterOne receipts from EAR (they belong to ExpensesReport)
                var anyReceipts = await _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.ItemType == (int)ItemType.Receipts && e.Date.Year == year && e.Date.Month == month && e.ExpensesSourceId != (int)ExpensesSourceEnum.MiscellaneousExpenses).ToListAsync();
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
                await CascadeUpdateExpenseAndReceiptReportsAsync(newReport.EndingBalance, year, month);
                return;
            }

            var hasExpenses = report.ExpensesOrReceipts != null && report.ExpensesOrReceipts.Any() && report.ExpensesOrReceipts.Any(x => x.ItemType == (int)ItemType.Expenses);
            // hasReceipts should ignore ChapterOne receipts
            var hasReceipts = report.ExpensesOrReceipts != null && report.ExpensesOrReceipts.Any() && report.ExpensesOrReceipts.Any(x => x.ItemType == (int)ItemType.Receipts && x.ExpensesSourceId != (int)ExpensesSourceEnum.MiscellaneousExpenses);


            if (!hasExpenses && !hasReceipts)
            {
                _unitOfWork.ExpenseAndReceiptReports.Delete(report);
                await _unitOfWork.CompleteAsync();
                // Cascade to future months since this month is now deleted
                var prevQuery2 = _unitOfWork.ExpenseAndReceiptReports.GetAsync(r => r.Year < year || (r.Year == year && r.Month < month));
                var prev2 = await prevQuery2.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                await CascadeUpdateExpenseAndReceiptReportsAsync(prev2 == null ? _beginningBalanceExpenseAndReceiptReportSeed : prev2.EndingBalance, year, month);
                return;
            }

            // Recalculate BeginningBalance from previous month
            var prevQuery3 = _unitOfWork.ExpenseAndReceiptReports.GetAsync(r => r.Year < year || (r.Year == year && r.Month < month));
            var prev3 = await prevQuery3.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
            report.BeginningBalance = prev3 == null ? _beginningBalanceExpenseAndReceiptReportSeed : prev3.EndingBalance;

            decimal totalExpensesWithVat2 = 0;
            if (hasExpenses)
            {
                foreach (var d in report.ExpensesOrReceipts)
                {
                    if (d.ItemType == (int)ItemType.Expenses)
                        totalExpensesWithVat2 += (d.Amount ?? 0) + (d.Vat ?? 0);
                }
            }

            decimal totalReceipts2 = 0;
            if (hasReceipts)
            {
                foreach (var r in report.ExpensesOrReceipts)
                {
                    if (r.ItemType != (int)ItemType.Receipts || r.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses) continue;
                    totalReceipts2 += (r.Amount ?? 0);
                }
            }

            report.EndingBalance = report.BeginningBalance - totalExpensesWithVat2 + totalReceipts2;
            _unitOfWork.ExpenseAndReceiptReports.Update(report);
            await _unitOfWork.CompleteAsync();
            await CascadeUpdateExpenseAndReceiptReportsAsync(report.EndingBalance, report.Year, report.Month);
        }
        private async Task RefreshExpensesReportForMonthAsync(int year, int month)
        {
            var report = await _unitOfWork.ExpensesReports.GetByColumnWithIncludesAsync(
                r => r.Year == year && r.Month == month,
                q => q.Include(r => r.ExpenseAndReceiptAndOthers).ThenInclude(e => e.Supplier));

            if (report == null)
            {
                var anyExpenses = await _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.ItemType == (int)ItemType.Expenses && e.Date.Year == year && e.Date.Month == month).ToListAsync();
                var anyReceipts = await _unitOfWork.ExpenseAndReceiptAndOther.GetAsync(e => e.ItemType == (int)ItemType.Receipts && e.Date.Year == year && e.Date.Month == month).ToListAsync();
                if (!anyExpenses.Any() && !anyReceipts.Any()) return;

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

                decimal totalExpensesWithVat = 0;
                foreach (var e in anyExpenses) totalExpensesWithVat += (e.Amount ?? 0) + (e.Vat ?? 0);
                decimal totalReceipts = 0;
                foreach (var r in anyReceipts) totalReceipts += (r.Amount ?? 0);

                var newReport = new Domain.Entities.ExpenseAndReceipt.ExpensesReport
                {
                    BeginningBalance = beginningBalance,
                    Year = year,
                    Month = month,
                    EndingBalance = beginningBalance - totalExpensesWithVat + totalReceipts
                };
                var newId = await _unitOfWork.ExpensesReports.AddAsyncThenGetLastId(newReport);
                await _unitOfWork.CompleteAsync();
                await CascadeUpdateExpensesReportsAsync(newReport.EndingBalance, year, month);
                return;
            }

            var hasExpenses = report.ExpenseAndReceiptAndOthers != null && report.ExpenseAndReceiptAndOthers.Any() && report.ExpenseAndReceiptAndOthers.Any(x => x.ItemType == (int)ItemType.Expenses);
            var hasReceipts = report.ExpenseAndReceiptAndOthers != null && report.ExpenseAndReceiptAndOthers.Any() && report.ExpenseAndReceiptAndOthers.Any(x => x.ItemType == (int)ItemType.Receipts);


            if (!hasExpenses && !hasReceipts)
            {
                _unitOfWork.ExpensesReports.Delete(report);
                await _unitOfWork.CompleteAsync();
                // Cascade to future months since this month is now deleted
                var prevQuery2 = _unitOfWork.ExpensesReports.GetAsync(r => r.Year < year || (r.Year == year && r.Month < month));
                var prev2 = await prevQuery2.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                await CascadeUpdateExpensesReportsAsync(prev2 == null ? _beginningBalanceExpensesReportSeed : prev2.EndingBalance, year, month);
                return;
            }

            // Recalculate BeginningBalance from previous month
            var prevQuery3 = _unitOfWork.ExpensesReports.GetAsync(r => r.Year < year || (r.Year == year && r.Month < month));
            var prev3 = await prevQuery3.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
            report.BeginningBalance = prev3 == null ? _beginningBalanceExpensesReportSeed : prev3.EndingBalance;

            decimal totalExpensesWithVat2 = 0;
            if (hasExpenses)
            {
                foreach (var d in report.ExpenseAndReceiptAndOthers)
                {
                    if (d.ItemType == (int)ItemType.Expenses)
                        totalExpensesWithVat2 += (d.Amount ?? 0) + (d.Vat ?? 0);
                }
            }

            decimal totalReceipts2 = 0;
            if (hasReceipts)
            {
                foreach (var r in report.ExpenseAndReceiptAndOthers)
                {
                    // if (r.ItemType == ItemType.Receipts && r.ExpensesSourceId != (int)ExpensesSourceEnum.MiscellaneousExpenses) // All Receipts Not MiscellaneousExpenses --> To ExpensesReports
                    if (r.ItemType == (int)ItemType.Receipts)
                        totalReceipts2 += (r.Amount ?? 0);
                }
            }

            report.EndingBalance = report.BeginningBalance - totalExpensesWithVat2 + totalReceipts2;
            _unitOfWork.ExpensesReports.Update(report);
            await _unitOfWork.CompleteAsync();
            await CascadeUpdateExpensesReportsAsync(report.EndingBalance, report.Year, report.Month);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ExpenseAndReceiptAndOther.GetByIdAsync(id);
            if (entity == null) return;
            var expenseReportId = entity.ExpensesReportId;
            var SourceId = entity.ExpensesSourceId;

            FileHelper.DeleteImageFile(entity.AttachmentPath);

            var expenseAndReceiptReportId = entity.ExpenseAndReceiptReportId;

            _unitOfWork.ExpenseAndReceiptAndOther.Delete(entity);
            await _unitOfWork.CompleteAsync();

            // Update or remove ExpenseAndReceiptReport
            if (expenseAndReceiptReportId != 0)
            {
                var report = await _unitOfWork.ExpenseAndReceiptReports.GetByColumnWithIncludesAsync(
                    r => r.Id == expenseAndReceiptReportId,
                    q => q.Include(r => r.ExpensesOrReceipts).ThenInclude(e => e.Supplier));

                if (report != null)
                {
                    var hasExpenses = report.ExpensesOrReceipts != null && report.ExpensesOrReceipts.Any() && report.ExpensesOrReceipts.Any(x => x.ItemType == (int)ItemType.Expenses);
                    // ignore ChapterOne receipts when deciding if EAR still has receipts
                    var hasReceipts = report.ExpensesOrReceipts != null && report.ExpensesOrReceipts.Any() && report.ExpensesOrReceipts.Any(x => x.ItemType == (int)ItemType.Receipts && x.ExpensesSourceId != (int)ExpensesSourceEnum.MiscellaneousExpenses);

                    if (!hasExpenses && !hasReceipts)
                    {
                        // Report will be deleted, cascade with previous month's ending balance
                        var prevQuery = _unitOfWork.ExpenseAndReceiptReports.GetAsync(r => r.Year < report.Year || (r.Year == report.Year && r.Month < report.Month));
                        var prev = await prevQuery.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                        
                        _unitOfWork.ExpenseAndReceiptReports.Delete(report);
                        await _unitOfWork.CompleteAsync();
                        await CascadeUpdateExpenseAndReceiptReportsAsync(prev == null ? _beginningBalanceExpenseAndReceiptReportSeed : prev.EndingBalance, report.Year, report.Month);
                    }
                    else
                    {
                        // Recalculate BeginningBalance from previous month
                        var prevQuery = _unitOfWork.ExpenseAndReceiptReports.GetAsync(r => r.Year < report.Year || (r.Year == report.Year && r.Month < report.Month));
                        var prev = await prevQuery.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                        report.BeginningBalance = prev == null ? _beginningBalanceExpenseAndReceiptReportSeed : prev.EndingBalance;
                        
                        decimal totalExpensesWithVat = 0;
                        if (hasExpenses)
                        {
                            foreach (var d in report.ExpensesOrReceipts)
                            {
                                if(d.ItemType == (int)ItemType.Expenses)
                                    totalExpensesWithVat += (d.Amount ?? 0) + (d.Vat ?? 0);
                            }
                        }

                        decimal totalReceipts = 0;
                        if (hasReceipts)
                        {
                            foreach (var detail in report.ExpensesOrReceipts)
                            {
                                if (detail.ItemType != (int)ItemType.Receipts || detail.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses) continue;
                                totalReceipts += (detail.Amount ?? 0);
                            }
                        }

                        report.EndingBalance = report.BeginningBalance - totalExpensesWithVat + totalReceipts;
                        _unitOfWork.ExpenseAndReceiptReports.Update(report);
                        await _unitOfWork.CompleteAsync();
                        await CascadeUpdateExpenseAndReceiptReportsAsync(report.EndingBalance, report.Year, report.Month);
                    }
                }
            }

            if (SourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses)
            {
                await DeleteExpensesReportsAsync(expenseReportId);
            }
        }

        public async Task DeleteExpensesReportsAsync(int? id)
        {
            if (id == null) return;
            var expenseAndReceiptReportId = id;

            // Update or remove ExpensesReport
            if (expenseAndReceiptReportId != 0)
            {
                var report = await _unitOfWork.ExpensesReports.GetByColumnWithIncludesAsync(
                    r => r.Id == expenseAndReceiptReportId,
                    q => q.Include(r => r.ExpenseAndReceiptAndOthers).ThenInclude(e => e.Supplier));

                if (report != null)
                {
                    var hasExpenses = report.ExpenseAndReceiptAndOthers != null && report.ExpenseAndReceiptAndOthers.Any() && report.ExpenseAndReceiptAndOthers.Any(x => x.ItemType == (int)ItemType.Expenses);
                    // only chapter-one receipts belong in ExpensesReport
                    var hasReceipts = report.ExpenseAndReceiptAndOthers != null && report.ExpenseAndReceiptAndOthers.Any() && report.ExpenseAndReceiptAndOthers.Any(x => x.ItemType == (int)ItemType.Receipts && x.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses);

                    if (!hasExpenses && !hasReceipts)
                    {
                        // Report will be deleted, cascade with previous month's ending balance
                        var prevQuery = _unitOfWork.ExpensesReports.GetAsync(r => r.Year < report.Year || (r.Year == report.Year && r.Month < report.Month));
                        var prev = await prevQuery.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();

                        _unitOfWork.ExpensesReports.Delete(report);
                        await _unitOfWork.CompleteAsync();
                        await CascadeUpdateExpensesReportsAsync(prev == null ? _beginningBalanceExpensesReportSeed : prev.EndingBalance, report.Year, report.Month);
                    }
                    else
                    {
                        // Recalculate BeginningBalance from previous month
                        var prevQuery = _unitOfWork.ExpensesReports.GetAsync(r => r.Year < report.Year || (r.Year == report.Year && r.Month < report.Month));
                        var prev = await prevQuery.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).FirstOrDefaultAsync();
                        report.BeginningBalance = prev == null ? _beginningBalanceExpensesReportSeed : prev.EndingBalance;

                        decimal totalExpensesWithVat = 0;
                        if (hasExpenses)
                        {
                            foreach (var d in report.ExpenseAndReceiptAndOthers)
                            {
                                if (d.ItemType == (int)ItemType.Expenses && d.Id != id)
                                    totalExpensesWithVat += (d.Amount ?? 0) + (d.Vat ?? 0);
                            }
                        }

                        decimal totalReceipts = 0;
                        if (hasReceipts)
                        {
                            foreach (var detail in report.ExpenseAndReceiptAndOthers)
                            {
                                if (detail.ItemType != (int)ItemType.Receipts || detail.Id == id || detail.ExpensesSourceId != (int)ExpensesSourceEnum.MiscellaneousExpenses) continue;
                                totalReceipts += (detail.Amount ?? 0);
                            }
                        }

                        report.EndingBalance = report.BeginningBalance - totalExpensesWithVat + totalReceipts;
                        _unitOfWork.ExpensesReports.Update(report);
                        await _unitOfWork.CompleteAsync();
                        await CascadeUpdateExpensesReportsAsync(report.EndingBalance, report.Year, report.Month);
                    }
                }
            }
        }



        private async Task CascadeUpdateExpenseAndReceiptReportsAsync(decimal startingEndingBalance, int year, int month)
        {
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
                        if (d.ItemType == (int)ItemType.Expenses)
                            totalExpensesWithVat += (d.Amount ?? 0) + (d.Vat ?? 0);
                    }
                }

                decimal totalReceipts = 0;
                if (rep.ExpensesOrReceipts?.Count() > 0)
                {
                    foreach (var r in rep.ExpensesOrReceipts)
                    {
                        if (r.ItemType != (int)ItemType.Receipts || r.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses) continue;
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

                decimal totalExpensesWithVat = 0;
                if (rep.ExpenseAndReceiptAndOthers?.Count() > 0)
                {
                    foreach (var d in rep.ExpenseAndReceiptAndOthers)
                    {
                        if (d.ItemType == (int)ItemType.Expenses)
                            totalExpensesWithVat += (d.Amount ?? 0) + (d.Vat ?? 0);
                    }
                }

                decimal totalReceipts = 0;
                if (rep.ExpenseAndReceiptAndOthers?.Count() > 0)
                {
                    foreach (var r in rep.ExpenseAndReceiptAndOthers)
                    {
                        // if (r.ItemType == ItemType.Receipts && r.ExpensesSourceId != (int)ExpensesSourceEnum.MiscellaneousExpenses) // All Receipts Not MiscellaneousExpenses --> To ExpensesReports
                        if (r.ItemType == (int)ItemType.Receipts)
                            totalReceipts += (r.Amount ?? 0);
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