using System;
using System.Collections.Generic;

namespace L.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string Contrasena { get; set; } = null!;
    public DateTime? FechaRegistro { get; set; }

    // Relación con Rol (uno a muchos)
    public int RolId { get; set; }       // Clave foránea
    public virtual Rol Rol { get; set; } // Navegación

    public virtual ICollection<Reseña> Reseña { get; set; } = new List<Reseña>();
}