using System.ComponentModel.DataAnnotations;

namespace SistemaConsultasUVV.Validation;

public class DataFuturaAttribute : ValidationAttribute
{
    public DataFuturaAttribute()
        : base("A consulta deve ser agendada para uma data e hora futuras.")
    {
    }

    public override bool IsValid(object? value)
    {
        return value is DateTime dataHora && dataHora > DateTime.Now;
    }
}
