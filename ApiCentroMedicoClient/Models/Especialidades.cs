using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("ESPECIALIDADES")]
    public class Especialidades
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ESPECIALIDAD")]
        public string Especialidad { get; set; }
    }
}
