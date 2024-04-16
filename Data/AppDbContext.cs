using GrpcDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base()
        {
        }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        DbSet<ToDoItem> ToDoItems { get; set; }
    }
}
