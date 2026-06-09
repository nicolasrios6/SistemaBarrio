using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SistemaBarrio.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        // Excepciones no controladas
        [Route("Error/Index")]
        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features
                .Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                _logger.LogError(exceptionFeature.Error,
                    "Excepción no controlada en {Path}",
                    exceptionFeature.Path);
            }

            return View();
        }

        // Errores HTTP por código
        [Route("Error/{statusCode}")]
        public IActionResult HttpError(int statusCode)
        {
            _logger.LogWarning("Error HTTP {StatusCode} en {Path}",
                statusCode,
                HttpContext.Request.Path);

            return statusCode switch
            {
                404 => View("NotFound"),
                403 => View("Forbidden"),
                _ => View("Index")
            };
        }
    }
}
