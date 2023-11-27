using Macroeconomics.BLS.Models.EconomicIndicatorField;
using Macroeconomics.BLS.Models.EconomicIndicatorGroup;
using Macroeconomics.DAL.Entities;
using AutoMapper;

namespace Macroeconomics.BLS.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //**CreateMap<TEntity, VEntity>();
            CreateMap<EconomicIndicatorGroup, EconomicIndicatorGroupModel>().ReverseMap();
            CreateMap<EconomicIndicatorField, EconomicIndicatorFieldModel>().ReverseMap();

        }

    }
}
