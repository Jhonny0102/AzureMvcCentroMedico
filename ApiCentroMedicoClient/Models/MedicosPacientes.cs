using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("V_PACIENTESMEDICOS")]
    public class MedicosPacientes
    {
        [Key]
        [Column("IDP")]
        public Int64 IdP { get; set; }
        [Column("ID_MEDICO")]
        public int Medico { get; set; }
        [Column("ID")]
        public int Paciente { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("APELLIDO")]
        public string Apellido { get; set; }
        [Column("CORREO")]
        public string Correo { get; set; }
    }
}
