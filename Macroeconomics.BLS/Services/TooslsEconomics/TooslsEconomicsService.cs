using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Data;
using ArgaamPlus.Shared;
using Common.BLS.Services.FiscalPeriods;
using Common.BLS.Services.Common;
using Macroeconomics.DAL.Entities;

using Macroeconomics.BLS.Services.ChartsEconomics;
using Macroeconomics.BLS.Services.MacroEconomics;
using Common.DAL.Entities;
using Common.BLS.Utils;
using ArgaamPlus.BLS.Services.MacroEconomics;
using Macroeconomics.BLS.Models.ToolsEconomics;

namespace ChartsEconomics.BLS.Services.ChartsEconomics
{
    public class TooslsEconomicsService : BaseService, ITooslsEconomicsService
    {
        private readonly ArgaamNext_IndicatorContext _context;
        private readonly IFiscalPeriodService _fiscalPeriodService;
        private readonly IArgaamPlusEconomicService _argaamPlusEconomicsService;


        public TooslsEconomicsService(ArgaamNext_IndicatorContext context, 
            IFiscalPeriodService fiscalPeriodService,
            IArgaamPlusEconomicService argaamPlusEconomicsService)
        {
            _context = context;
            _fiscalPeriodService = fiscalPeriodService;
            _argaamPlusEconomicsService = argaamPlusEconomicsService;
        }
        private IQueryable<EconomicIndicatorValue> GetIndicatorValues(List<int>? indicatorFieldIds, IEnumerable<short>? fiscalPeriodIds)
        {
            var indicatorValues = _context.EconomicIndicatorValues.Where(x =>
            (fiscalPeriodIds != null ? fiscalPeriodIds.Contains(x.EconomicIndicator.FiscalPeriodID) : true) &&
            (indicatorFieldIds != null ? indicatorFieldIds.Contains(x.EconomicIndicatorFieldID) : true));

            return indicatorValues;
        }


        public async Task<dynamic> ArgaamToolsPageQuery(ArgaamToolsPageQueryEnum indicatorsType)
        {
            switch (indicatorsType)
            {
                case ArgaamToolsPageQueryEnum.FuelPrices:
                    return await ArgaamToolsPageQueryFuelPrices();
                default:
                    return new List<dynamic>();
            }
        }

        private async Task<dynamic> ArgaamToolsPageQueryFuelPrices()
        {
            List<int> indicatorFieldIds = new List<int>() { 711, 686, 712, 77, 78, 79, 753, 760, 767, 774, 775, 776, 777, 778, 779, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 88, 81 };
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync()).Where(x => x.FiscalPeriodTypeID == 2 || x.FiscalPeriodTypeID == 4);

            var query = await GetIndicatorValues(indicatorFieldIds, fiscalPeriods.Select(x => x.FiscalPeriodID))
            .OrderBy(x => x.EconomicIndicator.ForYear)
            .ThenBy(x => x.EconomicIndicator.FiscalPeriodID)
            .Select(x => new
            {
                x.EconomicIndicatorID,
                x.EconomicIndicator.CountryID,
                x.EconomicIndicator.UpdatedOn,
                x.EconomicIndicator.IsPublished,
                x.EconomicIndicator.ForYear,
                x.EconomicIndicator.FiscalPeriodID,
                x.EconomicIndicator.SubGroupID,
                x.EconomicIndicatorFieldID,
                x.EconomicIndicatorField.DisplayNameAr,
                x.EconomicIndicatorField.DisplayNameEn,
                x.ValueAr,
                x.ValueEn
            })
            .ToListAsync();

            return query.Join(fiscalPeriods, x => x.FiscalPeriodID, y => y.FiscalPeriodID,
            (item, FiscalPeriod) => new
            {
                item.EconomicIndicatorID,
                item.CountryID,
                item.UpdatedOn,
                item.IsPublished,
                item.ForYear,
                item.FiscalPeriodID,
                item.SubGroupID,
                item.EconomicIndicatorFieldID,
                item.DisplayNameAr,
                item.DisplayNameEn,
                item.ValueAr,
                item.ValueEn,
                FiscalPeriod.FiscalPeriodTypeID,
                FiscalPeriodText = DatabaseFunctions.GetFiscalPeriodTextBankRanking(FiscalPeriod.FiscalPeriodTypeID, item.ForYear, item.FiscalPeriodID)
            })
            .OrderBy(x => x.FiscalPeriodTypeID)
            .ThenBy(x => x.ForYear)
            .ThenBy(x => x.FiscalPeriodID).ToList();
        }

