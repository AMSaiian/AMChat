using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AMChat.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase(ISender mediator,
                                        IMapper mapper)
    : ControllerBase
{
    protected ISender _mediator = mediator;
    protected IMapper _mapper = mapper;
}
