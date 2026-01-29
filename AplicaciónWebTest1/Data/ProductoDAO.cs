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

            pageIndex = Math.Max(1, pageIndex);

            int viewPageSize = pageSize > 0 ? pageSize : (_config.PageSizeDefault > 0 ? _config.PageSizeDefault : 10);
            if (_config.PageSizeMax > 0) viewPageSize = Math.Min(viewPageSize, _config.PageSizeMax);

            try
            {
                await using var connection = new SqlConnection(_config.CadenaConexion);
                await using var command = new SqlCommand(_config.SpListarProductos, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ordenarPor", orden);
                command.Parameters.AddWithValue("@paginaNum", pageIndex);
                command.Parameters.AddWithValue("@tamanoPagina", viewPageSize);
                command.Parameters.AddWithValue("@filtroNombre", string.IsNullOrWhiteSpace(busqueda) ? "" : busqueda);

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

            bool hayMas = productos.Count == viewPageSize;

            var totalEstimado = ((pageIndex - 1) * viewPageSize) + productos.Count + (hayMas ? 1 : 0);

            return new PagedResult<Producto>(productos, totalEstimado, pageIndex, viewPageSize);
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
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
