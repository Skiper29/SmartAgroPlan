using FluentAssertions;
using Moq;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.BLL.Validators.Calendar;
using SmartAgroPlan.BLL.Validators.Crops;
using SmartAgroPlan.DAL.Entities.Calendar;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.Validators.Crops;

public class BaseCropVarietyValidatorTests
{
    private readonly Mock<DayMonthValidator> _mockDayMonthValidator;
    private readonly BaseCropVarietyValidator _validator;

    public BaseCropVarietyValidatorTests()
    {
        _mockDayMonthValidator = new Mock<DayMonthValidator>();
        _validator = new BaseCropVarietyValidator(_mockDayMonthValidator.Object);
    }

    [Fact]
    public void Validate_ValidCropVariety_ShouldBeValid()
    {
        // Arrange
        var validCrop = new CropVarietyCreateUpdateDto
        {
            Name = "Test Crop",
            CropType = CropType.Wheat,
            WaterRequirement = 500.0,
            FertilizerRequirement = 100.0,
            GrowingDuration = 120,
            LIni = 20,
            LDev = 30,
            LMid = 50,
            LLate = 20,
            SowingStart = new DayMonth(15, 3),
            SowingEnd = new DayMonth(30, 4),
            MinTemperature = 10.0,
            MaxTemperature = 30.0,
            HarvestYield = 5.5,
            AdditionalNotes = "Test notes",
            OptimalSoilId = 1
        };

        // Act
        var result = _validator.Validate(validCrop);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyOrNullName_ShouldBeInvalid(string? name)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.Name = name;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Name is required.");
    }

