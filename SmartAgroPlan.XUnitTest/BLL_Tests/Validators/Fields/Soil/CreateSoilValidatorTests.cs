using FluentAssertions;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.BLL.Validators.Fields.Soil;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.Validators.Fields.Soil;

public class CreateSoilValidatorTests
{
    private readonly CreateSoilValidator _validator;

    public CreateSoilValidatorTests()
    {
        var baseSoilValidator = new BaseSoilValidator();
        _validator = new CreateSoilValidator(baseSoilValidator);
    }

    [Fact]
    public void Validate_ValidSoilCreateDto_ShouldBeValid()
    {
        // Arrange
        var validSoil = new SoilCreateDto
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

    [Fact]
    public void Validate_InvalidSoilCreateDto_ShouldBeInvalid()
    {
        // Arrange
        var invalidSoil = new SoilCreateDto
        {
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
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Validate_ShouldIncludeBaseSoilValidatorRules()
    {
        // Arrange
        var soil = new SoilCreateDto
        {
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
}