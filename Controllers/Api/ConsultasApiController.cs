using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Data;
using SistemaConsultasUVV.Dtos;

namespace SistemaConsultasUVV.Controllers.Api;

[ApiController]
[Route("api/consultas")]
[Authorize]
public class ConsultasApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ConsultasApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Lista somente as consultas do usuário autenticado.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ConsultaResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<ConsultaResponse>>> Get(
        CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var consultas = await _context.Consultas
            .AsNoTracking()
            .Where(consulta => consulta.UsuarioId == usuarioId.Value)
            .OrderBy(consulta => consulta.DataHora)
            .Select(consulta => new ConsultaResponse(
                consulta.Id,
                consulta.Especialidade,
                consulta.DataHora,
                consulta.Descricao))
            .ToListAsync(cancellationToken);

        return Ok(consultas);
    }

    /// <summary>Retorna uma consulta do usuário autenticado.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<ConsultaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsultaResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var consulta = await _context.Consultas
            .AsNoTracking()
            .Where(item => item.Id == id && item.UsuarioId == usuarioId.Value)
            .Select(item => new ConsultaResponse(
                item.Id,
                item.Especialidade,
                item.DataHora,
                item.Descricao))
            .SingleOrDefaultAsync(cancellationToken);

        return consulta is null ? NotFound() : Ok(consulta);
    }

    private int? ObterUsuarioId()
    {
        var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(valor, out var usuarioId) ? usuarioId : null;
    }
}
