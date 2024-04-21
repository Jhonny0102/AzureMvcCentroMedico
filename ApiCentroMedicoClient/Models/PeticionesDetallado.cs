using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("V_DETAILS_PETICIONES")]
    public class PeticionesDetallado
    {
        [Key]
        [Column("IDPETICION")]
        public int IdPeticion { get; set; }
        [Column("IDRECEPCIONISTA")]
        public int IdRecepcionista { get; set; }
        [Column("NUEVOESTADO")]
        public int NuevoEstado { get; set; }
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("APELLIDO")]
        public string Apellido { get; set; }
        [Column("ESTADO")]
        public string Estado { get; set; }
    }
}
