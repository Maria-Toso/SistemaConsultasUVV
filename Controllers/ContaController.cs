using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Data;
using SistemaConsultasUVV.Models;
using SistemaConsultasUVV.ViewModels;

namespace SistemaConsultasUVV.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ContaController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly ILogger<ContaController> _logger;

    public ContaController(
        ApplicationDbContext context,
        IPasswordHasher<Usuario> passwordHasher,
        ILogger<ContaController> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Cadastro()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Consultas");
        }

        return View(new CadastroViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastro(
        CadastroViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var emailNormalizado = model.Email.Trim().ToLowerInvariant();

        if (await _context.Usuarios.AnyAsync(
                usuario => usuario.Email == emailNormalizado,
                cancellationToken))
        {
            ModelState.AddModelError(nameof(model.Email), "Já existe uma conta cadastrada com este e-mail.");
            return View(model);
        }

        var usuario = new Usuario
        {
            Nome = model.Nome.Trim(),
            Email = emailNormalizado,
            DataCadastro = DateTime.Now
        };

        usuario.SenhaHash = _passwordHasher.HashPassword(usuario, model.Senha);

        _context.Usuarios.Add(usuario);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            _logger.LogWarning(exception, "Tentativa de cadastrar um e-mail já existente.");
            ModelState.AddModelError(nameof(model.Email), "Já existe uma conta cadastrada com este e-mail.");
            return View(model);
        }

        TempData["Sucesso"] = "Conta criada com sucesso. Agora você já pode entrar.";
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Consultas");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var emailNormalizado = model.Email.Trim().ToLowerInvariant();
        var usuario = await _context.Usuarios
            .SingleOrDefaultAsync(
                item => item.Email == emailNormalizado,
                cancellationToken);

        if (usuario is null)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(model);
        }

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            model.Senha);

        if (resultado == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(model);
        }

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.SenhaHash = _passwordHasher.HashPassword(usuario, model.Senha);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email)
        };

        var identidade = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var propriedades = new AuthenticationProperties
        {
            IsPersistent = model.LembrarMe,
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidade),
            propriedades);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Consultas");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Sucesso"] = "Você saiu da sua conta com segurança.";
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AcessoNegado()
    {
        return View();
    }
}
