using System.Data;
using Microsoft.Data.SqlClient;
using MyApp7.DTOs;

namespace MyApp7.Repositories;

public class WarehouseRepository: IWarehouseRepository
{
    private readonly IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    
    public async Task<bool> DoesProductExist(int id)
    {
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        
        
        command.CommandText = "SELECT 1 from Product where idProduct = @id";
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();

        return res is not null;
        
    }

    public async Task<bool> DoesWarehouseExist(int id)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 from Warehouse where IdWarehouse = @id";
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<bool> DoesProductExistInOrder(int prductId, int amount, DateTime time)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 from [Order]  where [Order].IdProduct = @id and [Order].Amount = @amount and [Order].CreatedAt < @time and [Order].FulfilledAt is null";
        command.Parameters.AddWithValue("@id", prductId);
        command.Parameters.AddWithValue("@amount", amount);
        command.Parameters.AddWithValue("@time", DateTime.Now);
        
        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<int> AddProduct(ProductOfWarehouse product)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();
        
        
        
        // get price from Product
        command.CommandText = "select Price from Product where Product.idProduct = @idProduct";
        command.Parameters.AddWithValue("@idProduct", product.IdProduct);
        decimal resultOfPrice;
        
        using (var reader = await command.ExecuteReaderAsync())
        {
            var priceOrdinal = reader.GetOrdinal("Price");
            await reader.ReadAsync();
            resultOfPrice = reader.GetDecimal(priceOrdinal);
            
        }
        
        //get idOrder from Order
        command.Parameters.Clear();
        command.CommandText = "select IdOrder from [Order] join Product on [Order].IdProduct = Product.IdProduct where [Order].IdProduct = @idProduct";
        command.Parameters.AddWithValue("@idProduct", product.IdProduct);
        var idOrder = await command.ExecuteScalarAsync();
        
        
        //insert into Product_Warehouse
        command.Parameters.Clear();
        command.CommandText =
            "insert into Product_Warehouse values (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt);";
        command.Parameters.AddWithValue("@IdWarehouse", product.IdWarehouse);
        command.Parameters.AddWithValue("@IdProduct", product.IdProduct);
        command.Parameters.AddWithValue("@IdOrder", idOrder);
        command.Parameters.AddWithValue("@Amount", product.Amount);
        command.Parameters.AddWithValue("@Price", resultOfPrice);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        
      int insertedProduct = Convert.ToInt32(await command.ExecuteNonQueryAsync());
      
      return insertedProduct;
    }

    public async Task<bool> DoesOrderDone(ProductOfWarehouse product)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();
        
        
        //get idOrder from Order
        command.Parameters.Clear();
        command.CommandText = "select IdOrder from [Order] join Product on [Order].IdProduct = Product.IdProduct where [Order].IdProduct = @idProduct";
        command.Parameters.AddWithValue("@idProduct", product.IdProduct);
        var idOrder = await command.ExecuteScalarAsync();
        
        
        command.Parameters.Clear();
        command.CommandText = "select 1 from Product_WareHouse where Product_WareHouse.IdOrder = @IdOrder";
        command.Parameters.AddWithValue("@IdOrder", idOrder);
        var orederCompleted = await command.ExecuteScalarAsync();

        return orederCompleted is not null;
    }

    public async Task UpdateFullfilledAtOrder(int idProduct)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();

        int IdOrder = await FindOrderByIdProduct(idProduct);
        
        command.CommandText = "update [Order] set [Order].FulfilledAt = @time where [Order].IdOrder = @IdOrder";
        command.Parameters.AddWithValue("@time", DateTime.Now);
        command.Parameters.AddWithValue("@IdOrder", IdOrder);
        var idOrder = await command.ExecuteScalarAsync();
    }

    public async Task<int> FindOrderByIdProduct(int idProduct)
    {
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();
        
        //get idOrder from Order
        command.Parameters.Clear();
        command.CommandText = "select IdOrder from [Order] join Product on [Order].IdProduct = Product.IdProduct where [Order].IdProduct = @idProduct";
        command.Parameters.AddWithValue("@idProduct", idProduct);
        int? idOrder = await command.ExecuteScalarAsync() as int?;
        int result = idOrder ?? 0;
        return result;
    }
}