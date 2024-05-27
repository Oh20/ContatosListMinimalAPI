using System.ComponentModel.DataAnnotations;

namespace ContatosListMinimalAPI.Core
{
    public class Contato
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [MinLength(1, ErrorMessage = "O nome não pode estar vazio")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [MinLength(1, ErrorMessage = "O telefone não pode estar vazio")]
        public string Telefone { get; set; }

        [EmailAddress(ErrorMessage = "O e-mail deve ser válido")]
        public string Email { get; set; }
    }
}
