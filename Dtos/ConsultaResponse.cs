namespace SistemaConsultasUVV.Dtos;

public sealed record ConsultaResponse(
    int Id,
    string Especialidade,
    DateTime DataHora,
    string Descricao);
