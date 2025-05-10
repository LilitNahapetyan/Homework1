using Homework1;
using Homework1.Controllers;
using Homework1.Models;
using Homework1.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text.Json;

public class PostsControllerTests
{
    private readonly ApiSettings _apiSettings = new ApiSettings
    {
        JsonPlaceholderBaseUrl = "https://jsonplaceholder.typicode.com/posts"
    };

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    [Fact]
    public async Task GetPost_ReturnsOk_WhenPostExists()
    {
        // Arrange
        var post = new Post
        {
            UserId = 1,
            Id = 1,
            Title = "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
            Body = "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
        };

        var postServiceMock = new Mock<IPostService>();
        postServiceMock
            .Setup(service => service.GetPostAsync(1))
            .ReturnsAsync(post);

        var controller = new PostsController(postServiceMock.Object);

        // Act
        var result = await controller.GetPost(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPost = Assert.IsType<Post>(okResult.Value);
        Assert.Equal(1, returnedPost.Id);
    }

    [Fact]
    public async Task GetPosts_ReturnsOk_WhenPostsExist()
    {
        // Arrange
        var expectedPosts = new List<Post>
        {
            new Post
            {
                UserId = 1,
                Id = 1,
                Title = "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
                Body = "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
            }
        };

        var postServiceMock = new Mock<IPostService>();
        postServiceMock
            .Setup(service => service.GetPostsAsync(1, "sunt aut facere repellat provident occaecati excepturi optio reprehenderit"))
            .ReturnsAsync(expectedPosts);

        var controller = new PostsController(postServiceMock.Object);

        // Act
        var result = await controller.GetPosts(1, "sunt aut facere repellat provident occaecati excepturi optio reprehenderit");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPosts = Assert.IsType<List<Post>>(okResult.Value);
        Assert.Single(returnedPosts);
        Assert.Equal(expectedPosts[0].Id, returnedPosts[0].Id);
    }

    [Fact]
    public async Task DeletePost_ReturnsNoContent_WhenSuccessOrNotFound()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        postServiceMock
            .Setup(service => service.DeletePostAsync(1))
            .ReturnsAsync(true); // Simulate successful deletion

        var controller = new PostsController(postServiceMock.Object);

        // Act
        var result = await controller.DeletePost(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetPost_ReturnsNotFound_WhenPostDoesNotExist()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        postServiceMock
            .Setup(service => service.GetPostAsync(999))
            .ReturnsAsync((Post)null); // Simulate not found

        var controller = new PostsController(postServiceMock.Object);

        // Act
        var result = await controller.GetPost(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task GetPosts_ReturnsNoContent_WhenNoPostsFound()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        postServiceMock
            .Setup(service => service.GetPostsAsync(99, "nonexistent"))
            .ReturnsAsync(new List<Post>()); // Simulate no posts found

        var controller = new PostsController(postServiceMock.Object);

        // Act
        var result = await controller.GetPosts(99, "nonexistent");

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task DeletePost_ReturnsError_WhenServerFails()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        postServiceMock
            .Setup(service => service.DeletePostAsync(999))
            .ReturnsAsync(false); // Simulate server error

        var controller = new PostsController(postServiceMock.Object);

        // Act
        var result = await controller.DeletePost(999);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
}
