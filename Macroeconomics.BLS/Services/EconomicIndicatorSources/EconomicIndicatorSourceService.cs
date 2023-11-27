using Macroeconomics.BLS.Exceptions;
using Macroeconomics.BLS.Models.EconomicIndicatorSource;
using Macroeconomics.DAL.Entities;
using ArgaamPlus.Shared;
using Microsoft.EntityFrameworkCore;
using Common.BLS.Services.Common;
using Macroeconomics.BLS.Services.Common;
using Common.BLS.Helper;

namespace Macroeconomics.BLS.Services.EconomicIndicatorSources
{
    public class EconomicIndicatorSourceService : BaseService, IEconomicIndicatorSourceService
    {
        IGenericRepository<EconomicIndicatorSource, EconomicIndicatorSourceModel> _repo;
        public EconomicIndicatorSourceService(IGenericRepository<EconomicIndicatorSource, EconomicIndicatorSourceModel> genericRepository)
        {
            _repo = genericRepository;
        }

        public async Task<IEnumerable<EconomicIndicatorSourceModel>> GetAllAsync()
        {
            var res = await Executor.Instance.GetDataAsync(async () =>
            {
                return await _repo.GetAllAsync();
            }, new { }, 180);
            return await res;
        }

        public async Task<EconomicIndicatorSourceModel> GetByIdAync(int sourceId)
        {
            var res = await _repo.GetAllAsync(Expression<EconomicIndicatorSource>().Where(x => x.EconomicIndicatorSourceID == sourceId));
            var data = res.FirstOrDefault();
            if (data == null)
            {
                throw new RecordNotFound($"SourceId {sourceId} does not exist");
            }
            return data;
        }

        public async Task<int> Save(EconomicIndicatorSourceModel data)
        {
            var entity = MapperHelper.Map<EconomicIndicatorSource, EconomicIndicatorSourceModel>(data);
            var res = await _repo.Save(entity);
            return res.EconomicIndicatorSourceID;
        }

        public async Task<bool> Update(EconomicIndicatorSourceModel data)
        {
            var isRecordExist = await _repo.GetQueryable().AnyAsync(x => x.EconomicIndicatorSourceID == data.EconomicIndicatorSourceId);
            if (!isRecordExist)
            {
                throw new RecordNotFound($"SourceId {data.EconomicIndicatorSourceId} does not exist");
            }
            var entity = MapperHelper.Map<EconomicIndicatorSource, EconomicIndicatorSourceModel>(data);
            var res = await _repo.Update(entity);
            return res;
        }

        public async Task<bool> Delete(int sourceId)
        {
            var entity = await _repo.GetQueryable().Where(x => x.EconomicIndicatorSourceID == sourceId).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new RecordNotFound($"SourceId {sourceId} does not exist");
            }
            var res = await _repo.Delete(entity);
            return res;
        }
    }
}
