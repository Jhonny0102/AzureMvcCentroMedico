 using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiCentroMedicoClient.Models
{
    [Table("V_CITASMEDICOS_FILTRO")]
    public class CitaDetalladaMedicos
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("FECHA")]
        public DateTime Fecha { get; set; }
        [Column("HORA")]
        public TimeSpan Hora { get; set; }
        [Column("ID_MEDICO")]
        public int IdMedico { get; set; }
        [Column("ID_PACIENTE")]
        public int IdPaciente { get; set; }
        [Column("NOMBRE")]
        public string NombrePaciente { get; set; }
        [Column("APELLIDO")]
        public string ApellidoPaciente { get; set; }
        [Column("ID_ESTADOCITA")]
        public int IdEstadoCita { get; set; }
        [Column("COMENTARIO")]
        public string? Comentario { get; set; }
    }
}
