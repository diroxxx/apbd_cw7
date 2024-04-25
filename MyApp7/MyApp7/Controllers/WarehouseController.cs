using Microsoft.AspNetCore.Mvc;
using MyApp7.DTOs;
using MyApp7.Repositories;

namespace MyApp7.Controllers;

[ApiController]
[Route("[controller]")]
public class WarehouseController:ControllerBase
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseController(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    
    [HttpPost]
    [Route("/api/warehouses")]
    public async Task<IActionResult> SomeAction(ProductOfWarehouse item)
    {
        // item.CreatedAt = DateTime.Now;

        if (! await _warehouseRepository.DoesProductExist(item.IdProduct))
        {
            return NotFound("Given product doesn't exist");
        }

        if (! await _warehouseRepository.DoesWarehouseExist(item.IdWarehouse))
        {
            return NotFound("Given warehouse doesn't exist");
        }

        if (!await _warehouseRepository.DoesProductExistInOrder(item.IdProduct, item.Amount, item.CreatedAt))
        {
            return NotFound("The order of given product doesn't exist");
        }

        if (await _warehouseRepository.DoesOrderDone(item))
        {
            return BadRequest("The order already exist in warehouse");
        }
        
        await _warehouseRepository.UpdateFullfilledAtOrder(item.IdProduct);
        
        int result = await _warehouseRepository.AddProduct(item);
        return Ok(result);
        
    }
}