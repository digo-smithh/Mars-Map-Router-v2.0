namespace apCaminhosMarte.Data
{
    class Vertice
    {
        public bool FoiVisitado { get; set; }
        public bool EstaAtivo { get; set; }

        public Vertice()
        {
            FoiVisitado = false;
            EstaAtivo = true;
        }
    }
}
