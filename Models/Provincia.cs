using System.ComponentModel.DataAnnotations;

namespace Final_MadonnaTizianoLab4.Models
{
    public class Provincia
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El nombre de la provincia es requerida")]
        [Display(Name ="Provincia")]
        public string NameProvincia { get; set; }
    }
}
