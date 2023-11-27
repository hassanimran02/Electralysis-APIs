using Macroeconomics.BLS.Exceptions;
using Macroeconomics.BLS.Models.EconomicIndicatorField;
using Macroeconomics.DAL.Entities;
using ArgaamPlus.Shared;
using Microsoft.EntityFrameworkCore;
using Common.BLS.Services.Common;
using Macroeconomics.BLS.Services.Common;
using Common.BLS.Helper;

namespace Macroeconomics.BLS.Services.EconomicIndicatorFields
{
    public class EconomicIndicatorFieldService : BaseService, IEconomicIndicatorFieldService
    {
        IGenericRepository<EconomicIndicatorField, EconomicIndicatorFieldModel> _repo;
        public EconomicIndicatorFieldService(IGenericRepository<EconomicIndicatorField, EconomicIndicatorFieldModel> genericRepository)
        {
            _repo = genericRepository;
        }

        public async Task<IEnumerable<EconomicIndicatorFieldModel>> GetAllAsync(int? groupId)
        {
            var res = await Executor.Instance.GetDataAsync(async () =>
            {
                return await _repo.GetAllAsync(Expression<EconomicIndicatorField>().Where(x => groupId == null ? true : (x.GroupID == groupId)));
            }, new { groupId }, 180);
            return await res;
        }


        public async Task<EconomicIndicatorFieldModel> GetByIdAync(int groupId, short id)
        {
            var res = await _repo.GetAllAsync(Expression<EconomicIndicatorField>().Where(x => x.GroupID == groupId && x.EconomicIndicatorFieldID == id));
            var data = res.FirstOrDefault();
            if (data == null)
            {
                throw new RecordNotFound($"FieldId {id} does not exist against GroupId {groupId}");
            }
            return data;
        }

        public async Task<short> Save(int groupId, EconomicIndicatorFieldModel data)
        {
            if (!(await _repo.context.EconomicIndicatorGroups.AnyAsync(x => x.GroupID == groupId)))
            {
                throw new RecordNotFound($"GroupId {groupId} does not exist");
            }
            var entity = MapperHelper.Map<EconomicIndicatorField, EconomicIndicatorFieldModel>(data);
            var res = await _repo.Save(entity);
            return res.EconomicIndicatorFieldID;
        }

        public async Task<bool> Update(int groupId, EconomicIndicatorFieldModel data)
        {
            var isRecordExist = await _repo.GetQueryable().AnyAsync(x => x.GroupID == groupId && x.EconomicIndicatorFieldID == data.EconomicIndicatorFieldId);
            if (!isRecordExist)
            {
                throw new RecordNotFound($"FieldId {data.EconomicIndicatorFieldId} does not exist against {groupId}");
            }
            //var entity = MapperHelper.Map<EconomicIndicatorField, EconomicIndicatorFieldModel>(data);

            EconomicIndicatorField entity = new EconomicIndicatorField()
            {
                EconomicIndicatorFieldID = data.EconomicIndicatorFieldId,
                DisplayNameEn = data.DisplayNameEn,
                DisplayNameAr = data.DisplayNameAr,
                DisplaySeqNo = data.DisplaySeqNo,
                MeasuringUnitID = data.MeasuringUnitId,
                GroupID = data.GroupId,
                IsChart = data.IsChart
            };

            var res = await _repo.Update(entity);
            return res;
        }
        public async Task<bool> Delete(int groupId, int id)
        {
            var entity = await _repo.GetQueryable().Where(x => x.GroupID == groupId && x.EconomicIndicatorFieldID == id).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new RecordNotFound($"FieldId {id} does not exist against {groupId}");
            }
            var res = await _repo.Delete(entity);
            return res;
        }
    }
}
