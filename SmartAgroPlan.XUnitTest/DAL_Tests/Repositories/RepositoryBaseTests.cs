using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Enums;
using SmartAgroPlan.DAL.Persistence;
using SmartAgroPlan.DAL.Repositories.Repositories.Realizations.Base;
using SmartAgroPlan.XUnitTest.DAL_Tests.Utils;

namespace SmartAgroPlan.XUnitTest.DAL_Tests.Repositories;

public class RepositoryBaseTests : IDisposable
{
    private readonly SmartAgroPlanDbContext _context;
    private readonly RepositoryBase<Soil> _repository;

    public RepositoryBaseTests()
    {
        _context = InMemoryDbContextFactory.CreateSeededInMemoryDbContext();
        _repository = new RepositoryBase<Soil>(_context);
    }

    #region Create Tests

    [Fact]
    public void Create_ValidEntity_ShouldAddEntityToContext()
    {
        // Arrange
        var newSoil = InMemoryDbContextFactory.GetTestSoil();

        // Act
        var result = _repository.Create(newSoil);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(newSoil);
        _context.Entry(result).State.Should().Be(EntityState.Added);
    }

    [Fact]
    public void Create_NullEntity_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<NullReferenceException>(() => _repository.Create(null!));
    }

    [Fact]
    public async Task CreateAsync_ValidEntity_ShouldAddEntityToContext()
    {
        // Arrange
        var newSoil = InMemoryDbContextFactory.GetTestSoil();

        // Act
        var result = await _repository.CreateAsync(newSoil);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(newSoil);
        _context.Entry(result).State.Should().Be(EntityState.Added);
    }

    [Fact]
    public async Task CreateAsync_NullEntity_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.CreateAsync(null!));
        exception.ParamName.Should().Be("entity");
    }

    [Fact]
    public async Task CreateRangeAsync_ValidEntities_ShouldAddAllEntitiesToContext()
    {
        // Arrange
        var soils = InMemoryDbContextFactory.GetTestSoils();

        // Act
        await _repository.CreateRangeAsync(soils);

        // Assert
        foreach (var soil in soils)
        {
            _context.Entry(soil).State.Should().Be(EntityState.Added);
        }
    }

    [Fact]
    public async Task CreateRangeAsync_EmptyCollection_ShouldNotThrow()
    {
        // Arrange
        var emptySoils = new List<Soil>();

        // Act & Assert
        await _repository.CreateRangeAsync(emptySoils);
        // Should not throw
    }

    #endregion

    #region Read Tests

    [Fact]
    public void FindAll_WithoutPredicate_ShouldReturnAllEntities()
    {
        // Act
        var result = _repository.FindAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeAssignableTo<IQueryable<Soil>>();
    }

    [Fact]
    public void FindAll_WithPredicate_ShouldReturnFilteredEntities()
    {
        // Arrange
        Expression<Func<Soil, bool>> predicate = s => s.Type == SoilType.Clay;

        // Act
        var result = _repository.FindAll(predicate);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Type.Should().Be(SoilType.Clay);
    }

    [Fact]
    public async Task GetAllAsync_WithoutPredicate_ShouldReturnAllEntities()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeAssignableTo<IEnumerable<Soil>>();
    }

    [Fact]
    public async Task GetAllAsync_WithPredicate_ShouldReturnFilteredEntities()
    {
        // Arrange
        Expression<Func<Soil, bool>> predicate = s => s.WaterRetention > 80;

        // Act
        var result = await _repository.GetAllAsync(predicate);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Type.Should().Be(SoilType.Clay);
    }

    [Fact]
    public async Task GetFirstOrDefaultAsync_WithPredicate_ShouldReturnFirstMatchingEntity()
    {
        // Arrange
        Expression<Func<Soil, bool>> predicate = s => s.Type == SoilType.Loamy;

        // Act
        var result = await _repository.GetFirstOrDefaultAsync(predicate);

        // Assert
        result.Should().NotBeNull();
        result!.Type.Should().Be(SoilType.Loamy);
    }

    [Fact]
    public async Task GetFirstOrDefaultAsync_WithNonMatchingPredicate_ShouldReturnNull()
    {
        // Arrange
        Expression<Func<Soil, bool>> predicate = s => s.Type == SoilType.Rocky && s.WaterRetention > 100;

        // Act
        var result = await _repository.GetFirstOrDefaultAsync(predicate);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetFirstOrDefaultAsync_WithSelector_ShouldReturnTransformedEntity()
    {
        // Arrange
        Expression<Func<Soil, Soil>> selector = s => new Soil
        {
            Id = s.Id,
            Type = s.Type,
            WaterRetention = s.WaterRetention
        };
        Expression<Func<Soil, bool>> predicate = s => s.Type == SoilType.Clay;

        // Act
        var result = await _repository.GetFirstOrDefaultAsync(selector, predicate);

        // Assert
        result.Should().NotBeNull();
        result!.Type.Should().Be(SoilType.Clay);
        result.WaterRetention.Should().Be(85.0);
    }

    [Fact]
    public async Task GetSingleOrDefaultAsync_WithUniquePredicate_ShouldReturnSingleEntity()
    {
        // Arrange
        Expression<Func<Soil, bool>> predicate = s => s.Type == SoilType.Sandy;

        // Act
        var result = await _repository.GetSingleOrDefaultAsync(predicate);

        // Assert
        result.Should().NotBeNull();
        result!.Type.Should().Be(SoilType.Sandy);
    }

    [Fact]
    public async Task GetSingleOrDefaultAsync_WithNonUniquePredicate_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Expression<Func<Soil, bool>> predicate = s => s.WaterRetention > 40; // Will match multiple

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.GetSingleOrDefaultAsync(predicate));
    }

    #endregion

    #region Update Tests

    [Fact]
    public void Update_ValidEntity_ShouldMarkEntityAsModified()
    {
        // Arrange
        var existingSoil = _context.Soils.First();
        existingSoil.WaterRetention = 99.9;

        // Act
        var result = _repository.Update(existingSoil);

        // Assert
        result.Should().NotBeNull();
        result.Entity.Should().Be(existingSoil);
        result.State.Should().Be(EntityState.Modified);
    }

    [Fact]
    public void Update_NullEntity_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<NullReferenceException>(() => _repository.Update(null!));
    }

    [Fact]
    public void UpdateRange_ValidEntities_ShouldMarkAllEntitiesAsModified()
    {
        // Arrange
        var existingSoils = _context.Soils.Take(2).ToList();
        foreach (var soil in existingSoils)
        {
            soil.WaterRetention += 10;
        }

        // Act
        _repository.UpdateRange(existingSoils);

        // Assert
        foreach (var soil in existingSoils)
        {
            _context.Entry(soil).State.Should().Be(EntityState.Modified);
        }
    }

    #endregion

    #region Delete Tests

    [Fact]
    public void Delete_ValidEntity_ShouldMarkEntityAsDeleted()
    {
        // Arrange
        var existingSoil = _context.Soils.First();

        // Act
        _repository.Delete(existingSoil);

        // Assert
        _context.Entry(existingSoil).State.Should().Be(EntityState.Deleted);
    }

    [Fact]
    public void Delete_NullEntity_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _repository.Delete(null!));
        exception.ParamName.Should().Be("entity");
    }

    [Fact]
    public void DeleteRange_ValidEntities_ShouldMarkAllEntitiesAsDeleted()
    {
        // Arrange
        var existingSoils = _context.Soils.Take(2).ToList();

        // Act
        _repository.DeleteRange(existingSoils);

        // Assert
        foreach (var soil in existingSoils)
        {
            _context.Entry(soil).State.Should().Be(EntityState.Deleted);
        }
    }

    #endregion

    #region Entity Management Tests

    [Fact]
    public void Attach_DetachedEntity_ShouldAttachEntityToContext()
    {
        // Arrange
        using var newContext = InMemoryDbContextFactory.CreateInMemoryDbContext();
        var detachedSoil = InMemoryDbContextFactory.GetTestSoil();
        detachedSoil.Id = 999; // Set an ID to simulate existing entity

        var newRepository = new RepositoryBase<Soil>(newContext);

        // Act
        newRepository.Attach(detachedSoil);

        // Assert
        newContext.Entry(detachedSoil).State.Should().Be(EntityState.Unchanged);
    }

    [Fact]
    public void Entry_ValidEntity_ShouldReturnEntityEntry()
    {
        // Arrange
        var existingSoil = _context.Soils.First();

        // Act
        var result = _repository.Entry(existingSoil);

        // Assert
        result.Should().NotBeNull();
        result.Entity.Should().Be(existingSoil);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task FullCrudWorkflow_ShouldWorkCorrectly()
    {
        // Arrange
        var newSoil = new Soil
        {
            Type = SoilType.Peaty,
            WaterRetention = 90.0,
            Acidity = 5.5,
            NutrientContent = 700.0,
            OrganicMatter = 8.0,
            SoilDensity = 0.8,
            ErosionRisk = 10.0
        };

        // Act & Assert - Create
        var createdSoil = await _repository.CreateAsync(newSoil);
        await _context.SaveChangesAsync();

        createdSoil.Should().NotBeNull();
        createdSoil.Id.Should().BeGreaterThan(0);

        // Act & Assert - Read
        var retrievedSoil = await _repository.GetFirstOrDefaultAsync(s => s.Id == createdSoil.Id);
        retrievedSoil.Should().NotBeNull();
        retrievedSoil!.Type.Should().Be(SoilType.Peaty);

        // Act & Assert - Update
        _context.ChangeTracker.Clear();
        retrievedSoil.WaterRetention = 95.0;
        _repository.Update(retrievedSoil);
        await _context.SaveChangesAsync();

        var updatedSoil = await _repository.GetFirstOrDefaultAsync(s => s.Id == createdSoil.Id);
        updatedSoil!.WaterRetention.Should().Be(95.0);

        // Act & Assert - Delete
        _context.ChangeTracker.Clear();
        _repository.Delete(updatedSoil);
        await _context.SaveChangesAsync();

        var deletedSoil = await _repository.GetFirstOrDefaultAsync(s => s.Id == createdSoil.Id);
        deletedSoil.Should().BeNull();
    }

    [Fact]
    public async Task ComplexQuery_WithMultiplePredicates_ShouldReturnCorrectResults()
    {
        // Arrange
        Expression<Func<Soil, bool>> complexPredicate = s =>
            s.WaterRetention > 50 &&
            s.Acidity >= 6.0 &&
            s.ErosionRisk < 30;

        // Act
        var results = await _repository.GetAllAsync(complexPredicate);

        // Assert
        results.Should().NotBeNull();
        results.Should().HaveCount(2); // Loamy and Clay soils should match
        results.Should().OnlyContain(s => s.WaterRetention > 50 && s.Acidity >= 6.0 && s.ErosionRisk < 30);
    }

    [Fact]
    public async Task BatchOperations_ShouldWorkCorrectly()
    {
        // Arrange
        var soilsToAdd = new List<Soil>
        {
            new()
            {
                Type = SoilType.Chalky,
                WaterRetention = 60.0,
                Acidity = 8.0,
                NutrientContent = 500.0,
                OrganicMatter = 1.5,
                SoilDensity = 1.3,
                ErosionRisk = 20.0
            },
            new()
            {
                Type = SoilType.Silty,
                WaterRetention = 70.0,
                Acidity = 6.8,
                NutrientContent = 800.0,
                OrganicMatter = 3.5,
                SoilDensity = 1.5,
                ErosionRisk = 18.0
            }
        };

        // Act - Batch Create
        await _repository.CreateRangeAsync(soilsToAdd);
        await _context.SaveChangesAsync();

        // Assert - Verify creation
        var allSoils = await _repository.GetAllAsync();
        allSoils.Should().HaveCount(5); // 3 original + 2 new

        // Act - Batch Update
        _context.ChangeTracker.Clear();
        var soilsToUpdate = allSoils.Where(s => s.Type == SoilType.Chalky || s.Type == SoilType.Silty).ToList();
        foreach (var soil in soilsToUpdate)
        {
            soil.WaterRetention += 5.0;
        }
        _repository.UpdateRange(soilsToUpdate);
        await _context.SaveChangesAsync();

        // Assert - Verify update
        var updatedSoils = await _repository.GetAllAsync(s => s.Type == SoilType.Chalky || s.Type == SoilType.Silty);
        updatedSoils.Should().OnlyContain(s => s.WaterRetention == 65.0 || s.WaterRetention == 75.0);

        // Act - Batch Delete
        _repository.DeleteRange(soilsToUpdate);
        await _context.SaveChangesAsync();

        // Assert - Verify deletion
        var remainingSoils = await _repository.GetAllAsync();
        remainingSoils.Should().HaveCount(3); // Back to original 3
    }

    #endregion

    #region Edge Cases and Error Handling

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ShouldReturnEmptyCollection()
    {
        // Arrange
        using var emptyContext = InMemoryDbContextFactory.CreateInMemoryDbContext("empty_db");
        var emptyRepository = new RepositoryBase<Soil>(emptyContext);

        // Act
        var result = await emptyRepository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void FindAll_WithComplexPredicate_ShouldCompileAndExecute()
    {
        // Arrange
        Expression<Func<Soil, bool>> complexPredicate = s =>
            (s.Type == SoilType.Clay || s.Type == SoilType.Loamy) &&
            s.WaterRetention * s.OrganicMatter > 200 &&
            Math.Abs(s.Acidity - 7.0) < 1.0;

        // Act
        var result = _repository.FindAll(complexPredicate);

        // Assert
        result.Should().NotBeNull();
        var materializedResult = result.ToList();
        materializedResult.Should().HaveCount(2); // Clay and Loamy should match
    }

    [Fact]
    public async Task ConcurrentOperations_ShouldNotInterfere()
    {
        // Arrange
        var tasks = new List<Task<Soil>>();

        // Act - Multiple concurrent reads
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(_repository.GetFirstOrDefaultAsync(s => s.Type == SoilType.Loamy)!);
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(5);
        results.Should().OnlyContain(r => r != null && r.Type == SoilType.Loamy);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}
