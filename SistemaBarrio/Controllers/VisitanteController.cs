using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaBarrio.Data;
using SistemaBarrio.Models;
using SistemaBarrio.ViewModels.Visitantes;

namespace SistemaBarrio.Controllers
{
    [Authorize(Roles = "Admin, Guardia")]
    public class VisitanteController : Controller
    {
        private readonly BarrioDbContext _context;

        public VisitanteController(BarrioDbContext context)
        {
            _context = context;
        }
        // GET: /Visitante
        public async Task<IActionResult> Index(string? buscar, int pagina = 1)
        {
            int porPagina = 10;

            var query = _context.Visitantes
                .Include(v => v.Visitas)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();
                query = query.Where(v => v.Nombre.Contains(buscar) ||
                                          v.Apellido.Contains(buscar) ||
                                          v.Dni.Contains(buscar));
            }

            var total = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(total / (double)porPagina);

            var visitantes = await query
                .OrderBy(v => v.Apellido)
                .ThenBy(v => v.Nombre)
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToListAsync();

            var vm = visitantes.Select(v => new VisitanteListaItemViewModel
            {
                Id = v.Id,
                NombreCompleto = $"{v.Apellido}, {v.Nombre}",
                Dni = v.Dni,
                Patente = v.Patente,
                CantidadVisitas = v.Visitas.Count,
                UltimaVisita = v.Visitas
                    .OrderByDescending(vi => vi.FechaHoraIngreso)
                    .FirstOrDefault()
                    ?.FechaHoraIngreso.ToString("dd/MM/yyyy HH:mm")
            }).ToList();

            ViewBag.Buscar = buscar;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(vm);
        }

        // GET: /Visitante/Perfil/5
        public async Task<IActionResult> Perfil(int id)
        {
            var visitante = await _context.Visitantes
                .Include(v => v.Visitas)
                    .ThenInclude(vi => vi.Domicilio)
                .Include(v => v.Visitas)
                    .ThenInclude(vi => vi.Propietario)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visitante == null)
                return NotFound();

            var visitasOrdenadas = visitante.Visitas
                .OrderByDescending(vi => vi.FechaHoraIngreso)
                .ToList();

            var vm = new VisitantePerfilViewModel
            {
                Id = visitante.Id,
                NombreCompleto = $"{visitante.Apellido}, {visitante.Nombre}",
                Dni = visitante.Dni,
                Patente = visitante.Patente,
                CantidadVisitas = visitante.Visitas.Count,
                PrimeraVisita = visitasOrdenadas.LastOrDefault()
                    ?.FechaHoraIngreso.ToString("dd/MM/yyyy"),
                UltimaVisita = visitasOrdenadas.FirstOrDefault()
                    ?.FechaHoraIngreso.ToString("dd/MM/yyyy"),
                Visitas = visitasOrdenadas.Select(vi =>
                {
                    string duracion;
                    if (vi.FechaHoraSalida.HasValue)
                    {
                        var minutos = (int)(vi.FechaHoraSalida.Value - vi.FechaHoraIngreso).TotalMinutes;
                        duracion = minutos < 60
                            ? $"{minutos} min"
                            : $"{minutos / 60}h {minutos % 60}min";
                    }
                    else
                    {
                        duracion = "-";
                    }

                    return new VisitaHistorialItemViewModel
                    {
                        FechaIngreso = vi.FechaHoraIngreso.ToString("dd/MM/yyyy HH:mm"),
                        FechaEgreso = vi.FechaHoraSalida.HasValue
                            ? vi.FechaHoraSalida.Value.ToString("dd/MM/yyyy HH:mm")
                            : null,
                        Domicilio = $"Manzana {vi.Domicilio.Manzana ?? "-"} - Casa {vi.Domicilio.Casa}",
                        Propietario = $"{vi.Propietario.Apellido}, {vi.Propietario.Nombre}",
                        Duracion = duracion,
                        Estado = vi.EstadoVisita == EstadoVisita.EnCurso
                            ? "En curso" : "Finalizada"
                    };
                }).ToList()
            };

            return View(vm);
        }
    }
}
