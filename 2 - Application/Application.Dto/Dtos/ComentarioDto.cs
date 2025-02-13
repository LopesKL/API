using API.Application.Dto.Request;
using Azure.Storage.Blobs.Models;
using System;

namespace API.Application.Dto
{
    public class ComentarioDto
    {
        public Guid IdComentario { get; set; }
        public Guid IdUsuario { get; set; }
        public Guid? IdProjetos { get; set; }
        public Guid? IdAtividadePai { get; set; }
        public Guid? IdAtividadeFilho { get; set; }
        public Guid? IdAtividade { get; set; }
        public Guid? IdLancamento { get; set; }
        public string Texto { get; set; }
        public string Files { get; set; }

        public ProjetosDto Projetos { get; set; }
        public AtividadeFilhoDto AtividadeFilho { get; set; }
        public AtividadePaiDto AtividadePai { get; set; }
        public AtividadeDto Atividade { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    // herda as props do requestAllDto
    public class RequestAllComentarioDto : RequestAllDto
    {
        public Guid IdComentario { get; set; }
        public Guid IdUsuario { get; set; }
        public Guid? IdProjetos { get; set; }
        public Guid? IdAtividadePai { get; set; }
        public Guid? IdAtividadeFilho { get; set; }
        public Guid? IdAtividade { get; set; }
        public Guid? IdLancamento { get; set; }
        public string Texto { get; set; }
        public string Files { get; set; }

        public ProjetosDto Projetos { get; set; }
        public AtividadeFilhoDto AtividadeFilho { get; set; }
        public AtividadePaiDto AtividadePai { get; set; }
        public AtividadeDto Atividade { get; set; }

    }
}
