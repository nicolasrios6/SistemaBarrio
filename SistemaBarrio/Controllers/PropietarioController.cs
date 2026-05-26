using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBarrio.Data;
using SistemaBarrio.Models;
using SistemaBarrio.ViewModels.Propietarios;

namespace SistemaBarrio.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly BarrioDbContext _context;

        public PropietarioController(BarrioDbContext context)
        {
            _context = context;
        }

        // Método privado para no repetir la carga del dropdown
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

        // GET: /Propietario
        public async Task<IActionResult> Index()
        {
            var propietarios = await _context.Propietarios
                .Include(p => p.Domicilio)
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToListAsync();

            return View(propietarios);
        }

        // GET: /Propietario/Crear
        public async Task<IActionResult> Crear()
        {
            var vm = new PropietarioViewModel
            {
                Domicilios = await GetDomiciliosSelectAsync()
            };

            return View(vm);
        }

        // POST: /Propietario/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(PropietarioViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Domicilios = await GetDomiciliosSelectAsync();
                return View(vm);
            }

            var propietario = new Propietario
            {
                Nombre = vm.Nombre.Trim(),
                Apellido = vm.Apellido.Trim(),
                Telefono = vm.Telefono.Trim(),
                DomicilioId = vm.DomicilioId!.Value,
                Activo = true
            };

            _context.Propietarios.Add(propietario);
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Propietario {propietario.Apellido}, {propietario.Nombre} creado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Propietario/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var propietario = await _context.Propietarios.FindAsync(id);

            if (propietario == null)
                return NotFound();

            var vm = new PropietarioViewModel
            {
                Id = propietario.Id,
                Nombre = propietario.Nombre,
                Apellido = propietario.Apellido,
                Telefono = propietario.Telefono,
                DomicilioId = propietario.DomicilioId,
                Domicilios = await GetDomiciliosSelectAsync()
            };

            return View(vm);
        }

        // POST: /Propietario/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, PropietarioViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Domicilios = await GetDomiciliosSelectAsync();
                return View(vm);
            }

            var propietario = await _context.Propietarios.FindAsync(id);

            if (propietario == null)
                return NotFound();

            propietario.Nombre = vm.Nombre.Trim();
            propietario.Apellido = vm.Apellido.Trim();
            propietario.Telefono = vm.Telefono?.Trim();
            propietario.DomicilioId = vm.DomicilioId!.Value;

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Propietario actualizado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Propietario/ToggleActivo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            var propietario = await _context.Propietarios.FindAsync(id);

            if (propietario == null)
                return NotFound();

            propietario.Activo = !propietario.Activo;
            await _context.SaveChangesAsync();

            var estado = propietario.Activo ? "activado" : "desactivado";
            TempData["Exito"] = $"Propietario {estado} correctamente";
            return RedirectToAction(nameof(Index));
        }
    }
}
