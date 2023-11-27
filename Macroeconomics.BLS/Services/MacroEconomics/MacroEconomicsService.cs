using Macroeconomics.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Macroeconomics.BLS.Models.MacroEconomics;
using System.Text.Json;
using System.Data;
using Macroeconomics.BLS.Services.EconomicIndicatorValues;
using ArgaamPlus.Shared;
using Common.BLS.Services.FiscalPeriods;
using Common.BLS.Services.Common;
using Common.BLS.Utils;
using ArgaamPlus.BLS.Models.SpModel;
using RestSharp;
using Macroeconomics.BLS.Models.FormulaFieldConfig;

namespace Macroeconomics.BLS.Services.MacroEconomics
{
    public class MacroEconomicsService : BaseService, IMacroEconomicsService
    {
        private readonly ArgaamNext_IndicatorContext _context;
        private readonly IFiscalPeriodService _fiscalPeriodService;
        private readonly IEconomicIndicatorValueService _valueService;
        private readonly RestClient _macroApiClient;
        public MacroEconomicsService(ArgaamNext_IndicatorContext context,
            IFiscalPeriodService fiscalPeriodService,
            IEconomicIndicatorValueService valueService)
        {
            _context = context;
            _fiscalPeriodService = fiscalPeriodService;
            _valueService = valueService;
            _macroApiClient = new RestClient("https://macroeconapi.edanat.com/");
        }

        public async Task<dynamic> GetIndicatorsFieldValue(int indicatorId, int year, int fiscalPeriodTypeId, int fiscalPeriodId)
        {
            year = year > 0 ? year : DateTime.Now.Year;
            var indicatorFieldValues = await Executor.Instance.GetData(async () =>
            {
                 return await _valueService.GetIndicatorsFieldValues(indicatorId, year, fiscalPeriodTypeId, fiscalPeriodId);
            }, new { indicatorId, year, fiscalPeriodTypeId, fiscalPeriodId }, 180);
            if (indicatorFieldValues != null && indicatorFieldValues.Count() > 0)
            {
                var indicatorField = indicatorFieldValues.FirstOrDefault();
                if (indicatorField != null)
                {
                    return new
                    {
                        EconomicIndicatorFieldID = indicatorField.EconomicIndicatorFieldId,
                        indicatorField.DisplayNameAr,
                        DisplayNameEn = indicatorField.DisplayNameEn.Replace("'", ""),
                        Values = indicatorFieldValues.Select(x => new
                        {
                            x.Year,
                            x.FiscalPeriodTypeID,
                            x.FiscalPeriodID,
                            x.Value
                        })
                    };
                }
            }
            return "";
        }


        public async Task<dynamic> GetMacroComparableIndicatorsFieldData(int lang, int fromYear, int toYear, string indicatorFieldIDs, int fiscalPeriodTypeID)
        {
            var indicatorFieldIDList = indicatorFieldIDs.Split(',').Where(x => int.TryParse(x, out int number)).Select(x => int.Parse(x)).ToList();
            return await Executor.Instance.GetData(async () =>
            {
                return await PP_Indicator_FieldData(lang, fromYear, toYear, indicatorFieldIDList, fiscalPeriodTypeID);
            }, new { lang, fromYear, toYear, indicatorFieldIDs, fiscalPeriodTypeID }, 180);
        }
        public async Task<List<Macro_Charts_GetMarketTurnOverDailyDataModel>> PP_Indicator_FieldData(int lang, int fromYear, int toYear, List<int> indicatorFieldIDs, int fiscalPeriodTypeID, string entityName = null)
        {
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeID));

