using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiCentroMedicoClient.Models
{
    [Table("V_ALL_PACIENTES_DETALLADO")]
    public class PacienteDetallado
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("APELLIDO")]
        public string Apellido { get; set; }
        [Column("CORREO")]
        public string Correo { get; set; }
        [Column("CONTRA")]
        public string Contra { get; set; }
        [Column("TELEFONO")]
        public int? Telefono { get; set; }
        [Column("DIRECCION")]
        public string? Direccion { get; set; }
        [Column("EDAD")]
        public int? Edad { get; set; }
        [Column("GENERO")]
        public string? Genero { get; set; }
        [Column("ESTADO")]
        public string Estado { get; set; }
        [Column("TIPO")]
        public string Tipo { get; set; }
    }
}
