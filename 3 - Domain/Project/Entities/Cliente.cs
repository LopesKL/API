using System;

namespace API.Domain.Projeto {
    public class Cliente {
        public Guid IdCliente { get; set; }
        public string Nome { get; set; }
        public string Moeda { get; set; } //BRL, ARS ... 

        // controls
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
