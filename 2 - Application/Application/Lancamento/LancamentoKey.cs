using System;

public class LancamentoKey
{
    public Guid IdProjeto { get; set; }
    public Guid IdAtividadePai { get; set; }
    public Guid IdAtividadeFilho { get; set; }
    public Guid IdAtividade { get; set; } // Novo campo
    public string Descricao { get; set; }
    public DateTime Data { get; set; }

    public override bool Equals(object obj) =>
        obj is LancamentoKey other &&
        IdProjeto == other.IdProjeto &&
        IdAtividadePai == other.IdAtividadePai &&
        IdAtividadeFilho == other.IdAtividadeFilho &&
        IdAtividade == other.IdAtividade &&
        Descricao == other.Descricao &&
        Data == other.Data;

    public override int GetHashCode() =>
        HashCode.Combine(IdProjeto, IdAtividadePai, IdAtividadeFilho, IdAtividade, Descricao, Data);
}
