using System.ComponentModel.DataAnnotations;
using SistemaConsultasUVV.Validation;

namespace SistemaConsultasUVV.Models;

public class Consulta
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe a especialidade.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "A especialidade deve ter entre 3 e 100 caracteres.")]
    public string Especialidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a data e a hora.")]
    [Display(Name = "Data e hora")]
    [DataType(DataType.DateTime)]
    [DataFutura]
    public DateTime DataHora { get; set; }

    [Required(ErrorMessage = "Informe uma descrição.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "A descrição deve ter entre 5 e 500 caracteres.")]
    [DataType(DataType.MultilineText)]
    public string Descricao { get; set; } = string.Empty;

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}