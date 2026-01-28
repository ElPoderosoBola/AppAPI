namespace AplicaciónWebTest1.Models
{
    public class ProductoIndexViewModel
    {
        public PagedResult<Producto> Resultado { get; set; } = new PagedResult<Producto>(new List<Producto>(), 0, 1, 10);
        public string? Busqueda { get; set; }
        public string OrdenActual { get; set; } = "ID";
        public bool OrdenDesc { get; set; }
    }
}
