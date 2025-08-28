using FluentAssertions;
using SmartAgroPlan.BLL.DTO.Fields.FieldCropHistory;
using SmartAgroPlan.BLL.Validators.Fields.FieldCropHistory;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.Validators.Fields.FieldCropHistory;

public class CreateFieldCropHistoryValidatorTests
{
    private readonly CreateFieldCropHistoryValidator _validator;

    public CreateFieldCropHistoryValidatorTests()
    {
        var baseValidator = new BaseFieldCropHistoryValidator();
        _validator = new CreateFieldCropHistoryValidator(baseValidator);
    }

    [Fact]
    public void Validate_ValidFieldCropHistoryCreateDto_ShouldBeValid()
    {
        // Arrange
        var validHistory = new FieldCropHistoryCreateDto
        {
            FieldId = 1,
            CropId = 1,
            PlantedDate = new DateOnly(2024, 3, 15),
            HarvestedDate = new DateOnly(2024, 8, 20),
            Yield = 5.5,
            Notes = "Good harvest this season"
        };

        // Act
        var result = _validator.Validate(validHistory);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_InvalidFieldCropHistoryCreateDto_ShouldBeInvalid()
    {
        // Arrange
        var invalidHistory = new FieldCropHistoryCreateDto
        {
            FieldId = 0,              // Invalid
            CropId = -1,              // Invalid
            PlantedDate = default,    // Invalid
            HarvestedDate = new DateOnly(2024, 1, 1), // Will be invalid
            Yield = -10.0,            // Invalid
            Notes = new string('a', 301) // Invalid - too long
        };

        // Act
        var result = _validator.Validate(invalidHistory);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Validate_ShouldIncludeBaseValidatorRules()
    {
        // Arrange
        var history = new FieldCropHistoryCreateDto
        {
            FieldId = 0, // Invalid - should trigger base validator rule
            CropId = 1,
            PlantedDate = new DateOnly(2024, 3, 15),
            HarvestedDate = new DateOnly(2024, 8, 20),
            Yield = 5.5,
            Notes = "Good harvest this season"
        };

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FieldId");
        result.Errors.Should().Contain(e => e.ErrorMessage == "FieldId must be a positive integer.");
    }

    [Fact]
    public void Validate_MinimalValidData_ShouldBeValid()
    {
        // Arrange
        var history = new FieldCropHistoryCreateDto
        {
            FieldId = 1,
            CropId = 1,
            PlantedDate = new DateOnly(2024, 3, 15),
            HarvestedDate = null, // Optional
            Yield = null,         // Optional
            Notes = null          // Optional
        };

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}