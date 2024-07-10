using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AMChat.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase(ISender mediator) : ControllerBase
{
    protected ISender _mediator = mediator;
}
