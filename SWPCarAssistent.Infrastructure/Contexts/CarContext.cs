using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using SWPCarAssistent.Core.Common.Entities;


namespace SWPCarAssistent.Infrastructure.Context
{
    public class CarContext : DbContext
    {
        public CarContext()
            : base("name=CarContext")
        {
        }

        public virtual DbSet<Contacts> Contacts { get; set; }
        public virtual DbSet<Radio> Radio { get; set; }
        public virtual DbSet<StartupParams> StartupParams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contacts>()
                .Property(e => e.FullName)
                .IsUnicode(false);

            modelBuilder.Entity<Contacts>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Radio>()
                .Property(e => e.Frequency)
                .IsFixedLength();
        }
    }
}