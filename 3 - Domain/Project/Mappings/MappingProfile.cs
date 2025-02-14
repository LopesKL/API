using API.Application.Dto;
using API.Domain.Projeto;
using AutoMapper;

namespace API.Domain.Mapper
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<Alteracoes, AlteracoesDto>();
            CreateMap<AlteracoesDto, Alteracoes>();            
            CreateMap<Atividade, AtividadeDto>();
            CreateMap<AtividadeDto, Atividade>();
            CreateMap<AtividadeFilho, AtividadeFilhoDto>();
            CreateMap<AtividadeFilhoDto, AtividadeFilho>();
            CreateMap<AtividadeUsuario, AtividadeUsuarioDto>();
            CreateMap<AtividadeUsuarioDto, AtividadeUsuario>();
            CreateMap<AtividadePai, AtividadePaiDto>();
            CreateMap<AtividadePaiDto, AtividadePai>();
            CreateMap<Comentario, ComentarioDto>();
            CreateMap<ComentarioDto, Comentario>();
            CreateMap<Cliente, ClienteDto>();
            CreateMap<ClienteDto, Cliente>();            
            CreateMap<Empresa, EmpresaDto>();
            CreateMap<EmpresaDto, Empresa>();
            CreateMap<Habilidade, HabilidadeDto>();
            CreateMap<HabilidadeDto, Habilidade>();
            CreateMap<Lancamento, LancamentoDto>();
            CreateMap<LancamentoDto, Lancamento>();
            CreateMap<Projetos, ProjetosDto>();
            CreateMap<ProjetosDto, Projetos>();
            CreateMap<ProjetoUsuario, ProjetoUsuarioDto>();
            CreateMap<ProjetoUsuarioDto, ProjetoUsuario>();
            CreateMap<Tag, TagDto>();
            CreateMap<TagDto, Tag>();
            CreateMap<UsuarioHabilidade, UsuarioHabilidadeDto>();
            CreateMap<UsuarioHabilidadeDto, UsuarioHabilidade>();
        }
    }
}
