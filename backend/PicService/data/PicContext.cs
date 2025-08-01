using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PicService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

    public class PicContext : DbContext
    {
        public PicContext(DbContextOptions<PicContext> options) : base(options)
        {
                
        }

        public DbSet<PicModel> PicModel { get; set; } = default!;
    }
