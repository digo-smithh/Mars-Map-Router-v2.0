namespace apCaminhosMarte.Data
{
    class DistanciaOriginal
    {
        public int Distancia { get; set; }
        public int VerticePai { get; set; }

        public DistanciaOriginal(int vp, int d)
        {
            Distancia = d;
            VerticePai = vp;
        }
    }
}
