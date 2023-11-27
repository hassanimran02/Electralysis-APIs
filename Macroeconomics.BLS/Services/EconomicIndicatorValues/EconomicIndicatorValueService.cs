using Macroeconomics.BLS.Services.Common;
using Macroeconomics.BLS.Models.EconomicIndicatorValue;
using Macroeconomics.BLS.Exceptions;
using Macroeconomics.DAL.Entities;
using ArgaamPlus.Shared;
using Microsoft.EntityFrameworkCore;
using Common.BLS.Services.Common;
using Common.BLS.Helper;

namespace Macroeconomics.BLS.Services.EconomicIndicatorValues
{
    public class EconomicIndicatorValueService : BaseService, IEconomicIndicatorValueService
    {
        private readonly IGenericRepository<EconomicIndicatorValue, EconomicIndicatorValueModel> _repo;

        public EconomicIndicatorValueService(IGenericRepository<EconomicIndicatorValue,EconomicIndicatorValueModel> genericRepository)
        {
            _repo = genericRepository;
        }

        public async Task<IEnumerable<EconomicIndicatorValueModel>> GetAllAsync(int fieldId)
        {
            var res = await Executor.Instance.GetDataAsync(async () =>
            {
               return await _repo.GetAllAsync(Expression<EconomicIndicatorValue>().Where(x => x.EconomicIndicatorFieldID == fieldId));
            }, new { fieldId }, 180);

            return await res;
        }


        

        public async Task<IEnumerable<IndicatorsFieldValue>> GetIndicatorsFieldValues(int indicatorFieldId, int year, int fiscalPeriodTypeId, int fiscalPeriodId)
        {
            year = year > 0 ? year : DateTime.Now.Year;
            return await _repo.context.EconomicIndicatorValues.Where(x => x.EconomicIndicatorFieldID == indicatorFieldId &&
            x.EconomicIndicator.ForYear == year &&
            x.EconomicIndicator.FiscalPeriodID == fiscalPeriodId).Select(x => new IndicatorsFieldValue()
            {
                EconomicIndicatorFieldId = x.EconomicIndicatorFieldID,
                DisplayNameAr = x.EconomicIndicatorField.DisplayNameAr,
                DisplayNameEn = x.EconomicIndicatorField.DisplayNameEn,
                Value = x.ValueAr,
                Year = x.EconomicIndicator.ForYear,
                FiscalPeriodID = x.EconomicIndicator.FiscalPeriodID,
                FiscalPeriodTypeID = fiscalPeriodTypeId
            }).ToListAsync();
        }

        public async Task<EconomicIndicatorValueModel> GetByIdAync(int fieldId, int valueId)
        {
            var res = await _repo.GetAllAsync(Expression<EconomicIndicatorValue>().Where(x => x.EconomicIndicatorFieldID == fieldId && x.EconomicIndicatorValueID == valueId));
            var data = res.FirstOrDefault();
            if (data == null)
            {
                throw new RecordNotFound($"EconomicIndicatorFieldId {valueId} does not exist against {fieldId}");
            }
            return data;
        }

        public async Task<int> Save(int fieldId,EconomicIndicatorValueModel data)
        {
            if (!(await _repo.context.EconomicIndicatorValues.AnyAsync(x => x.EconomicIndicatorFieldID == fieldId)))
            {
                throw new RecordNotFound($"FieldId {fieldId} does not exist");
            }
            var entity = MapperHelper.Map<EconomicIndicatorValue, EconomicIndicatorValueModel>(data);
             var res = await _repo.Save(entity);
            return res.EconomicIndicatorValueID;
        }

        public async Task<bool> Update(int fieldId, EconomicIndicatorValueModel data)
        {
            var isRecordExist = await _repo.GetQueryable().AnyAsync(x => x.EconomicIndicatorFieldID == fieldId && x.EconomicIndicatorValueID == data.EconomicIndicatorValueId);
            if (!isRecordExist)
            {
                throw new RecordNotFound($"ValueId {data.EconomicIndicatorValueId} does not exist against FieldId {fieldId}");
            }
            var entity = MapperHelper.Map<EconomicIndicatorValue, EconomicIndicatorValueModel>(data);
            var res = await _repo.Update(entity);
            return res;
        }
        public async Task<bool> Delete(int fieldId, int ValueId)
        {
            var entity = await _repo.GetQueryable().Where(x => x.EconomicIndicatorFieldID == fieldId && x.EconomicIndicatorValueID == ValueId).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new RecordNotFound($"EconomicIndicatorValueID {ValueId} does not exist against {fieldId}");
            }
            var res = await _repo.Delete(entity);
            return res;
        }

        public async Task<bool> UpdateEconomicIndicatorValueNotes(int economicIndicatorID, int economicIndicatorFieldID, EconomicIndicatorValuesNotesModel data)
        {
            var dataExists = await _repo.context.EconomicIndicatorValues
            .Where(x => x.EconomicIndicatorID == economicIndicatorID && x.EconomicIndicatorFieldID == economicIndicatorFieldID)
            .FirstOrDefaultAsync();

            if (dataExists == null)
            {
                throw new RecordNotFound($"FieldId {economicIndicatorFieldID} does not exist against IndicatorID {economicIndicatorID}");
            }
            dataExists.NoteEn = data.NoteEn;
            dataExists.NoteAr = data.NoteAr;

            var res = await _repo.Update(dataExists);
            return true;
        }
    }
}
