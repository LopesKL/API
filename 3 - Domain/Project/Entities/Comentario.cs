using API.Application.Dto;
using System;
using System.Collections;

namespace API.Domain.Projeto
{
    public class Comentario
    {
        public Guid IdComentario { get; set; }
        public Guid IdUsuario { get; set; }
        public Guid? IdProjetos { get; set; }
        public Guid? IdAtividadePai {  get; set; }
        public Guid? IdAtividadeFilho { get; set; }
        public Guid? IdAtividade { get; set; }
        public Guid? IdLancamento { get; set; }
        public string Texto { get; set; }
        public string Files { get; set; }

        public Projetos Projetos { get; set; }
        public AtividadeFilho AtividadeFilho { get; set; }
        public AtividadePai AtividadePai { get; set; }
        public Atividade Atividade { get; set; }

        // controls
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
