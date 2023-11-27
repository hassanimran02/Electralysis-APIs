using ArgaamPlus.BLS.Models.MeasuringUnits;
using ArgaamPlus.BLS.Services.MacroEconomics;
using ArgaamPlus.BLS.Services.MeasuringUnits;
using ArgaamPlus.Core.Shared.Enums;
using ArgaamPlus.Shared;
using Common.BLS.Models.FiscalPeriod;
using Common.BLS.Services.Common;
using Common.BLS.Services.FiscalPeriods;
using Macroeconomics.BLS.Services.MacroEconomics;
using Macroeconomics.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Macroeconomics.BLS.Services.ArgaamEconomics
{
    public class ArgaamEconomicsService : BaseService, IArgaamEconomicsService
    {

        private readonly ArgaamNext_IndicatorContext _context;
        private readonly IFiscalPeriodService _fiscalPeriodService;
        private readonly IMeasuringUnitService _argaamPlusMeasuringService;


        public ArgaamEconomicsService(ArgaamNext_IndicatorContext context, IFiscalPeriodService fiscalPeriodService,
            IMacroEconomicsService economicsService,
            IMeasuringUnitService argaamPlusMeasuringService)
        {
            _context = context;
            _fiscalPeriodService = fiscalPeriodService;
            _argaamPlusMeasuringService = argaamPlusMeasuringService;
        }

        public async Task<EconomicIndicatorApiModel> GetAllDataForIndexPage(int countryId, int? parentGroupId, int? subGroupId, int? fiscalPeriodTypeId, int? year, bool? isVertical = false)
        {
            return await Executor.Instance.GetData(async () =>
            {
                var fiscalPeriodType = fiscalPeriodTypeId ?? 4;
                var model = new EconomicIndicatorApiModel();
                model.Groups = await GetGroups(countryId);
                if (model.Groups.Any())
                {
                    model.SubGroupId = subGroupId ?? 0;
                    model.ParentGroupId = parentGroupId ?? model.Groups.OrderBy(x => x.DisplaySeqNo).FirstOrDefault(x => x.ParentGroupID == null)?.GroupID ?? 17;
                    if (!subGroupId.HasValue)
                    {
                        model.SubGroupId = model.Groups.OrderBy(x => x.DisplaySeqNo).FirstOrDefault(x => x.ParentGroupID == model.ParentGroupId)?.GroupID ?? 0;
                    }
                    model.FiscalPeriodTypes = await GetGroupFiscalPeriodTypes(model.SubGroupId);
                    if(!model.FiscalPeriodTypes.Any(x => x.FiscalPeriodTypeID == fiscalPeriodType))
                    {
                        fiscalPeriodType = model.FiscalPeriodTypes.Select(x => x.FiscalPeriodTypeID).OrderBy(x => x)?.FirstOrDefault() ?? 0;
                    }
                    model.FiscalPeriodTypeId = fiscalPeriodType;
                    model.Fields = await GetGroupFields(model.SubGroupId);
                    model.EconomicIndicators = await GetIndicators(countryId, (short)fiscalPeriodType, model.SubGroupId, year);
                    model.EconomicIndicatorValues = await GetGroupValues((isVertical ?? false) ? model.ParentGroupId : model.SubGroupId, (short)fiscalPeriodType, year, isVertical ?? false);
                }
                return model;
            }, new { countryId, parentGroupId, subGroupId, fiscalPeriodTypeId, year, isVertical }, 240);
        }

        public async Task<List<IndicatorGroupModel>> GetGroups(int? countryID = null, int? parentGroupId = null)
        {
            return await Executor.Instance.GetData(async () =>
            {
                var EconomicIndicators = await _context.EconomicIndicators.Where(e =>
                (countryID.HasValue ? (countryID == e.CountryID) : true) &&
                e.EconomicIndicatorValues.Any()).Select(x => x.SubGroupID ?? 0).Distinct().ToListAsync();

                var groups = await _context.EconomicIndicatorGroups.Where(x =>
                (parentGroupId.HasValue ? (parentGroupId == x.ParentGroupID) : true)).ToListAsync();

                List<IndicatorGroupModel> results = new List<IndicatorGroupModel>();
                foreach (var item in groups)
                {
                    var childGroupIds = groups.Where(d => d.ParentGroupID == item.GroupID).Select(x => x.GroupID).Distinct().ToList();
                    var shouldBeAdded = EconomicIndicators.Any(e => (e == item.GroupID) || childGroupIds.Contains(e));
                    if (shouldBeAdded)
                    {
                        results.Add(new IndicatorGroupModel()
                        {
                            GroupID = item.GroupID,
                            NameEn = item.NameEn,
                            NameAr = item.NameAr,
                            ParentGroupID = item.ParentGroupID,
                            DisplaySeqNo = item.DisplaySeqNo,
                        });
                    }
                }
                return results;
            }, new { countryID, parentGroupId }, 240);
        }
        public async Task<List<IndicatorGroupModel>> GetGroupsBySector(int sectorId, string tourismKey)
        {
            var result = await Executor.Instance.GetData(async () =>
            {
                int tourismID = _context.EconomicIndicatorGroups.Where(e => e.NameEn == tourismKey)
                .Select(e => e.GroupID)?.FirstOrDefault() ?? 0;

                var parentGroupID = (sectorId == (int)SectorEnum.Insurance) ? 91 :
                (sectorId == (int)SectorEnum.Tourism) ? tourismID :
                (sectorId == 218) ? 57 : 76;

                return await GetGroups(null, parentGroupID);
               
            }, new { sectorId, tourismKey }, 180);
            return result;
        }
        public async Task<List<FiscalFeriodTypeModel>> GetGroupFiscalPeriodTypes(int subGroupId)
        {
            var res = await Executor.Instance.GetData(async () =>
            {
                var fiscalPeriods = await _fiscalPeriodService.GetAllAsync();

                var data = await _context.EconomicIndicators.Where(x =>
                x.IsPublished == true && x.SubGroupID == subGroupId)
                .Select(x => x.FiscalPeriodID).Distinct().ToListAsync();

                var result = data.Join(fiscalPeriods.Where(x => x.FiscalPeriodTypeID != 1),
                indicator => indicator, fiscalperiod => fiscalperiod.FiscalPeriodID,
                (indicator, fiscalperiod) =>
                new FiscalFeriodTypeModel()
                {
                    FiscalPeriodTypeName = fiscalperiod.FiscalPeriodType.FiscalPeriodTypeName,
                    FiscalPeriodTypeID = fiscalperiod.FiscalPeriodTypeID
                })
                .OrderBy(x => x.FiscalPeriodTypeID)
                .GroupBy(x => x.FiscalPeriodTypeID)
                .Select(x => x.FirstOrDefault() ?? new FiscalFeriodTypeModel()).ToList();
                return result;
            }, new { subGroupId }, 240);

            return res;
        }
        public async Task<List<int>> GetGroupYears(int countryId, int? subGroupId, int? fiscalPeriodTypeId = null)
        {
            var res = await Executor.Instance.GetData(async () =>
            {
                var fiscalPeriods = fiscalPeriodTypeId.HasValue ? (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeId)) : new List<FiscalPeriodModel>();

                var data = _context.EconomicIndicators.Where(x => x.CountryID == countryId &&
                (subGroupId.HasValue ? (x.SubGroupID == subGroupId) : true) &&
                (fiscalPeriodTypeId.HasValue ? (fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.FiscalPeriodID)) : true))
                .Select(x => x.ForYear).Distinct().OrderByDescending(x => x).ToList();
                return data;
            }, new { countryId, subGroupId, fiscalPeriodTypeId }, 240);
            return res;
        }
        public async Task<List<EconomicIndicatorFieldModel>> GetGroupFields(int subGroupId)
        {
            var results = await Executor.Instance.GetData(async () =>
            {
                var measuingUnits = await _argaamPlusMeasuringService.GetAllAsync();
                var results = await _context.EconomicIndicatorFields.Where(x => x.GroupID == subGroupId).ToListAsync();

                var res = new List<EconomicIndicatorFieldModel>();
                foreach (var indValue in results)
                {
                    var measuringUnit = measuingUnits.FirstOrDefault(x => x.MeasuringUnitID == indValue.MeasuringUnitID);
                    res.Add(new EconomicIndicatorFieldModel()
                    {
                        EconomicIndicatorFieldID = indValue.EconomicIndicatorFieldID,
                        DisplayNameAr = indValue.DisplayNameAr,
                        DisplayNameEn = indValue.DisplayNameEn,
                        DisplaySeqNo = indValue.DisplaySeqNo,
                        MeasuringUnitID = measuringUnit?.MeasuringUnitID,
                        MeasuringUnitNameAr = measuringUnit?.MeasuringUnitNameAr,
                        MeasuringUnitNameEn = measuringUnit?.MeasuringUnitNameEn,
                        GroupID = indValue.GroupID,
                        IsChart = indValue.IsChart ?? false,
                    });
                }
                return res;
            }, new { subGroupId }, 240);
            return results;
        }
        public async Task<List<EconomicIndicatorValueModel>> GetGroupValues(int groupId, short fiscalPeriodTypeId, int? year, bool isVertical = false)
        {
            var results = await Executor.Instance.GetData(async () =>
            {
                var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeId));

                var query = await _context.EconomicIndicatorValues.Where(x =>
                fiscalPeriods.Select(x =>
                x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID) &&
                x.EconomicIndicator.IsPublished == true &&
                ((fiscalPeriodTypeId == 4 || !year.HasValue) ? true : (x.EconomicIndicator.ForYear == year)) &&
                (isVertical ? (x.EconomicIndicatorField.Group.ParentGroupID == groupId) : (x.EconomicIndicator.SubGroupID == groupId)))
                .Select(indValue => new EconomicIndicatorValueModel()
                {
                    EconomicIndicatorFieldID = indValue.EconomicIndicatorFieldID,
                    EconomicIndicatorID = indValue.EconomicIndicatorID,
                    EconomicIndicatorValueID = indValue.EconomicIndicatorValueID,
                    ValueAr = indValue.ValueEn,
                    ValueEn = indValue.ValueAr,
                    NoteAr = indValue.NoteAr,
                    NoteEn = indValue.NoteEn,
                }).ToListAsync();
                return query;
            }, new { groupId, fiscalPeriodTypeId, year, isVertical }, 240);
            return results;
        }
        public async Task<List<EconomicIndicatorModel>> GetIndicators(int countryID, short fiscalPeriodTypeId, int? subgroupId, int? year)
        {
            var results = await Executor.Instance.GetData(async () =>
            {
                var fiscalPeriods = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeId));

                var query = _context.EconomicIndicators.Where(x =>
                x.IsPublished == true && x.CountryID == countryID &&
                ((fiscalPeriodTypeId != 4 && year.HasValue) ? (x.ForYear == year) : true) &&
                (subgroupId == null ? true : (x.SubGroupID == subgroupId)) &&
                fiscalPeriods.Select(x => x.FiscalPeriodID).Contains(x.FiscalPeriodID))
                .Select(x => new EconomicIndicatorModel()
                {
                    EconomicIndicatorID = x.EconomicIndicatorID,
                    CountryID = x.CountryID,
                    UpdatedOn = x.UpdatedOn,
                    IsPublished = x.IsPublished,
                    ForYear = x.ForYear,
                    FiscalPeriodID = x.FiscalPeriodID,
                    SubGroupID = x.SubGroupID
                }).ToList();
                return query;
            }, new { countryID, fiscalPeriodTypeId, subgroupId, year }, 240);
            return results;
        }
        public async Task<List<EconomicIndicatorChartEntity>> GetChartDataForEconomicIndicator(int fieldid, int subgroupid, int fromYear)
        {
            var result = await Executor.Instance.GetData(async () =>
            {
                var data = await _context.EconomicIndicatorValues.Where(x =>
                        x.EconomicIndicatorField.EconomicIndicatorFieldID == fieldid &&
                        x.EconomicIndicator.ForYear > fromYear &&
                        x.EconomicIndicator.SubGroupID == subgroupid &&
                        x.EconomicIndicator.IsPublished == true).OrderBy(x => x.EconomicIndicator.ForYear)
                        .Select(x => new EconomicIndicatorChartEntity()
                        {
                            Value = x.ValueEn,
                            Description = x.ValueEn,
                            Year = x.EconomicIndicator.ForYear
                        }).ToListAsync();
                return data;

            }, new { fieldid, subgroupid, fromYear }, 180);
            return result;
        }

       
            //public async Task<List<EconomicIndicatorValueModel>> GetGroupValues(int countryId, int groupId, int fiscalPeriodTypeId, int? year, bool isVertical = false)
            //{
            //    var fiscalPeriod = (await _fiscalPeriodService.GetAllAsync(fiscalPeriodTypeId));

            //    var data = await _context.EconomicIndicatorValues
            //    .Where(x =>
            //    fiscalPeriod.Select(x => x.FiscalPeriodID).Contains(x.EconomicIndicator.FiscalPeriodID) &&
            //    (isVertical ? (x.EconomicIndicatorField.Group.ParentGroupID == groupId) : (x.EconomicIndicator.SubGroupID == groupId)) &&
            //    x.EconomicIndicator.IsPublished == true &&
            //    x.EconomicIndicator.CountryID == countryId &&
            //    (year == null ? true : (x.EconomicIndicator.ForYear == year)))
            //    .OrderBy(x => x.EconomicIndicatorField.Group.DisplaySeqNo).ThenBy(x => x.EconomicIndicatorField.DisplaySeqNo)
            //    .Select(x => new
            //    {
            //        x.EconomicIndicatorID,
            //        x.EconomicIndicatorFieldID,
            //        x.EconomicIndicatorValueID,
            //        x.EconomicIndicator.ForYear,
            //        GroupName = x.EconomicIndicatorField.Group.NameEn,
            //        x.EconomicIndicatorField.GroupID,
            //        x.EconomicIndicatorField.MeasuringUnitID,
            //        x.EconomicIndicator.FiscalPeriodID,
            //        Name = LanguageExtractor.GetName(x.EconomicIndicatorField, LanguagesEnum.en),
            //        x.ValueEn,
            //        x.ValueAr,
            //        x.NoteAr,
            //        x.NoteEn,

            //        x.EconomicIndicatorField.DisplayNameEn,
            //        x.EconomicIndicatorField.DisplayNameAr,
            //        x.EconomicIndicatorField.IsChart,
            //        x.EconomicIndicatorField.DisplaySeqNo
            //    })
            //    .ToListAsync();


            //    return data.Join(fiscalPeriod, indValue => indValue.FiscalPeriodID, fiscalperiod => fiscalperiod.FiscalPeriodID,
            //    (indicator, fiscalperiod) => new { indicator, fiscalperiod })
            //    .OrderBy(x => x.fiscalperiod.FiscalPeriodTypeID).Distinct()
            //    .Select(indValue => new EconomicIndicatorValueModel()
            //    {
            //        EconomicIndicatorFieldID = indValue.indicator.EconomicIndicatorFieldID,
            //        EconomicIndicatorID = indValue.indicator.EconomicIndicatorID,
            //        EconomicIndicatorValueID = indValue.indicator.EconomicIndicatorValueID,
            //        ValueAr = indValue.indicator.ValueEn,
            //        ValueEn = indValue.indicator.ValueAr,
            //        NoteAr = indValue.indicator.NoteAr ?? "",
            //        NoteEn = indValue.indicator.NoteEn ?? "",
            //    })
            //    .ToList();
            //    //var measuingUnits = await _argaamPlusMeasuringService.GetAllAsync();
            //    //return data2.Join(measuingUnits, indValue => indValue.indicator.MeasuringUnitID, measuringUnit => measuringUnit.MeasuringUnitID,
            //    //(indValue, measuringUnit) => new EconomicIndicatorValueModel()
            //    //{
            //    //    EconomicIndicatorFieldID = indValue.indicator.EconomicIndicatorFieldID,
            //    //    EconomicIndicatorID = indValue.indicator.EconomicIndicatorID,
            //    //    EconomicIndicatorValueID = indValue.indicator.EconomicIndicatorValueID,
            //    //    GroupName = indValue.indicator.GroupName,
            //    //    ValueAr = indValue.indicator.ValueEn,
            //    //    ValueEn = indValue.indicator.ValueAr,
            //    //    NoteAr = indValue.indicator.NoteAr ?? "",
            //    //    NoteEn = indValue.indicator.NoteEn ?? "",
            //    //}).ToList();
            //}

        }

    public class EconomicIndicatorChartEntity
    {
        public string Value { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Description { get; set; } = string.Empty;
    }
    public class EconomicIndicatorApiModel
    {
        public EconomicIndicatorApiModel() {
            Groups = new List<IndicatorGroupModel>();
            Fields = new List<EconomicIndicatorFieldModel>();
            EconomicIndicators = new List<EconomicIndicatorModel>();
            EconomicIndicatorValues = new List<EconomicIndicatorValueModel>();
            FiscalPeriodTypes = new List<FiscalFeriodTypeModel>();
        }
        public List<IndicatorGroupModel> Groups { get; set; }
        public List<EconomicIndicatorFieldModel> Fields { get; set; }
        public List<EconomicIndicatorModel> EconomicIndicators { get; set; }
        public List<EconomicIndicatorValueModel> EconomicIndicatorValues { get; set; }
        public List<FiscalFeriodTypeModel> FiscalPeriodTypes { get; set; }
        public int ParentGroupId { get; set; }
        public int SubGroupId { get; set; }
        public int FiscalPeriodTypeId { get; set; }
    }
    public class FiscalFeriodTypeModel
    {
        public string FiscalPeriodTypeName { get; set; } = "";
        public int FiscalPeriodTypeID { get; set; }
    }
    public class IndicatorGroupModel
    {
        public int GroupID { get; set; }
        public string NameEn { get; set; } = "";
        public string NameAr { get; set; } = "";
        public int? ParentGroupID { get; set; }
        public int DisplaySeqNo { get; set; }
    }
    public class EconomicIndicatorModel
    {
        public int EconomicIndicatorID { get; set; }
        public short? CountryID { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsPublished { get; set; }
        public int ForYear { get; set; }
        public short FiscalPeriodID { get; set; }
        public int? SubGroupID { get; set; }
    }
    public class EconomicIndicatorValueModel
    {
        public int EconomicIndicatorValueID { get; set; }
        public int EconomicIndicatorID { get; set; }
        public short EconomicIndicatorFieldID { get; set; }
        public string? NoteAr { get; set; }
        public string? NoteEn { get; set; }
        public string? ValueAr { get; set; }
        public string? ValueEn { get; set; }

    }
    public class EconomicIndicatorFieldModel
    {
        public short EconomicIndicatorFieldID { get; set; }
        public string DisplayNameAr { get; set; } = "";
        public string DisplayNameEn { get; set; } = "";
        public short DisplaySeqNo { get; set; }
        public int? MeasuringUnitID { get; set; }
        public string? MeasuringUnitNameAr { get; set; }
        public string? MeasuringUnitNameEn { get; set; }
        public int GroupID { get; set; }
        public bool IsChart { get; set; }
    }
}