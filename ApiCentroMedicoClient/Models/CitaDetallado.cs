using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiCentroMedicoClient.Models
{
    [Table("V_CITAS_DETALLADO")]
    public class CitaDetallado
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("FECHA")]
        public DateTime Fecha { get; set; }
        [Column("HORA")]
        public TimeSpan Hora { get; set; }
        [Column("ESTADO")]
        public string SeguimientoCita { get; set; }
        [Column("MEDICO")]
        public string Medico { get; set; }
        [Column("COMENTARIO")]
        public string? Comentario { get; set; }

    }
}
