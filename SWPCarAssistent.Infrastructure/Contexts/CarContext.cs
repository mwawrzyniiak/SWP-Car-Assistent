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

        public virtual DbSet<EntityContacts> Contacts { get; set; }
        public virtual DbSet<EntityRadio> Radio { get; set; }
        public virtual DbSet<EntityStartupParams> StartupParams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntityContacts>()
                .Property(e => e.FullName)
                .IsUnicode(false);

            modelBuilder.Entity<EntityContacts>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<EntityRadio>()
                .Property(e => e.Frequency)
                .IsFixedLength();
        }
    }
}