using System;
using System.Collections.Generic;

namespace API.Domain.Projeto {
    public class Habilidade {
        public Guid IdHabilidade { get; set; }
        public string Nome { get; set; }   // Exemplo: Figma, Support

        public ICollection<UsuarioHabilidade> UsuarioHabilidades { get; set; }
        // controls
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
