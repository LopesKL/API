using API.Domain.Projeto;
using API.Domain.Users.Auth;
using API.Infra.Repositories.Interfaces;
using System.Threading.Tasks;

namespace API.Domain.Interfaces.Write {
    public interface IUnitOfWork {
        IRepository<AppUser> AppUserRepository { get; }
        IRepository<AppRole> AppRoleRepository { get; }

        IRepository<Alteracoes> AlteracoesRepository { get; }
        IRepository<AtividadePai> AtividadePaiRepository { get; }
        IRepository<AtividadeFilho> AtividadeFilhoRepository { get; }
        IRepository<Atividade> AtividadeRepository { get; } 
        IRepository<AtividadeUsuario> AtividadeUsuarioRepository { get; }
        IRepository<Comentario> ComentarioRepository { get; }
        IRepository<Empresa> EmpresaRepository { get; }
        IRepository<Cliente> ClienteRepository { get; }
        IRepository<Habilidade> HabilidadeRepository { get; }
        IRepository<Lancamento> LancamentoRepository { get; }
        IRepository<Projetos> ProjetosRepository { get; }
        IRepository<ProjetoUsuario> ProjetoUsuarioRepository { get; }
        IRepository<Tag> TagRepository { get; }
        IRepository<UsuarioHabilidade> UsuarioHabilidadeRepository { get; }

        Task Save();
    }
}
