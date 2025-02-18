using API.Application.Dto.Request;
using System;
using System.Collections.Generic;

namespace API.Application.Dto {
    public class AtividadeFilhoDto {
        public Guid IdAtividadeFilho { get; set; }
        public Guid IdAtividadePai { get; set; }
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int HorasEstimadas { get; set; }
        public string? Descricao { get; set; }
        public string? Files { get; set; }

        public AtividadePaiDto AtividadePai { get; set; }

        public List<ComentarioDto> Comentarios { get; set; }
        public List<AtividadeUsuarioDto> AtividadesUsuario { get; set; }
        public List<LancamentoDto> Lancamentos { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
        public class RequestAllAtividadeFilhoDto : RequestAllDto {
            public Guid IdAtividadeFilho { get; set; }
            public Guid IdAtividadePai { get; set; }
            public string Nome { get; set; }
            public DateTime DataInicio { get; set; }
            public DateTime DataFim { get; set; }
            public int Progresso { get; set; }
            public int HorasEstimadas { get; set; }
            public int? HorasCobradas { get; set; }
            public int? HorasNaoCobradas { get; set; }
            public string? Descricao { get; set; }
            public string? Files { get; set; }

        public AtividadePaiDto AtividadePai { get; set; }

            public List<Guid> ComentarioIds { get; set; }
            public List<Guid> AtividadeFilhoUsuarioIds { get; set; }
            public List<Guid> LancamentoIds { get; set; }
        }
    

}
