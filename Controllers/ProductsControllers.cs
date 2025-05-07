using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab8_BernieOrtiz.Models;
using Lab8_BernieOrtiz.Repositories;

namespace Lab8_BernieOrtiz.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    //EJERCICIO 2
    [HttpGet("filter-by-price")]
    public async Task<IActionResult> GetProductsByMinPrice([FromQuery] decimal minPrice) {
        var productsRepo = _unitOfWork.Repository<Product>();
        var products = await productsRepo.GetAllAsync();

        var result = products
            .Where(p => p.Price > minPrice)
            .ToList();

        return Ok(result);
    }
    //EJERCICIO 5
    [HttpGet("GetMostExpensiveProduct")]
    public async Task<IActionResult> GetMostExpensiveProduct() {
        var productsRepo = _unitOfWork.Repository<Product>();
        var products = await productsRepo.GetAllAsync();
        
        var mostExpensiveProduct = products
            .OrderByDescending(p => p.Price)
            .FirstOrDefault();

        if (mostExpensiveProduct == null)
            return NotFound("No hay productos en la base de datos.");

        return Ok(mostExpensiveProduct);
    }
    // EJERCICIO 7: 
    [HttpGet("GetAveragePrice")]
    public async Task<IActionResult> GetAveragePrice() {
        var productsRepo = _unitOfWork.Repository<Product>();

        var products = await productsRepo.GetAllAsync();

        var averagePrice = products
            .Select(p => p.Price)
            .Average();

        return Ok(new { AveragePrice = averagePrice });
    }
    // EJERCICIO 8: 
    [HttpGet("GetProductsWithoutDescription")]
    public async Task<IActionResult> GetProductsWithoutDescription() {
        var productsRepo = _unitOfWork.Repository<Product>();

        var products = await productsRepo.GetAllAsync();

        var productsWithoutDescription = products
            .Where(p => string.IsNullOrEmpty(p.Description))
            .ToList();

        if (!productsWithoutDescription.Any())
            return NotFound("No hay productos sin descripción.");

        return Ok(productsWithoutDescription);
    }
    
}