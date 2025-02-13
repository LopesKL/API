using System;
using System.Collections.Generic;

namespace API.Domain.Projeto {
    public class AtividadePai {
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

        public Projetos Projetos { get; set; }

        public ICollection<AtividadeFilho> AtividadeFilho { get; set; }
        public ICollection<Comentario> Comentario { get; set; }
        public ICollection<Lancamento> Lancamento { get; set; }


        // controls
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
