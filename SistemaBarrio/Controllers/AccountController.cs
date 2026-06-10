using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaBarrio.Filters;
using SistemaBarrio.Models;

namespace SistemaBarrio.Controllers
{
    [SkipIpFilter]
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public AccountController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLoginViewModel usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var user = await _userManager.FindByEmailAsync(usuario.Email);

            var result = await _signInManager.PasswordSignInAsync(
                usuario.Email,
                usuario.Password,
                usuario.RememberMe,
                false
            );

            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Index", "Visita");

                if (await _userManager.IsInRoleAsync(user, "Guardia"))
                    return RedirectToAction("Index", "Visita");


                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Credenciales incorrectas");
            return View(usuario);
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
