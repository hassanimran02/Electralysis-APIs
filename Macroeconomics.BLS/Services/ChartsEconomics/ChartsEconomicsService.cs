using Microsoft.EntityFrameworkCore;
using System.Data;
using ArgaamPlus.Shared;
using Common.BLS.Services.FiscalPeriods;
using Common.BLS.Services.Common;
using Macroeconomics.DAL.Entities;
using Macroeconomics.BLS.Services.ChartsEconomics;
using Macroeconomics.BLS.Services.MacroEconomics;
using Common.BLS.Utils;
using ArgaamPlus.BLS.Services.MacroEconomics;
using System.Globalization;
using ArgaamPlus.BLS.Models.SpModel;

namespace ChartsEconomics.BLS.Services.ChartsEconomics
{
    public class ChartsEconomicsService : BaseService, IChartsEconomicsService
    {
        private readonly ArgaamNext_IndicatorContext _context;
        private readonly IFiscalPeriodService _fiscalPeriodService;
        private readonly IMacroEconomicsService _economicsService;
        private readonly IArgaamPlusEconomicService _argaamPlusEconomicsService;


        public ChartsEconomicsService(ArgaamNext_IndicatorContext context,
            IFiscalPeriodService fiscalPeriodService,
            IArgaamPlusEconomicService argaamPlusEconomicsService,
            IMacroEconomicsService economicsService)
        {
            _context = context;
            _fiscalPeriodService = fiscalPeriodService;
            _economicsService = economicsService;
            _argaamPlusEconomicsService = argaamPlusEconomicsService;
        }

        public async Task<dynamic> MacroEconomicData(string indicatorFieldIDs, int fiscalPeriodTypeID, int fromYear, int toYear, int lang, int companyID)
        {
            var indicatorFieldIDList = indicatorFieldIDs.Split(',').Where(x => int.TryParse(x, out int number)).Select(x => int.Parse(x)).ToList();
            return await Executor.Instance.GetData(async () =>
            {
                return await Charts_GetMacroeconomyData(lang, fromYear, toYear, indicatorFieldIDList, fiscalPeriodTypeID, companyID);
            }, new { lang, fromYear, toYear, indicatorFieldIDs, fiscalPeriodTypeID, companyID }, 180);
        }
        private static string GetFiscalPeriodDescriptiveText(string argaamSectorName, string fiscalPeriodValue)
        {
            string descriptive = string.Empty;
            string reitSectorAppend = (fiscalPeriodValue == "REITs") ? "Reits_" : "";
            argaamSectorName = argaamSectorName.ToUpper();
            switch (argaamSectorName)
            {
                case "YEAR":
                    descriptive = "Year";
                    break;
                case "Q1":
                case "Q2":
                case "Q3":
                case "Q4":
                case "QUARTER1":
                case "QUARTER2":
                case "QUARTER3":
                case "QUARTER4":
                    descriptive = reitSectorAppend + "Quarter";
                    break;
                case "I1":
                case "I2":
                case "I3":
                case "I4":
                    descriptive = reitSectorAppend + "Annualize";
                    break;
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "10":
                case "11":
                case "12":
                    descriptive = reitSectorAppend + "Month";
                    break;
            }

            return descriptive;
        }

        private async Task<dynamic> Charts_GetMacroeconomyData(int lang, int fromYear, int toYear, List<int> indicatorFieldIDs, int fiscalPeriodTypeID, int companyID)
        {
            fromYear = fromYear < 2000 ? 1950 : fromYear;
            var argaamSectors = await _argaamPlusEconomicsService.GetCompanyArgaamSector(companyID);
            //var argaamSectors = await getArgaamSectorByComanyId(companyID);


            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeID));

