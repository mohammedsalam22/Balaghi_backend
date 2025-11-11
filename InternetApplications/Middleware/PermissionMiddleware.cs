using Application.Authorization;
using Domain.Interfaces;
using InternetApplications.Extensions;
namespace InternetApplications.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, IPermissionService permissionService)
        {
            var endpoint = context.GetEndpoint();
            var permissionAttr = endpoint?.Metadata.GetMetadata<PermissionAttribute>();
            if (permissionAttr != null)
            {
                var userId = context.User.GetUserId(); 
                var hasPermission = await permissionService.HasPermissionAsync(userId, permissionAttr.Name);
                if (!hasPermission)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }
            await _next(context);
        }
    }
}
