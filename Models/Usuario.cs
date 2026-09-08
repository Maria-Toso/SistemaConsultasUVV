using System.ComponentModel.DataAnnotations;

namespace SistemaConsultasUVV.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Display(Name = "Senha")]
    public string SenhaHash { get; set; } = string.Empty;

    [Display(Name = "Data de cadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
}