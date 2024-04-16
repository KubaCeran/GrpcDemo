using GrpcDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcDemo.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<ToDoItem> ToDoItems { get; set; }
    }
}