            var query = await _context.EconomicIndicatorValues.AsNoTracking().Where(x =>
            indicatorFieldIDs.Contains(x.EconomicIndicatorFieldID) &&
            x.EconomicIndicator.ForYear <= toYear &&
            x.EconomicIndicator.ForYear >= fromYear &&
            fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID))
            .Select(x => new
            {
                FiscalPeriodID = x.EconomicIndicator.FiscalPeriodID,
                EntityID = x.EconomicIndicatorFieldID,
                EntityName = entityName == null ? (lang == 1 ? x.EconomicIndicatorField.DisplayNameAr : x.EconomicIndicatorField.DisplayNameEn.Replace("'", "")) : entityName,
                ForYear = x.EconomicIndicator.ForYear,
                IndicatorValue = Conversions.StringToDecimalParsing(x.ValueAr)
            }).ToListAsync();

            return query.Join(fiscalPeriods, x => x.FiscalPeriodID, y => y.FiscalPeriodID,
            (item, FiscalPeriod) => new Macro_Charts_GetMarketTurnOverDailyDataModel()
            {
                EntityID = item.EntityID,
                EntityName = item.EntityName,
                Labels = item.ForYear.ToString() + " " + FiscalPeriod.FiscalPeriodValue,
                ForDate = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(item.ForYear, FiscalPeriod.FiscalPeriodTypeID),
                ForYear = item.ForYear,
                FiscalPeriodValue = FiscalPeriod.FiscalPeriodValue,
                Value = item.IndicatorValue,
                IndicatorValue = item.IndicatorValue,
            }).OrderBy(x => x.ForYear).ToList();
        }


        public async Task<dynamic> GetMacroComparableIndicatorsFieldData2(int lang, int fromYear, int toYear, string indicatorFieldIDs, int usePie, int fiscalPeriodTypeID)
        {
            var indicatorFieldIDList = indicatorFieldIDs.Split(',').Where(x => int.TryParse(x, out int number)).Select(x => int.Parse(x)).ToList();
            if (indicatorFieldIDs == "302" && usePie == 1)
            {
                return await Executor.Instance.GetData(async () =>
                {
                    return await PP_Indicator_FieldData_Summation(lang, fromYear, toYear, indicatorFieldIDList, usePie, fiscalPeriodTypeID);
                }, new { lang, fromYear, toYear, indicatorFieldIDs, usePie, fiscalPeriodTypeID }, 180);
            }
            else
            {
                return await Executor.Instance.GetData(async () =>
                {
                    return await PP_Indicator_FieldData_V2(lang, fromYear, toYear, indicatorFieldIDList, usePie, fiscalPeriodTypeID);
                }, new { lang, fromYear, toYear, indicatorFieldIDs, usePie, fiscalPeriodTypeID }, 180);
            }
        }
        private async Task<dynamic> PP_Indicator_FieldData_Summation(int lang, int fromYear, int toYear, List<int> indicatorFieldIDs, int usePie, int fiscalPeriodTypeID)
        {
            fromYear = fromYear < 2000 ? 1950 : fromYear;
            if (indicatorFieldIDs.Contains(302) && indicatorFieldIDs.Count == 1)
            {
                indicatorFieldIDs = new List<int>() { 302, 698, 697, 696, 695, 694, 688, 692 };
            }
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeID));

            var query = await _context.EconomicIndicatorValues.AsNoTracking().Where(x =>
            indicatorFieldIDs.Contains(x.EconomicIndicatorFieldID) &&
            x.EconomicIndicator.ForYear >= fromYear &&
            x.EconomicIndicator.ForYear >= fromYear &&
            x.EconomicIndicator.ForYear <= toYear &&
            fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID) &&
            x.EconomicIndicatorField.GroupID == x.EconomicIndicator.SubGroupID)
            .Select(x => new
            {
                FiscalPeriodID = x.EconomicIndicator.FiscalPeriodID,
                EntityID = x.EconomicIndicatorFieldID,
                EntityName = lang == 1 ? x.EconomicIndicatorField.DisplayNameAr : x.EconomicIndicatorField.DisplayNameEn.Replace("'", ""),
                Label = DatabaseFunctions.GetFiscalPeriodTextBankRanking(fiscalPeriodTypeID, x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID, true),
                ForDate = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID),
                ForYear = x.EconomicIndicator.ForYear,
                FiscalPeriodValue = DatabaseFunctions.GetFiscalPeriodDescriptiveText(fiscalPeriodTypeID),
                Value = Conversions.StringToDecimalParsing(x.ValueAr)
            }).ToListAsync();


            var res = query.Where(x => x.Value > 0);


            var notIn302 = res.Where(x => x.EntityID != 302);
            var notIn302Obj = new
            {
                EntityID = (short)303,
                EntityName = lang == 1 ? "22" : "Operating Expenditures",
                Labels = notIn302.Max(x => x.Label) ?? "",
                ForDate = notIn302.Max(x => x.ForDate),
                ForYear = notIn302.Max(x => x.ForYear),
                FiscalPeriodValue = notIn302.Max(x => x.FiscalPeriodValue) ?? "",
                Value = notIn302.Sum(x => x.Value)
            };
            var in302Obj = res.Where(x => x.EntityID == 302).Select(x => new
            {
                EntityID = x.EntityID,
                EntityName = x.EntityName ?? "",
                Labels = x.Label,
                ForDate = x.ForDate,
                ForYear = x.ForYear,
                FiscalPeriodValue = x.FiscalPeriodValue,
                Value = x.Value,
            }).ToList();
            in302Obj.Insert(0, notIn302Obj);
            return in302Obj;

            //List<dynamic> result = new List<dynamic>() { notIn302Obj };
            //result.AddRange(in302Obj);
            //return result;
        }
        private async Task<dynamic> PP_Indicator_FieldData_V2(int lang, int fromYear, int toYear, List<int> indicatorFieldIDs, int usePie, int fiscalPeriodTypeID)
        {
            fromYear = fromYear < 2000 ? 1950 : fromYear;
            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeID));

            var query = await _context.EconomicIndicatorValues.Where(x =>
                indicatorFieldIDs.Contains(x.EconomicIndicatorFieldID) &&
                x.EconomicIndicator.ForYear >= fromYear &&
                x.EconomicIndicator.ForYear <= toYear &&
                fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID) &&
                x.EconomicIndicatorField.GroupID == x.EconomicIndicator.SubGroupID) 
                
                .Select(x => new
                {
                    EntityID = x.EconomicIndicatorField.EconomicIndicatorFieldID,
                    EntityName = lang == 1
                    ? (x.EconomicIndicatorFieldID == 1168 ? "عدد المستثمرين الأفراء" :
                      x.EconomicIndicatorField.EconomicIndicatorFieldID == 1171 ? "عدد محافظ الأفراد" :
                      x.EconomicIndicatorFieldID == 1169 ? "اعداد المؤسسات المستثمره" :
                      x.EconomicIndicatorFieldID == 1172 ? "محافظ المؤسسات" :
                      x.EconomicIndicatorField.DisplayNameAr)
                    : (x.EconomicIndicatorFieldID == 1168 ? "No. of Individual Investors" :
                      x.EconomicIndicatorFieldID == 1171 ? "No. of Investment Portfolio" :
                      x.EconomicIndicatorFieldID == 1169 ? "No. of Institutional Investors" :
                      x.EconomicIndicatorFieldID == 1172 ? "No. of Institutional Portfolio" :
                      x.EconomicIndicatorField.DisplayNameEn.Replace("'", "")),
                    Labels = DatabaseFunctions.GetFiscalPeriodTextBankRanking(fiscalPeriodTypeID, x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID, true),
                    ForDate = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID),
                    ForYear = x.EconomicIndicator.ForYear,
                    FiscalPeriodValue = DatabaseFunctions.GetFiscalPeriodDescriptiveText(fiscalPeriodTypeID),
                    Value = Conversions.StringToDecimalParsing(x.ValueAr)
                }).ToListAsync();

            if (usePie == 1)
            {
                return query.Where(x => x.Value > 0).OrderByDescending(x => x.Value).ToList();
            }
            else
            {
                return query.OrderBy(x => x.ForDate);
            }
        }


        public async Task<dynamic> GetIndicatorFieldDataForFormula(int lang, int fromYear, int toYear, int indicatorFormulaId, int fiscalPeriodTypeID)
        {
            List<PP_Indicator_FormulaDataModel> macroIndicatorFormulaData = await Executor.Instance.GetData(async () =>
            {
                return await PP_Indicator_FormulaData(lang, fromYear, toYear, indicatorFormulaId, fiscalPeriodTypeID);
            }, new { lang, fromYear, toYear, indicatorFormulaId, fiscalPeriodTypeID }, 240);
            string preiousFormulaValue = "0";
            foreach (var item in macroIndicatorFormulaData)
            {

                List<string> formulaIds = item.FormulaIds.Split(',').Distinct().ToList();
                string[] entityIds = item.EntityID.Split(',');
                string[] indicatorValues = item.IndicatorValues.Split(',');

                string updatedFormulaValue = item.FormulaValue ?? "";

                for (int i = 0; i < entityIds.Length; i++)
                {
                    string thisValue = i >= indicatorValues.Length ? "0" : indicatorValues[i].Replace(" ", ""); //if value does not exist againt this key 
                    string thisId = i >= entityIds.Length ? "0" : entityIds[i].Replace(" ", "");
                    formulaIds.Remove(thisId);
                    updatedFormulaValue = updatedFormulaValue.Replace("#" + thisId + "#", "(" + thisValue + ")");
                }
                foreach (var toReplaceWithZero in formulaIds)
                {

                    updatedFormulaValue = updatedFormulaValue.Replace("#" + toReplaceWithZero + "#", "(null)");
                }
                if (item.ConstantValues != null)
                {
                    foreach (var constKeyVal in item.ConstantValues.Split(','))
                    {
                        var keyValue = constKeyVal.Split(':');
                        keyValue[1] = keyValue[1].Replace(" ", "");
                        updatedFormulaValue = updatedFormulaValue.Replace("#" + keyValue[0] + "#", "(" + keyValue[1] + ")");
                    }
                }

                item.Formula = updatedFormulaValue;
                item.FormulaValue = CalculateExpression(updatedFormulaValue);
                item.FormulaValue = item.FormulaValue == "-0" || item.FormulaValue == "-0.00" || item.FormulaValue == "" ? "0" : item.FormulaValue;

                preiousFormulaValue = item.PrevValue != null ? item.PrevValue : "0";
                if (item.FormulaValue != null)
                {
                    if (item.FormulaType == "Growth")
                        item.CalulatedValue = (((double.Parse(item.FormulaValue) / double.Parse(preiousFormulaValue)) - 1) * 100).ToString("0.00");
                    else if (item.FormulaType == "Change")
                        item.CalulatedValue = (double.Parse(item.FormulaValue) - double.Parse(preiousFormulaValue)).ToString("0.00");
                    else
                        item.CalulatedValue = item.FormulaValue;
                    //preiousFormulaValue = item.FormulaValue;
                }
                else
                {
                    preiousFormulaValue = "0";
                }
            }
            if (macroIndicatorFormulaData.Count > 0)
            {
                macroIndicatorFormulaData.RemoveAll(c => int.Parse(c.ForYear) == fromYear - 1 || c.FormulaValue == null); // removing first value which was retreived to fetch previous year FormulaValue to calculate starting growth
            }

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };
            macroIndicatorFormulaData = macroIndicatorFormulaData.Where(m => decimal.TryParse(m.CalulatedValue, out decimal parsedValue) && parsedValue != 0)
                .ToList();
            return macroIndicatorFormulaData
                .Select(s =>
                new {
                    EntityID = indicatorFormulaId,
                    EntityName = s.EntityName,
                    Labels = s.Labels,
                    ForDate = s.ForDate?.ToString("yyyy-MM-dd"),
                    ForYear = s.ForYear,
                    FiscalPeriodValue = s.FiscalPeriodValue,
                    Value = decimal.TryParse(s.CalulatedValue, out var parsedValue) ? (decimal?)parsedValue : 0
                })
                .Cast<dynamic>()
                .ToList();
        }
        private async Task<List<PP_Indicator_FormulaDataModel>> PP_Indicator_FormulaData(int lang, int fromYear, int toYear, int indicatorFormulaId, int fiscalPeriodTypeID)
        {
            var request = new RestRequest($"api/v1/json/macro/ui/formula-field-config/{indicatorFormulaId}", Method.Get);
            var formulaFieldConfigObj = await _macroApiClient.ExecuteAsync<FormulaFieldConfigModel>(request);
            var formulaFieldConfig = formulaFieldConfigObj?.Data;

            if (formulaFieldConfig != null)
            {
                fromYear = fromYear < 2000 ? 1950 : fromYear;
                fromYear = fromYear - 1;
                var formula = formulaFieldConfig.FormulaFieldIDs ?? "";
                var constantValues = formulaFieldConfig.ConstantValues;
                var formulaType = formulaFieldConfig.FormulaType ?? "";
                var entityName = (lang == 1 ? formulaFieldConfig.FieldNameAr : formulaFieldConfig.FieldNameEn) ?? "";
                var numberPartList = formula.Split('#').Where(x => long.TryParse(x, out long number)).Select(x => int.Parse(x)) ?? new List<int>();

                var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeID));
                var EconomicIndicatorValues = _context.EconomicIndicatorValues;
                var EconomicIndicators = _context.EconomicIndicators;


                var query = EconomicIndicatorValues.
                    Where(x =>
                    numberPartList.Contains(x.EconomicIndicatorFieldID) &&
                    x.EconomicIndicator.ForYear >= fromYear &&
                    x.EconomicIndicator.ForYear <= toYear &&
                    fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID) &&
                    x.EconomicIndicatorField.GroupID == x.EconomicIndicator.SubGroupID)
                    .Select(x => new
                    {
                        FiscalPeriodID = x.EconomicIndicator.FiscalPeriodID,
                        EntityID = x.EconomicIndicatorFieldID,
                        EntityName = lang == 1 ? x.EconomicIndicatorField.DisplayNameAr : x.EconomicIndicatorField.DisplayNameEn.Replace("'", ""),
                        Labels = DatabaseFunctions.GetFiscalPeriodTextBankRanking(fiscalPeriodTypeID, x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID, true),
                        ForDate = DatabaseFunctions.GetDateByYearAndFiscalPeriodID(x.EconomicIndicator.ForYear, x.EconomicIndicator.FiscalPeriodID),
                        ForYear = x.EconomicIndicator.ForYear,
                        FiscalPeriodValue = DatabaseFunctions.GetFiscalPeriodDescriptiveText(fiscalPeriodTypeID),
                        IndicatorValue = Conversions.StringToDecimalParsing(x.ValueAr),
                        PrevValue = Conversions.StringToDecimalParsing(Conversions.DefaultValueForNullables(EconomicIndicatorValues.
                        FirstOrDefault(y => y.EconomicIndicatorFieldID == x.EconomicIndicatorFieldID &&
                        y.EconomicIndicator.ForYear == (x.EconomicIndicator.ForYear - 1) &&
                        y.EconomicIndicator.FiscalPeriodID == x.EconomicIndicator.FiscalPeriodID &&
                        y.EconomicIndicator.SubGroupID == x.EconomicIndicatorField.GroupID)).ValueAr)
                    });

                var res = await query.GroupBy(x => new { x.ForYear, fiscalPeriodTypeID, x.FiscalPeriodID }).ToListAsync();


                return res.Select(x => new PP_Indicator_FormulaDataModel
                {
                    EntityID = string.Join(',', x.Select(y => y.EntityID)),
                    EntityName = entityName,
                    ForYear = Conversions.DefaultValueForNullables(x.FirstOrDefault()).ForYear.ToString(),
                    IndicatorValues = string.Join(',', x.Select(y => y.IndicatorValue)),
                    PrevValue = string.Join(',', x.Select(y => y.PrevValue)),
                    FiscalPeriodValue = Conversions.DefaultValueForNullables(x.FirstOrDefault()).FiscalPeriodValue,
                    FormulaValue = formula,
                    FormulaIds = string.Join(',', numberPartList),
                    ConstantValues = constantValues,
                    FormulaType = formulaType,
                    Labels = Conversions.DefaultValueForNullables(x.FirstOrDefault()).Labels,
                    ForDate = Conversions.DefaultValueForNullables(x.FirstOrDefault()).ForDate,
                }).OrderBy(x => x.ForDate).ToList();

            }
            return new List<PP_Indicator_FormulaDataModel>();
        }
        

        public async Task<dynamic> GetIndicatorsFreeFloatIndex()
        {
            return await Executor.Instance.GetData(async () =>
            {
                return await GetLatestEconomicIndicatorIndexData();
            }, new { }, 180);

        }
        public async Task<dynamic> GetLatestEconomicIndicatorIndexData()
        {
            var currentDate = DateTime.Now;
            var previousYear = currentDate.Year - 1;

            var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(3));

            var query = _context.EconomicIndicatorValues.Where(x =>
                fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID) &&
                (x.EconomicIndicator.ForYear == previousYear || x.EconomicIndicator.ForYear == currentDate.Year) &&
                x.EconomicIndicator.SubGroupID == 180 &&
                x.EconomicIndicatorFieldID != 1187)
                .Include(x => x.EconomicIndicatorField)
                .GroupBy(x => x.EconomicIndicatorFieldID)
                .Select(x => x.OrderByDescending(x => x.EconomicIndicator.ForYear).ThenBy(x => x.EconomicIndicator.FiscalPeriodID).First()).ToList();

            return query.Select(x => new
            {
                rankNo = 1,
                Companyid = x.EconomicIndicatorFieldID,
                NameAr = x.EconomicIndicatorField.DisplayNameAr,
                NameEn = x.EconomicIndicatorField.DisplayNameEn.Replace("'", ""),
                Value = Conversions.StringToDecimalParsing(x.ValueEn)
            }).OrderByDescending(x => x.Value).Take(10).ToList();
        }


        public static string CalculateExpression(string expression)
        {
            DataTable dt = new DataTable();
            object result = dt.Compute(expression, "");

            if (result != DBNull.Value)
            {
                return Convert.ToDouble(result).ToString("0.00");
            }
            else
            {
                // Handle the case when the result is DBNull
                // You might want to return a default value or throw an exception, depending on your use case.
                return "";
            }
        }
    }
}