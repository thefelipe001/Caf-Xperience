using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Usuarios
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)] // Limita el tamaño del nombre a 50 caracteres
    public string Nombre { get; set; }

    [ForeignKey("TiposUsuarios")]
    public int TipoUsuarioId { get; set; }

    [Column("Limite_de_Credito", TypeName = "decimal(18,2)")]
    public decimal Limite_de_Credito { get; set; }

    [Column("Fecha_Registro")]
    public DateTime FechaRegistro { get; set; }

    [Required]
    [StringLength(1)] // Limita el tamaño del estado a 1 carácter
    public string Estado { get; set; }

    [Required]
    [StringLength(50)]
    public string Correo { get; set; }

    [Required]
    [StringLength(20)]
    public string Contraseña { get; set; }

    public virtual Cafeteria Cafeteria { get; set; }

    public virtual ICollection<TiposUsuarios> TiposUsuarios { get; set; } = new List<TiposUsuarios>(); // Utiliza ICollection para las relaciones
}
