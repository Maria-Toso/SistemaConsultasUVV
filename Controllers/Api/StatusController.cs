using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaConsultasUVV.Dtos;

namespace SistemaConsultasUVV.Controllers.Api;

[ApiController]
[Route("api/status")]
[AllowAnonymous]
public class StatusController : ControllerBase
{
    /// <summary>Confirma que a aplicação está respondendo.</summary>
    [HttpGet]
    [ProducesResponseType<StatusResponse>(StatusCodes.Status200OK)]
    public ActionResult<StatusResponse> Get()
    {
        return Ok(new StatusResponse(
            "Sistema de Gestão de Consultas UVV",
            "online",
            DateTime.Now));
    }
}
