using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SistemaBarrio.Filters
{
    public class IpGuardiaFilter : IAsyncActionFilter
    {
        private readonly string _ipPermitida;
        private readonly ILogger<IpGuardiaFilter> _logger;

        public IpGuardiaFilter(IConfiguration config, ILogger<IpGuardiaFilter> logger)
        {
            _ipPermitida = config["IpPorteria"] ?? string.Empty;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context,
                                          ActionExecutionDelegate next)
        {
            // Si el controller tiene el atributo SkipIpFilter, no aplicar restricción
            var skipFilter = context.ActionDescriptor.EndpointMetadata
                .OfType<SkipIpFilterAttribute>()
                .Any();

            if (skipFilter)
            {
                await next();
                return;
            }

            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated == true && user.IsInRole("Guardia"))
            {
                var ipActual = context.HttpContext.Connection.RemoteIpAddress?.ToString();

                if (ipActual == "::1") ipActual = "127.0.0.1";

                if (ipActual != _ipPermitida)
                {
                    _logger.LogWarning(
                        "Acceso denegado al Guardia desde IP no autorizada: {Ip}", ipActual);

                    context.Result = new RedirectToActionResult("IpNoAutorizada", "Error", null);
                    return;
                }
            }

            await next();
        }
    }
}
