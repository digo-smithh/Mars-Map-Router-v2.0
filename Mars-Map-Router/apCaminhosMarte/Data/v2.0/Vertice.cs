namespace apCaminhosMarte.Data
{
    class Vertice
    {
        public bool FoiVisitado { get; set; }
        public bool EstaAtivo { get; set; }
        public Cidade Cidade { get; set; }

        public Vertice(Cidade cidade)
        {
            Cidade = cidade;
            FoiVisitado = false;
            EstaAtivo = true;
        }
    }
}
