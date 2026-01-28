using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AplicaciónWebTest1.Models;
using Microsoft.Data.SqlClient;

namespace AplicaciónWebTest1.Data
{
    public class ProductoDAO
    {
        private readonly AppConfig _config;

        public ProductoDAO(AppConfig config)
        {
            _config = config;
        }

        public async Task<PagedResult<Producto>> ListarAsync(string? busqueda, string? ordenarPor, bool ordenarDesc, int pageIndex, int pageSize)
        {
            var productos = new List<Producto>();

            var ordenColumna = MapearColumnaOrden(ordenarPor);
            var orden = ordenarDesc ? $"{ordenColumna} DESC" : ordenColumna;
            pageSize = Math.Clamp(pageSize, 1, _config.PageSizeMax > 0 ? _config.PageSizeMax : pageSize);
            pageIndex = Math.Max(1, pageIndex);

            try
            {
                await using var connection = new SqlConnection(_config.CadenaConexion);
                await using var command = new SqlCommand(_config.SpListarProductos, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ordenarPor", orden);
                command.Parameters.AddWithValue("@paginaNum", pageIndex);
                command.Parameters.AddWithValue("@tamanoPagina", pageSize);
                command.Parameters.AddWithValue("@filtroNombre", string.IsNullOrWhiteSpace(busqueda) ? (object)DBNull.Value : busqueda);

                await connection.OpenAsync();
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    productos.Add(new Producto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ID")),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                        Precio = reader.GetDecimal(reader.GetOrdinal("Precio"))
                    });
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Error al listar productos.", ex);
            }

            // Sin total devuelto por el SP, estimamos en base a la página actual
            var total = ((pageIndex - 1) * pageSize) + productos.Count;
            return new PagedResult<Producto>(productos, total, pageIndex, pageSize);
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            // No hay SP dedicado; usamos Listar con un pageSize amplio y filtramos en memoria
            var resultado = await ListarAsync(null, "ID", false, 1, _config.PageSizeMax > 0 ? _config.PageSizeMax : 100);
            return resultado.Items.Find(p => p.Id == id);
        }

        public async Task<int> InsertarAsync(Producto producto)
        {
            try
            {
                await using var connection = new SqlConnection(_config.CadenaConexion);
                await using var command = new SqlCommand(_config.SpInsertarProducto, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@nombre", producto.Nombre);
                command.Parameters.AddWithValue("@precio", producto.Precio);

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Error al insertar producto.", ex);
            }
        }

        public async Task<int> ActualizarAsync(Producto producto)
        {
            try
            {
                await using var connection = new SqlConnection(_config.CadenaConexion);
                await using var command = new SqlCommand(_config.SpActualizarProducto, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id", producto.Id);
                command.Parameters.AddWithValue("@nombre", producto.Nombre);
                command.Parameters.AddWithValue("@precio", producto.Precio);

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Error al actualizar producto.", ex);
            }
        }

        public async Task<int> BorrarAsync(int id)
        {
            try
            {
                await using var connection = new SqlConnection(_config.CadenaConexion);
                await using var command = new SqlCommand(_config.SpBorrarProducto, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id", id);

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Error al borrar producto.", ex);
            }
        }

        private static string MapearColumnaOrden(string? ordenarPor) => ordenarPor?.ToLower() switch
        {
            "nombre" => "Nombre",
            "precio" => "Precio",
            _ => "ID"
        };
    }
}
