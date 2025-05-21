using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace L.Models;

    public partial class Rol 
    {
    public int RolId { get; set; }

    public required string Nombre { get; set; }


    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

}

