using API.Application.Dto.Request;
using Azure.Storage.Blobs.Models;
using System;

namespace API.Application.Dto
{
    public class AtividadeUsuarioDto {
        public Guid IdUsuario { get; set; }
        public Guid IdAtividade { get; set; }
        public AtividadeDto Atividade { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    // herda as props do requestAllDto
    public class RequestAllAtividadeUsuarioDto : RequestAllDto
    {
        public Guid IdUsuario { get; set; }
        public Guid IdAtividade { get; set; }
        public AtividadeDto Atividade { get; set; }

    }
}
