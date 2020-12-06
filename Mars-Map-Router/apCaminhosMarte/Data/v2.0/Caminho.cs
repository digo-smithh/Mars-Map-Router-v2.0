namespace apCaminhosMarte.Data
{
    class Caminho
    {
        public int Distancia { get; set; }
        public int VerticePai { get; set; }

        public Caminho(int vp, int d)
        {
            Distancia = d;
            VerticePai = vp;
        }
    }
}
