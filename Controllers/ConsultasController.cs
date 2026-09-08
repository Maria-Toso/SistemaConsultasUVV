using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Data;
using SistemaConsultasUVV.Models;
using SistemaConsultasUVV.ViewModels;

namespace SistemaConsultasUVV.Controllers;

[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class ConsultasController : Controller
{
    private readonly ApplicationDbContext _context;

    public ConsultasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId is null)
        {
            return Challenge();
        }

        var consultas = await _context.Consultas
            .AsNoTracking()
            .Where(consulta => consulta.UsuarioId == usuarioId.Value)
            .OrderBy(consulta => consulta.DataHora)
            .ToListAsync(cancellationToken);

        return View(consultas);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ConsultaFormViewModel
        {
            DataHora = DateTime.Now.AddDays(1).Date.AddHours(9)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        ConsultaFormViewModel model,
        CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var consulta = new Consulta
        {
            Especialidade = model.Especialidade.Trim(),
            DataHora = model.DataHora,
            Descricao = model.Descricao.Trim(),
            UsuarioId = usuarioId.Value
        };

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync(cancellationToken);

        TempData["Sucesso"] = "Consulta agendada com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return NotFound();
        }

        var consulta = await BuscarConsultaDoUsuario(
            id.Value,
            usarRastreamento: false,
            cancellationToken);
        return consulta is null ? NotFound() : View(consulta);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return NotFound();
        }

        var consulta = await BuscarConsultaDoUsuario(
            id.Value,
            usarRastreamento: false,
            cancellationToken);

        if (consulta is null)
        {
            return NotFound();
        }

        return View(new ConsultaFormViewModel
        {
            Id = consulta.Id,
            Especialidade = consulta.Especialidade,
            DataHora = consulta.DataHora,
            Descricao = consulta.Descricao
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        int id,
        ConsultaFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var consulta = await BuscarConsultaDoUsuario(
            id,
            usarRastreamento: true,
            cancellationToken);
        if (consulta is null)
        {
            return NotFound();
        }

        consulta.Especialidade = model.Especialidade.Trim();
        consulta.DataHora = model.DataHora;
        consulta.Descricao = model.Descricao.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        TempData["Sucesso"] = "Consulta atualizada com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return NotFound();
        }

        var consulta = await BuscarConsultaDoUsuario(
            id.Value,
            usarRastreamento: false,
            cancellationToken);
        return consulta is null ? NotFound() : View(consulta);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var consulta = await BuscarConsultaDoUsuario(
            id,
            usarRastreamento: true,
            cancellationToken);
        if (consulta is null)
        {
            return NotFound();
        }

        _context.Consultas.Remove(consulta);
        await _context.SaveChangesAsync(cancellationToken);

        TempData["Sucesso"] = "Consulta excluída com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private int? ObterUsuarioId()
    {
        var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(valor, out var usuarioId) ? usuarioId : null;
    }

    private async Task<Consulta?> BuscarConsultaDoUsuario(
        int consultaId,
        bool usarRastreamento,
        CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId is null)
        {
            return null;
        }

        IQueryable<Consulta> query = _context.Consultas;
        if (!usarRastreamento)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(consulta =>
            consulta.Id == consultaId && consulta.UsuarioId == usuarioId.Value,
            cancellationToken);
    }
}
