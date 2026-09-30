using LucroImpresso.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LucroImpresso.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Maquina> Maquinas { get; set; }
        public DbSet<Material> Materiais { get; set; }
        public DbSet<Peca> Pecas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<PessoaFisica> PessoasFisicas { get; set; }
        public DbSet<PessoaJuridica> PessoasJuridicas { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Orcamento> Orcamentos { get; set; }
        public DbSet<HistoricoFinanceiro> HistoricoFinanceiro { get; set; }
        public DbSet<Acabamento> Acabamentos { get; set; }
        public DbSet<OrcamentoAcabamento> OrcamentoAcabamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Usuario: Índices Únicos para integridade ACID no MariaDB ──
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NomeUsuario)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ── Cliente ──────────────────────────────────────────────────
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.PessoaFisica)
                .WithOne(pf => pf.Cliente)
                .HasForeignKey<PessoaFisica>(pf => pf.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.PessoaJuridica)
                .WithOne(pj => pj.Cliente)
                .HasForeignKey<PessoaJuridica>(pj => pj.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Orcamento ─────────────────────────────────────────────────
            // Máquina: SET NULL ao deletar (orçamentos ficam sem máquina)
            modelBuilder.Entity<Orcamento>()
                .HasOne(o => o.Maquina)
                .WithMany()
                .HasForeignKey(o => o.MaquinaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Material: RESTRICT (não pode deletar material em uso)
            modelBuilder.Entity<Orcamento>()
                .HasOne(o => o.Material)
                .WithMany()
                .HasForeignKey(o => o.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            // Peca: RESTRICT (peça é obrigatória no orçamento)
            modelBuilder.Entity<Orcamento>()
                .HasOne(o => o.Peca)
                .WithMany(p => p.Orcamentos)
                .HasForeignKey(o => o.PecaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cliente: RESTRICT (não pode deletar cliente com orçamentos)
            modelBuilder.Entity<Orcamento>()
                .HasOne(o => o.Cliente)
                .WithMany(c => c.Orcamentos)
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario → Orcamento: RESTRICT
            modelBuilder.Entity<Orcamento>()
                .HasOne(o => o.Usuario)
                .WithMany()
                .HasForeignKey(o => o.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario → Maquina: RESTRICT
            modelBuilder.Entity<Maquina>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario → Material: RESTRICT
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario → Peca: RESTRICT
            modelBuilder.Entity<Peca>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario → Cliente: RESTRICT
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Multi-Tenant: UsuarioId propagado para PF e PJ ────────────
            modelBuilder.Entity<PessoaFisica>()
                .HasOne(pf => pf.Usuario)
                .WithMany()
                .HasForeignKey(pf => pf.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PessoaJuridica>()
                .HasOne(pj => pj.Usuario)
                .WithMany()
                .HasForeignKey(pj => pj.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Endereco: UsuarioId multi-tenant ─────────────────────────
            modelBuilder.Entity<Endereco>()
                .HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Endereco → Cliente (1:1) ──────────────────────────────────
            modelBuilder.Entity<Endereco>()
                .HasOne(e => e.Cliente)
                .WithOne(c => c.Endereco)
                .HasForeignKey<Endereco>(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Usuario → HistoricoFinanceiro: RESTRICT ───────────────────
            modelBuilder.Entity<HistoricoFinanceiro>()
                .HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── OrcamentoAcabamento (N:N) ──────────────────────────────────────
            modelBuilder.Entity<OrcamentoAcabamento>()
                .HasKey(oa => new { oa.OrcamentoId, oa.AcabamentoId });

            modelBuilder.Entity<OrcamentoAcabamento>()
                .HasOne(oa => oa.Orcamento)
                .WithMany(o => o.OrcamentoAcabamentos)
                .HasForeignKey(oa => oa.OrcamentoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrcamentoAcabamento>()
                .HasOne(oa => oa.Acabamento)
                .WithMany()
                .HasForeignKey(oa => oa.AcabamentoId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Acabamento>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrcamentoAcabamento>()
                .HasOne(oa => oa.Usuario)
                .WithMany()
                .HasForeignKey(oa => oa.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}