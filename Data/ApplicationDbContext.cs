﻿using ContactProAltair.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContactProAltair.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser> //This is using C# generics<>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Contact> Contacts { get; set; } = default!;

        public virtual DbSet<Category> Categories { get; set; } = default!;
    }
}