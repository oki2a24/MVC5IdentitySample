using System.Data.Entity;

namespace MVC5IdentitySample.Models
{
    public class MVC5IdentitySampleContext : DbContext
    {
        public MVC5IdentitySampleContext() : base("name=MVC5IdentitySampleContext")
        {
        }

        public static MVC5IdentitySampleContext Create()
        {
            return new MVC5IdentitySampleContext();
        }

        public DbSet<User> Users { get; set; }
    }
}
