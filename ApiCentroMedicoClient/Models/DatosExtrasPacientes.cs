using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("DATOSEXTRASPACIENTES")]
    public class DatosExtrasPacientes
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ID_USUARIO")]
        public int Id_Usuario { get; set; }
        [Column("TELEFONO")]
        public int Telefono { get; set; }
        [Column("DIRECCION")]
        public string Direccion { get; set; }
        [Column("GENERO")]
        public string Genero{ get; set; }
        [Column("EDAD")]
        public int Edad { get; set; }
    }
}
