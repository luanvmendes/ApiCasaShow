using System;

namespace CasaShowAPI.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public Usuario User { get; set; }
        public DateTime Data { get; set; }
        public Evento Evento { get; set; }
        public int Quantidade { get; set; }
        public float Total { get; set; }
        
    }
}