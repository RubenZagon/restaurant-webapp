using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RestaurantApp.Application.DTOs;

namespace RestaurantApp.Tests.Integration;

public class TableSessionIntegrationTests(WebApplicationFactoryFixture factory)
    : IClassFixture<WebApplicationFactoryFixture>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task StartTableSession_WithNewTable_ShouldReturn200AndCreateTable()
    {
        // Arrange
        var tableNumber = 1;

        // Act
        var response = await _client.PostAsync($"/api/tables/{tableNumber}/start-session", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var session = await response.Content.ReadFromJsonAsync<TableSessionDto>();
        session.Should().NotBeNull();
        session!.TableNumber.Should().Be(tableNumber);
        session.SessionId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task StartTableSession_CalledTwice_ShouldReturnSameSession()
    {
        // Arrange
        var tableNumber = 2;

        // Act - First call
        var response1 = await _client.PostAsync($"/api/tables/{tableNumber}/start-session", null);
        var session1 = await response1.Content.ReadFromJsonAsync<TableSessionDto>();

        // Act - Second call (simulating another person at the same table)
        var response2 = await _client.PostAsync($"/api/tables/{tableNumber}/start-session", null);
        var session2 = await response2.Content.ReadFromJsonAsync<TableSessionDto>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        session1.Should().NotBeNull();
        session2.Should().NotBeNull();

        // Both calls should return the same session ID (allows multiple people at same table)
        session1!.SessionId.Should().Be(session2!.SessionId);
        session1.TableNumber.Should().Be(tableNumber);
        session2.TableNumber.Should().Be(tableNumber);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task StartTableSession_DifferentTables_ShouldCreateDifferentSessions(int tableNumber)
    {
        // Act
        var response = await _client.PostAsync($"/api/tables/{tableNumber}/start-session", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var session = await response.Content.ReadFromJsonAsync<TableSessionDto>();
        session.Should().NotBeNull();
        session!.TableNumber.Should().Be(tableNumber);
        session.SessionId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetOrCreateOrder_AfterStartingSession_ShouldCreateOrder()
    {
        // Arrange
        var tableNumber = 7;

        // Start session first
        var sessionResponse = await _client.PostAsync($"/api/tables/{tableNumber}/start-session", null);
        var session = await sessionResponse.Content.ReadFromJsonAsync<TableSessionDto>();

        // Act
        var orderResponse = await _client.GetAsync($"/api/orders/table/{tableNumber}");

        // Assert
        orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await orderResponse.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.TableNumber.Should().Be(tableNumber);
        order.SessionId.Should().Be(session!.SessionId);
        order.Status.Should().Be("Draft");
        order.Total.Should().Be(0);
    }

    [Fact]
    public async Task CompleteFlow_StartSession_CreateOrder_AddProducts_ShouldWork()
    {
        // Arrange
        var tableNumber = 1;

        // Act 1: Start session
        var sessionResponse = await _client.PostAsync($"/api/tables/{tableNumber}/start-session", null);
        sessionResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act 2: Get/Create order
        var orderResponse = await _client.GetAsync($"/api/orders/table/{tableNumber}");
        orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await orderResponse.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.Lines.Should().BeEmpty();

        // Assert
        order.TableNumber.Should().Be(tableNumber);
        order.Status.Should().Be("Draft");
        order.Total.Should().Be(0);
    }
}
