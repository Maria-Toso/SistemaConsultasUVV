namespace SistemaConsultasUVV.Dtos;

public sealed record StatusResponse(
    string Sistema,
    string Status,
    DateTime DataHoraServidor);
