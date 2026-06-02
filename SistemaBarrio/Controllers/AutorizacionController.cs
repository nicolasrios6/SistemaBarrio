using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBarrio.Data;
using SistemaBarrio.Models;
using SistemaBarrio.ViewModels.Autorizaciones;
using SistemaBarrio.ViewModels.Visitas;

namespace SistemaBarrio.Controllers
{
    public class AutorizacionController : Controller
    {
        private readonly BarrioDbContext _context;

        public AutorizacionController(BarrioDbContext context)
        {
            _context = context;
        }

        private async Task<List<SelectListItem>> GetDomiciliosSelectAsync()
        {
            return await _context.Domicilios
                .OrderBy(d => d.Manzana)
                .ThenBy(d => d.Casa)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = $"Manzana {d.Manzana ?? "-"} - Casa {d.Casa}"
                })
                .ToListAsync();
        }
        // GET: /Autorizacion
        public async Task<IActionResult> Index()
        {
            var autorizaciones = await AutorizacionesVigentes()
                .Include(a => a.Visitante)
                .Include(a => a.Propietario)
                .Include(a => a.Domicilio)
                .OrderBy(a => a.Visitante.Apellido)
                .ToListAsync();

            var vm = autorizaciones.Select(a => new AutorizacionListaItemViewModel
            {
                Id = a.Id,
                NombreVisitante = $"{a.Visitante.Apellido}, {a.Visitante.Nombre}",
                Dni = a.Visitante.Dni,
                Domicilio = $"Manzana {a.Domicilio.Manzana ?? "-"} - Casa {a.Domicilio.Casa}",
                Propietario = $"{a.Propietario.Apellido}, {a.Propietario.Nombre}",
                Tipo = a.TipoAutorizacion == TipoAutorizacion.Temporal
                                    ? "Temporal" : "Frecuente",
                FechaAlta = a.FechaAlta.ToString("dd/MM/yyyy"),
                FechaVencimiento = a.FechaVencimiento.HasValue
                                    ? a.FechaVencimiento.Value.ToString("dd/MM/yyyy")
                                    : "Sin vencimiento"
            }).ToList();

            return View(vm);
        }

        // GET: /Autorizacion/Crear
        public async Task<IActionResult> Crear()
        {
            var vm = new AutorizacionViewModel
            {
                Domicilios = await GetDomiciliosSelectAsync(),
                Propietarios = new List<SelectListItem>()
            };

            return View(vm);
        }

        // POST: /Autorizacion/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(AutorizacionViewModel vm)
        {
            // Visitante nuevo — validar campos manuales
            if (vm.VisitanteId == null)
            {
                if (string.IsNullOrWhiteSpace(vm.Nombre))
                    ModelState.AddModelError(nameof(vm.Nombre), "El nombre es obligatorio");

                if (string.IsNullOrWhiteSpace(vm.Apellido))
                    ModelState.AddModelError(nameof(vm.Apellido), "El apellido es obligatorio");

                if (string.IsNullOrWhiteSpace(vm.Dni))
                    ModelState.AddModelError(nameof(vm.Dni), "El DNI es obligatorio");
            }

            if (!ModelState.IsValid)
            {
                vm.Domicilios = await GetDomiciliosSelectAsync();
                vm.Propietarios = new List<SelectListItem>();
                return View(vm);
            }

            int visitanteId;

            if (vm.VisitanteId != null)
            {
                visitanteId = vm.VisitanteId.Value;
            }
            else
            {
                // Verificar que el DNI no esté ya registrado
                bool dniExiste = await _context.Visitantes
                    .AnyAsync(v => v.Dni == vm.Dni!.Trim());

                if (dniExiste)
                {
                    // Si existe lo usamos — no creamos duplicado
                    var visitanteExistente = await _context.Visitantes
                        .FirstAsync(v => v.Dni == vm.Dni!.Trim());
                    visitanteId = visitanteExistente.Id;
                }
                else
                {
                    var nuevoVisitante = new Visitante
                    {
                        Nombre = vm.Nombre!.Trim(),
                        Apellido = vm.Apellido!.Trim(),
                        Dni = vm.Dni!.Trim(),
                    };

                    _context.Visitantes.Add(nuevoVisitante);
                    await _context.SaveChangesAsync();
                    visitanteId = nuevoVisitante.Id;
                }
            }

            // Verificar que no tenga ya una autorización vigente para ese domicilio
            bool yaAutorizado = await _context.Autorizaciones
                .AnyAsync(a => a.VisitanteId == visitanteId
                            && a.DomicilioId == vm.DomicilioId
                            && a.EstadoAutorizacion == EstadoAutorizacion.Vigente);

            if (yaAutorizado)
            {
                TempData["Error"] = "El visitante ya tiene una autorización vigente para ese domicilio";
                vm.Domicilios = await GetDomiciliosSelectAsync();
                vm.Propietarios = new List<SelectListItem>();
                return View(vm);
            }

            // Calcular fecha de vencimiento según el tipo
            DateTime? fechaVencimiento = vm.TipoAutorizacion == TipoAutorizacion.Temporal
                ? DateTime.Today.AddDays(1).AddTicks(-1) // hoy a las 23:59:59
                : null;                                   // frecuente = sin vencimiento

            var autorizacion = new Autorizacion
            {
                VisitanteId = visitanteId,
                PropietarioId = vm.PropietarioId!.Value,
                DomicilioId = vm.DomicilioId!.Value,
                TipoAutorizacion = vm.TipoAutorizacion!.Value,
                EstadoAutorizacion = EstadoAutorizacion.Vigente,
                FechaAlta = DateTime.Now,
                FechaVencimiento = fechaVencimiento
            };

            _context.Autorizaciones.Add(autorizacion);
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Autorización creada correctamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Autorizacion/Finalizar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id)
        {
            var autorizacion = await _context.Autorizaciones
                .Include(a => a.Visitante)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (autorizacion == null)
                return NotFound();

            autorizacion.EstadoAutorizacion = EstadoAutorizacion.Finalizada;
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Autorización de {autorizacion.Visitante.Apellido}, {autorizacion.Visitante.Nombre} finalizada";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Autorizacion/BuscarPorDni?dni=12345678
        // Llamado por AJAX — mismo patrón que en Visita
        [HttpGet]
        public async Task<IActionResult> BuscarPorDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return Json(new VisitanteEncontradoDto { Encontrado = false });

            var visitante = await _context.Visitantes
                .FirstOrDefaultAsync(v => v.Dni == dni.Trim());

            if (visitante == null)
                return Json(new VisitanteEncontradoDto { Encontrado = false });

            return Json(new VisitanteEncontradoDto
            {
                Encontrado = true,
                Id = visitante.Id,
                Nombre = visitante.Nombre,
                Apellido = visitante.Apellido,
                Dni = visitante.Dni
            });
        }

        // GET: /Autorizacion/PropietariosPorDomicilio?domicilioId=3
        [HttpGet]
        public async Task<IActionResult> PropietariosPorDomicilio(int domicilioId)
        {
            var propietarios = await _context.Propietarios
                .Where(p => p.DomicilioId == domicilioId && p.Activo)
                .OrderBy(p => p.Apellido)
                .Select(p => new { value = p.Id, text = $"{p.Apellido}, {p.Nombre}" })
                .ToListAsync();

            return Json(propietarios);
        }

        private IQueryable<Autorizacion> AutorizacionesVigentes()
        {
            return _context.Autorizaciones
                .Where(a => a.EstadoAutorizacion == EstadoAutorizacion.Vigente &&
                            (a.FechaVencimiento == null || a.FechaVencimiento > DateTime.Now));
        }
    }
}
