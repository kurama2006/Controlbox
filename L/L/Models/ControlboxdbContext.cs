using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using L.Models;

namespace L.Models;

public partial class ControlboxdbContext : DbContext
{
    public ControlboxdbContext()
    {
    }

    public ControlboxdbContext(DbContextOptions<ControlboxdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<Libro> Libros { get; set; }

    public virtual DbSet<Reseña> Reseña { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public DbSet<Rol> Rol { get; set; } 

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
 //       => optionsBuilder.UseMySql("server=localhost;port=3306;database=controlboxdb;uid=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PRIMARY");

            entity.ToTable("categorias");

            entity.HasIndex(e => e.Nombre, "Nombre").IsUnique();

            entity.Property(e => e.CategoriaId)
                .HasColumnType("int(11)")
                .HasColumnName("CategoriaID");
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__efmigrationshistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.LibroId).HasName("PRIMARY");

            entity.ToTable("libros");

            entity.HasIndex(e => e.CategoriaId, "CategoriaID");

            entity.HasIndex(e => new { e.Titulo, e.Autor }, "idx_titulo_autor");

            entity.Property(e => e.LibroId)
                .HasColumnType("int(11)")
                .HasColumnName("LibroID");
            entity.Property(e => e.Autor).HasMaxLength(150);
            entity.Property(e => e.CategoriaId)
                .HasColumnType("int(11)")
                .HasColumnName("CategoriaID");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Resumen).HasColumnType("text");
            entity.Property(e => e.Titulo).HasMaxLength(200);

            entity.HasOne(d => d.Categoria).WithMany(p => p.Libros)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("libros_ibfk_1");
        });

        modelBuilder.Entity<Reseña>(entity =>
        {
            entity.HasKey(e => e.ReseñaId).HasName("PRIMARY");

            entity.ToTable("reseñas");

            entity.HasIndex(e => e.UsuarioId, "UsuarioID");

            entity.HasIndex(e => new { e.LibroId, e.FechaReseña }, "idx_libro_fecha");

            entity.Property(e => e.ReseñaId)
                .HasColumnType("int(11)")
                .HasColumnName("ReseñaID");
            entity.Property(e => e.Calificacion).HasColumnType("tinyint(4)");
            entity.Property(e => e.Comentario).HasColumnType("text");
            entity.Property(e => e.FechaReseña)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.LibroId)
                .HasColumnType("int(11)")
                .HasColumnName("LibroID");
            entity.Property(e => e.UsuarioId)
                .HasColumnType("int(11)")
                .HasColumnName("UsuarioID");

            entity.HasOne(d => d.Libro).WithMany(p => p.Reseña)
                .HasForeignKey(d => d.LibroId)
                .HasConstraintName("reseñas_ibfk_2");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Reseña)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("reseñas_ibfk_1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PRIMARY");
            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Correo, "Correo").IsUnique();

            // Configuración de la relación con Rol
            entity.HasOne(u => u.Rol)           // Un usuario tiene un rol
                .WithMany(r => r.Usuarios)      // Un rol tiene muchos usuarios
                .HasForeignKey(u => u.RolId)    // Clave foránea
                .OnDelete(DeleteBehavior.Restrict); // Comportamiento al eliminar

            entity.Property(e => e.UsuarioId)
                .HasColumnType("int(11)")
                .HasColumnName("UsuarioID");
            entity.Property(e => e.Contrasena).HasMaxLength(255);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(100);

            // Asegurar que RolId está mapeado
            entity.Property(e => e.RolId)
                .HasColumnType("int(11)")
                .HasColumnName("RolId"); // Nombre de columna en la base de datos
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("rol"); // Nombre de la tabla en la base de datos
            entity.HasKey(e => e.RolId); // Clave primaria

            entity.Property(e => e.RolId)
                .HasColumnType("int(11)")
                .HasColumnName("RolId");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            // Navegación inversa
            entity.HasMany(r => r.Usuarios)
                .WithOne(u => u.Rol)
                .HasForeignKey(u => u.RolId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


}
