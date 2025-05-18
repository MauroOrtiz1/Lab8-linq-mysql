using Microsoft.EntityFrameworkCore;
using Lab8_BernieOrtiz.Models;

namespace Lab8_BernieOrtiz.DTOs;

public class ClientOrderDto
{
    public string ClientName { get; set; }
    public List<OrderDto> Orders { get; set; }
}