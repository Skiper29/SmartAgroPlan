using FluentAssertions;
using SmartAgroPlan.DAL.Entities.Calendar;
using SmartAgroPlan.DAL.Persistence.Converters;

namespace SmartAgroPlan.XUnitTest.DAL_Tests.Persistence.Converters;

public class DayMonthConverterTests
{
    [Fact]
    public void Convert_DayMonth_To_String_Works()
    {
        // Arrange
        var converter = new DayMonthConverter();
        var dayMonth = new DayMonth(5, 7);

        // Act
        var result = converter.ConvertToProvider(dayMonth);

        // Assert
        result.Should().Be("05-07");
    }

    [Fact]
    public void Convert_String_To_DayMonth_Works()
    {
        // Arrange
        var converter = new DayMonthConverter();
        var str = "15-12";

        // Act
        var result = (DayMonth)converter.ConvertFromProvider(str)!;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<DayMonth>();
        result.Day.Should().Be(15);
        result.Month.Should().Be(12);
    }

    [Theory]
    [InlineData("")]
    [InlineData("bad-format")]
    [InlineData("not-a-number-not-a-number")]
    public void Convert_Invalid_String_Throws(string input)
    {
        // Arrange
        var converter = new DayMonthConverter();

        // Act
        var exception = Record.Exception(() => (DayMonth)converter.ConvertFromProvider(input)!);

        // Assert
        exception.Should().NotBeNull();
        exception.Should().BeOfType<FormatException>();
    }
}

