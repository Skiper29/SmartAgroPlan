using SmartAgroPlan.DAL.Entities.FertilizerForecasting;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

/// <summary>
///     Service for managing fertilizer products
/// </summary>
public interface IFertilizerProductService
{
    /// <summary>
    ///     Gets all available fertilizer products
    /// </summary>
    Task<IEnumerable<FertilizerProduct>> GetAllProductsAsync();

    /// <summary>
    ///     Gets products filtered by type
    /// </summary>
    Task<IEnumerable<FertilizerProduct>> GetProductsByTypeAsync(FertilizerType type);

    /// <summary>
    ///     Searches products by name or description
    /// </summary>
    Task<IEnumerable<FertilizerProduct>> SearchProductsAsync(string searchTerm);

    /// <summary>
    ///     Gets a specific product by ID
    /// </summary>
    Task<FertilizerProduct?> GetProductByIdAsync(int productId);

    /// <summary>
    ///     Creates a new fertilizer product
    /// </summary>
    Task<int> CreateProductAsync(FertilizerProduct product);

    /// <summary>
    ///     Updates an existing product
    /// </summary>
    Task UpdateProductAsync(FertilizerProduct product);

    /// <summary>
    ///     Deletes a product
    /// </summary>
    Task DeleteProductAsync(int productId);
}