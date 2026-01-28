using System;
using System.Threading.Tasks;
using AplicaciónWebTest1.Data;
using AplicaciónWebTest1.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicaciónWebTest1.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoDAO _dao;
        private readonly AppConfig _config;

        public ProductosController(ProductoDAO dao, AppConfig config)
        {
            _dao = dao;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? busqueda, string? ordenarPor = "ID", bool desc = false, int page = 1, int pageSize = 0)
        {
            var size = pageSize > 0 ? pageSize : _config.PageSizeDefault;
            var resultado = await _dao.ListarAsync(busqueda, ordenarPor, desc, page, size);

            var vm = new ProductoIndexViewModel
            {
                Busqueda = busqueda,
                OrdenActual = ordenarPor ?? "ID",
                OrdenDesc = desc,
                Resultado = resultado
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create() => View(new Producto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                return View(producto);
            }

            try
            {
                await _dao.InsertarAsync(producto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(producto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _dao.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(producto);
            }

            try
            {
                await _dao.ActualizarAsync(producto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(producto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _dao.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _dao.BorrarAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var producto = await _dao.ObtenerPorIdAsync(id);
                return View("Delete", producto);
            }
        }
    }
}
