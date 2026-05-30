using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBarrio.Data;
using SistemaBarrio.Models;
using SistemaBarrio.ViewModels.Visitas;

namespace SistemaBarrio.Controllers
{
    public class VisitaController : Controller
    {
        private BarrioDbContext _context;
        public VisitaController(BarrioDbContext context)
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

        // GET: /Visita/Registrar
        public async Task<IActionResult> Registrar()
        {
            var vm = new RegistrarVisitaViewModel
            {
                Domicilios = await GetDomiciliosSelectAsync(),
                Propietarios = new List<SelectListItem>()
            };

            return View(vm);
        }

        // GET: /Visita/BuscarPorDni?dni=12345678
        // Llamado por AJAX — devuelve JSON
        [HttpGet]
        public async Task<IActionResult> BuscarPorDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return BadRequest();

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

        // GET: /Visita/PropietariosPorDomicilio?domicilioId=3
        // Llamado por AJAX cuando se selecciona un domicilio — devuelve JSON
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

        // POST: /Visita/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistrarVisitaViewModel vm)
        {
            // Si no viene VisitanteId es visitante nuevo — validar campos manuales
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
                // Visitante ya registrado
                visitanteId = vm.VisitanteId.Value;
            }
            else
            {
                // Visitante nuevo — crear y guardar
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

            // ── Validar que no esté adentro del barrio ──────────────────────
            bool estaAdentro = await _context.Visitas
                .AnyAsync(v => v.VisitanteId == visitanteId
                            && v.EstadoVisita == EstadoVisita.EnCurso);

            if (estaAdentro)
            {
                TempData["Error"] = "El visitante ya se encuentra dentro del barrio";
                return RedirectToAction(nameof(Index));
            }

            var visita = new Visita
            {
                VisitanteId = visitanteId,
                DomicilioId = vm.DomicilioId!.Value,
                PropietarioId = vm.PropietarioId!.Value,
                FechaHoraIngreso = DateTime.Now,
                EstadoVisita = EstadoVisita.EnCurso
            };

            _context.Visitas.Add(visita);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Ingreso registrado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Visita — panel con visitas en curso
        public async Task<IActionResult> Index()
        {
            var visitas = await _context.Visitas
                .Include(v => v.Visitante)
                .Include(v => v.Propietario)
                .Include(v => v.Domicilio)
                .Where(v => v.EstadoVisita == EstadoVisita.EnCurso)
                .OrderBy(v => v.FechaHoraIngreso)
                .ToListAsync();

            return View(visitas);
        }

        // GET: /Visita/BuscarVisitaActiva?dni=12345678
        [HttpGet]
        public async Task<IActionResult> BuscarVisitaActiva(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return Json(new VisitaActivaDto
                {
                    Encontrado = false,
                    Mensaje = "Ingresá un DNI válido"
                });

            var visitante = await _context.Visitantes
                .FirstOrDefaultAsync(v => v.Dni == dni.Trim());

            if (visitante == null)
                return Json(new VisitaActivaDto
                {
                    Encontrado = false,
                    Mensaje = "No se encontró ningún visitante con ese DNI"
                });

            var visita = await _context.Visitas
                .Include(v => v.Domicilio)
                .Include(v => v.Propietario)
                .FirstOrDefaultAsync(v => v.VisitanteId == visitante.Id
                                       && v.EstadoVisita == EstadoVisita.EnCurso);

            if (visita == null)
                return Json(new VisitaActivaDto
                {
                    Encontrado = false,
                    Mensaje = $"{visitante.Apellido}, {visitante.Nombre} no tiene una visita activa"
                });

            var minutos = (int)(DateTime.Now - visita.FechaHoraIngreso).TotalMinutes;
            var tiempoTexto = minutos < 60
                ? $"{minutos} min"
                : $"{minutos / 60}h {minutos % 60}min";

            return Json(new VisitaActivaDto
            {
                Encontrado = true,
                VisitaId = visita.Id,
                NombreVisitante = $"{visitante.Apellido}, {visitante.Nombre}",
                Domicilio = $"Manzana {visita.Domicilio.Manzana ?? "-"} - Casa {visita.Domicilio.Casa}",
                Propietario = $"{visita.Propietario.Apellido}, {visita.Propietario.Nombre}",
                HoraIngreso = visita.FechaHoraIngreso.ToString("HH:mm"),
                TiempoAdentro = tiempoTexto
            });
        }

        // POST: /Visita/ConfirmarEgreso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEgreso(int visitaId)
        {
            var visita = await _context.Visitas
                .Include(v => v.Visitante)
                .FirstOrDefaultAsync(v => v.Id == visitaId);

            if (visita == null)
                return NotFound();

            if (visita.EstadoVisita == EstadoVisita.Finalizada)
            {
                TempData["Error"] = "La visita ya fue finalizada";
                return RedirectToAction(nameof(Index));
            }

            visita.FechaHoraSalida = DateTime.Now;
            visita.EstadoVisita = EstadoVisita.Finalizada;

            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Egreso de {visita.Visitante.Apellido}, {visita.Visitante.Nombre} registrado correctamente";
            return RedirectToAction(nameof(Index));
        }
    }
}
