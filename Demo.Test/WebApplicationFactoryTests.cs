using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Demo.Client;
using Microsoft.AspNetCore.TestHost;
using Xunit.Abstractions;

namespace Demo.Test;

// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
public class WebApplicationFactoryTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public WebApplicationFactoryTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _factory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services => services.AddDemoClient()));
    }

    [Fact]
    public async Task GetAllBooksTest()
    {
        var graphQlDocument =
            _factory.Services.GetRequiredService<IDemoClient>().GetAllBooksQuery.Create(null).Document;
        var request = new { Query = Encoding.UTF8.GetString(graphQlDocument.Body) };
        _testOutputHelper.WriteLine($"request: {request}");

        var client = _factory.CreateDefaultClient();
        var response = await client.PostAsJsonAsync("/graphql", request);
        var content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"response: {response.StatusCode}, body: {content}");
        response.EnsureSuccessStatusCode();

        using var jsonDocument = JsonDocument.Parse(content);
        var booksElement = jsonDocument.RootElement.GetProperty("data").GetProperty("books").EnumerateArray();

        Assert.True(booksElement.Any());
    }

    [Theory]
    [InlineData(1, "C# in depth.")]
    public async Task GetBookByIdTest(int id, string expectedTitle)
    {
        var graphQlDocument =
            _factory.Services.GetRequiredService<IDemoClient>().GetBookByIdQuery.Create(null).Document;
        var request = new { Query = Encoding.UTF8.GetString(graphQlDocument.Body), Variables = new { Id = id } };
        _testOutputHelper.WriteLine($"request: {request}");

        var client = _factory.CreateDefaultClient();
        var response = await client.PostAsJsonAsync("/graphql", request);
        var content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"response: {response.StatusCode}, body: {content}");
        response.EnsureSuccessStatusCode();

        using var jsonDocument = JsonDocument.Parse(content);
        var titleElement = jsonDocument.RootElement.GetProperty("data").GetProperty("bookById").GetProperty("book")
            .GetProperty("title").GetString();

        Assert.Equal(expectedTitle, titleElement);
    }

    [Fact]
    public async Task AddBookTest()
    {
        var graphQlDocument =
            _factory.Services.GetRequiredService<IDemoClient>().AddBookMutation.Create(null).Document;
        var request = new
        {
            Query = Encoding.UTF8.GetString(graphQlDocument.Body),
            Variables = new
            {
                Input = new
                {
                    Book = new { Title = "To foo or not to foo...", Author = new { Name = "Lord Test McTestish" } }
                }
            }
        };
        _testOutputHelper.WriteLine($"request: {request}");
        
        var client = _factory.CreateDefaultClient();
        var response = await client.PostAsJsonAsync("/graphql", request);
        var content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"response: {response.StatusCode}, body: {content}");
        response.EnsureSuccessStatusCode();
        
        using var jsonDocument = JsonDocument.Parse(content);
        var idElement = jsonDocument.RootElement.GetProperty("data").GetProperty("addBook").GetProperty("bookEntity")
            .GetProperty("id").GetInt32();
        
        Assert.Equal(2, idElement);
    }
}