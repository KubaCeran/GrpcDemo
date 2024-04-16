using Grpc.Core;
using GrpcDemo.Data;
using GrpcDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcDemo.Services
{
    public class ToDoService(DataContext dbContext) : ToDoIt.ToDoItBase
    {
        public override async Task<CreateToDoResponse> CreateToDoItem(CreateToDoRequest request, ServerCallContext context)
        {
            if (String.IsNullOrEmpty(request.Title) || String.IsNullOrEmpty(request.Description))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid input data"));

            var ToDoItem = new ToDoItem
            {
                Title = request.Title,
                Description = request.Description
            };

            await dbContext.ToDoItems.AddAsync(ToDoItem);
            await dbContext.SaveChangesAsync();
            return new CreateToDoResponse
            {
                Id = ToDoItem.Id,
                Description = ToDoItem.Description,
                Title = ToDoItem.Title,
                Status = ToDoItem.ToDoStatus
            };
        }

        public override async Task<GetToDoResponse> GetToDoItemById(GetToDoRequest request, ServerCallContext context)
        {
            var item = await dbContext.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id) ??
                throw new RpcException(new Status(StatusCode.NotFound, $"Item with Id: {request.Id} not found in DB"));

            return await Task.FromResult(new GetToDoResponse
            {
                Id = item.Id,
                Description = item.Description,
                Title = item.Title,
                Status = item.ToDoStatus
            });
        }

        public override async Task GetAllToDoItems(GetAllToDoRequest request, IServerStreamWriter<GetToDoResponse> responseStream, ServerCallContext context)
        {
            var items = await dbContext.ToDoItems.ToListAsync();
            foreach(var item in items)
            {
                await responseStream.WriteAsync(new GetToDoResponse
                {
                    Id = item.Id,
                    Description = item.Description,
                    Status = item.ToDoStatus,
                    Title = item.Title
                });
            }
        }

        public override async Task<UpdateToDoResponse> UpdateToDoItem(UpdateToDoRequest request, ServerCallContext context)
        {
            var item = dbContext.ToDoItems.FirstOrDefault(x => x.Id == request.Id) ??
                throw new RpcException(new Status(StatusCode.NotFound, $"Item with Id: {request.Id} not found in DB"));

            if (String.IsNullOrEmpty(request.Title) || String.IsNullOrEmpty(request.Description) || String.IsNullOrEmpty(request.Status))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid input data"));

            item.Title = request.Title;
            item.Description = request.Description;
            item.ToDoStatus = request.Status;

            await dbContext.SaveChangesAsync();

            return await Task.FromResult(new UpdateToDoResponse
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Status = item.ToDoStatus
            });
        }

        public override async Task<DeleteToDoResponse> DeleteToDoItem(DeleteToDoRequest request, ServerCallContext context)
        {
            var item = dbContext.ToDoItems.FirstOrDefault(x => x.Id == request.Id) ??
                throw new RpcException(new Status(StatusCode.NotFound, $"Item with Id: {request.Id} not found in DB"));

            dbContext.ToDoItems.Remove(item);
            await dbContext.SaveChangesAsync();

            return await Task.FromResult(new DeleteToDoResponse
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Status = item.ToDoStatus
            });
        }
    }
}
