using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiCentroMedicoClient.Models
{
    [Table("V_ALL_USUARIOS_DETALLADOS")]
    public class UsuarioDetallado
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
        [Column("ESTADO")]
        public string Estado { get; set; }
        [Column("TIPO")]
        public string Tipo { get; set; }
    }
}
