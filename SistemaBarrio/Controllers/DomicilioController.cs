using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaBarrio.Data;
using SistemaBarrio.Models;
using SistemaBarrio.ViewModels.Domicilios;

namespace SistemaBarrio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DomicilioController : Controller
    {
        private readonly BarrioDbContext _context;

        public DomicilioController(BarrioDbContext context)
        {
            _context = context;
        }
        // GET: /Domicilio
        public async Task<IActionResult> Index()
        {
            var domicilios = await _context.Domicilios
                .Include(d => d.Propietarios.Where(p => p.Activo))
                .OrderBy(d => d.Manzana)
                .ThenBy(d => d.Casa)
                .ToListAsync();

            return View(domicilios);
        }

        //GET: /Domicilio/Crear
        public IActionResult Crear()
        {
            return View(new DomicilioViewModel());
        }

        // POST: /Domicilio/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(DomicilioViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            bool existe = await _context.Domicilios
                .AnyAsync(d => d.Manzana == vm.Manzana &&  d.Casa == vm.Casa);

            if(existe)
            {
                ModelState.AddModelError(string.Empty, "Ya existe un domicilio con esa Manzana y número de Casa");
                return View(vm);
            }

            var domicilio = new Domicilio
            {
                Manzana = vm.Manzana.Trim().ToUpper(),
                Casa = (int)vm.Casa
            };

            _context.Domicilios.Add(domicilio);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Domicilio creado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Domicilio/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var domicilio = await _context.Domicilios.FindAsync(id);

            if (domicilio == null)
                return NotFound();

            var vm = new DomicilioViewModel
            {
                Id = domicilio.Id,
                Manzana = domicilio.Manzana,
                Casa = domicilio.Casa
            };

            return View(vm);
        }

        // POST: /Domicilio/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, DomicilioViewModel vm)
        {
            if(!ModelState.IsValid)
                return View(vm);

            bool existe = await _context.Domicilios
                .AnyAsync(d => d.Manzana == vm.Manzana && d.Casa == vm.Casa && d.Id != vm.Id);

            if(existe)
            {
                ModelState.AddModelError(string.Empty,
                    "Ya existe otro domicilio con esa manzana y número de casa");
                return View(vm);
            }

            var domicilio = await _context.Domicilios.FindAsync(id);

            if (domicilio == null)
                return NotFound();

            domicilio.Manzana = vm.Manzana.Trim().ToUpper();
            domicilio.Casa = (int)vm.Casa;

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Domicilio actualizado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Domicilio/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var domicilio = await _context.Domicilios
                .Include(d => d.Propietarios)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (domicilio == null)
                return NotFound();

            // No eliminar si tiene propietarios asociados
            if (domicilio.Propietarios.Any())
            {
                TempData["Error"] = "No se puede eliminar un domicilio con propietarios asociados";
                return RedirectToAction(nameof(Index));
            }

            _context.Domicilios.Remove(domicilio);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Domicilio eliminado correctamente";
            return RedirectToAction(nameof(Index));
        }
    }
}
