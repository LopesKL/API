using System;

namespace API.Domain.Projeto {
    public class UsuarioHabilidade {
        public Guid IdUsuario { get; set; }
        public Guid IdHabilidade { get; set; }
        public Habilidade Habilidade { get; set; }

        // Controle de auditoria
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