    [Fact]
    public void Validate_NameTooLong_ShouldBeInvalid()
    {
        // Arrange
        var longName = new string('a', BaseCropVarietyValidator.MaxNameLength + 1); //exceeds limit
        var crop = CreateValidCrop();
        crop.Name = longName;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e =>
            e.ErrorMessage == $"Name cannot exceed {BaseCropVarietyValidator.MaxNameLength} characters.");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Test Crop Variety")]
    public void Validate_ValidName_ShouldBeValid(string name)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.Name = name;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NameExactly100Characters_ShouldBeValid()
    {
        // Arrange
        var exactLengthName = new string('a', BaseCropVarietyValidator.MaxNameLength); // Exactly 100 characters
        var crop = CreateValidCrop();
        crop.Name = exactLengthName;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NegativeWaterRequirement_ShouldBeInvalid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.WaterRequirement = -10.0;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "WaterRequirement");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Water requirement must be non-negative.");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(100.0)]
    [InlineData(1000.0)]
    public void Validate_ValidWaterRequirement_ShouldBeValid(double waterRequirement)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.WaterRequirement = waterRequirement;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NegativeFertilizerRequirement_ShouldBeInvalid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.FertilizerRequirement = -5.0;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FertilizerRequirement");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Fertilizer requirement must be non-negative.");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(50.0)]
    [InlineData(500.0)]
    public void Validate_ValidFertilizerRequirement_ShouldBeValid(double fertilizerRequirement)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.FertilizerRequirement = fertilizerRequirement;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_InvalidGrowingDuration_ShouldBeInvalid(int growingDuration)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.GrowingDuration = growingDuration;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "GrowingDuration");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Growing duration must be a positive integer.");
    }

    [Theory]
    [InlineData(4, 1, 1, 1, 1)]
    [InlineData(90, 20, 30, 30, 10)]
    [InlineData(365, 50, 100, 150, 65)]
    public void Validate_ValidGrowingDuration_ShouldBeValid(int growingDuration, int LIni, int LDev, int LMid,
        int LLate)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.GrowingDuration = growingDuration;
        crop.LIni = LIni;
        crop.LDev = LDev;
        crop.LMid = LMid;
        crop.LLate = LLate;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_MinTemperatureGreaterThanMaxTemperature_ShouldBeInvalid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.MinTemperature = 30.0;
        crop.MaxTemperature = 10.0; // Less than min temperature

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MinTemperature");
        result.Errors.Should().Contain(e =>
            e.ErrorMessage == "Minimum temperature must be less than or equal to maximum temperature.");
    }

    [Fact]
    public void Validate_MinTemperatureEqualToMaxTemperature_ShouldBeValid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.MinTemperature = 20.0;
        crop.MaxTemperature = 20.0; // Equal to min temperature

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_MinTemperatureLessThanMaxTemperature_ShouldBeValid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.MinTemperature = 10.0;
        crop.MaxTemperature = 30.0; // Greater than min temperature

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NegativeHarvestYield_ShouldBeInvalid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.HarvestYield = -2.5;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "HarvestYield");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Harvest yield must be non-negative.");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(5.5)]
    [InlineData(100.0)]
    public void Validate_ValidHarvestYield_ShouldBeValid(double harvestYield)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.HarvestYield = harvestYield;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_AdditionalNotesTooLong_ShouldBeInvalid()
    {
        // Arrange
        var longNotes = new string('a', BaseCropVarietyValidator.MaxAdditionalNotesLength + 1); //exceeds limit
        var crop = CreateValidCrop();
        crop.AdditionalNotes = longNotes;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AdditionalNotes");
        result.Errors.Should().Contain(e =>
            e.ErrorMessage ==
            $"Additional notes cannot exceed {BaseCropVarietyValidator.MaxAdditionalNotesLength} characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("Short notes")]
    [InlineData(null)]
    public void Validate_ValidAdditionalNotes_ShouldBeValid(string? notes)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.AdditionalNotes = notes;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_AdditionalNotesExactly500Characters_ShouldBeValid()
    {
        // Arrange
        var exactLengthNotes =
            new string('a', BaseCropVarietyValidator.MaxAdditionalNotesLength); // Exactly 500 characters
        var crop = CreateValidCrop();
        crop.AdditionalNotes = exactLengthNotes;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_SowingEndBeforeSowingStart_ShouldBeInvalid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.SowingStart = new DayMonth(30, 4); // April 30
        crop.SowingEnd = new DayMonth(15, 3); // March 15 - before start

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SowingEnd");
        result.Errors.Should()
            .Contain(e => e.ErrorMessage == "Sowing end date must be after or equal to sowing start date.");
    }

    [Fact]
    public void Validate_SowingEndEqualToSowingStart_ShouldBeValid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.SowingStart = new DayMonth(15, 3); // March 15
        crop.SowingEnd = new DayMonth(15, 3); // March 15 - same date

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_SowingEndAfterSowingStart_ShouldBeValid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.SowingStart = new DayMonth(15, 3); // March 15
        crop.SowingEnd = new DayMonth(30, 4); // April 30 - after start

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_SowingEndSameMonthLaterDay_ShouldBeValid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.SowingStart = new DayMonth(15, 3); // March 15
        crop.SowingEnd = new DayMonth(25, 3); // March 25 - later in same month

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidLIni_ShouldBeInvalid(int lIni)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.LIni = lIni;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LIni");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidLDev_ShouldBeInvalid(int lDev)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.LDev = lDev;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LDev");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidLMid_ShouldBeInvalid(int lMid)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.LMid = lMid;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LMid");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidLLate_ShouldBeInvalid(int lLate)
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.LLate = lLate;

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LLate");
    }

    [Fact]
    public void Validate_GrowthStagesSumNotEqualToGrowingDuration_ShouldBeInvalid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.GrowingDuration = 120;
        crop.LIni = 20;
        crop.LDev = 30;
        crop.LMid = 50;
        crop.LLate = 15; // Sum is 115, not 120

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("sum of all growth stage durations"));
    }

    [Fact]
    public void Validate_GrowthStagesSumEqualsGrowingDuration_ShouldBeValid()
    {
        // Arrange
        var crop = CreateValidCrop();
        crop.GrowingDuration = 100;
        crop.LIni = 20;
        crop.LDev = 25;
        crop.LMid = 35;
        crop.LLate = 20; // Sum is 100

        // Act
        var result = _validator.Validate(crop);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_AllInvalidProperties_ShouldHaveMultipleErrors()
    {
        // Arrange
        var invalidCrop = new CropVarietyCreateUpdateDto
        {
            Name = "", // Invalid
            CropType = CropType.Wheat,
            WaterRequirement = -10.0, // Invalid
            FertilizerRequirement = -5.0, // Invalid
            GrowingDuration = 0, // Invalid
            SowingStart = new DayMonth(30, 4),
            SowingEnd = new DayMonth(15, 3), // Invalid - before start
            MinTemperature = 30.0, // Invalid - greater than max
            MaxTemperature = 10.0,
            HarvestYield = -2.5, // Invalid
            AdditionalNotes = new string('a', 501), // Invalid - too long
            OptimalSoilId = 1
        };

        // Act
        var result = _validator.Validate(invalidCrop);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(6);
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "WaterRequirement");
        result.Errors.Should().Contain(e => e.PropertyName == "FertilizerRequirement");
        result.Errors.Should().Contain(e => e.PropertyName == "GrowingDuration");
        result.Errors.Should().Contain(e => e.PropertyName == "SowingEnd");
        result.Errors.Should().Contain(e => e.PropertyName == "MinTemperature");
        result.Errors.Should().Contain(e => e.PropertyName == "HarvestYield");
        result.Errors.Should().Contain(e => e.PropertyName == "AdditionalNotes");
    }

    private static CropVarietyCreateUpdateDto CreateValidCrop()
    {
        return new CropVarietyCreateUpdateDto
        {
            Name = "Test Crop",
            CropType = CropType.Wheat,
            WaterRequirement = 500.0,
            FertilizerRequirement = 100.0,
            GrowingDuration = 120,
            LIni = 20,
            LDev = 30,
            LMid = 50,
            LLate = 20,
            SowingStart = new DayMonth(15, 3),
            SowingEnd = new DayMonth(30, 4),
            MinTemperature = 10.0,
            MaxTemperature = 30.0,
            HarvestYield = 5.5,
            AdditionalNotes = "Test notes",
            OptimalSoilId = 1
        };
    }
}
