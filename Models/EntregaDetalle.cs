using System.ComponentModel.DataAnnotations;

namespace Final_MadonnaTizianoLab4.Models
{
    public class EntregaDetalle
    {
        public int Id { get; set; }
        [Display(Name ="Código de entrega")]
        [Required(ErrorMessage ="La entrega es requerida para ponerle los detalles")]
        public int EntregaId { get; set; }
        [Display(Name ="Entrega")]
        public Entrega? EntregaReferencial { get; set; }

        [Required(ErrorMessage ="El producto es necesario para los detalles")]
        [Display(Name ="Producto")]
        public int ProductoId { get; set; }
        [Display(Name ="Producto")]
        public Producto? ProductoReferencial { get; set; }

        [Display(Name ="Cantidad de cajas")]
        [Required(ErrorMessage ="Ingrese la cantidad de cajas del producto")]
        public int CantidadCajas {  get; set; }
    }
}
