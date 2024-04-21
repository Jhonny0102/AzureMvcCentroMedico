using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("V_MEDICAMENTOSYPACIENTES")]
    public class MedicamentoYPaciente
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("IDMEDICAMENTO")]
        public int IdMedicamento { get; set; }
        [Column("NOMBREMEDICAMENTO")]
        public string NombreMedicamento { get; set; }
        [Column("DESCRIPCION")]
        public string DescripcionMedicamento { get; set; }
        [Column("IDMEDICO")]
        public int IdMedico { get; set; }
        [Column("NOMBRE")]
        public string NombreMedico { get; set; }
        [Column("APELLIDO")]
        public string ApellidoMedico { get; set; }
        [Column("IDPACIENTE")]
        public int IdPaciente { get; set; }
        [Column("IDDISPOMEDICAMENTO")]
        public int IdDispoMedicamento { get; set; }
        [Column("ESTADO")]
        public string NombreDispo { get; set; }
    }
}
