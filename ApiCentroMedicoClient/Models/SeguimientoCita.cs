using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("SEGUIMIENTOCITA")]
    public class SeguimientoCita
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ESTADO")]
        public string Estado { get; set; }
    }
}
