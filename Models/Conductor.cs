using System.ComponentModel.DataAnnotations;

namespace Final_MadonnaTizianoLab4.Models
{
    public class Conductor
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El nombre del conductor es requerido")]
        [Display(Name ="Conductor")]
        public string? NameConductor { get; set; }

    }
}
