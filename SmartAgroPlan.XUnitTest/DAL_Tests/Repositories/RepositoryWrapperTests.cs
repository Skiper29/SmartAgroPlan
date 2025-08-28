using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.DAL.Persistence;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Base;
using SmartAgroPlan.XUnitTest.DAL_Tests.Utils;

namespace SmartAgroPlan.XUnitTest.DAL_Tests.Repositories;

public class RepositoryWrapperTests
{
    private readonly SmartAgroPlanDbContext _context;
    private readonly RepositoryWrapper _wrapper;

    public RepositoryWrapperTests()
    {
        _context = InMemoryDbContextFactory.CreateSeededInMemoryDbContext();
        _wrapper = new RepositoryWrapper(_context);
    }

    [Fact]
    public void FieldRepository_ShouldReturnInstance()
    {
        // Act
        var repo = _wrapper.FieldRepository;

        // Assert
        repo.Should().NotBeNull();
    }

    [Fact]
    public void CropVarietyRepository_ShouldReturnInstance()
    {
        // Act
        var repo = _wrapper.CropVarietyRepository;

        // Assert
        repo.Should().NotBeNull();
    }

    [Fact]
    public void SoilRepository_ShouldReturnInstance()
    {
        // Act
        var repo = _wrapper.SoilRepository;

        // Assert
        repo.Should().NotBeNull();
    }

    [Fact]
    public void FieldCropHistoryRepository_ShouldReturnInstance()
    {
        // Act
        var repo = _wrapper.FieldCropHistoryRepository;

        // Assert
        repo.Should().NotBeNull();
    }

    [Fact]
    public void SaveChanges_ShouldPersistData()
    {
        // Arrange
        var soil = InMemoryDbContextFactory.GetTestSoil();

        // Act
        _wrapper.SoilRepository.Create(soil);
        _wrapper.SaveChanges();

        // Assert
        _context.Soils.Any(s => s.Type == soil.Type).Should().BeTrue();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistData()
    {
        // Arrange
        var soil = InMemoryDbContextFactory.GetTestSoil();

        // Act
        await _wrapper.SoilRepository.CreateAsync(soil);
        await _wrapper.SaveChangesAsync();

        // Assert
        (await _context.Soils.AnyAsync(s => s.Type == soil.Type)).Should().BeTrue();
    }

    [Fact]
    public void BeginTransaction_ShouldReturnTransactionScope()
    {
        // Act & Assert
        using var transaction = _wrapper.BeginTransaction();
        transaction.Should().NotBeNull();
        transaction.Dispose();
    }
}
