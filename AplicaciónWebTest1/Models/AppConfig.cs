namespace AplicaciónWebTest1.Models
{
    public class AppConfig
    {
        public string CadenaConexion { get; set; } = "Server=.\\SQLSERVER2022;Database=TiendaDAM;User Id=app_dam;Password=Vegetta777;TrustServerCertificate=True;";
        public string SpListarProductos { get; set; } = "dbo.ListarProductos";
        public string SpInsertarProducto { get; set; } = "dbo.InsertarProducto";
        public string SpActualizarProducto { get; set; } = "dbo.ActualizarProducto";
        public string SpBorrarProducto { get; set; } = "dbo.BorrarProducto";
        public int PageSizeDefault { get; set; } = 10;
        public int PageSizeMax { get; set; } = 50;
    }
}
