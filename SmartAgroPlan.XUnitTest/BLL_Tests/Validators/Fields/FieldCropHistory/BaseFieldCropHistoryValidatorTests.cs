using FluentAssertions;
using SmartAgroPlan.BLL.DTO.Fields.FieldCropHistory;
using SmartAgroPlan.BLL.Validators.Fields.FieldCropHistory;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.Validators.Fields.FieldCropHistory;

public class BaseFieldCropHistoryValidatorTests
{
    private readonly BaseFieldCropHistoryValidator _validator;

    public BaseFieldCropHistoryValidatorTests()
    {
        _validator = new BaseFieldCropHistoryValidator();
    }

    [Fact]
    public void Validate_ValidFieldCropHistory_ShouldBeValid()
    {
        // Arrange
        var validHistory = CreateValidHistory();

        // Act
        var result = _validator.Validate(validHistory);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_InvalidFieldId_ShouldBeInvalid(int fieldId)
    {
        // Arrange
        var history = CreateValidHistory();
        history.FieldId = fieldId;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FieldId");
        result.Errors.Should().Contain(e => e.ErrorMessage == "FieldId must be a positive integer.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public void Validate_ValidFieldId_ShouldBeValid(int fieldId)
    {
        // Arrange
        var history = CreateValidHistory();
        history.FieldId = fieldId;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_InvalidCropId_ShouldBeInvalid(int cropId)
    {
        // Arrange
        var history = CreateValidHistory();
        history.CropId = cropId;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CropId");
        result.Errors.Should().Contain(e => e.ErrorMessage == "CropId must be a positive integer.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public void Validate_ValidCropId_ShouldBeValid(int cropId)
    {
        // Arrange
        var history = CreateValidHistory();
        history.CropId = cropId;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_DefaultPlantedDate_ShouldBeInvalid()
    {
        // Arrange
        var history = CreateValidHistory();
        history.PlantedDate = null;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PlantedDate");
        result.Errors.Should().Contain(e => e.ErrorMessage == "PlantedDate is required.");
    }

    [Fact]
    public void Validate_ValidPlantedDate_ShouldBeValid()
    {
        // Arrange
        var history = CreateValidHistory();
        history.PlantedDate = new DateOnly(2024, 4, 1);

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_HarvestedDateBeforePlantedDate_ShouldBeInvalid()
    {
        // Arrange
        var history = CreateValidHistory();
        history.PlantedDate = new DateOnly(2024, 6, 1);
        history.HarvestedDate = new DateOnly(2024, 5, 1); // Before planted date

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "HarvestedDate");
        result.Errors.Should().Contain(e => e.ErrorMessage == "HarvestedDate must be after PlantedDate.");
    }

    [Fact]
    public void Validate_HarvestedDateEqualToPlantedDate_ShouldBeInValid()
    {
        // Arrange
        var plantedDate = new DateOnly(2024, 4, 15);
        var history = CreateValidHistory();
        history.PlantedDate = plantedDate;
        history.HarvestedDate = plantedDate; // Equal to planted date

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "HarvestedDate");
        result.Errors.Should().Contain(e => e.ErrorMessage == "HarvestedDate must be after PlantedDate.");
    }

    [Fact]
    public void Validate_HarvestedDateAfterPlantedDate_ShouldBeValid()
    {
        // Arrange
        var history = CreateValidHistory();
        history.PlantedDate = new DateOnly(2024, 5, 1);
        history.HarvestedDate = new DateOnly(2024, 9, 1); // After planted date

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NullHarvestedDate_ShouldBeValid()
    {
        // Arrange
        var history = CreateValidHistory();
        history.HarvestedDate = null;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NegativeYield_ShouldBeInvalid()
    {
        // Arrange
        var history = CreateValidHistory();
        history.Yield = -5.0;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Yield");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Yield must be non-negative.");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(5.5)]
    [InlineData(100.0)]
    public void Validate_ValidYield_ShouldBeValid(double yield)
    {
        // Arrange
        var history = CreateValidHistory();
        history.Yield = yield;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NullYield_ShouldBeValid()
    {
        // Arrange
        var history = CreateValidHistory();
        history.Yield = null;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NotesTooLong_ShouldBeInvalid()
    {
        // Arrange
        var longNotes = new string('a', BaseFieldCropHistoryValidator.MaxNotesLength + 1); // 301 characters - exceeds limit
        var history = CreateValidHistory();
        history.Notes = longNotes;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Notes");
        result.Errors.Should().Contain(e => e.ErrorMessage == $"Notes cannot exceed {BaseFieldCropHistoryValidator.MaxNotesLength} characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("Short note")]
    [InlineData(null)]
    public void Validate_ValidNotes_ShouldBeValid(string? notes)
    {
        // Arrange
        var history = CreateValidHistory();
        history.Notes = notes;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NotesExactly300Characters_ShouldBeValid()
    {
        // Arrange
        var exactLengthNotes = new string('a', BaseFieldCropHistoryValidator.MaxNotesLength); // Exactly 300 characters
        var history = CreateValidHistory();
        history.Notes = exactLengthNotes;

        // Act
        var result = _validator.Validate(history);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_AllInvalidProperties_ShouldHaveMultipleErrors()
    {
        // Arrange
        var invalidHistory = new FieldCropHistoryCreateUpdateDto
        {
            FieldId = 0,          // Invalid
            CropId = -1,          // Invalid
            PlantedDate = null,   // Invalid
            HarvestedDate = new DateOnly(2024, 1, 1), // Will be invalid due to planted date
            Yield = -10.0,        // Invalid
            Notes = new string('a', 301) // Invalid - too long
        };

        // Act
        var result = _validator.Validate(invalidHistory);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(5);
        result.Errors.Should().Contain(e => e.PropertyName == "FieldId");
        result.Errors.Should().Contain(e => e.PropertyName == "CropId");
        result.Errors.Should().Contain(e => e.PropertyName == "PlantedDate");
        result.Errors.Should().Contain(e => e.PropertyName == "Yield");
        result.Errors.Should().Contain(e => e.PropertyName == "Notes");
    }

    private static FieldCropHistoryCreateUpdateDto CreateValidHistory()
    {
        return new FieldCropHistoryCreateUpdateDto
        {
            FieldId = 1,
            CropId = 1,
            PlantedDate = new DateOnly(2024, 3, 15),
            HarvestedDate = new DateOnly(2024, 8, 20),
            Yield = 5.5,
            Notes = "Good harvest this season"
        };
    }
}