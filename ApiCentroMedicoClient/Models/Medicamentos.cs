using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("MEDICAMENTOS")]
    public class Medicamentos
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }
        [Column("ID_ESTADO")]
        public int Id_Estado { get; set; }
    }
}
