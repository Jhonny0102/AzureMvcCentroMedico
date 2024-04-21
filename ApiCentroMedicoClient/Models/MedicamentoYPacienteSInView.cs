using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("MEDICAMENTOPACIENTE")]
    public class MedicamentoYPacienteSInView
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ID_MEDICAMENTO")]
        public int IdMedicamento { get; set; }
        [Column("ID_MEDICO")]
        public int IdMedico { get; set; }
        [Column("ID_PACIENTE")]
        public int IdPaciente { get; set; }
        [Column("ID_DISPOMEDICAMENTO")]
        public int IdDispoMedicamento { get; set; }
    }
}
