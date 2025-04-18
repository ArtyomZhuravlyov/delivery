using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Api.Adapters.Http;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Http;

public class BasketControllerShould
{
    private readonly Guid _basketId = Guid.NewGuid();
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    [Fact]
    public async Task CheckoutBasketCorrectly()
    {
        // Arrange
        _mediator.Send(Arg.Any<CreateOrderCommand>())
            .Returns(UnitResult.Success<Error>());

        // Act
        var basketController = new DeliveryController(_mediator);
        var result = await basketController.CreateOrder();

        // Assert
        result.Should().BeOfType<OkResult>();
    }

}