using System;
using System.Collections.Generic;

namespace L.Models;

public partial class Libro
{
    public int LibroId { get; set; }

    public string? UrlImagen { get; set; }


    public string Titulo { get; set; } = null!;

    public string Autor { get; set; } = null!;


    public int? CategoriaId { get; set; }

    public string? Resumen { get; set; }

    public DateOnly? FechaPublicacion { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual Categoria? Categoria { get; set; }

    public virtual ICollection<Reseña> Reseña { get; set; } = new List<Reseña>();
}
