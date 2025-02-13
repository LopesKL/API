using System;

namespace API.Domain.Projeto
{
    public class ProjetoUsuario
    {
        public Guid IdUsuario { get; set; }
        public Guid IdProjeto { get; set; }
        public Projetos Projetos { get; set; }

        // controls
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
