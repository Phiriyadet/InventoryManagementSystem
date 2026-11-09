using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Common.Errors;
using Inventory.Application.Customers;
using Inventory.Application.Customers.DTOs;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly CustomerService _sut;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CustomerServiceTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _sut = new CustomerService(_customerRepositoryMock.Object);
    }

    #region Request Builders

    private static CreateCustomerRequest BuildCreateRequest(
        string cusId = "C0001",
        string cusName = "Test Customer")
        => new(cusId, cusName);

    private static UpdateCustomerRequest BuildUpdateRequest(
        string cusName = "Updated Customer")
        => new(cusName);

    #endregion

    #region AddCustomerAsync

    [Fact]
    public async Task AddCustomerAsync_WhenCusIdIsEmpty_ReturnsEmptyIdError()
    {
        var request = BuildCreateRequest(cusId: "");

        var result = await _sut.AddCustomerAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.EmptyId, result.Error);
        _customerRepositoryMock.Verify(
            x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task AddCustomerAsync_WhenCusNameIsEmpty_ReturnsEmptyNameError()
    {
        var request = BuildCreateRequest(cusName: "");

        var result = await _sut.AddCustomerAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.EmptyName, result.Error);
    }

    [Theory]
    [InlineData("C001")]
    [InlineData("C00001")]
    public async Task AddCustomerAsync_WhenCusIdLengthIsInvalid_ReturnsLengthIdError(string cusId)
    {
        var request = BuildCreateRequest(cusId: cusId);

        var result = await _sut.AddCustomerAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.LengthId, result.Error);
    }

    [Fact]
    public async Task AddCustomerAsync_WhenCusNameExceeds30Characters_ReturnsLengthNameError()
    {
        var request = BuildCreateRequest(cusName: new string('A', 31));

        var result = await _sut.AddCustomerAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.LengthName, result.Error);
    }

    [Fact]
    public async Task AddCustomerAsync_WhenCustomerAlreadyExists_ReturnsDuplicateIdError()
    {
        var request = BuildCreateRequest();

        _customerRepositoryMock
            .Setup(x => x.ExistsAsync(request.CusId, _cancellationToken))
            .ReturnsAsync(true);

        var result = await _sut.AddCustomerAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.DuplicateId(request.CusId), result.Error);

        _customerRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task AddCustomerAsync_WhenRequestIsValid_ReturnsCreatedCustomer()
    {
        var request = BuildCreateRequest();

        var expectedCustomer = new Customer
        {
            CusId = request.CusId,
            CusName = request.CusName
        };

        _customerRepositoryMock
            .Setup(x => x.ExistsAsync(request.CusId, _cancellationToken))
            .ReturnsAsync(false);

        _customerRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Customer>(), _cancellationToken))
            .ReturnsAsync(expectedCustomer);

        var result = await _sut.AddCustomerAsync(request, _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedCustomer, result.Value);

        _customerRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Customer>(c =>
                    c.CusId == request.CusId &&
                    c.CusName == request.CusName),
                _cancellationToken),
            Times.Once);
    }

    #endregion

    #region DeleteCustomerAsync

    [Fact]
    public async Task DeleteCustomerAsync_WhenCustomerExists_ReturnsSuccess()
    {
        var id = "C0001";

        _customerRepositoryMock
            .Setup(x => x.DeleteAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var result = await _sut.DeleteCustomerAsync(id, _cancellationToken);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteCustomerAsync_WhenCustomerDoesNotExist_ReturnsNotFoundError()
    {
        var id = "C0001";

        _customerRepositoryMock
            .Setup(x => x.DeleteAsync(id, _cancellationToken))
            .ReturnsAsync(false);

        var result = await _sut.DeleteCustomerAsync(id, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.NotFound(id), result.Error);
    }

    #endregion

    #region GetAllCustomerAsync

    [Fact]
    public async Task GetAllCustomerAsync_ReturnsAllCustomers()
    {
        var customers = new List<Customer>
        {
            new() { CusId = "C0001", CusName = "Customer 1" },
            new() { CusId = "C0002", CusName = "Customer 2" }
        };

        _customerRepositoryMock
            .Setup(x => x.GetAllAsync(_cancellationToken))
            .ReturnsAsync(customers);

        var result = await _sut.GetAllCustomerAsync(_cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }

    #endregion

    #region GetCustomerByIdAsync

    [Fact]
    public async Task GetCustomerByIdAsync_WhenCustomerExists_ReturnsCustomer()
    {
        var customer = new Customer
        {
            CusId = "C0001",
            CusName = "Test Customer"
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customer.CusId, _cancellationToken))
            .ReturnsAsync(customer);

        var result = await _sut.GetCustomerByIdAsync(customer.CusId, _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(customer, result.Value);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WhenCustomerDoesNotExist_ReturnsNotFoundError()
    {
        var id = "C0001";

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(id, _cancellationToken))
            .ReturnsAsync((Customer?)null);

        var result = await _sut.GetCustomerByIdAsync(id, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.NotFound(id), result.Error);
    }

    #endregion

    #region GetCustomerByNameAsync

    [Fact]
    public async Task GetCustomerByNameAsync_ReturnsMatchingCustomers()
    {
        var customers = new List<Customer>
        {
            new() { CusId = "C0001", CusName = "Test Customer" }
        };

        _customerRepositoryMock
            .Setup(x => x.GetByNameAsync("Test", _cancellationToken))
            .ReturnsAsync(customers);

        var result = await _sut.GetCustomerByNameAsync("Test", _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(customers, result.Value);
    }

    #endregion

    #region UpdateCustomerAsync

    [Fact]
    public async Task UpdateCustomerAsync_WhenCusNameIsEmpty_ReturnsEmptyNameError()
    {
        var request = BuildUpdateRequest(cusName: "");

        var result = await _sut.UpdateCustomerAsync("C0001", request, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.EmptyName, result.Error);

        _customerRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerAsync_WhenCustomerExists_ReturnsUpdatedCustomer()
    {
        var id = "C0001";
        var request = BuildUpdateRequest();

        _customerRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Customer>(), _cancellationToken))
            .ReturnsAsync(true);

        var result = await _sut.UpdateCustomerAsync(id, request, _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.CusId);
        Assert.Equal(request.CusName, result.Value.CusName);

        _customerRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<Customer>(c =>
                    c.CusId == id &&
                    c.CusName == request.CusName),
                _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerAsync_WhenCustomerDoesNotExist_ReturnsNotFoundError()
    {
        var id = "C0001";
        var request = BuildUpdateRequest();

        _customerRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Customer>(), _cancellationToken))
            .ReturnsAsync(false);

        var result = await _sut.UpdateCustomerAsync(id, request, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.NotFound(id), result.Error);
    }

    #endregion
}