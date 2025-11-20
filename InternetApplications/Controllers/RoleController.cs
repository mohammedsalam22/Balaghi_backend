using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleDao  _roleDao;
    public RoleController(IRoleDao roleDao)
    {
        _roleDao = roleDao;
    }
}
