using System;
using System.Text.Json;

namespace API.Domain.Projeto {
    public class Alteracoes {
        public Guid IdAlteracao { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime DataEHora { get; set; }
        public string NomeTabela { get; set; }
        public string TipoOperacao { get; set; }
        public string ValoresAntigos { get; set; }
        public string ValoresNovos { get; set; }

        // controls
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        // Métodos para converter objetos para JSON
        public void SetValoresAntigos(object valores) {
            ValoresAntigos = JsonSerializer.Serialize(valores);
        }

        public void SetValoresNovos(object valores) {
            ValoresNovos = JsonSerializer.Serialize(valores);
        }

        // Método para desserializar os valores
        public T GetValoresAntigos<T>() {
            return JsonSerializer.Deserialize<T>(ValoresAntigos);
        }

        public T GetValoresNovos<T>() {
            return JsonSerializer.Deserialize<T>(ValoresNovos);
        }
    }
}
