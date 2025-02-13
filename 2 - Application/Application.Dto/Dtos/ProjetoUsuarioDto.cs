using API.Application.Dto.Request;
using Azure.Storage.Blobs.Models;
using System;

namespace API.Application.Dto
{
    public class ProjetoUsuarioDto
    {
        public Guid IdUsuario { get; set; }
        public Guid IdProjeto { get; set; }
        public ProjetosDto Projetos { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    // herda as props do requestAllDto
    public class RequestAllProjetoUsuarioDto : RequestAllDto
    {
        public Guid IdUsuario { get; set; }
        public Guid IdProjeto { get; set; }
        public ProjetosDto Projetos { get; set; }
    }
}
