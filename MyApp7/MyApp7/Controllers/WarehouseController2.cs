using System.Data;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyApp7.DTOs;
using MyApp7.Repositories;

namespace MyApp7.Controllers;
[ApiController]
[Route("[controller]")]
public class WarehouseController2:ControllerBase
{
    private readonly IConfiguration _configuration;

    public WarehouseController2(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    [Route("/api/warehouses2")]
    public async Task<IActionResult> doSomeThings(ProductOfWarehouse item)
    {

        try
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("AddProductToWarehouse", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        

                        command.Parameters.AddWithValue("@IdProduct", item.IdProduct);
                        command.Parameters.AddWithValue("@IdWarehouse", item.IdWarehouse);
                        command.Parameters.AddWithValue("@Amount", item.Amount);
                        command.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }
                scope.Complete();
            }
        }
        catch (TransactionAbortedException e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return Ok();
    }
    
}