using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosMarte.Data
{
    class SolucionadorDijkstra
    {
        private const int NUM_VERTICES = 20;
        private Vertice[] vertices;
        private int[,] adjMatrix;
        private int numVerts;

        private DistanciaOriginal[] percurso;
        private const int INFINITY = int.MaxValue;
        private int verticeAtual;
        private int doInicioAteAtual;

        public SolucionadorDijkstra()
        {
            vertices = new Vertice[NUM_VERTICES];
            adjMatrix = new int[NUM_VERTICES, NUM_VERTICES];
            numVerts = 0;

            for (int j = 0; j < NUM_VERTICES; j++)
                for (int k = 0; k < NUM_VERTICES; k++)
                    adjMatrix[j, k] = INFINITY;

            percurso = new DistanciaOriginal[NUM_VERTICES];
        }

        public void NovoVertice()
        {
            vertices[numVerts] = new Vertice();
            numVerts++;
        }

        public void NovaAresta(int origem, int destino)
        {
            adjMatrix[origem, destino] = 1;
        }

        public void NovaAresta(int origem, int destino, int peso)
        {
            adjMatrix[origem, destino] = peso;
        }

        public void ObterMenorCaminho(int inicioDoPercurso, int finalDoPercurso)
        {
            for (int j = 0; j < numVerts; j++)
                vertices[j].FoiVisitado = false;

            vertices[inicioDoPercurso].FoiVisitado = true;
            for (int j = 0; j < numVerts; j++)
            {
                int tempDist = adjMatrix[inicioDoPercurso, j];
                percurso[j] = new DistanciaOriginal(inicioDoPercurso, tempDist);
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

        public int ObterMenor()
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

        public void AjustarMenorCaminho()
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
