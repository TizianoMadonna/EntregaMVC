using System.ComponentModel.DataAnnotations;

namespace Final_MadonnaTizianoLab4.Models
{
    public class Producto
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Producto requerido")]
        [Display(Name ="Producto")]
        public string NameProducto { get; set; }
    }
}
