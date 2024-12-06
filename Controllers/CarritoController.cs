using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Usuario")]
public class CarritoController(CarritoClientService carrito, PedidosClientService pedidos) : Controller
{
    public async Task<IActionResult> Index()
    {
        List<Producto>? lista = [];
        try
        {
            //var carritoService = HttpContext.RequestServices.GetService<CarritoClientService>();
            lista = await carrito.ObtenerCarrito();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Salir", "Auth");
            }
        }

        return View(lista);
    }

    public async Task<IActionResult> SeguirComprando()
    {
        return RedirectToAction("Index", "Comprar");
    }

    public async Task<IActionResult> LimpiarCarrito()
    {
        try
        {
            //var carritoService = HttpContext.RequestServices.GetService<CarritoClientService>();
            await carrito.LimpiarCarrito();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Salir", "Auth");
            }
        }
        return RedirectToAction("Index", "Carrito");
    }

    public async Task<IActionResult> FinalizarCompra()
    {
        if (await carrito.ObtenerCarrito() == null)
        {
            return RedirectToAction("Index", "Carrito");
        }

        try
        {
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Salir", "Auth");
            }
            
            await pedidos.PostAsync(userName);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Salir", "Auth");
            }
        }

        return RedirectToAction("Index", "Carrito");
    }

    [HttpPost]
    public async Task<IActionResult> EliminarDelCarrito(int id)
    {
        try
        {
            await carrito.EliminarDelCarrito(id);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Salir", "Auth");
            }
        }

        return RedirectToAction("Index", "Carrito");
    }
}