            var query = _context.EconomicIndicatorValues.Where(x =>
            indicatorFieldIDs.Contains(x.EconomicIndicatorFieldID) &&
            x.EconomicIndicator.ForYear >= fromYear &&
            x.EconomicIndicator.ForYear <= toYear &&
            fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID))
            .Select(x => new
            {
                CompanyId = companyID,
                EntityID = x.EconomicIndicatorField.EconomicIndicatorFieldID,
                Labels = DatabaseFunctions.GetFiscalPeriodTextBankRanking(fiscalPeriodTypeID, x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID, true),
                ForDate = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID),
                ForYear = x.EconomicIndicator.ForYear,
                IndicatorValue = Conversions.StringToDecimalParsing(x.ValueAr),
                FiscalPeriodID = x.EconomicIndicator.FiscalPeriodID,
            })
            .ToList();

            var queryWithFiscal = query.Join(fiscalPeriods, x => x.FiscalPeriodID, y => y.FiscalPeriodID,
            (x, FiscalPeriod) => new
            {
                CompanyId = companyID,
                EntityID = x.EntityID,
                Labels = x.Labels,
                ForDate = x.ForDate,
                ForYear = x.ForYear,
                FiscalPeriodValue = GetFiscalPeriodDescriptiveText(FiscalPeriod.FiscalPeriodValue, "NotReits"),
                IndicatorValue = x.IndicatorValue,
                FiscalPeriodID = x.FiscalPeriodID
            }).ToList();

            var queryWithArgaamSectors = queryWithFiscal.Join(argaamSectors, x => x.CompanyId, y => y.CompanyID,
            (x, ArgamSector) => new
            {
                EntityID = x.EntityID,
                EntityName = lang == 1 ? ArgamSector.ArgaamSectorNameAr : ArgamSector.ArgaamSectorNameEn,
                Labels = x.Labels,
                ForDate = x.ForDate,
                ForYear = x.ForYear,
                FiscalPeriodValue = x.FiscalPeriodValue,
                IndicatorValue = x.IndicatorValue
            }).OrderBy(x => x.ForDate).ToList();

            return queryWithArgaamSectors;
        }


        private async Task<dynamic> Macro_MargintoDailyTradingValueData(int fromYear, int toYear, int fiscalPeriodTypeID, int lang)
        {
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeID));

            var ct2 = await _argaamPlusEconomicsService.Macro_MargintoDailyTradingValueData(fromYear, toYear, fiscalPeriodTypeID);

            var cte3Query = await _context.EconomicIndicatorValues.Where(x =>
            x.EconomicIndicatorField.EconomicIndicatorFieldID == 1163 &&
            x.EconomicIndicator.ForYear >= fromYear &&
            x.EconomicIndicator.ForYear <= toYear &&
            fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID))
            .Select(x => new
            {
                DataValue = Conversions.StringToDecimalParsing(x.ValueEn),
                x.EconomicIndicatorFieldID,
                x.EconomicIndicator.ForYear,
                ForDate = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, (x.EconomicIndicator.FiscalPeriodID == 1 ? (short)5 : x.EconomicIndicator.FiscalPeriodID)),
                x.EconomicIndicator.FiscalPeriodID,
                FiscalPeriodTypeID = fiscalPeriodTypeID,
                FiscalPeriodText = DatabaseFunctions.GetFiscalPeriodTextBankRanking(fiscalPeriodTypeID, x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID, true)
            }).ToListAsync();


            var queryWithArgaamSectors = cte3Query.Join(ct2,
                x => new { FiscalPeriodTypeID = x.FiscalPeriodTypeID, ForYear = x.ForYear, FiscalPeriodText = x.FiscalPeriodText },
                y => new { FiscalPeriodTypeID = y.FiscalPeriodTypeID ?? 0, ForYear = y.y, FiscalPeriodText = y.fp ?? "" },
            (ct3, ct2) => new
            {
                EntityName = lang == 1 ? "نسبة التمويل المستخدم لحجم التداول" : "Margin to Daily Trading Value",
                EntityID = 1163,
                Labels = DatabaseFunctions.GetFiscalPeriodTextBankRanking(ct3.FiscalPeriodTypeID, ct3.ForYear, ct3.FiscalPeriodID),
                FiscalPeriodValue = fiscalPeriodTypeID == 4 ? "Year" : (fiscalPeriodTypeID == 3 || fiscalPeriodTypeID == 7) ? "Quarter" : null,
                ct3.ForDate,
                ct3.ForYear,
                Value = (ct3.DataValue / (fiscalPeriodTypeID == 3 ? ct2.tvolumemqtravg : ct2.tvolumemyearlyavg)) * 100
            }).Distinct().OrderBy(result => result.ForDate).ToList();

            return queryWithArgaamSectors;
        }
        public async Task<dynamic> MargintoDailyTradingValueData(short fromYear, short toYear, int fiscalPeriodType, int lang)
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await Macro_MargintoDailyTradingValueData(fromYear, toYear, fiscalPeriodType, lang);
            }, new { fromYear, toYear, fiscalPeriodType, lang }, 300);
        }


        private async Task<dynamic> Macro_Charts_GetMarketTradingValue(short fromYear, short toYear, int lang, string indicatorFieldIDs)
        {
            var qtrEntityName = lang == 1 ? "عدل الربعي" : "Quarterly average";
            var yearEntityName = lang == 1 ? "عدل السنوي" : "Yearly average";

            var tradingResults1 = await _argaamPlusEconomicsService.Macro_Charts_GetMarketTurnOverDailyData(fromYear, toYear, lang);

            var indicatorFieldIDList = indicatorFieldIDs.Split(',').Where(x => int.TryParse(x, out int number)).Select(x => int.Parse(x)).ToList();

            var fiscalPeriodTypeId = 3;
            var tradingResults2 = await Executor.Instance.GetData(async () =>
            {
                return await _economicsService.PP_Indicator_FieldData(lang, fromYear, toYear, indicatorFieldIDList, fiscalPeriodTypeId, qtrEntityName);
            }, new { lang, fromYear, toYear, indicatorFieldIDList, fiscalPeriodTypeId, qtrEntityName }, 180);

            fiscalPeriodTypeId = 4;
            var tradingResults3 = await Executor.Instance.GetData(async () =>
            {
                return await _economicsService.PP_Indicator_FieldData(lang, fromYear, toYear, indicatorFieldIDList, fiscalPeriodTypeId, yearEntityName);
            }, new { lang, fromYear, toYear, indicatorFieldIDList, fiscalPeriodTypeId, qtrEntityName }, 180);

            List<Macro_Charts_GetMarketTurnOverDailyDataModel> res = new List<Macro_Charts_GetMarketTurnOverDailyDataModel>();
            res.AddRange(tradingResults1);
            res.AddRange(tradingResults2);
            res.AddRange(tradingResults3);
            return res;
        }
        public async Task<dynamic> MacroChartsGetMarketTradingValue(short fromYear, short toYear, int lang, string indicatorFieldIDs)
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await Macro_Charts_GetMarketTradingValue(fromYear, toYear, lang, indicatorFieldIDs);
            }, new { fromYear, toYear, lang, indicatorFieldIDs }, 180);
        }


        private async Task<dynamic> Macro_Charts_GetBankAssetsByGDP_Current(int lang, int fromYear, int toYear, int fiscalPeriodTypeID, string attributeIds, string indicatorFieldIds)
        {
            fromYear = fromYear < 2000 ? 1950 : fromYear;

            var cte_FinalValues = await _argaamPlusEconomicsService.Macro_Charts_GetBankAssetsByGDP_Current(fromYear, toYear, fiscalPeriodTypeID, lang, attributeIds);


            var indicatorFieldIdsList = indicatorFieldIds.Split(',').Where(x => int.TryParse(x, out int number)).Select(x => int.Parse(x)) ?? new List<int>();
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeID));

            var indicatorData = _context.EconomicIndicatorValues.Where(x =>
            indicatorFieldIdsList.Contains(x.EconomicIndicatorFieldID) &&
            x.EconomicIndicator.ForYear >= fromYear &&
            x.EconomicIndicator.ForYear <= toYear &&
            fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID))
            .Select(x => new
            {
                EntityID = x.EconomicIndicatorFieldID,
                EntityName = (lang == 1 ? x.EconomicIndicatorField.DisplayNameAr : x.EconomicIndicatorField.DisplayNameEn.Replace("'", "")),
                Labels = DatabaseFunctions.GetFiscalPeriodTextBankRanking(fiscalPeriodTypeID, x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID, true),
                ForDate = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, 1),
                ForYear = x.EconomicIndicator.ForYear,
                Value = Conversions.StringToDecimalParsing(x.ValueAr),
                FiscalPeriodID = x.EconomicIndicator.FiscalPeriodID,
            })
            .ToList();


            var result = indicatorData.Join(cte_FinalValues,
            ind => new { FiscalPeriodID = ind.FiscalPeriodID, ForYear = ind.ForYear },
            attr => new { FiscalPeriodID = (short)attr.FiscalPeriodID, ForYear = attr.ForYear },
            (ind, attr) => new
            {
                EntityID = attr.AttributeId,
                EntityName = attr.DisplayName,
                Labels = ind.ForYear,
                ForDate = ind.ForDate,
                attr.ForYear,
                attr.FiscalPeriodValue,
                Value = (attr.Value / (ind.Value)) * 100
            })
            .OrderBy(entry => entry.ForDate)
            .ToList();

            return result;
        }
        public async Task<dynamic> GetBankAssetsByGDPCurrent(int lang, int fromYear, int toYear, int fiscalPeriodTypeID, string attributeIds, string indicatorFieldIds)
        {
            return await Executor.Instance.GetData(async () =>
            {
                var result =  await Macro_Charts_GetBankAssetsByGDP_Current(lang, fromYear, toYear, fiscalPeriodTypeID, attributeIds, indicatorFieldIds);
                return  result;
            }, new { lang, fromYear, toYear, fiscalPeriodTypeID, attributeIds, indicatorFieldIds }, 180);
        }


        private async Task<dynamic> Charts_ImportPerCapita(int lang, int fromYear, int toYear, int fiscalPeriodTypeID, string indicatorFieldIds)
        {
            fromYear = (DateTime.Now.Year - fromYear == 30) ? (fromYear - 100) : fromYear;
            var indicatorFieldIdsList = indicatorFieldIds.Split(',').Where(x => int.TryParse(x, out int number)).Select(x => int.Parse(x)) ?? new List<int>();
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeID));

            var cte = _context.EconomicIndicatorValues.Where(x =>
            indicatorFieldIdsList.Contains(x.EconomicIndicatorFieldID) &&
            x.EconomicIndicator.ForYear >= fromYear &&
            x.EconomicIndicator.ForYear <= toYear &&
            fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID))
            .Select(x => new
            {
                EntityID = x.EconomicIndicatorFieldID,
                EntityName = (lang == 1) ? x.EconomicIndicatorField.DisplayNameAr : x.EconomicIndicatorField.DisplayNameEn.Replace("'", ""),
                Labels = DatabaseFunctions.GetFiscalPeriodTextBankRanking(fiscalPeriodTypeID, x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID, true),
                ForDate = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID),
                ForYear = x.EconomicIndicator.ForYear,
                FiscalPeriodValue = DatabaseFunctions.GetFiscalPeriodDescriptiveText(fiscalPeriodTypeID),
                Value = Conversions.StringToDecimalParsing(x.ValueEn),
                FiscalPeriodID = x.EconomicIndicator.FiscalPeriodID
            }).OrderBy(x => x.EntityID).ToList();

            return cte.Where(x => x.EntityID == 169)
            .OrderBy(x => x.ForYear).ThenBy(x => x.FiscalPeriodID)
            .Select(x => new
            {
                EntityID = x.EntityID,
                EntityName = x.EntityName,
                Labels = x.Labels,
                ForDate = x.ForDate,
                ForYear = x.ForYear,
                FiscalPeriodValue = x.FiscalPeriodValue,
                Value = decimal.Round((x.Value / ((cte.FirstOrDefault(y => y.ForYear == x.ForYear && y.FiscalPeriodID == x.FiscalPeriodID)?.Value) ?? 0)), 20, MidpointRounding.ToZero),
            }).ToList();
        }
        public async Task<dynamic> ChartsImportPerCapita(int lang, int fromYear, int toYear, int fiscalPeriodTypeID, string indicatorFieldIds)
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await Charts_ImportPerCapita(lang, fromYear, toYear, fiscalPeriodTypeID, indicatorFieldIds);
            }, new { lang, fromYear, toYear, fiscalPeriodTypeID, indicatorFieldIds }, 180);
        }


        private async Task<List<Macro_Charts_ConsumerMortgageAttributesModel>> Macro_Charts_SamaAttributeFieldPrices(string indicatorFieldIDs, int fromYear, int toYear, int fiscalPeriodTypeID, int lang, string field, string source)
        {
            fromYear = (fromYear < 2000) ? 1950 : fromYear;
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeID));

            var indicatorFieldIDList = indicatorFieldIDs.Split(',').Where(x => int.TryParse(x, out int number)).Select(x => int.Parse(x)).ToList();


            if (source == "attr")
            {
                return await _argaamPlusEconomicsService.Macro_Charts_ConsumerMortgageAttributes(indicatorFieldIDs, (short)fromYear, (short)toYear, lang, fiscalPeriodTypeID, field);
            }
            else if (field == "907169,90716912")
            {
                var res = await Macro_Charts_SamaAttributeFieldPrices(indicatorFieldIDs, fromYear, toYear, fiscalPeriodTypeID, lang, "907169", source);
                foreach (var item in res)
                {
                    item.EntityName = "Total Reserves Assets %";
                    item.EntityID = 907169;
                }
                res.AddRange(await Macro_Charts_SamaAttributeFieldPrices(indicatorFieldIDs, fromYear, toYear, fiscalPeriodTypeID, lang, "90716912", source));
                res = res.OrderBy(e => e.ForDate).ToList();
                return res;
            }
            else
            {
                var compulsaryFieldIds = new List<int>() { 125, 169, 24 };
                var records = await _context.EconomicIndicatorValues.Where(x =>
                x.EconomicIndicator.ForYear >= (fromYear - 1) &&
                x.EconomicIndicator.ForYear <= toYear &&
                (fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID) || (x.EconomicIndicator.FiscalPeriodID == 1)) &&
                (indicatorFieldIDList.Contains(x.EconomicIndicatorFieldID) || compulsaryFieldIds.Contains(x.EconomicIndicatorFieldID)))
                .Include(x => x.EconomicIndicatorField)
                .Include(x => x.EconomicIndicator)
                .ToListAsync();

                var query = records.Where(x =>
                x.EconomicIndicator.ForYear >= fromYear &&
                x.EconomicIndicator.ForYear <= toYear &&
                indicatorFieldIDList.Contains(x.EconomicIndicatorFieldID) &&
                (fiscalPeriods.Where(x => x.FiscalPeriodTypeID == fiscalPeriodTypeID).Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID)) &&
                x.EconomicIndicator.SubGroupID == (x.EconomicIndicatorField.EconomicIndicatorFieldID == 21 ? 22 : x.EconomicIndicator.SubGroupID))
                .Select(x => new
                {
                    EntityID = x.EconomicIndicatorFieldID,
                    EntityName = lang == 1 ? Conversions.ReplaceGarbage(x.EconomicIndicatorField.DisplayNameAr) : Conversions.ReplaceGarbage(x.EconomicIndicatorField.DisplayNameEn),
                    ForDate = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID),
                    Labels = DatabaseFunctions.GetFiscalPeriodTextBankRanking(fiscalPeriodTypeID, x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID),
                    ForYear = x.EconomicIndicator.ForYear,
                    FiscalPeriodID = x.EconomicIndicator.FiscalPeriodID,
                    Value = Conversions.StringToDecimalParsing(x.ValueEn),
                    EconomicIndicatorFieldID = x.EconomicIndicatorFieldID,
                    SubGroupID = x.EconomicIndicator.SubGroupID
                })
                .OrderBy(x => x.ForDate)
                .ToList();

                var res = new List<Macro_Charts_ConsumerMortgageAttributesModel>();
                foreach (var item in query)
                {
                    var FiscalPeriod = Conversions.DefaultValueForNullables(fiscalPeriods.FirstOrDefault(x => x.FiscalPeriodID == item.FiscalPeriodID));
                    var PrevValue = Conversions.StringToDecimalParsing(records.FirstOrDefault(x =>
                    x.EconomicIndicator.ForYear == (item.ForYear - 1) &&
                    x.EconomicIndicator.FiscalPeriodID == item.FiscalPeriodID &&
                    x.EconomicIndicatorFieldID == item.EconomicIndicatorFieldID &&
                    x.EconomicIndicator.SubGroupID == item.SubGroupID)?.ValueEn);
                    decimal? value = null;
                    if (FiscalPeriod.FiscalPeriodID > 0)
                    {
                        if (field == "growth")
                        {
                            value = PrevValue == 0 ? null : (((item.Value / PrevValue) - 1) * 100);
                        }
                        else if (field == "90716912" || field == "907125" || field == "907169")
                        {
                            int tempValue = Convert.ToInt32(Conversions.StringToDecimalParsing(records.FirstOrDefault(x =>
                            x.EconomicIndicatorFieldID == (field == "907125" ? 125 : 169) &&
                            x.EconomicIndicator.ForYear == (item.ForYear - 1) &&
                            x.EconomicIndicator.FiscalPeriodID == 1)?.ValueEn));

                            value = tempValue == 0 ? null : ((item.Value / tempValue) * (field == "90716912" ? 12 : 100));

                        }
                        else if (field == "2124" && indicatorFieldIDs == "21")
                        {
                            int tempValue = Convert.ToInt32(Conversions.StringToDecimalParsing(records.FirstOrDefault(x =>
                            x.EconomicIndicatorFieldID == 24 &&
                            x.EconomicIndicator.ForYear == (item.ForYear) &&
                            x.EconomicIndicator.FiscalPeriodID == item.FiscalPeriodID)?.ValueEn));

                            value = tempValue == 0 ? null : ((item.Value / tempValue) * 100);
                        }
                        else
                        {
                            value = item.Value;
                        }
                    }
                    res.Add(new Macro_Charts_ConsumerMortgageAttributesModel()
                    {
                        EntityID = item.EntityID,
                        EntityName = item.EntityName,
                        ForDate = item.ForDate,
                        Labels = item.Labels,
                        ForYear = item.ForYear,
                        FiscalPeriodValue = DatabaseFunctions.GetFiscalPeriodDescriptiveText(FiscalPeriod.FiscalPeriodValue, ""),
                        Value = decimal.Round((value ?? 0), 11),
                    });
                }
                return res.OrderBy(x => x.ForYear).ToList();
            }
        }
        public async Task<dynamic> SamaAttributeFieldPrices(string indicatorFieldIDs, short fromYear, short toYear, int fiscalPeriodType, int lang, string field, string source)
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await Macro_Charts_SamaAttributeFieldPrices(indicatorFieldIDs, fromYear, toYear, fiscalPeriodType, lang, field, source);
            }, new { indicatorFieldIDs, fromYear, toYear, fiscalPeriodType, lang, field, source }, 180);
        }


        private async Task<dynamic> Macro_Charts_MarketTradingComputedMacroData(int fromYear, int toYear, int lang, string indicatorFieldIds)
        {
            var indicatorFieldIdsList = indicatorFieldIds.Split(',').Where(x => int.TryParse(x, out int number)).Select(x => int.Parse(x)).ToList() ?? new List<int>();

            var query = await _argaamPlusEconomicsService.argaamPlusContext.MarketTradingDataTools.Where(x =>
            indicatorFieldIdsList.Contains(x.EntityID ?? 0) &&
            ((x.EntityID == 10003 || x.EntityID == 10011) ? (x.ForYear > 2006) : true) &&
            x.ForYear >= fromYear &&
            x.ForYear <= toYear &&
            x.ForDate != new DateTime(2020, 06, 14))
            .Select(x => new
            {
                x.EntityID,
                EntityName = lang == 2 ? x.EnglishName : x.ArabicName,
                x.ForDate,
                x.Labels,
                x.ForYear,
                x.FiscalPeriodValue,
                Value = x.DataValue
            }).ToListAsync();

            return query;
        }
        public async Task<dynamic> TradingData(string attributesIds, short fromYear, short toYear, int lang)
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await Macro_Charts_MarketTradingComputedMacroData(fromYear, toYear, lang, attributesIds);
            }, new { fromYear, toYear, lang, attributesIds }, 180);
        }



        static CultureInfo provider = CultureInfo.InvariantCulture;
        public async Task Charts_INSERT_DATA_INTO_MarketTradingDataTools()
        {
            var cte2 = _context.EconomicIndicatorValues
            .Where(x => x.EconomicIndicator.IsPublished == true && x.EconomicIndicatorFieldID == 1164)
            .Select(x => new
            {
                TransactionsThAvg = x.ValueEn == null ? (decimal?)(null) : Conversions.StringToDecimalParsing(x.ValueEn),
                EconomicIndicatorFieldID = x.EconomicIndicatorFieldID,
                Y = x.EconomicIndicator.ForYear,
                M = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID).ToString("MMM"),
                FD = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID),
                FiscalPeriodID = x.EconomicIndicator.FiscalPeriodID,
                x.EconomicIndicator.ForYear,
            }).ToList();

            var fiscalPeriods = await _fiscalPeriodService.GetAllAsync();

            var CTE_Transacations = cte2.Join(fiscalPeriods,
            x => new { FiscalPeriodID = x.FiscalPeriodID },
            y => new { FiscalPeriodID = y.FiscalPeriodID },
            (x, FiscalPeriod) => new
            {
                TransactionsThAvg = x.TransactionsThAvg ?? 0,
                EconomicIndicatorFieldID = x.EconomicIndicatorFieldID,
                Y = x.Y,
                M = x.M,
                FD = x.FD,
                FiscalPeriodID = x.FiscalPeriodID,
                FiscalPeriodTypeID = FiscalPeriod.FiscalPeriodTypeID,
                fp = DatabaseFunctions.GetFiscalPeriodTextBankRanking(FiscalPeriod.FiscalPeriodTypeID, x.ForYear, x.FiscalPeriodID)
            }).ToList();


      
            var datacountfromCTE = await _argaamPlusEconomicsService.Charts_INSERT_DATA_INTO_MarketTradingDataTools(CTE_Transacations);
        }

       
    }
}

