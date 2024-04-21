using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("V_ALL_PACIENTES")]
    public class Paciente
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("APELLIDO")]
        public string Apellido { get; set; }
        [Column("CORREO")]
        public string Correo { get; set; }
        [Column("CONTRA")]
        public string Contra { get; set; }
        [Column("TELEFONO")]
        public int Telefono { get; set; }
        [Column("DIRECCION")]
        public string? Direccion { get; set; }
        [Column("EDAD")]
        public int Edad { get; set; }
        [Column("GENERO")]
        public string Genero { get; set; }
        [Column("ID_ESTADOUSUARIO")]
        public int EstadoUsuario { get; set; }
        [Column("ID_TIPOUSUARIO")]
        public int TipoUsuario { get; set; }
    }
}
