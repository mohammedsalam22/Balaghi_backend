// using Domain.Entities;
// using Domain.Interfaces;
// using Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;

// namespace Infrastructure.DataAccess
// {
//     public class GovernmentEntityDao : IGovernmentEntityDao
//     {
//         private readonly AppDbContext _context;

//         public GovernmentEntityDao(AppDbContext context)
//         {
//             _context = context;
//         }

//         public async Task<IReadOnlyList<GovernmentEntity>> GetAllWithEmployeesAsync(CancellationToken ct = default)
//         {
//             return await _context.GovernmentEntities
//                 .AsNoTracking()
//                 .Include(ge => ge.Employees!)
//                     .ThenInclude(u => u.UserRoles!)
//                         .ThenInclude(ur => ur.Role)
//                 .OrderBy(ge => ge.NameAr)
//                 .ToListAsync(ct);
//         }

//         public async Task<GovernmentEntity?> GetByIdWithEmployeesAsync(Guid id, CancellationToken ct = default)
//         {
//             return await _context.GovernmentEntities
//                 .AsNoTracking()
//                 .Include(ge => ge.Employees!)
//                     .ThenInclude(u => u.UserRoles!)
//                         .ThenInclude(ur => ur.Role)
//                 .FirstOrDefaultAsync(ge => ge.Id == id, ct);
//         }
//     }
// }