using AMChat.Application.Common.Models.Pagination;
using AMChat.Common.Contract.Queries;
using Profile = AutoMapper.Profile;

namespace AMChat.Common.Mapping;

public sealed class QueriesProfile : Profile
{
    public QueriesProfile()
    {
        CreateMap<PaginationQuery, PageContext>();
        CreateMap<OrderQuery, OrderContext>();
    }
}
