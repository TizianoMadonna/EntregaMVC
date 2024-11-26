using System.ComponentModel.DataAnnotations;

namespace Final_MadonnaTizianoLab4.Models
{
    public class Localidad
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre de la localidad es requerido")]
        [Display(Name = "Localidad")]
        public string NameLocalidad { get; set; }
        [Required(ErrorMessage = "La provincia de la localidad es requerida")]
        [Display(Name = "Provincia")]
        public int ProvinciaId { get; set; }
        [Display(Name ="Provincia")]
        public Provincia? ProvinciaReferencial { get; set; }
    }
}
