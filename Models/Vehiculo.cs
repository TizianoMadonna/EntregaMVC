using System.ComponentModel.DataAnnotations;

namespace Final_MadonnaTizianoLab4.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "La matrícula del vehículo es requerida")]
        [Display(Name = "Matrícula")]
        public string NameMatricula { get; set; }
    }
}
