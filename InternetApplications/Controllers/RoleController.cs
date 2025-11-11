using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;
    public RoleController(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }
}
