using System.ComponentModel.DataAnnotations;

namespace MyApp7.DTOs;

public class ProductOfWarehouse
{
    [Required]
    public int IdProduct { get; set; }
    
    [Required]
    public int IdWarehouse { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "The amount has to be 1 or grather")]
    public int Amount { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
}