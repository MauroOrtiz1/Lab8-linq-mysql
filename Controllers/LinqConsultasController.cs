using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab8_BernieOrtiz.DTOs;
using Lab8_BernieOrtiz.Models;  

namespace Lab8_BernieOrtiz.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LinqConsultasController : ControllerBase
    {
        private readonly dbcontext _context; 

        public LinqConsultasController(dbcontext context)  
        {
            _context = context;
        }

        [HttpGet("clientes-con-pedidos")]
        public async Task<IActionResult> GetClientesConPedidos()
        {
            var clientOrders = await _context.Clients
                .AsNoTracking()
                .Select(client => new ClientOrderDto
                {
                    ClientName = client.Name,
                    Orders = client.Orders
                        .Select(order => new OrderDto
                        {
                            OrderId = order.OrderId,
                            OrderDate = order.OrderDate
                        }).ToList()
                })
                .ToListAsync();

            return Ok(clientOrders);
        }
        [HttpGet("pedido-con-detalles")]
        public async Task<IActionResult> GetPedidosConDetalles()
        {
            var ordersWithDetails = await _context.Orders
                .Include(order => order.Orderdetails)  
                .ThenInclude(orderDetail => orderDetail.Product)
                .AsNoTracking()
                .Select(order => new OrderDetailsDto
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    Products = order.Orderdetails.Select(od => new ProductDto
                    {
                        ProductName = od.Product.Name,
                        Quantity = od.Quantity,
                        Price = od.Product.Price
                    }).ToList()
                })
                .ToListAsync();

            return Ok(ordersWithDetails);
        }
        
    }
}