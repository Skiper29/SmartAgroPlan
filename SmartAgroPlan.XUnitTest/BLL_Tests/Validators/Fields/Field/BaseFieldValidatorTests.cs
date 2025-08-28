using FluentAssertions;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.BLL.Validators.Fields.Field;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.Validators.Fields.Field;

public class BaseFieldValidatorTests
{
    private readonly BaseFieldValidator _validator;

    public BaseFieldValidatorTests()
    {
        _validator = new BaseFieldValidator();
    }

    [Fact]
    public void Validate_ValidField_ShouldBeValid()
    {
        // Arrange
        var validField = new FieldCreateUpdateDto
        {
            Name = "Test Field",
            Location = "Test Location",
            BoundaryGeoJson = """{"type":"Polygon","coordinates":[[[0,0],[1,0],[1,1],[0,1],[0,0]]]}""",
            FieldType = FieldType.Arable,
            CurrentCropId = 1,
            SoilId = 1
        };

        // Act
        var result = _validator.Validate(validField);

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
        var field = CreateValidField();
        field.Name = name;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Field name is required.");
    }

    [Fact]
    public void Validate_NameTooLong_ShouldBeInvalid()
    {
        // Arrange
        var longName = new string('a', BaseFieldValidator.MaxNameLength + 1); // exceeds limit
        var field = CreateValidField();
        field.Name = longName;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.ErrorMessage == $"Field name cannot exceed {BaseFieldValidator.MaxNameLength} characters.");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Test Field")]
    public void Validate_ValidName_ShouldBeValid(string name)
    {
        // Arrange
        var field = CreateValidField();
        field.Name = name;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NameExactly100Characters_ShouldBeValid()
    {
        // Arrange
        var exactLengthName = new string('a', BaseFieldValidator.MaxNameLength); // Exactly 100 characters
        var field = CreateValidField();
        field.Name = exactLengthName;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyOrNullLocation_ShouldBeInvalid(string? location)
    {
        // Arrange
        var field = CreateValidField();
        field.Location = location;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Location");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Location is required.");
    }

    [Fact]
    public void Validate_LocationTooLong_ShouldBeInvalid()
    {
        // Arrange
        var longLocation = new string('a', BaseFieldValidator.MaxLocationLength + 1); // exceeds limit
        var field = CreateValidField();
        field.Location = longLocation;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Location");
        result.Errors.Should().Contain(e => e.ErrorMessage == $"Location cannot exceed {BaseFieldValidator.MaxLocationLength} characters.");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Test Location, City, Country")]
    public void Validate_ValidLocation_ShouldBeValid(string location)
    {
        // Arrange
        var field = CreateValidField();
        field.Location = location;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_LocationExactly200Characters_ShouldBeValid()
    {
        // Arrange
        var exactLengthLocation = new string('a', BaseFieldValidator.MaxLocationLength); // Exactly 200 characters
        var field = CreateValidField();
        field.Location = exactLengthLocation;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyOrNullBoundaryGeoJson_ShouldBeInvalid(string? geoJson)
    {
        // Arrange
        var field = CreateValidField();
        field.BoundaryGeoJson = geoJson;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BoundaryGeoJson");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Boundary GeoJSON is required.");
    }

    [Theory]
    [InlineData("invalid json")]
    [InlineData("{\"type\":\"Point\",\"coordinates\":[0,0]}")]  // Not a polygon
    public void Validate_InvalidBoundaryGeoJson_ShouldBeInvalid(string geoJson)
    {
        // Arrange
        var field = CreateValidField();
        field.BoundaryGeoJson = geoJson;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BoundaryGeoJson");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Boundary GeoJSON must be a valid GeoJSON Polygon.");
    }

    [Theory]
    [InlineData("""{"type":"Polygon","coordinates":[[[0,0],[1,0],[1,1],[0,1],[0,0]]]}""")]
    [InlineData("""{"type":"Polygon","coordinates":[[[0,0],[2,0],[2,2],[0,2],[0,0]]]}""")]
    public void Validate_ValidBoundaryGeoJson_ShouldBeValid(string geoJson)
    {
        // Arrange
        var field = CreateValidField();
        field.BoundaryGeoJson = geoJson;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(FieldType.Arable)]
    [InlineData(FieldType.Pasture)]
    [InlineData(FieldType.Orchard)]
    [InlineData(FieldType.Greenhouse)]
    [InlineData(FieldType.Fallow)]
    public void Validate_ValidFieldType_ShouldBeValid(FieldType fieldType)
    {
        // Arrange
        var field = CreateValidField();
        field.FieldType = fieldType;

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidFieldType_ShouldBeInvalid()
    {
        // Arrange
        var field = CreateValidField();
        field.FieldType = (FieldType)999; // Invalid enum value

        // Act
        var result = _validator.Validate(field);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FieldType");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Invalid field type.");
    }

    [Fact]
    public void Validate_AllInvalidProperties_ShouldHaveMultipleErrors()
    {
        // Arrange
        var invalidField = new FieldCreateUpdateDto
        {
            Name = "",                    // Invalid
            Location = "",                // Invalid
            BoundaryGeoJson = "",         // Invalid
            FieldType = (FieldType)999,   // Invalid
            CurrentCropId = 1,
            SoilId = 1
        };

        // Act
        var result = _validator.Validate(invalidField);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(5);
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "Location");
        result.Errors.Should().Contain(e => e.PropertyName == "BoundaryGeoJson");
        result.Errors.Should().Contain(e => e.PropertyName == "FieldType");
    }

    private static FieldCreateUpdateDto CreateValidField()
    {
        return new FieldCreateUpdateDto
        {
            Name = "Test Field",
            Location = "Test Location",
            BoundaryGeoJson = """{"type":"Polygon","coordinates":[[[0,0],[1,0],[1,1],[0,1],[0,0]]]}""",
            FieldType = FieldType.Arable,
            CurrentCropId = 1,
            SoilId = 1
        };
    }
}