using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application;

public class CreateOrderCommandShould
{
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeoClient _geoClientMock;

    public CreateOrderCommandShould()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _geoClientMock = Substitute.For<IGeoClient>();
    }

    private Maybe<Order> EmptyOrder()
    {
        return null;
    }

    private Maybe<Order> ExistedOrder()
    {
        return Order.Create(Guid.NewGuid(), Location.Create(1, 1).Value).Value;
    }

    private Result<Location, Error> DefaultLocation()
    {
        return Location.Create(1, 1).Value;
    }

    [Fact]
    public async Task ReturnTrueWhenOrderExists()
    {
        //Arrange
        _orderRepositoryMock.GetAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(ExistedOrder()));
        _unitOfWork.SaveChangesAsync()
            .Returns(Task.FromResult(true));
        _geoClientMock.GetGeolocationAsync(Arg.Any<string>(), CancellationToken.None)
            .Returns(Task.FromResult(DefaultLocation()));

        //Act
        var command = new CreateOrderCommand(Guid.NewGuid(), "улица");
        var handler = new CreateOrderHandler(_unitOfWork, _orderRepositoryMock, _geoClientMock);
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ReturnTrueWhenOrderCreatedSuccessfully()
    {
        //Arrange
        _orderRepositoryMock.GetAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(EmptyOrder()));
        _unitOfWork.SaveChangesAsync()
            .Returns(Task.FromResult(true));
        _geoClientMock.GetGeolocationAsync(Arg.Any<string>(), CancellationToken.None)
            .Returns(Task.FromResult(DefaultLocation()));

        //Act
        var command = new CreateOrderCommand(Guid.NewGuid(), "улица");
        var handler = new CreateOrderHandler(_unitOfWork, _orderRepositoryMock, _geoClientMock);
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        await _orderRepositoryMock.Received(1).GetAsync(Arg.Any<Guid>());
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
    }
}
