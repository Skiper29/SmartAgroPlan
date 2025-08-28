using FluentAssertions;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.BLL.Validators.Fields.Soil;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.Validators.Fields.Soil;

public class UpdateSoilValidatorTests
{
    private readonly UpdateSoilValidator _validator;

    public UpdateSoilValidatorTests()
    {
        var baseSoilValidator = new BaseSoilValidator();
        _validator = new UpdateSoilValidator(baseSoilValidator);
    }

    [Fact]
    public void Validate_ValidSoilUpdateDto_ShouldBeValid()
    {
        // Arrange
        var validSoil = new SoilUpdateDto
        {
            Id = 1,
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
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_InvalidId_ShouldBeInvalid(int id)
    {
        // Arrange
        var soil = new SoilUpdateDto
        {
            Id = id,
            Type = SoilType.Loamy,
            WaterRetention = 50.0,
            Acidity = 7.0,
            NutrientContent = 500.0,
            OrganicMatter = 25.0,
            SoilDensity = 1.3,
            ErosionRisk = 15.0
        };

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Id must be a positive integer.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public void Validate_ValidId_ShouldBeValid(int id)
    {
        // Arrange
        var soil = new SoilUpdateDto
        {
            Id = id,
            Type = SoilType.Loamy,
            WaterRetention = 50.0,
            Acidity = 7.0,
            NutrientContent = 500.0,
            OrganicMatter = 25.0,
            SoilDensity = 1.3,
            ErosionRisk = 15.0
        };

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldIncludeBaseSoilValidatorRules()
    {
        // Arrange
        var soil = new SoilUpdateDto
        {
            Id = 1,
            Type = SoilType.Clay,
            WaterRetention = 101.0, // Invalid - should trigger base validator rule
            Acidity = 7.0,
            NutrientContent = 500.0,
            OrganicMatter = 25.0,
            SoilDensity = 1.3,
            ErosionRisk = 15.0
        };

        // Act
        var result = _validator.Validate(soil);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "WaterRetention");
        result.Errors.Should().Contain(e =>
            e.ErrorMessage == $"Water retention must be between {BaseSoilValidator.MinPercentage} and {BaseSoilValidator.MaxPercentage} percent.");
    }

    [Fact]
    public void Validate_InvalidIdAndOtherProperties_ShouldHaveMultipleErrors()
    {
        // Arrange
        var invalidSoil = new SoilUpdateDto
        {
            Id = 0,              // Invalid
            Type = SoilType.Loamy,
            WaterRetention = -10.0, // Invalid
            Acidity = 2.0,          // Invalid
            NutrientContent = -50.0, // Invalid
            OrganicMatter = 150.0,  // Invalid
            SoilDensity = -1.0,     // Invalid
            ErosionRisk = 200.0     // Invalid
        };

        // Act
        var result = _validator.Validate(invalidSoil);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(7); // Id + 6 base validator errors
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
        result.Errors.Should().Contain(e => e.PropertyName == "WaterRetention");
        result.Errors.Should().Contain(e => e.PropertyName == "Acidity");
        result.Errors.Should().Contain(e => e.PropertyName == "NutrientContent");
        result.Errors.Should().Contain(e => e.PropertyName == "OrganicMatter");
        result.Errors.Should().Contain(e => e.PropertyName == "SoilDensity");
        result.Errors.Should().Contain(e => e.PropertyName == "ErosionRisk");
    }
}