using System;
using System.Collections.Generic;

namespace API.Domain.Projeto {
    public class AtividadeUsuario {
        public Guid IdUsuario { get; set; }
        public Guid IdAtividade { get; set; }
        public Atividade Atividade { get; set; }

        // Controle de auditoria
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
