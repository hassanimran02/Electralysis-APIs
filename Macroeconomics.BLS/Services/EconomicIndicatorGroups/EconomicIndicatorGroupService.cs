using Macroeconomics.BLS.Exceptions;
using Macroeconomics.BLS.Models.EconomicIndicatorGroup;
using Macroeconomics.DAL.Entities;
using ArgaamPlus.Shared;
using Microsoft.EntityFrameworkCore;
using Common.BLS.Services.Common;
using Common.BLS.Helper;
using Macroeconomics.BLS.Services.Common;
using Macroeconomics.DAL.Petapoco;
using pocoTable = Macroeconomics.DAL.Petapoco.TableModels;


namespace Macroeconomics.BLS.Services.EconomicIndicatorGroups
{
    public class EconomicIndicatorGroupService : BaseService, IEconomicIndicatorGroupService
    {
        IGenericRepository<EconomicIndicatorGroup, EconomicIndicatorGroupModel> _repo;
        private readonly PetapocoServices _petapocService;
        public EconomicIndicatorGroupService(IGenericRepository<EconomicIndicatorGroup, EconomicIndicatorGroupModel> genericRepository, PetapocoServices petapocService)
        {
            _repo = genericRepository;
            _petapocService = petapocService;
        }

        public async Task<IEnumerable<EconomicIndicatorGroupModel>> GetAllAsync(int? parentGroupId, bool? includeParentGroups)
        {
            var res = await Executor.Instance.GetData(async () =>
            {
                var res =  await _repo.GetAllAsync(Expression<EconomicIndicatorGroup>().Where(x => 
                (parentGroupId == null ? true : (x.ParentGroupID == parentGroupId)) ||
                (includeParentGroups == true ? (x.ParentGroupID == null) : true)
                ).OrderBy(x => x.DisplaySeqNo));
                return res;
            }, new { parentGroupId, includeParentGroups }, 180);
            return res;
        }
        public async Task<EconomicIndicatorGroupModel> GetByIdAync(int id)
        {
            var res = await _repo.GetAllAsync(Expression<EconomicIndicatorGroup>().Where(x => x.GroupID == id));
            var data = res.FirstOrDefault();
            if (data == null)
            {
                throw new RecordNotFound($"GroupId {id} does not exist");
            }
            return data;
        }
        public async Task<int> Save(EconomicIndicatorGroupModel data)
        {
            var entity = MapperHelper.Map<EconomicIndicatorGroup, EconomicIndicatorGroupModel>(data);
            var res = await _repo.Save(entity);
            return res.GroupID;
        }

        public async Task<bool> Update(EconomicIndicatorGroupModel data)
        {
            var isRecordExist = await _repo.GetQueryable().AnyAsync(x => x.GroupID == data.GroupID);
            if (!isRecordExist)
            {
                throw new RecordNotFound($"GroupID {data.GroupID} does not exist");
            }
            var entity = MapperHelper.Map<EconomicIndicatorGroup, EconomicIndicatorGroupModel>(data);
            var res = await _repo.Update(entity);
            return res;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _repo.GetQueryable().Where(x => x.GroupID == id).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new RecordNotFound($"GroupId {id} does not exist");
            }
            var res = await _repo.Delete(entity);
            return res;
        }

        public async Task<IEnumerable<pocoTable.EconomicIndicatorGroup>> GetAllIndicatorGroups()
        {
            using (var context = _petapocService.GetInstance())
            {
                var ppSql = PetoPocoSql.Builder.Select("*").From("EconomicIndicatorGroups").OrderBy("DisplaySeqNo");
                return await context.FetchAsync<pocoTable.EconomicIndicatorGroup>(ppSql);
            }
        }
    }
}
