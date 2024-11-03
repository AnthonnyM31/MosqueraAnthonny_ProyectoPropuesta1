using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MosqueraAnthonny_ProyectoPropuesta1.Models;

    public class UsuarioContext : DbContext
    {
        public UsuarioContext (DbContextOptions<UsuarioContext> options)
            : base(options)
        {
        }

        public DbSet<MosqueraAnthonny_ProyectoPropuesta1.Models.Usuario> Usuario { get; set; } = default!;
    }
