using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {     
            try
            {
                var builder = new DbContextOptionsBuilder<TestDbContext>();
                builder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Integrated Security=true;Database=Test");
                using (var db = new TestDbContext(builder.Options))
                {
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    db.Things.Add(new Thing("Thing1", new User("User1")));
                    db.SaveChanges();
                }
                using (var db = new TestDbContext(builder.Options))
                {
                    //See https://docs.microsoft.com/en-us/ef/core/querying/tracking
                    //
                    //The following query should not track any entity objects:
                    var result = (from t in db.Things
                                  select new
                                  {
                                      t.Id,
                                      LastModifiedBy = t.LastModifiedBy.Username
                                  }).ToArray();
                }
            }
            catch(ConstructorCalledException ex)
            {
                //But, as you can see, it does try to instantiate the 'Thing' entity.
                Console.WriteLine("Error: Constructor was called during request.");
            }
            Console.ReadLine();
        }
    }

    public class ConstructorCalledException : Exception
    {
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<Thing> Things { get; set; }
        public DbSet<User> Users { get; set; }
    }

    public class Thing
    {
        public Thing()
        {
            throw new ConstructorCalledException();
        }
        public Thing(string name, User lastModifiedBy)
        {
            this.Name = name;
            this.LastModifiedBy = lastModifiedBy;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public User LastModifiedBy { get; set; }
    }

    public class User
    {
        public User()
        {
            throw new ConstructorCalledException();
        }
        public User(string username)
        {
            this.Username = username;
        }
        public int Id { get; set; }
        public string Username { get; set; }
    }
}