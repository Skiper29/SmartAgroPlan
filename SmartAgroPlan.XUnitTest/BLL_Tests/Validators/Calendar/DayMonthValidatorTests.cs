using FluentAssertions;
using SmartAgroPlan.BLL.Validators.Calendar;
using SmartAgroPlan.DAL.Entities.Calendar;

namespace SmartAgroPlan.XUnitTest.BLL_Tests.Validators.Calendar;

public class DayMonthValidatorTests
{
    private readonly DayMonthValidator _validator;

    public DayMonthValidatorTests()
    {
        _validator = new DayMonthValidator();
    }

    [Theory]
    [InlineData(1, 1, true)]   // January 1st
    [InlineData(31, 1, true)]  // January 31st
    [InlineData(29, 2, true)]  // February 29th (leap year consideration)
    [InlineData(31, 3, true)]  // March 31st
    [InlineData(30, 4, true)]  // April 30th
    [InlineData(31, 5, true)]  // May 31st
    [InlineData(30, 6, true)]  // June 30th
    [InlineData(31, 7, true)]  // July 31st
    [InlineData(31, 8, true)]  // August 31st
    [InlineData(30, 9, true)]  // September 30th
    [InlineData(31, 10, true)] // October 31st
    [InlineData(30, 11, true)] // November 30th
    [InlineData(31, 12, true)] // December 31st
    public void Validate_ValidDayMonth_ShouldBeValid(int day, int month, bool expectedIsValid)
    {
        // Arrange
        var dayMonth = new DayMonth(day, month);

        // Act
        var result = _validator.Validate(dayMonth);

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }

    [Theory]
    [InlineData(0, 1)]    // Invalid day (0)
    [InlineData(32, 1)]   // Invalid day for January
    [InlineData(30, 2)]   // Invalid day for February
    [InlineData(32, 3)]   // Invalid day for March
    [InlineData(31, 4)]   // Invalid day for April
    [InlineData(32, 5)]   // Invalid day for May
    [InlineData(31, 6)]   // Invalid day for June
    [InlineData(32, 7)]   // Invalid day for July
    [InlineData(32, 8)]   // Invalid day for August
    [InlineData(31, 9)]   // Invalid day for September
    [InlineData(32, 10)]  // Invalid day for October
    [InlineData(31, 11)]  // Invalid day for November
    [InlineData(32, 12)]  // Invalid day for December
    public void Validate_InvalidDay_ShouldBeInvalid(int day, int month)
    {
        // Arrange
        var dayMonth = new DayMonth(day, month);

        // Act
        var result = _validator.Validate(dayMonth);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Day");
    }

    [Theory]
    [InlineData(1, 0)]   // Invalid month (0)
    [InlineData(1, 13)]  // Invalid month (13)
    [InlineData(1, -1)]  // Invalid month (-1)
    public void Validate_InvalidMonth_ShouldBeInvalid(int day, int month)
    {
        // Arrange
        var dayMonth = new DayMonth(day, month);

        // Act
        var result = _validator.Validate(dayMonth);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Month");
    }

    [Fact]
    public void Validate_February29_ShouldBeValid()
    {
        // Arrange
        var dayMonth = new DayMonth(29, 2);

        // Act
        var result = _validator.Validate(dayMonth);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, "Day must be between 1 and 31 for month 1.")]
    [InlineData(2, "Day must be between 1 and 29 for month 2.")]
    [InlineData(4, "Day must be between 1 and 30 for month 4.")]
    public void Validate_InvalidDay_ShouldHaveCorrectErrorMessage(int month, string expectedMessage)
    {
        // Arrange
        var dayMonth = new DayMonth(0, month); // Invalid day

        // Act
        var result = _validator.Validate(dayMonth);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(expectedMessage));
    }

    [Fact]
    public void Validate_InvalidMonth_ShouldHaveCorrectErrorMessage()
    {
        // Arrange
        var dayMonth = new DayMonth(1, 13);
        var expectedMessage = "Month must be between 1 and 12.";

        // Act
        var result = _validator.Validate(dayMonth);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == expectedMessage);
    }
}