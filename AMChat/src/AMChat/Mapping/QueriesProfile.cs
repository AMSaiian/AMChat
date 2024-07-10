using AMChat.Application.Common.Models.Pagination;
using AMChat.Contract.Queries;
using Profile = AutoMapper.Profile;

namespace AMChat.Mapping;

public sealed class QueriesProfile : Profile
{
    public QueriesProfile()
    {
        CreateMap<PaginationQuery, PageContext>();
        CreateMap<OrderQuery, OrderContext>();
    }
}
