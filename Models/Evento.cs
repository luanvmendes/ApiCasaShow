using System;

namespace CasaShowAPI.Models
{
    public class Evento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Capacidade { get; set; }
        public DateTime Data { get; set; }
        public float ValorIngresso { get; set; }
        public CasaDeShow CasaShow { get; set; }
        public Categoria Categoria { get; set; }
        public string Imagem { get; set; }
    }
}