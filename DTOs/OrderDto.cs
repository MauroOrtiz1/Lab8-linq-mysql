namespace Lab8_BernieOrtiz.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderDetailsDto> OrderDetails { get; set; }  
        
    }
}