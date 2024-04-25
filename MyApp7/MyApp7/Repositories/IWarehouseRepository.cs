using MyApp7.DTOs;

namespace MyApp7.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExist(int id);
    Task<bool> DoesWarehouseExist(int id);
    Task<bool> DoesProductExistInOrder(int prductId, int amount, DateTime time);
    Task<int> AddProduct(ProductOfWarehouse product);

    Task<bool> DoesOrderDone(ProductOfWarehouse product);
    Task UpdateFullfilledAtOrder(int idProduct);

    Task<int> FindOrderByIdProduct(int idProduct);
}