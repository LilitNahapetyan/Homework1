using Homework1;
using Homework1.Models;
using Homework1.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

public class PostServiceTests
{
    private readonly ApiSettings _apiSettings = new ApiSettings
    {
        JsonPlaceholderBaseUrl = "https://jsonplaceholder.typicode.com/posts"
    };

    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    private HttpClient CreateMockHttpClient(HttpResponseMessage responseMessage)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.IsAny<HttpRequestMessage>(),
               ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(responseMessage);

        return new HttpClient(handlerMock.Object);
    }

    [Fact]
    public async Task GetPostAsync_ReturnsCorrectPost_WhenSuccess()
    {
        // Arrange
        var expectedPost = new Post
        {
            Id = 1,
            UserId = 1,
            Title = "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
            Body = "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
        };

        var json = JsonSerializer.Serialize(expectedPost, _jsonOptions);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new PostService(httpClient, Options.Create(_apiSettings), _jsonOptions);

        // Act
        var result = await service.GetPostAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPost.Id, result.Id);
        Assert.Equal(expectedPost.UserId, result.UserId);
        Assert.Equal(expectedPost.Title, result.Title);
        Assert.Equal(expectedPost.Body, result.Body);
    }


    [Fact]
    public async Task GetPostAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var httpClient = CreateMockHttpClient(response);
        var service = new PostService(httpClient, Options.Create(_apiSettings), _jsonOptions);

        // Act
        var result = await service.GetPostAsync(123);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPostsAsync_ReturnsPosts_WhenSuccess()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new PostService(httpClient, Options.Create(_apiSettings), _jsonOptions);

        // Act
        var result = (await service.GetPostsAsync(1, "sunt aut facere repellat provident occaecati excepturi optio reprehenderit")).ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, post =>
        {
            Assert.Equal(1, post.UserId);
        });
    }


    [Fact]
    public async Task GetPostsAsync_ReturnsEmpty_WhenNotSuccess()
    {
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        var httpClient = CreateMockHttpClient(response);
        var service = new PostService(httpClient, Options.Create(_apiSettings), _jsonOptions);

        var result = await service.GetPostsAsync(1, "t");

        Assert.Empty(result);
    }

    [Fact]
    public async Task DeletePostAsync_ReturnsTrue_WhenSuccess()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var httpClient = CreateMockHttpClient(response);
        var service = new PostService(httpClient, Options.Create(_apiSettings), _jsonOptions);

        var result = await service.DeletePostAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeletePostAsync_ReturnsTrue_WhenNotFound()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var httpClient = CreateMockHttpClient(response);
        var service = new PostService(httpClient, Options.Create(_apiSettings), _jsonOptions);

        var result = await service.DeletePostAsync(123);

        Assert.True(result);
    }

    [Fact]
    public async Task DeletePostAsync_ReturnsFalse_WhenServerError()
    {
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        var httpClient = CreateMockHttpClient(response);
        var service = new PostService(httpClient, Options.Create(_apiSettings), _jsonOptions);

        var result = await service.DeletePostAsync(1);

        Assert.False(result);
    }
}
