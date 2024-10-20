﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CafeXperienceApp.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cafeteria> Cafeterias { get; set; }

    public virtual DbSet<Campus> Campuses { get; set; }

    public virtual DbSet<TipoUsuario> TipoUsuarios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Empleados> Empleados { get; set; }
    public virtual DbSet<Marca> Marcas { get; set; }
    public DbSet<Articulo> Articulos { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=CafeXperienceDB.mssql.somee.com;Database=CafeXperienceDB;User Id=CafeUnapec_SQLLogin_1;Password=iicrm8eafc;Integrated Security=False;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cafeteria>(entity =>
        {
            entity.HasKey(e => e.IdCafeteria).HasName("PK__Cafeteri__0431E68009A29291");

            entity.Property(e => e.IdCafeteria).HasColumnName("idCafeteria");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.IdCampus).HasColumnName("idCampus");
            entity.Property(e => e.IdEncargado).HasColumnName("idEncargado");

            entity.HasOne(d => d.IdCampusNavigation).WithMany(p => p.Cafeteria)
                .HasForeignKey(d => d.IdCampus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cafeteria__idCam__6FE99F9F");

            entity.HasOne(d => d.IdEncargadoNavigation).WithMany(p => p.Cafeteria)
                .HasForeignKey(d => d.IdEncargado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cafeteria__idEnc__70DDC3D8");
        });

        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.IdCampus).HasName("PK__Campus__DE4751C2EE2CD9D0");

            entity.ToTable("Campus");

            entity.Property(e => e.IdCampus).HasColumnName("idCampus");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .IsFixedLength();
        });
        modelBuilder.Entity<Usuario>()
      .ToTable("Usuarios", t => t.HasTrigger("trg_InsertCodigoVerificacion"));

        modelBuilder.Entity<TipoUsuario>(entity =>
        {
            entity.HasKey(e => e.IdTipoUsuarios).HasName("PK__TipoUsua__672DE0CD3FB9EF19");

            entity.Property(e => e.IdTipoUsuarios).HasColumnName("idTipoUsuarios");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .IsFixedLength();
        });

        // Configuración de la relación entre Usuario y CodigoVerificacion
        modelBuilder.Entity<CodigoVerificacion>()
            .HasOne(c => c.Usuario)
            .WithMany()
            .HasForeignKey(c => c.UsuarioId);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__645723A61C8BA098");

            entity.HasIndex(e => e.Correo, "UQ__Usuarios__60695A19EE4CC523").IsUnique();

            entity.HasIndex(e => e.Cedula, "UQ__Usuarios__B4ADFE383B4C3648").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Cedula)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Contraseña)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LimiteCredito).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.TipoUsuario).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.TipoUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__TipoUs__60A75C0F");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.IdMarca).HasName("PK__Marcas__703318120C1B0F84");

            entity.Property(e => e.IdMarca).HasColumnName("idMarca");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
