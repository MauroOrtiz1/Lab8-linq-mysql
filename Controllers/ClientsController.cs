using Lab8_BernieOrtiz.Models;
using Lab8_BernieOrtiz.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase{
    private readonly IUnitOfWork _unitOfWork;

    public ClientsController(IUnitOfWork unitOfWork)    {
        _unitOfWork = unitOfWork;
    }
    // EJERCICIO 1
    [HttpGet("search")]
    public async Task<IActionResult> SearchClientsByName([FromQuery] string name) {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("El parámetro 'name' no puede estar vacío.");

        var clientsRepo = _unitOfWork.Repository<Client>();
        var clients = await clientsRepo.GetAllAsync();

        var result = clients
            .Where(c => c.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(result);
    }
    // EJERCICIO 9
    [HttpGet("GetClientWithMostOrders")]
    public async Task<IActionResult> GetClientWithMostOrders() {
        var ordersRepo = _unitOfWork.Repository<Order>();
        var orders = await ordersRepo.GetAllAsync();

        // Agrupar los pedidos por ClientId
        var clientOrdersCount = orders
            .GroupBy(o => o.ClientId)  // Agrupar por ClientId
            .Select(g => new {
                ClientId = g.Key,
                OrdersCount = g.Count() 
            })
            .OrderByDescending(g => g.OrdersCount)  
            .FirstOrDefault(); 

        if (clientOrdersCount == null)
            return NotFound("No hay pedidos en la base de datos.");

        return Ok(clientOrdersCount);
    }
    //Ejercicio 11:
    [HttpGet("products-sold")]
    public async Task<IActionResult> GetProductsSoldToClient([FromQuery] int clientId) {
        var ordersRepo = _unitOfWork.Repository<Order>();
        var orderDetailsRepo = _unitOfWork.Repository<Orderdetail>();
        var productsRepo = _unitOfWork.Repository<Product>();

        var orders = await ordersRepo.GetAllAsync();
        var clientOrders = orders.Where(o => o.ClientId == clientId).ToList();

        if (!clientOrders.Any())
            return NotFound($"El cliente con ID {clientId} no tiene pedidos registrados.");

        var orderDetails = await orderDetailsRepo.GetAllAsync();
        var productIds = orderDetails
            .Where(od => clientOrders.Any(o => o.OrderId == od.OrderId)) 
            .Select(od => od.ProductId)                                   
            .Distinct()
            .ToList();

        if (!productIds.Any())
            return NotFound("No se encontraron productos vendidos a este cliente.");

        var products = await productsRepo.GetAllAsync();
        var productNames = products
            .Where(p => productIds.Contains(p.ProductId))                 
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return Ok(productNames);
    }
    // Ejercicio 12:
    [HttpGet("clients-by-product")]
    public async Task<IActionResult> GetClientsByProduct([FromQuery] int productId)
    {
        var orderDetailsRepo = _unitOfWork.Repository<Orderdetail>();
        var ordersRepo = _unitOfWork.Repository<Order>();
        var clientsRepo = _unitOfWork.Repository<Client>();

        // Obtener los OrderIds que incluyen ese ProductId
        var orderDetails = await orderDetailsRepo.GetAllAsync();
        var orderIdsWithProduct = orderDetails
            .Where(od => od.ProductId == productId).Select(od => od.OrderId)
            .Distinct().ToList();

        if (!orderIdsWithProduct.Any())
            return NotFound($"No se encontraron órdenes con el producto ID {productId}.");

        // Obtener los ClientIds asociados a esos OrderIds
        var orders = await ordersRepo.GetAllAsync();
        var clientIds = orders
            .Where(o => orderIdsWithProduct.Contains(o.OrderId)).Select(o => o.ClientId)
            .Distinct().ToList();

        if (!clientIds.Any())
            return NotFound($"No se encontraron clientes que hayan comprado el producto ID {productId}.");

        // 3️⃣ Obtener los nombres de esos clientes
        var clients = await clientsRepo.GetAllAsync();
        var clientNames = clients
            .Where(c => clientIds.Contains(c.ClientId)).Select(c => c.Name)
            .Distinct().ToList();

        return Ok(clientNames);
    }
}