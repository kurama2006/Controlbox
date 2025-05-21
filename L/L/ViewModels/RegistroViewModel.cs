using System.ComponentModel.DataAnnotations;

namespace L.ViewModels
{

    public class RegistroViewModel
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        [Required]
        [Compare("Contrasena")]
        [DataType(DataType.Password)]
        public string ConfirmarContrasena { get; set; }
    }
}