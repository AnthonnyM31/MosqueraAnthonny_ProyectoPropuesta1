using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MosqueraAnthonny_ProyectoPropuesta1.Models;

    public class DiarioContext : DbContext
    {
        public DiarioContext (DbContextOptions<DiarioContext> options)
            : base(options)
        {
        }

        public DbSet<MosqueraAnthonny_ProyectoPropuesta1.Models.Diario> Diario { get; set; } = default!;
    }
