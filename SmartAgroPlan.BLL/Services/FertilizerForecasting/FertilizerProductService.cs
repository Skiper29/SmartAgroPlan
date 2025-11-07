using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.Services.FertilizerForecasting;

public class FertilizerProductService : IFertilizerProductService
{
    private readonly IRepositoryWrapper _repository;

    public FertilizerProductService(IRepositoryWrapper repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FertilizerProduct>> GetAllProductsAsync()
    {
        return (await _repository.FertilizerProductRepository.GetAllAsync()).ToList();
    }

    public async Task<IEnumerable<FertilizerProduct>> GetProductsByTypeAsync(FertilizerType type)
    {
        return await _repository.FertilizerProductRepository.GetAllAsync(p => p.Type == type);
    }

    public async Task<IEnumerable<FertilizerProduct>> SearchProductsAsync(string searchTerm)
    {
        return await _repository.FertilizerProductRepository.GetAllAsync(p =>
            (p.Name != null && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (p.Manufacturer != null && p.Manufacturer.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        );
    }

    public async Task<FertilizerProduct?> GetProductByIdAsync(int productId)
    {
        return await _repository.FertilizerProductRepository
            .GetFirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<int> CreateProductAsync(FertilizerProduct product)
    {
        var created = await _repository.FertilizerProductRepository.CreateAsync(product);
        await _repository.SaveChangesAsync();
        return created.Id;
    }

    public async Task UpdateProductAsync(FertilizerProduct product)
    {
        _repository.FertilizerProductRepository.Update(product);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int productId)
    {
        var product = await _repository.FertilizerProductRepository
            .GetFirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
            throw new ArgumentException($"Product {productId} not found");

        _repository.FertilizerProductRepository.Delete(product);
        await _repository.SaveChangesAsync();
    }
}
