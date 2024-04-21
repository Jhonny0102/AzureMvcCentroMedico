using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("MEDICOESPECIALIDAD")]
    public class MedicoEspecialidad
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ID_MEDICO")]
        public int Id_Medico { get; set; }
        [Column("ID_ESPECIALIDAD")]
        public int Id_Especialidad { get; set; }
    }
}
