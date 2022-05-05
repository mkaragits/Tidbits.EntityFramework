using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8618

namespace Tidbits.EntityFramework.Tests
{
    public class TestDbContext : CrudDbContext<TestDbContext>
    {
        public TestDbContext() : base(new DbContextOptions<TestDbContext>()) { }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {

        }

        public virtual DbSet<User> Users { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Property1 { get; set; }
        public int Property2 { get; set; }
        public int Property3 { get; set; }
        public int Property4 { get; set; }

        public User Clone()
        {
            return (User)MemberwiseClone();
        }
    }

    public class ProjectedUser
    {
        public int UserId { get; set; }

        public static Expression<Func<User, ProjectedUser>> FromUser => u => new ProjectedUser() { UserId = u.Id };
    }

    internal static class C
    {
        public static List<User> Users = new List<User>()
        {
            new User() {Id = 1, Name = "John", Property1 = 1, Property2 = 1, Property3 = 1, Property4 = 1},
            new User() {Id = 2, Name = "Jane", Property1 = 1, Property2 = 1, Property3 = 1, Property4 = 1},
            new User() {Id = 3, Name = "Tim", Property1 = 1, Property2 = 1, Property3 = 1, Property4 = 1},
            new User() {Id = 4, Name = "Tom", Property1 = 1, Property2 = 1, Property3 = 1, Property4 = 1},
            new User() {Id = 5, Name = "Sheila", Property1 = 1, Property2 = 1, Property3 = 1, Property4 = 1},
        };

        public static bool ExistsLocally<TContext, TEntity>(this TContext context, TEntity entity)
            where TContext : DbContext
            where TEntity : class
        {
            return context.Set<TEntity>().Local.Any(e => e == entity);
        }
    }
}
