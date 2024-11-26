using System.ComponentModel.DataAnnotations;

namespace Final_MadonnaTizianoLab4.Models
{
    public class Entrega
    {
        [Display(Name ="Código")]
        public int Id { get; set; }
        [Display(Name ="Conductor")]
        [Required(ErrorMessage ="El conductor es requerido para la entrega")]
        public int ConductorId { get; set; }
        [Display(Name ="Conductor")]
        public Conductor? ConductorReferencial { get; set; }

        [Display(Name ="Vehículo")]
        [Required(ErrorMessage ="El vehículo es requerido para la entrega")]
        public int VehiculoId { get; set; }
        [Display(Name ="Vehículo")]
        public Vehiculo? VehiculoReferencial { get; set; }

        [Display(Name ="Localidad")]
        [Required(ErrorMessage ="Se necesita una localidad para la entrega")]
        public int LocalidadId { get; set; }
        [Display(Name ="Localidad")]
        public Localidad? LocalidadReferencial { get; set; }

        [Display(Name ="Fecha")]
        [Required(ErrorMessage ="Fecha de la entrega requerida")]
        public DateTime? FechaEntrega { get; set; }

        public ICollection<EntregaDetalle>? Detalles { get; set; }

    }
}
