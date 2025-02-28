using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.SharedKernel;

public class LocationShould
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 10)]
    public void BeCorrectWhenParamsIsCorrectOnCreated(int x, int y)
    {
        // Arrange
        
        // Act
        var location = Location.Create(x, y);
        
        // Assert
        location.IsSuccess.Should().BeTrue();
        location.Value.X.Should().Be(x);
        location.Value.Y.Should().Be(y);
    }
    
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    [InlineData(11, 1)]
    [InlineData(1, 11)]
    public void ReturnErrorWhenParamsIsInCorrectOnCreated(int x, int y)
    {
        // Arrange
        
        // Act
        var location = Location.Create(x, y);
        
        // Assert
        location.IsSuccess.Should().BeFalse();
        location.Error.Should().NotBeNull();
    }
    
    [Fact]
    public void BeEqualWhenAllPropertiesIsEqual()
    {
        // Arrange
        var first = Location.Create(5, 5).Value;
        var second = Location.Create(5, 5).Value;
        
        // Act
        var result = first == second;
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1, 5)]
    [InlineData(5, 1)]
    public void BeNotEqualWhenPartPropertiesIsNotEqual(int x, int y)
    {
        // Arrange
        var first = Location.Create(5, 5).Value;
        var second = Location.Create(x, y).Value;
        
        // Act
        var result = first == second;
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(1, 1, 8)]
    [InlineData(10, 10, 10)]
    [InlineData(5, 5, 0)]
    public void BeCorrectWhenCalculateDistance(int x, int y, int resultDistance)
    {
        // Arrange
        var first = Location.Create(5, 5).Value;
        var second = Location.Create(x, y).Value;
        
        // Act
        var result = first.CalculateDistance(second);
        
        // Assert
        result.Should().Be(resultDistance);
    }
    
    [Fact]
    public void BeCorrectWhenRandomOnCreated()
    {
        // Arrange
        
        // Act
        var location = Location.Random;
        
        // Assert
        location.Should().NotBeNull();
        location.X.Should().BeGreaterThan(0);
        location.Y.Should().BeGreaterThan(0);
        location.X.Should().BeLessThan(11);
        location.Y.Should().BeLessThan(11);
    }
}