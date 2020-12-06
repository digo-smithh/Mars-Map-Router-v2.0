namespace apCaminhosMarte.Data
{
    class SolucionadorDijkstra
    {
        static private Vertice[] vertices;
        static private int[,] adjMatrix;
        static private int numVerts;

        static public Caminho[] percurso;
        static private readonly int INFINITY = int.MaxValue;
        static private int verticeAtual;
        static private int doInicioAteAtual;

        static public void Build(int length)
        {
            percurso = new Caminho[length];
            adjMatrix = new int[length, length];
            vertices = new Vertice[length];
            numVerts = 0;

            for (int j = 0; j < length; j++)
                for (int k = 0; k < length; k++)
                    adjMatrix[j, k] = INFINITY;
        }

        static public void NovoVertice()
        {
            vertices[numVerts] = new Vertice();
            numVerts++;
        }

        static public void NovaAresta(int origem, int destino, int peso)
        {
            adjMatrix[origem, destino] = peso;
        }

        static public void ObterMenorCaminho(int inicioDoPercurso, int finalDoPercurso)
        {
            for (int j = 0; j < numVerts; j++)
                vertices[j].FoiVisitado = false;

            vertices[inicioDoPercurso].FoiVisitado = true;
            for (int j = 0; j < numVerts; j++)
            {
                int tempDist = adjMatrix[inicioDoPercurso, j];
                percurso[j] = new Caminho(inicioDoPercurso, tempDist);
            }

            for (int nTree = 0; nTree < numVerts; nTree++)
            {
                int indiceDoMenor = ObterMenor();
                int distanciaMinima = percurso[indiceDoMenor].Distancia;
                verticeAtual = indiceDoMenor;
                doInicioAteAtual = percurso[indiceDoMenor].Distancia;
                vertices[verticeAtual].FoiVisitado = true;
                AjustarMenorCaminho();
            }
        }

        static private int ObterMenor()
        {

            int distanciaMinima = INFINITY;
            int indiceDaMinima = 0;
            for (int j = 0; j < numVerts; j++)
            {
                if (!(vertices[j].FoiVisitado) && (percurso[j].Distancia < distanciaMinima))
                {
                    distanciaMinima = percurso[j].Distancia;
                    indiceDaMinima = j;
                }
            }
            return indiceDaMinima;
        }

        static private void AjustarMenorCaminho()
        {
            for (int coluna = 0; coluna < numVerts; coluna++)
            {
                if (!vertices[coluna].FoiVisitado)
                {
                    int atualAteMargem = adjMatrix[verticeAtual, coluna];
                    int doInicioAteMargem = doInicioAteAtual + atualAteMargem;

                    int distanciaDoCaminho = percurso[coluna].Distancia;
                    if (doInicioAteMargem < distanciaDoCaminho)
                    {
                        percurso[coluna].VerticePai = verticeAtual;
                        percurso[coluna].Distancia = doInicioAteMargem;
                    }
                }
            }
        }
    }
}
