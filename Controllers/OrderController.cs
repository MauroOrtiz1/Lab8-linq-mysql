using Microsoft.AspNetCore.Mvc;
using Lab8_BernieOrtiz.Models;
using Lab8_BernieOrtiz.Repositories;
namespace Lab8_BernieOrtiz.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // EJERCICIO 3: Obtener detalles de la orden
    [HttpGet("GetOrderDetails")]
    public async Task<IActionResult> GetOrderDetails([FromQuery] int orderId)
    {
        var orderdetailsRepo = _unitOfWork.Repository<Orderdetail>();

        var details = await orderdetailsRepo.GetAllAsync();

        var orderDetails = details
            .Where(od => od.OrderId == orderId)
            .Select(od => new
            {
                ProductId = od.ProductId,
                ProductObjectIsNull = od.Product == null,
                Quantity = od.Quantity
            })

            .ToList();

        return Ok(orderDetails);
    }

    // EJERCICIO 4:
    [HttpGet("GetTotalQuantityByOrder")]
    public async Task<IActionResult> GetTotalQuantityByOrder([FromQuery] int orderId) {
        var orderdetailsRepo = _unitOfWork.Repository<Orderdetail>();

        var details = await orderdetailsRepo.GetAllAsync();

        var totalQuantity = details
            .Where(od => od.OrderId == orderId)
            .Sum(od => od.Quantity);

        return Ok(new { OrderId = orderId, TotalQuantity = totalQuantity });
    }

    // EJERCICIO 6: 
    [HttpGet("GetOrdersAfterDate")]
    public async Task<IActionResult> GetOrdersAfterDate([FromQuery] DateTime specificDate) {
        var ordersRepo = _unitOfWork.Repository<Order>();

        var orders = await ordersRepo.GetAllAsync();

        var filteredOrders = orders
            .Where(o => o.OrderDate > specificDate)
            .ToList();

        if (!filteredOrders.Any())
            return NotFound("No se encontraron pedidos después de la fecha especificada.");

        return Ok(filteredOrders);
    }//EJERCICIO 10
    [HttpGet("GetAllOrdersWithDetails")]
    public async Task<IActionResult> GetAllOrdersWithDetails() {
        var orderDetailsRepo = _unitOfWork.Repository<Orderdetail>();
        var orderDetails = await orderDetailsRepo.GetAllAsync();

        var result = orderDetails
            .Where(od => od.Product != null) 
            .Select(od => new {
                OrderId = od.OrderId,
                ProductName = od.Product.Name,
                Quantity = od.Quantity
            })
            .ToList();

        return Ok(result);
    }
    
    

}
