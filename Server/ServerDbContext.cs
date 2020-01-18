using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Entity;

namespace DataBase
{
    public class ServerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public void SaveUser(User user)
        {
            if (user.Id == 0)
                this.Users.Add(user);
            else
            {
                User dbEntry = this.Users.Find(user.Id);
                if (dbEntry != null)
                {
                    dbEntry.Change(user.Name, user.Password, user.Salt, user.Email);
                }
            }
            this.SaveChanges();
        }

        public ServerDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Server;Trusted_Connection=True;");
        }
    }
}
