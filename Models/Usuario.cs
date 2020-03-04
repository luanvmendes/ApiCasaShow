using System.ComponentModel.DataAnnotations;

namespace CasaShowAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Senha { get; set; }
        public bool Admin { get; set; }
    }
}