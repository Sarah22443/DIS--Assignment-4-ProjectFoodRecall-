using Microsoft.EntityFrameworkCore;
using ProjectFoodRecall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectFoodRecall.Data_Access
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        //public DbSet<Recall_Items> Recall_Items_data { get; set; }



        public DbSet<Recall_Item> Recall_Items_data { get; set; }
        public DbSet<Recall_Items> Recall_Items { get; set; }
    }
}
