using API.Application.Dto.Request;
using System;
using System.Collections.Generic;

namespace API.Application.Dto {
    public class AtividadePaiDto {
        public Guid IdAtividadePai { get; set; }
        public Guid IdProjeto { get; set; }
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int HorasEstimadas { get; set; }
        public int? HorasCobradas { get; set; }
        public int? HorasNaoCobradas { get; set; }
        public string? Descricao { get; set; }
        public string? Files { get; set; }

        public ProjetosDto Projetos { get; set; }

        public ICollection<AtividadeFilhoDto> AtividadeFilho { get; set; }
        public ICollection<ComentarioDto> Comentario { get; set; }
        public ICollection<LancamentoDto> Lancamento { get; set; }


        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    // herda as props do requestAllDto
    public class RequestAllAtividadePaiDto : RequestAllDto {
        public Guid IdAtividadePai { get; set; }
        public Guid IdProjeto { get; set; }
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int HorasEstimadas { get; set; }
        public int? HorasCobradas { get; set; }
        public int? HorasNaoCobradas { get; set; }
        public string? Descricao { get; set; }
        public string? Files { get; set; }
        public ProjetosDto Projetos { get; set; }

    }
}
