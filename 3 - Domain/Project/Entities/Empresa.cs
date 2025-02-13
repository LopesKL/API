using System;

namespace API.Domain.Projeto {
    public class Empresa {
        public Guid IdEmpresa { get; set; }
        public string Nome { get; set; }

        // controls
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
