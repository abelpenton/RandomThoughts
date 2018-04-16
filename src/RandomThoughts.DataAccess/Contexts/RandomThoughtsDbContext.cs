﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RandomThoughts.Domain;

namespace RandomThoughts.DataAccess.Contexts
{
    public class RandomThoughtsDbContext : IdentityDbContext<ApplicationUser>
    {
        public RandomThoughtsDbContext(DbContextOptions<RandomThoughtsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    
    }
}