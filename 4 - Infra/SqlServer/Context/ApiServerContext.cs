using API.Domain.Projeto;
using API.Domain.Users.Auth;
using API.Infra.SqlServer.Interfaces;
using API.Infra.SqlServer.ModelBuilders;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Infra.SqlServer.Context
{
    // Serviço que executa tarefas programadas (timer) para procedimentos armazenados
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service iniciado.");
            _timer = new Timer(ExecuteStoredProcedure, null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
            return Task.CompletedTask;
        }

        private void ExecuteStoredProcedure(object state)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApiServerContext>();
            context.ExecuteStoredProcedure();
            _logger.LogInformation("Stored Procedure executada.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service parando.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

    // Contexto do servidor de API
    public class ApiServerContext : IdentityDbContext<AppUser>, IDbContext, IDisposable
    {
        public ApiServerContext(DbContextOptions<ApiServerContext> options) : base(options)
        {
        }

        // Método para executar stored procedure
        public void ExecuteStoredProcedure()
        {
            // Implementação do SQL Raw para Stored Procedure
            // this.Database.ExecuteSqlRaw("EXEC NomeDaProcedure");
        }

        // Configuração de entidades no OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplicar configurações específicas de entidade
            ApplyEntityConfigurations(modelBuilder);

            // Configuração global para evitar delete cascade
            SetGlobalDeleteBehavior(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        // Método para configurar o comportamento global de deleção
        private void SetGlobalDeleteBehavior(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        // Método para aplicar as configurações de entidade
        private void ApplyEntityConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AppRoleConfiguration());
            modelBuilder.ApplyConfiguration(new AppUserConfiguration());

            // Entidades

            modelBuilder.Entity<Alteracoes>().HasKey(x => x.IdAlteracao);

            modelBuilder.Entity<Atividade>() .HasKey(x => x.IdAtividade);

            modelBuilder.Entity<Atividade>().HasOne(a => a.AtividadeFilho).WithMany(af => af.Atividades) .HasForeignKey(a => a.IdAtividadeFilho).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AtividadeFilho>().HasKey(x => x.IdAtividadeFilho);
            modelBuilder.Entity<AtividadeFilho>().HasOne(af => af.AtividadePai) .WithMany(ap => ap.AtividadeFilho) .HasForeignKey(af => af.IdAtividadePai).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AtividadeUsuario>().HasKey(afu => new { afu.IdUsuario, afu.IdAtividade });
            modelBuilder.Entity<AtividadeUsuario>().HasOne(uh => uh.Atividade).WithMany(h => h.AtividadeUsuario).HasForeignKey(uh => uh.IdAtividade).OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<AtividadePai>().HasOne(ap => ap.Projetos).WithMany(p => p.AtividadePai) .HasForeignKey(ap => ap.IdProjeto) .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cliente>().HasKey(x => x.IdCliente);

            modelBuilder.Entity<Comentario>().HasKey(x => x.IdComentario);
            modelBuilder.Entity<Comentario>().HasOne(c => c.Projetos).WithMany(p => p.Comentario).HasForeignKey(c => c.IdProjetos).OnDelete(DeleteBehavior.Cascade); 
            modelBuilder.Entity<Comentario>().HasOne(c => c.AtividadePai).WithMany(ap => ap.Comentario).HasForeignKey(c => c.IdAtividadePai).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Comentario>().HasOne(c => c.AtividadeFilho).WithMany(af => af.Comentario).HasForeignKey(c => c.IdAtividadeFilho).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Comentario>().HasOne(c => c.Atividade).WithMany(af => af.Comentario).HasForeignKey(c => c.IdAtividade).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Empresa>().HasKey(x => x.IdEmpresa);

            modelBuilder.Entity<Habilidade>().HasKey(x => x.IdHabilidade);

            modelBuilder.Entity<UsuarioHabilidade>().HasKey(uh => new { uh.IdUsuario, uh.IdHabilidade });
            modelBuilder.Entity<UsuarioHabilidade>().HasOne(uh => uh.Habilidade).WithMany(h => h.UsuarioHabilidades).HasForeignKey(uh => uh.IdHabilidade) .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lancamento>().HasKey(l => l.IdLancamento);
            modelBuilder.Entity<Lancamento>().HasOne(l => l.Projetos).WithMany(p => p.Lancamento).HasForeignKey(l => l.IdProjeto).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Lancamento>().HasOne(l => l.AtividadePai).WithMany(ap => ap.Lancamento).HasForeignKey(l => l.IdAtividadePai).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Lancamento>().HasOne(l => l.AtividadeFilho).WithMany(af => af.Lancamento).HasForeignKey(l => l.IdAtividadeFilho).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Lancamento>().HasOne(l => l.Tag).WithMany(t => t.Lancamento).HasForeignKey(l => l.idTag).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Projetos>().HasKey(p => p.IdProjetos);
            modelBuilder.Entity<Projetos>().HasOne(p => p.Empresa).WithMany().HasForeignKey(p => p.IdEmpresa).OnDelete(DeleteBehavior.Restrict); 
            modelBuilder.Entity<Projetos>().HasOne(p => p.Cliente).WithMany() .HasForeignKey(p => p.IdCliente).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Projetos>().HasMany(p => p.AtividadePai).WithOne(ap => ap.Projetos).HasForeignKey(ap => ap.IdProjeto).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Projetos>().HasMany(p => p.Comentario).WithOne(c => c.Projetos).HasForeignKey(c => c.IdProjetos).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Projetos>().HasMany(p => p.Lancamento) .WithOne(l => l.Projetos) .HasForeignKey(l => l.IdProjeto).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjetoUsuario>().HasKey(pu => new { pu.IdUsuario, pu.IdProjeto }); 
            modelBuilder.Entity<ProjetoUsuario>().HasOne(pu => pu.Projetos) .WithMany(p => p.ProjetoUsuario).HasForeignKey(pu => pu.IdProjeto).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tag>().HasKey(t => t.IdTag); 
            modelBuilder.Entity<Tag>().HasMany(t => t.Lancamento) .WithOne(l => l.Tag) .HasForeignKey(l => l.idTag).OnDelete(DeleteBehavior.SetNull);

        }

        public async Task<int> SaveChangesAsync() => await base.SaveChangesAsync();
    }
}
