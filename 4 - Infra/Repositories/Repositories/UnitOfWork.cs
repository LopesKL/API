using API.Domain.Interfaces.Write;
using API.Domain.Notifications;
using API.Domain.Projeto;
using API.Domain.Users.Auth;
using API.Infra.Repositories.Interfaces;
using API.Infra.SqlServer.Interfaces;
using System;
using System.Threading.Tasks;

namespace API.Infra.Repositories.Repositories {
    public class UnitOfWork : IUnitOfWork, IDisposable {
        private readonly INotificationHandler _notificationHandler;
        private readonly IDbContext _context;

        public UnitOfWork(IDbContext context, INotificationHandler notificationHandler) {
            _notificationHandler = notificationHandler;
            _context = context;

            AppRoleRepository = new Repository<AppRole>(context);
            AppUserRepository = new Repository<AppUser>(context);
            AlteracoesRepository = new Repository<Alteracoes>(context);
            AtividadeFilhoRepository = new Repository<AtividadeFilho>(context);
            AtividadeRepository = new Repository<Atividade>(context);
            AtividadeUsuarioRepository = new Repository<AtividadeUsuario>(context);
            AtividadePaiRepository = new Repository<AtividadePai>(context);
            ClienteRepository = new Repository<Cliente>(context);
            ComentarioRepository = new Repository<Comentario>(context);
            EmpresaRepository = new Repository<Empresa>(context);
            HabilidadeRepository = new Repository<Habilidade>(context);
            LancamentoRepository = new Repository<Lancamento>(context);
            ProjetosRepository = new Repository<Projetos>(context);
            ProjetoUsuarioRepository = new Repository<ProjetoUsuario>(context);
            TagRepository = new Repository<Tag>(context);
            UsuarioHabilidadeRepository = new Repository<UsuarioHabilidade>(context);
        }

        public IRepository<AppRole> AppRoleRepository { get; }
        public IRepository<AppUser> AppUserRepository { get; }
        public IRepository<Alteracoes> AlteracoesRepository { get; }
        public IRepository<Atividade> AtividadeRepository { get; }
        public IRepository<AtividadeFilho> AtividadeFilhoRepository { get; }
        public IRepository<AtividadeUsuario> AtividadeUsuarioRepository { get; }
        public IRepository<AtividadePai> AtividadePaiRepository { get; }
        public IRepository<Comentario> ComentarioRepository { get; }
        public IRepository<Cliente> ClienteRepository { get; }
        public IRepository<Empresa> EmpresaRepository { get; }
        public IRepository<Habilidade> HabilidadeRepository { get; }
        public IRepository<Lancamento> LancamentoRepository { get; }
        public IRepository<Projetos> ProjetosRepository { get; }
        public IRepository<ProjetoUsuario> ProjetoUsuarioRepository { get; }
        public IRepository<Tag> TagRepository { get; }
        public IRepository<UsuarioHabilidade> UsuarioHabilidadeRepository { get; }

        public void Dispose() => _context.Dispose();

        public async Task Save() => await _context.SaveChangesAsync();
    }
}