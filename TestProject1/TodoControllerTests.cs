using Microsoft.AspNetCore.Mvc;
using Xunit;
using Task1.Models;
using Task1;
using Task1.Controllers;
using Microsoft.EntityFrameworkCore;


namespace TestProject1;

public class TodoControllerTests
{
    private async Task<TodoContext> GetDatabaseContextAsync()
    {
        var dbName = System.Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var dbContext = new TodoContext(options);
        dbContext.TodoItems.AddRange(GetTestTodoItems());
        await dbContext.SaveChangesAsync();
        return dbContext;
    }
    private List<Todo> GetTestTodoItems()
    {
        return new List<Todo>
        {
            new Todo { Id = 1, Title = "Todo 1", Description = "Description 1" },
            new Todo { Id = 2, Title = "Todo 2", Description = "Description 2" },
            new Todo { Id = 3, Title = "Todo 3", Description = "Description 3" }
        };
    }
    [Fact]
    public async Task GetTodoItems_ShouldReturnAllItems()
    {
        // Arrange
        var dbContext = await GetDatabaseContextAsync();
        var controller = new TodoItemsController(dbContext);

        // Act
        var result = await controller.GetTodoItems();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Todo>>>(result);
        var items = Assert.IsAssignableFrom<IEnumerable<Todo>>(actionResult.Value);
        Assert.Equal(3, ((List<Todo>)items).Count);
    }
    [Fact]
    public async Task GetTodoItem_ShouldReturnItemById()
    {
        // Arrange
        var dbContext = await GetDatabaseContextAsync();
        var controller = new TodoItemsController(dbContext);
        long itemId = 1;

        // Act
        var result = await controller.GetTodoItem(itemId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Todo>>(result);
        var item = actionResult.Value as Todo;
        Assert.NotNull(item);
        Assert.Equal("Todo 1", item.Title);
    }

    [Fact]
    public async Task PutTodoItem_ShouldUpdateItem()
    {
        // Arrange
        var dbContext = await GetDatabaseContextAsync();
        var controller = new TodoItemsController(dbContext);
        long itemId = 1;
        var updatedTodoItem = new Todo
        {
            Id = itemId,
            Title = "Updated Todo 1",
            Description = "Updated Description 1"
        };

        // To avoid entity tracking conflict, detach the existing entity
        var originalItem = await dbContext.TodoItems.FindAsync(itemId);
        dbContext.Entry(originalItem).State = EntityState.Detached;
        
        // Act
        var putResult = await controller.PutTodoItem(itemId, updatedTodoItem);
        var updatedItemResult = await controller.GetTodoItem(itemId);

        // Assert
        Assert.IsType<NoContentResult>(putResult);
        var actionResult = Assert.IsType<ActionResult<Todo>>(updatedItemResult);
        var updatedItem = actionResult.Value as Todo;
        Assert.NotNull(updatedItem);
        Assert.Equal(updatedTodoItem.Title, updatedItem.Title);
        Assert.Equal(updatedTodoItem.Description, updatedItem.Description);
    
    }
    [Fact]
    public async Task PostTodoItem_ShouldCreateNewItem()
    {
        // Arrange
        var dbContext = await GetDatabaseContextAsync();
        var controller = new TodoItemsController(dbContext);
        var newTodoItem = new Todo
        {
            Title = "New Todo",
            Description = "New Description"
        };

        // Act
        var postResult = await controller.PostTodoItem(newTodoItem);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(postResult.Result);
        var createdItem = Assert.IsAssignableFrom<Todo>(actionResult.Value);
        Assert.Equal("New Todo", createdItem.Title);
        Assert.Equal("New Description", createdItem.Description);
    }
    [Fact]
    public async Task DeleteTodoItem_ShouldRemoveItem()
    {
        // Arrange
        var dbContext = await GetDatabaseContextAsync();
        var controller = new TodoItemsController(dbContext);
        long itemId = 1;
        var itemToDelete = await dbContext.TodoItems.FindAsync(itemId);

        // Act
        await controller.DeleteTodoItem(itemId);
        var deletedResult = await dbContext.TodoItems.FindAsync(itemId);
        var remainingItems = await controller.GetTodoItems();

        // Assert
        Assert.Null(deletedResult);
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Todo>>>(remainingItems);
        var items = Assert.IsAssignableFrom<IEnumerable<Todo>>(actionResult.Value);
        Assert.Equal(2, items.Count());
    }
    

}