        public async Task<dynamic> ArgaamToolsGovtBudgetPieChart()
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await GovtBudgetPieChart(); ;
            }, new { }, 180);

        }
        private async Task<dynamic> GovtBudgetPieChart()
        {
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync()).Where(x => x.FiscalPeriodTypeID == 3 || x.FiscalPeriodTypeID == 4);
            List<int> indicatorFieldIds = new List<int>() { 692, 688, 694, 695, 696, 697, 698 };
            List<int> indicatorFieldIdsForQuery2 = new List<int>() { 302 };


            var indicatorValues = GetIndicatorValues(indicatorFieldIds.Concat(indicatorFieldIdsForQuery2).ToList(),
            fiscalPeriods.Select(x => x.FiscalPeriodID));

            var query = (await indicatorValues.Where(x => indicatorFieldIds.Contains(x.EconomicIndicatorFieldID))
            .GroupBy(x => new { x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID })
            .ToListAsync())
            .Select(x => new
            {
                DisplayNameAr = "النفقات التشغيلية",
                DisplayNameEn = "Operating Expenditures",
                Distributionofexpendituresbysector = x.Sum(x => Conversions.StringToDecimalParsing(x.ValueEn)),
                ForYear = x.Key.ForYear,
                FiscalPeriodID = x.Key.FiscalPeriodID,
            });

            var query2 = await indicatorValues.Where(x => indicatorFieldIdsForQuery2.Contains(x.EconomicIndicatorFieldID))
            .Select(x => new
            {
                DisplayNameAr = "النفقات الرأسمالية (مليار)",
                DisplayNameEn = "Capital Expenditures (Billion)",
                Distributionofexpendituresbysector = Conversions.StringToDecimalParsing(x.ValueEn),
                ForYear = x.EconomicIndicator.ForYear,
                FiscalPeriodID = x.EconomicIndicator.FiscalPeriodID,
            }).ToListAsync();


            var query3 = query.Union(query2).ToList();

            var final = query3.Join(fiscalPeriods, x => x.FiscalPeriodID, y => y.FiscalPeriodID,
            (item, FiscalPeriod) => new
            {
                item.DisplayNameAr,
                item.DisplayNameEn,
                item.Distributionofexpendituresbysector,
                item.ForYear,
                item.FiscalPeriodID,
                FiscalPeriod.FiscalPeriodTypeID,
                FiscalPeriodText = DatabaseFunctions.GetFiscalPeriodTextBankRanking(FiscalPeriod.FiscalPeriodTypeID, item.ForYear, item.FiscalPeriodID)
            }).OrderByDescending(x => x.ForYear).ToList();
            return final;
        }

        public async Task<dynamic> ArgaamToolsGovtBudget()
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await GovtBudget(); ;
            }, new { }, 180);

        }
        private async Task<dynamic> GovtBudget()
        {
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync()).Where(x => x.FiscalPeriodTypeID == 2 || x.FiscalPeriodTypeID == 3 || x.FiscalPeriodTypeID == 4);
            List<int> economicIndicatorFieldIds = new List<int>() { 106, 103, 299, 300, 107, 302, 692, 688, 694, 123, 696, 697, 698, 108, 112, 113, 104, 121, 1203, 1204, 119, 114, 117, 116, 115, 296, 120, 118, 122, 303, 518, 516, 165, 168, 169, 705, 710 };
            
            var cte = await GetIndicatorValues(economicIndicatorFieldIds, fiscalPeriods.Select(x => x.FiscalPeriodID))
            .Select(eif => new
            {
                EconomicIndicatorID = eif.EconomicIndicatorFieldID == 106 ? "TotalActualRevenues"
                : eif.EconomicIndicatorFieldID == 103 ? "TotalRevenues"
                : eif.EconomicIndicatorFieldID == 299 ? "OilRevenues"
                : eif.EconomicIndicatorFieldID == 300 ? "NonOilRevenues"
                : eif.EconomicIndicatorFieldID == 107 ? "TotalActualExpenditures"
                : eif.EconomicIndicatorFieldID == 302 ? "CapitalExpenditures"
                : eif.EconomicIndicatorFieldID == 123 ? "Subsidies"
                : eif.EconomicIndicatorFieldID == 108 ? "ActualSurplus"
                : eif.EconomicIndicatorFieldID == 112 ? "OutstandingPublicDebtbyEndofPeriod"
                : eif.EconomicIndicatorFieldID == 113 ? "PublicDebttoGDP"
                : eif.EconomicIndicatorFieldID == 104 ? "TotalExpenditures"
                : eif.EconomicIndicatorFieldID == 121 ? "PublicAdministration"
                : eif.EconomicIndicatorFieldID == 1203 ? "Military"
                : eif.EconomicIndicatorFieldID == 1204 ? "SecurityandRegionalAdministration"
                : eif.EconomicIndicatorFieldID == 119 ? "MunicipalServices"
                : eif.EconomicIndicatorFieldID == 114 ? "Education"
                : eif.EconomicIndicatorFieldID == 117 ? "HealthandSocialDevelopment"
                : eif.EconomicIndicatorFieldID == 116 ? "EconomicResource"
                : eif.EconomicIndicatorFieldID == 115 ? "InfrastructureandTransportation"
                : eif.EconomicIndicatorFieldID == 296 ? "GeneralItems"
                : eif.EconomicIndicatorFieldID == 120 ? "DefenceandNationalSecurity"
                : eif.EconomicIndicatorFieldID == 118 ? "InfrastructureDevelopment"
                : eif.EconomicIndicatorFieldID == 122 ? "GovernmentSpecializedCreditInstitutions"
                : eif.EconomicIndicatorFieldID == 303 ? "GovernmentLendingInstitutions"
                : eif.EconomicIndicatorFieldID == 518 ? "GovtFinalConsumptionExpenditure"
                : eif.EconomicIndicatorFieldID == 516 ? "TotalCurrentprice"
                : eif.EconomicIndicatorFieldID == 165 ? "population"
                : eif.EconomicIndicatorFieldID == 168 ? "TotalofMerchandiseExports"
                : eif.EconomicIndicatorFieldID == 169 ? "TotalofMerchandiseImports"
                : eif.EconomicIndicatorFieldID == 705 ? "DomesticEndofPeriodBalance"
                : eif.EconomicIndicatorFieldID == 710 ? "ExternalEndofPeriodBalance"
                : null,
                ValueEn = decimal.Round(Conversions.StringToDecimalParsing(eif.ValueEn), 2),
                M = DatabaseFunctions.GetMonthByQuarter(eif.EconomicIndicator.FiscalPeriodID),
                eif.EconomicIndicator.ForYear,
                eif.EconomicIndicator.FiscalPeriodID,
            }).ToListAsync();

            var cte1 = cte.Join(fiscalPeriods, x => x.FiscalPeriodID, y => y.FiscalPeriodID,
            (item, FiscalPeriod) => new
            {
                item.EconomicIndicatorID,
                item.ValueEn,
                item.M,
                item.ForYear,
                item.FiscalPeriodID,
                FiscalPeriod.FiscalPeriodTypeID,
                FiscalPeriodText = DatabaseFunctions.GetFiscalPeriodTextBankRanking(FiscalPeriod.FiscalPeriodTypeID, item.ForYear, item.FiscalPeriodID)
            })
            .OrderBy(x => x.ForYear).ThenByDescending(x => x.FiscalPeriodTypeID).ThenBy(x => x.FiscalPeriodID)
            .GroupBy(e => new { e.ForYear, e.FiscalPeriodID, e.FiscalPeriodTypeID, e.FiscalPeriodText })
            .ToList();

            return cte1.Select(g => new
            {
                Y = g.Key.ForYear,
                D = DatabaseFunctions.GetOptionalDate(1, Convert.ToInt16(DatabaseFunctions.GetMonthByFiscalPeriodID(g.Key.FiscalPeriodID)), Convert.ToInt16(g.Key.ForYear)),
                FiscalPeriodID = g.Key.FiscalPeriodID,
                FiscalPeriodTypeID = g.Key.FiscalPeriodTypeID,
                FPT = g.Key.FiscalPeriodText,
                TotalActualRevenues = g.Where(e => e.EconomicIndicatorID == "TotalActualRevenues").DefaultIfEmpty().Min(x => x?.ValueEn),
                TotalRevenues = g.Where(e => e.EconomicIndicatorID == "TotalRevenues").DefaultIfEmpty().Min(x => x?.ValueEn),
                TotalActualRevenuesPerTotalRevenues = (g.Where(e => e.EconomicIndicatorID == "TotalActualRevenues").DefaultIfEmpty().Min(x => x?.ValueEn)) / g.Where(e => e.EconomicIndicatorID == "TotalRevenues").DefaultIfEmpty().Min(x => x?.ValueEn),
                OilRevenues = g.Where(e => e.EconomicIndicatorID == "OilRevenues").DefaultIfEmpty().Min(x => x?.ValueEn),
                OilRevenuesPerTotalActualRevenues = (g.Where(e => e.EconomicIndicatorID == "OilRevenues").DefaultIfEmpty().Min(x => x?.ValueEn) / g.Where(e => e.EconomicIndicatorID == "TotalActualRevenues").DefaultIfEmpty().Min(x => x?.ValueEn)) * 100,
                NonOilRevenues = g.Where(e => e.EconomicIndicatorID == "NonOilRevenues").DefaultIfEmpty().Min(x => x?.ValueEn),
                NonOilRevenuesPerTotalActualRevenues = (g.Where(e => e.EconomicIndicatorID == "NonOilRevenues").DefaultIfEmpty().Min(x => x?.ValueEn) / g.Where(e => e.EconomicIndicatorID == "TotalActualRevenues").DefaultIfEmpty().Min(x => x?.ValueEn) * 100),
                TotalActualExpenditures = g.Where(e => e.EconomicIndicatorID == "TotalActualExpenditures").DefaultIfEmpty().Min(x => x?.ValueEn),
                CapitalExpenditures = g.Where(e => e.EconomicIndicatorID == "CapitalExpenditures").DefaultIfEmpty().Min(x => x?.ValueEn),
                Subsidies = g.Where(e => e.EconomicIndicatorID == "Subsidies").DefaultIfEmpty().Min(x => x?.ValueEn),
                ActualSurplus = g.Where(e => e.EconomicIndicatorID == "ActualSurplus").DefaultIfEmpty().Min(x => x?.ValueEn),
                OutstandingPublicDebtbyEndofPeriod = g.Where(e => e.EconomicIndicatorID == "OutstandingPublicDebtbyEndofPeriod").DefaultIfEmpty().Min(x => x?.ValueEn),
                PublicDebttoGDP = g.Where(e => e.EconomicIndicatorID == "PublicDebttoGDP").DefaultIfEmpty().Min(x => x?.ValueEn),
                TotalExpenditures = g.Where(e => e.EconomicIndicatorID == "TotalExpenditures").DefaultIfEmpty().Min(x => x?.ValueEn),
                PublicAdministration = g.Where(e => e.EconomicIndicatorID == "PublicAdministration").DefaultIfEmpty().Min(x => x?.ValueEn),
                Military = g.Where(e => e.EconomicIndicatorID == "Military").DefaultIfEmpty().Min(x => x?.ValueEn),
                SecurityandRegionalAdministration = g.Where(e => e.EconomicIndicatorID == "SecurityandRegionalAdministration").DefaultIfEmpty().Min(x => x?.ValueEn),
                MunicipalServices = g.Where(e => e.EconomicIndicatorID == "MunicipalServices").DefaultIfEmpty().Min(x => x?.ValueEn),
                Education = g.Where(e => e.EconomicIndicatorID == "Education").DefaultIfEmpty().Min(x => x?.ValueEn),
                HealthandSocialDevelopment = g.Where(e => e.EconomicIndicatorID == "HealthandSocialDevelopment").DefaultIfEmpty().Min(x => x?.ValueEn),
                EconomicResource = g.Where(e => e.EconomicIndicatorID == "EconomicResource").DefaultIfEmpty().Min(x => x?.ValueEn),
                InfrastructureandTransportation = g.Where(e => e.EconomicIndicatorID == "InfrastructureandTransportation").DefaultIfEmpty().Min(x => x?.ValueEn),
                GeneralItems = g.Where(e => e.EconomicIndicatorID == "GeneralItems").DefaultIfEmpty().Min(x => x?.ValueEn),
                DefenceandNationalSecurity = g.Where(e => e.EconomicIndicatorID == "DefenceandNationalSecurity").DefaultIfEmpty().Min(x => x?.ValueEn),
                InfrastructureDevelopment = g.Where(e => e.EconomicIndicatorID == "InfrastructureDevelopment").DefaultIfEmpty().Min(x => x?.ValueEn),
                GovernmentSpecializedCreditInstitutions = g.Where(e => e.EconomicIndicatorID == "GovernmentSpecializedCreditInstitutions").DefaultIfEmpty().Min(x => x?.ValueEn),
                GovernmentLendingInstitutions = g.Where(e => e.EconomicIndicatorID == "GovernmentLendingInstitutions").DefaultIfEmpty().Min(x => x?.ValueEn),
                GovtExpendituretoGDP = (g.Where(e => e.EconomicIndicatorID == "GovtFinalConsumptionExpenditure").DefaultIfEmpty().Min(x => x?.ValueEn) / g.Where(e => e.EconomicIndicatorID == "TotalCurrentprice").DefaultIfEmpty().Min(x => x?.ValueEn)) * 100 ?? null,
                GovtExpenditureperCapita = g.Where(e => e.EconomicIndicatorID == "TotalCurrentprice").DefaultIfEmpty().Min(x => x?.ValueEn) / g.Where(e => e.EconomicIndicatorID == "population").DefaultIfEmpty().Min(x => x?.ValueEn),
                CurrentAccount = (g.Where(e => e.EconomicIndicatorID == "TotalofMerchandiseExports").DefaultIfEmpty().Min(x => x?.ValueEn)) - (g.Where(e => e.EconomicIndicatorID == "TotalofMerchandiseImports").DefaultIfEmpty().Min(x => x?.ValueEn)),
                CurrentAccounttoGDP = (g.Where(e => e.EconomicIndicatorID == "TotalofMerchandiseExports").DefaultIfEmpty().Min(x => x?.ValueEn)) - (g.Where(e => e.EconomicIndicatorID == "TotalofMerchandiseImports").DefaultIfEmpty().Min(x => x?.ValueEn)) / g.Where(e => e.EconomicIndicatorID == "TotalCurrentprice").DefaultIfEmpty().Min(x => x?.ValueEn),
                DomesticDebttoPublicDebt = (g.Where(e => e.EconomicIndicatorID == "DomesticEndofPeriodBalance").DefaultIfEmpty().Min(x => x?.ValueEn)) / g.Where(e => e.EconomicIndicatorID == "OutstandingPublicDebtbyEndofPeriod").DefaultIfEmpty().Min(x => x?.ValueEn),
                ExternalDebttoPublicDebt = (g.Where(e => e.EconomicIndicatorID == "ExternalEndofPeriodBalance").DefaultIfEmpty().Min(x => x?.ValueEn)) / g.Where(e => e.EconomicIndicatorID == "OutstandingPublicDebtbyEndofPeriod").DefaultIfEmpty().Min(x => x?.ValueEn),
            });
        }

        public async Task<dynamic> ArgaamToolsGetMarketQuarterlyAndYearlyTradingDataTransactions(string fiscalPeriodTypeIDs)
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await GetMarketQuarterlyAndYearlyTradingDataTransactions(fiscalPeriodTypeIDs);
            }, new { fiscalPeriodTypeIDs }, 180);

        }
        private async Task<dynamic> GetMarketQuarterlyAndYearlyTradingDataTransactions(string fiscalPeriodTypeIDs)
        {
            var fiscalPeriodTypeIds = fiscalPeriodTypeIDs.Split(',').Select(x => x.Trim());
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync()).Where(x => fiscalPeriodTypeIds.Contains(x.FiscalPeriodTypeID.ToString()));

            var query = await GetIndicatorValues(new List<int>() { 1164 }, fiscalPeriods.Select(x => x.FiscalPeriodID))
            .Where(x => x.EconomicIndicator.IsPublished)
            .Select(x => new
            {
                TransactionsThAvg = Conversions.StringToDecimalParsing(x.ValueEn),
                EconomicIndicatorFieldID = x.EconomicIndicatorFieldID,
                Y = x.EconomicIndicator.ForYear,
                M = ((DateTime)DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID)).ToString("MMM"),
                FD = ((DateTime)DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID)).ToString("yyyy-MM-dd"),
                x.EconomicIndicator.FiscalPeriodID,
            }).ToListAsync();

            var final = query.Join(fiscalPeriods, x => x.FiscalPeriodID, y => y.FiscalPeriodID,
            (item, FiscalPeriod) => new
            {
                item.TransactionsThAvg,
                item.EconomicIndicatorFieldID,
                item.Y,
                item.M,
                item.FD,
                fp = DatabaseFunctions.GetFiscalPeriodTextBankRanking(FiscalPeriod.FiscalPeriodTypeID, item.Y, item.FiscalPeriodID, false),
                item.FiscalPeriodID,
                FiscalPeriod.FiscalPeriodTypeID
            });
            return final;
        }

        public async Task<dynamic> ArgaamToolsTradingInformationGetEconomicIndicatorData()
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await TradingInformationGetEconomicIndicatorData();
            }, new { }, 180);
        }

        private async Task<dynamic> TradingInformationGetEconomicIndicatorData()
        {
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync()).Where(x => x.FiscalPeriodTypeID == 3 || x.FiscalPeriodTypeID == 4);
            List<int> economicIndicatorFieldIds = new List<int>() { 1168, 1169, 1171, 1172, 1173, 1174, 1162, 1163 };

            var query = await GetIndicatorValues(economicIndicatorFieldIds, fiscalPeriods.Select(x => x.FiscalPeriodID))
            .Where(x => x.EconomicIndicator.IsPublished)
            .Select(x => new
            {
                DataValue = Conversions.StringToDecimalParsing(x.ValueEn),
                EconomicIndicatorFieldID = x.EconomicIndicatorFieldID,
                ForYear = x.EconomicIndicator.ForYear,
                ShortNameEn = x.EconomicIndicatorFieldID == 1168 ? "No. of Individual Investors" : 
                    x.EconomicIndicatorFieldID == 1169 ? "No. of Institutional Investors":
                    x.EconomicIndicatorFieldID == 1171 ? "No. of Investment Portfolio":
                    x.EconomicIndicatorFieldID == 1172 ? "No. of Institutional Portfolio":
                    x.EconomicIndicatorField.DisplayNameEn,
                    ShortNameAr = x.EconomicIndicatorFieldID == 1168 ? "No. of Individual Investors":
                    x.EconomicIndicatorFieldID == 1169 ? "No. of Institutional Investors":
                    x.EconomicIndicatorFieldID == 1171 ? "No. of Investment Portfolio":
                    x.EconomicIndicatorFieldID == 1172 ? "No. of Institutional Portfolio":
                    x.EconomicIndicatorField.DisplayNameAr,
                ForDate = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID == 1 ? (short)5 : (x.EconomicIndicator.FiscalPeriodID)).ToString("yyyy-MM-dd"),
                x.EconomicIndicator.FiscalPeriodID,
            })
            .ToListAsync();

            var final = query.Join(fiscalPeriods, x => x.FiscalPeriodID, y => y.FiscalPeriodID,
            (item, FiscalPeriod) => new
            {
                item.DataValue,
                item.EconomicIndicatorFieldID,
                item.ForYear,
                item.ShortNameEn,
                item.ShortNameAr,
                item.ForDate,
                item.FiscalPeriodID,
                FiscalPeriod.FiscalPeriodTypeID,
                FiscalPeriodText = DatabaseFunctions.GetFiscalPeriodTextBankRanking(FiscalPeriod.FiscalPeriodTypeID, item.ForYear, item.FiscalPeriodID)
            }).OrderBy(x => x.ForYear).ThenBy(x => x.ForDate);
            return final;
        }

        public async Task<dynamic> ArgaamToolsOperatingExpendituresStack()
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await OperatingExpendituresStack();
            }, new { }, 180);
        }

        private async Task<dynamic> OperatingExpendituresStack()
        {
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync()).Where(x => x.FiscalPeriodTypeID == 3 || x.FiscalPeriodTypeID == 4);

            List<int> economicIndicatorFieldIds = new List<int>() { 692, 688, 694, 695, 696, 697, 698 };
            var query = await GetIndicatorValues(economicIndicatorFieldIds, fiscalPeriods.Select(x => x.FiscalPeriodID))
            .Select(x => new
            {
                EconomicIndicatorID = x.EconomicIndicatorFieldID == 692 ? "CompensationofEmployees" :
                    x.EconomicIndicatorFieldID == 688 ? "TaxesonGoodsandServices" :
                    x.EconomicIndicatorFieldID == 694 ? "FinancingExpenses" :
                    x.EconomicIndicatorFieldID == 695 ? "Subsidies" :
                    x.EconomicIndicatorFieldID == 696 ? "Grants" :
                    x.EconomicIndicatorFieldID == 697 ? "SocialBenefits" :
                    x.EconomicIndicatorFieldID == 698 ? "OtherExpenses" : null,
                ValueEn = decimal.Round(Conversions.StringToDecimalParsing(x.ValueEn), 2),
                M = DatabaseFunctions.GetMonthByQuarter(x.EconomicIndicator.FiscalPeriodID),
                x.EconomicIndicator.ForYear,
                x.EconomicIndicator.FiscalPeriodID,
            }).ToListAsync();

            var result = query.Join(fiscalPeriods, x => x.FiscalPeriodID, y => y.FiscalPeriodID,
            (item, FiscalPeriod) => new
            {
                item.EconomicIndicatorID,
                item.ValueEn,
                item.M,
                item.ForYear,
                item.FiscalPeriodID,
                FiscalPeriod.FiscalPeriodTypeID,
                FiscalPeriodText = DatabaseFunctions.GetFiscalPeriodTextBankRanking(FiscalPeriod.FiscalPeriodTypeID, item.ForYear, item.FiscalPeriodID)
            }).OrderByDescending(x => x.ForYear).ThenBy(x => x.FiscalPeriodID)
            .GroupBy(e => new { e.ForYear, e.FiscalPeriodID, e.FiscalPeriodTypeID, e.FiscalPeriodText })
            .ToList();

            return result.Select(x => new
            {
                x.Key.ForYear,
                D = DatabaseFunctions.GetOptionalDate(1, Convert.ToInt16(DatabaseFunctions.GetMonthByFiscalPeriodID(x.Key.FiscalPeriodID)), Convert.ToInt16(x.Key.ForYear)).ToString("yyyy-MM-dd"),
                x.Key.FiscalPeriodID,
                x.Key.FiscalPeriodTypeID,
                x.Key.FiscalPeriodText,
                CompensationofEmployees = x.Where(e => e.EconomicIndicatorID == "CompensationofEmployees").DefaultIfEmpty().Max(v => v?.ValueEn),
                TaxesonGoodsandServices = x.Where(e => e.EconomicIndicatorID == "TaxesonGoodsandServices").DefaultIfEmpty().Max(v => v?.ValueEn),
                FinancingExpenses = x.Where(e => e.EconomicIndicatorID == "FinancingExpenses").DefaultIfEmpty().Max(v => v?.ValueEn),
                Subsidies = x.Where(e => e.EconomicIndicatorID == "Subsidies").DefaultIfEmpty().Max(v => v?.ValueEn),
                Grants = x.Where(e => e.EconomicIndicatorID == "Grants").DefaultIfEmpty().Max(v => v?.ValueEn),
                SocialBenefits = x.Where(e => e.EconomicIndicatorID == "SocialBenefits").DefaultIfEmpty().Max(v => v?.ValueEn),
                OtherExpenses = x.Where(e => e.EconomicIndicatorID == "OtherExpenses").DefaultIfEmpty().Max(v => v?.ValueEn),
            });

        }

        public async Task<dynamic> ArgaamToolsEstimatedExpenditureAreasGovtBudgetPieChart()
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await EstimatedExpenditureAreasGovtBudgetPieChart();
            }, new { }, 180);
        }
        private async Task<dynamic> EstimatedExpenditureAreasGovtBudgetPieChart()
        {
            var fiscalPeriods = await _fiscalPeriodService.GetAllAsync(4);
            List<int> economicIndicatorFieldIds = new List<int>() { 123, 121, 1203, 1204, 119, 114, 117, 116, 115, 296, 120, 118, 122, 303 };
            
            var query = await GetIndicatorValues(economicIndicatorFieldIds, fiscalPeriods.Select(x => x.FiscalPeriodID))
            .Select(s => new
            {
                Fields = s.EconomicIndicatorFieldID == 123 ? "Subsidies" :
                    s.EconomicIndicatorFieldID == 121 ? "PublicAdministration" :
                    s.EconomicIndicatorFieldID == 1203 ? "Military" :
                    s.EconomicIndicatorFieldID == 1204 ? "SecurityandRegionalAdministration" :
                    s.EconomicIndicatorFieldID == 119 ? "MunicipalServices" :
                    s.EconomicIndicatorFieldID == 114 ? "Education" :
                    s.EconomicIndicatorFieldID == 117 ? "HealthandSocialDevelopment" :
                    s.EconomicIndicatorFieldID == 116 ? "EconomicResource" :
                    s.EconomicIndicatorFieldID == 115 ? "InfrastructureandTransportation" :
                    s.EconomicIndicatorFieldID == 296 ? "GeneralItems" :
                    s.EconomicIndicatorFieldID == 120 ? "DefenceandNationalSecurity" :
                    s.EconomicIndicatorFieldID == 118 ? "InfrastructureDevelopment" :
                    s.EconomicIndicatorFieldID == 122 ? "GovernmentSpecializedCreditInstitutions" :
                    s.EconomicIndicatorFieldID == 303 ? "GovernmentLendingInstitutions" : null,
                Distributionofexpendituresbysector = decimal.Parse(s.ValueEn),
                s.EconomicIndicatorField.DisplayNameAr,
                s.EconomicIndicatorField.DisplayNameEn,
                s.EconomicIndicatorFieldID,
                s.EconomicIndicator.ForYear,
                FinancialYear = s.EconomicIndicator.ForYear,
                s.EconomicIndicator.FiscalPeriodID,
            }).ToListAsync();
            
            
            return query.Join(fiscalPeriods, x => x.FiscalPeriodID, y => y.FiscalPeriodID,
            (item, FiscalPeriod) => new
            {
                item.Fields,
                item.Distributionofexpendituresbysector,
                item.DisplayNameAr,
                item.DisplayNameEn,
                item.EconomicIndicatorFieldID,
                item.FinancialYear,
                item.FiscalPeriodID,
                FiscalPeriod.FiscalPeriodTypeID,
                FiscalPeriodText = DatabaseFunctions.GetFiscalPeriodTextBankRanking(FiscalPeriod.FiscalPeriodTypeID, item.FinancialYear, item.FiscalPeriodID)
            }).OrderByDescending(x => x.FinancialYear).ThenByDescending(x => x.FiscalPeriodTypeID);
        }

        public async Task<dynamic> ArgaamToolsMargintoDailyTradingValue()
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await MargintoDailyTradingValue();
            }, new { }, 180);
        }
        private async Task<dynamic> MargintoDailyTradingValue()
        {
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync()).Where(x => x.FiscalPeriodTypeID == 3 || x.FiscalPeriodTypeID == 4);

            var cte = await _argaamPlusEconomicsService.Macro_MarketStocks();
            
            var cte_fp = cte.GroupBy(c => c.fp).Select(g => new
            {
                Key = g.Key,
                tvolumemqtravg = g.Average(cte => cte.tvolume) / 1000000,
            }).ToList();

            var cte_y = cte.GroupBy(c => c.y).Select(g => new
            {
                Key = g.Key,
                tvolumemyearlyavg = g.Average(cte => cte.tvolume) / 1000000,
            }).ToList();

            var cte2 = await GetIndicatorValues(new List<int>() { 1163 }, fiscalPeriods.Select(x => x.FiscalPeriodID))
            .Where(x => x.EconomicIndicator.IsPublished)
            .Select(s => new
            {
                DataValue = s.ValueEn.ToString(),
                s.EconomicIndicatorFieldID,
                s.EconomicIndicator.ForYear,
                ForDate = (s.EconomicIndicator.FiscalPeriodID == 1 ? DatabaseFunctions.GetDateByYearAndFiscalPeriodID(s.EconomicIndicator.ForYear, 5) : DatabaseFunctions.GetDateByYearAndFiscalPeriodID(s.EconomicIndicator.ForYear, s.EconomicIndicator.FiscalPeriodID)).ToString("yyyy-MM-dd"),
                s.EconomicIndicator.FiscalPeriodID,
            }).ToListAsync();

            var cte3 = cte2.Join(fiscalPeriods, x => x.FiscalPeriodID, y => y.FiscalPeriodID,
            (item, FiscalPeriod) => new
            {
                item.DataValue,
                item.EconomicIndicatorFieldID,
                item.ForYear,
                item.ForDate,
                FiscalPeriod.FiscalPeriodTypeID,
                FiscalPeriodText = DatabaseFunctions.GetFiscalPeriodTextBankRanking(FiscalPeriod.FiscalPeriodTypeID, item.ForYear, item.FiscalPeriodID)
            });

            var CT_QUARTER = cte_fp.Join(cte3, ct2 => ct2.Key, ct3 => ct3.FiscalPeriodText,
            (ct2, ct3) => new
            {
                FiscalPeriodTypeID = 3,
                ct3.FiscalPeriodText,
                ct3.ForYear,
                ct3.ForDate,
                DataValue = (Conversions.StringToDecimalParsing(ct3.DataValue) / ct2.tvolumemqtravg) * 100
            });

            var CT_Year = cte_y.Join(cte3.Where(ct => ct.FiscalPeriodTypeID == 4), ct2 => ct2.Key, ct3 => ct3.ForYear,
            (ct2, ct3) => new
            {
                FiscalPeriodTypeID = 4,
                ct3.FiscalPeriodText,
                ct3.ForYear,
                ct3.ForDate,
                DataValue = (Conversions.StringToDecimalParsing(ct3.DataValue) / ct2.tvolumemyearlyavg) * 100
            });

            return CT_QUARTER.Concat(CT_Year).OrderBy(data => data.ForDate).ToList();
        }
    }
}


