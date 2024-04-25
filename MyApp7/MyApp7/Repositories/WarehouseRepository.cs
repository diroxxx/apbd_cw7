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
        // var query = "Select"
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.

    }

    public Task<bool> DoesWarehouseExist(int id)
    {
        throw new NotImplementedException();
    }
}