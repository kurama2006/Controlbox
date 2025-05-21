using System.ComponentModel.DataAnnotations;

    namespace L.ViewModels
    {
        public class LoginViewModel
        {
            [Required(ErrorMessage = "El correo es obligatorio.")]
            [EmailAddress(ErrorMessage = "El correo no es válido.")]
            public string Correo { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [DataType(DataType.Password)]
            public string Contraseña { get; set; }
        }
    }



