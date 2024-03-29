using Grpc.Core;
using LearnGrpc.Data;
using LearnGrpc.Models;
using LearnGrpc;
using Microsoft.EntityFrameworkCore;
namespace LearnGrpc.Services;

public class ToDoService : TodoIt.TodoItBase
{
    private readonly AppDbContext _dbContext;
    public ToDoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
    {
        if (request.Title == string.Empty || request.Description == string.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must provide a valid object"));
        }

        var todoItem = new ToDoItem
        {
            Title = request.Title,
            Description = request.Description
        };

        await _dbContext.AddAsync(todoItem);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new CreateToDoResponse
        {
            Id = todoItem.Id
        });
    }

    public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index must be greater than zero"));

        var todoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);
        if (todoItem != null)
        {
            return await Task.FromResult(new ReadToDoResponse
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description
            });
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"No item with Id {request.Id}"));
    }

    public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
    {
        var response = new GetAllResponse();
        var todoItems = await _dbContext.ToDoItems.ToListAsync();

        foreach (var todo in todoItems)
        {
            response.ToDo.Add(new ReadToDoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description
            });
        }

        return await Task.FromResult(response);
    }

    public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0 || request.Title == string.Empty || request.Description == string.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index must be greater than zero"));

        var todoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);
        if (todoItem != null)
        {
            todoItem.Title = request.Title;
            todoItem.Description = request.Description;
            todoItem.Status = request.Status;

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new UpdateToDoResponse{
                Id = todoItem.Id
            });
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"No item with Id {request.Id}"));
    }

    public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index must be greater than zero"));

        var todoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);
         if (todoItem != null)
        {
           _dbContext.ToDoItems.Remove(todoItem);
           await _dbContext.SaveChangesAsync();

           return await Task.FromResult(new DeleteToDoResponse{
            Id = todoItem.Id
           });
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"No item with Id {request.Id}"));
    }
}