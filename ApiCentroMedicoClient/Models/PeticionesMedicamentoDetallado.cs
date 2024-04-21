using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCentroMedicoClient.Models
{
    [Table("V_DETAILS_PETICIONES_MEDICAMENTOS")]
    public class PeticionesMedicamentoDetallado
    {
        [Key]
        [Column("IDPETICION")]
        public int IdPeticion { get; set; }
        [Column("IDMEDICO_SOLICITANTE")]
        public int IdMedico_Solicitante { get; set; }
        [Column("PETISMEDICAMENTO_NOMBRE")]
        public string? PetisMedicamentoNombre { get; set; }
        [Column("PETISMEDICAMENTO_DESC")]
        public string? PetisMedicamentoDesc { get; set; }
        [Column("PETISMEDICAMENTO_ESTADO")]
        public int PetisMedicamentoEstado{ get; set; }
        [Column("NOMBREMEDICO")]
        public string NombreMedico { get; set; }
        [Column("APELLIDOMEDICO")]
        public string ApellidoMedico { get; set; }
        [Column("IDMEDICAMENTO")]
        public int? IdMedicamento { get; set; }
        [Column("NOMBRE")]
        public string? NombreMedicamento { get; set; }
        [Column("ESTADO")]
        public string Estado { get; set; }

    }
}
