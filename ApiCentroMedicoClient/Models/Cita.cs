using Microsoft.VisualBasic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("CITAS")]
    public class Cita
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("FECHA")]
        public DateTime Fecha { get; set; }
        [Column("HORA")]
        public TimeSpan Hora { get; set; }
        [Column("ID_SEGUIMIENTOCITA")]
        public int SeguimientoCita { get; set; }
        [Column("ID_MEDICO")]
        public int Medico { get; set; }
        [Column("ID_PACIENTE")]
        public int Paciente { get; set; }
        [Column("COMENTARIO")]
        public string? Comentario { get; set; }
        [Column("ID_ESTADOCITA")]
        public int IdEstado { get; set; }
    }
}
