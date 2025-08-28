using FluentAssertions;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.BLL.Validators.Fields.Soil;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.Validators.Fields.Soil;

public class BaseSoilValidatorTests
{
    private readonly BaseSoilValidator _validator;

    public BaseSoilValidatorTests()
    {
        _validator = new BaseSoilValidator();
    }

    [Fact]
    public void Validate_ValidSoil_ShouldBeValid()
    {
        // Arrange
        var validSoil = new SoilCreateUpdateDto
        {
            Type = SoilType.Loamy,
            WaterRetention = 50.0,
            Acidity = 7.0,
            NutrientContent = 500.0,
            OrganicMatter = 25.0,
            SoilDensity = 1.3,
            ErosionRisk = 15.0
        };

        // Act
        var result = _validator.Validate(validSoil);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(SoilType.Clay)]
    [InlineData(SoilType.Sandy)]
    [InlineData(SoilType.Loamy)]
    [InlineData(SoilType.Peaty)]
    [InlineData(SoilType.Saline)]
    [InlineData(SoilType.Chalky)]
    [InlineData(SoilType.Silty)]
    [InlineData(SoilType.Rocky)]
    public void Validate_AllValidSoilTypes_ShouldBeValid(SoilType soilType)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.Type = soilType;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(101.0)]
    public void Validate_InvalidWaterRetention_ShouldBeInvalid(double waterRetention)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.WaterRetention = waterRetention;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "WaterRetention");
        result.Errors.Should().Contain(e =>
            e.ErrorMessage == $"Water retention must be between {BaseSoilValidator.MinPercentage} and {BaseSoilValidator.MaxPercentage} percent.");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(50.0)]
    [InlineData(100.0)]
    public void Validate_ValidWaterRetention_ShouldBeValid(double waterRetention)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.WaterRetention = waterRetention;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(3.4)]
    [InlineData(9.6)]
    public void Validate_InvalidAcidity_ShouldBeInvalid(double acidity)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.Acidity = acidity;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Acidity");
        result.Errors.Should().Contain(e => e.ErrorMessage == $"Acidity must be between {BaseSoilValidator.MinAcidity} and {BaseSoilValidator.MaxAcidity}.");
    }

    [Theory]
    [InlineData(3.5)]
    [InlineData(7.0)]
    [InlineData(9.5)]
    public void Validate_ValidAcidity_ShouldBeValid(double acidity)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.Acidity = acidity;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NegativeNutrientContent_ShouldBeInvalid()
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.NutrientContent = -10.0;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NutrientContent");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Nutrient content must be non-negative.");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(100.0)]
    [InlineData(1000.0)]
    public void Validate_ValidNutrientContent_ShouldBeValid(double nutrientContent)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.NutrientContent = nutrientContent;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(101.0)]
    public void Validate_InvalidOrganicMatter_ShouldBeInvalid(double organicMatter)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.OrganicMatter = organicMatter;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrganicMatter");
        result.Errors.Should().Contain(e =>
            e.ErrorMessage == $"Organic matter must be between {BaseSoilValidator.MinPercentage} and {BaseSoilValidator.MaxPercentage} percent.");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(50.0)]
    [InlineData(100.0)]
    public void Validate_ValidOrganicMatter_ShouldBeValid(double organicMatter)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.OrganicMatter = organicMatter;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Validate_InvalidSoilDensity_ShouldBeInvalid(double soilDensity)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.SoilDensity = soilDensity;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SoilDensity");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Soil density must be positive.");
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    public void Validate_ValidSoilDensity_ShouldBeValid(double soilDensity)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.SoilDensity = soilDensity;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(101.0)]
    public void Validate_InvalidErosionRisk_ShouldBeInvalid(double erosionRisk)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.ErosionRisk = erosionRisk;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ErosionRisk");
        result.Errors.Should().Contain(e =>
            e.ErrorMessage == $"Erosion risk must be between {BaseSoilValidator.MinPercentage} and {BaseSoilValidator.MaxPercentage} percent.");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(50.0)]
    [InlineData(100.0)]
    public void Validate_ValidErosionRisk_ShouldBeValid(double erosionRisk)
    {
        // Arrange
        var soil = CreateValidSoil();
        soil.ErosionRisk = erosionRisk;

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_AllInvalidProperties_ShouldHaveMultipleErrors()
    {
        // Arrange
        var invalidSoil = new SoilCreateUpdateDto
        {
            Type = (SoilType)999, // Invalid enum value
            WaterRetention = -10.0,
            Acidity = 2.0,
            NutrientContent = -50.0,
            OrganicMatter = 150.0,
            SoilDensity = -1.0,
            ErosionRisk = 200.0
        };

        // Act
        var result = _validator.Validate(invalidSoil);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(7);
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
        result.Errors.Should().Contain(e => e.PropertyName == "WaterRetention");
        result.Errors.Should().Contain(e => e.PropertyName == "Acidity");
        result.Errors.Should().Contain(e => e.PropertyName == "NutrientContent");
        result.Errors.Should().Contain(e => e.PropertyName == "OrganicMatter");
        result.Errors.Should().Contain(e => e.PropertyName == "SoilDensity");
        result.Errors.Should().Contain(e => e.PropertyName == "ErosionRisk");
    }

    private static SoilCreateUpdateDto CreateValidSoil()
    {
        return new SoilCreateUpdateDto
        {
            Type = SoilType.Loamy,
            WaterRetention = 50.0,
            Acidity = 7.0,
            NutrientContent = 500.0,
            OrganicMatter = 25.0,
            SoilDensity = 1.3,
            ErosionRisk = 15.0
        };
    }
}