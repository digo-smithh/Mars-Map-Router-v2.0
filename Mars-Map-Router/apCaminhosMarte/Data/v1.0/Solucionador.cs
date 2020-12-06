//Eduardo Migueis - 19167 e Rodrigo Smith - 19197

using System;
using System.Collections.Generic;
using System.Linq;

namespace apCaminhosMarte.Data
{
    static class Solucionador
    {
        static public bool BuscarCaminhosR(ref Stack<AvancoCaminho> caminhoEncontrado, ref List<AvancoCaminho[]> resultados, ArvoreBinaria<Cidade> arvore, Cidade origem, Cidade destino, ref AvancoCaminho[,] matrizCaminhos)
        {
            caminhoEncontrado = new Stack<AvancoCaminho>();
            resultados = new List<AvancoCaminho[]>();
            var passou = new bool[arvore.Qtd];

            BuscarCaminhosRecursivo(origem, ref destino, ref matrizCaminhos, ref caminhoEncontrado, ref resultados, ref passou);

            if (resultados.Count <= 0)
                return false;

            return true;
        }

        static public bool BuscarCaminhosP(ref Stack<AvancoCaminho> caminhoEncontrado, ref List<AvancoCaminho[]> resultados, ArvoreBinaria<Cidade> arvore, Cidade origem, Cidade destino, ref AvancoCaminho[,] matrizCaminhos)
        {
            caminhoEncontrado = new Stack<AvancoCaminho>();
            resultados = new List<AvancoCaminho[]>();
            Cidade atual = origem;

            BuscarCaminhosPilhas(ref atual, ref destino, ref matrizCaminhos, ref caminhoEncontrado, ref resultados);
        
            if (resultados.Count <= 0)
                return false;

            return true;
        }

        static private void BuscarCaminhosRecursivo(Cidade atual, ref Cidade destino, ref AvancoCaminho[,] matrizCaminhos, ref Stack<AvancoCaminho> caminhoEncontrado, ref List<AvancoCaminho[]> resultados, ref bool[] passou)
        {
            for (int j = 0; j < matrizCaminhos.GetLength(1); j++)
            {
                AvancoCaminho ac = matrizCaminhos[atual.Id, j];

                if (ac != null && !passou[j])
                {
                    passou[atual.Id] = true;
                    caminhoEncontrado.Push(ac);

                    if (j == destino.Id)
                    {
                        var caminho = new AvancoCaminho[caminhoEncontrado.Count];
                        caminhoEncontrado.CopyTo(caminho, 0);

                        resultados.Add(caminho);
                        caminhoEncontrado.Pop();
                        passou[atual.Id] = false;
                    }
                    else
                    {
                        BuscarCaminhosRecursivo(ac.Destino, ref destino, ref matrizCaminhos, ref caminhoEncontrado, ref resultados, ref passou);
                    }
                }
            }

            if (caminhoEncontrado.Count != 0)
            {
                caminhoEncontrado.Pop();
                passou[atual.Id] = false;
            }
        }

        static public void BuscarCaminhosPilhas(ref Cidade atual, ref Cidade destino, ref AvancoCaminho[,] matrizCaminhos, ref Stack<AvancoCaminho> caminhoEncontrado, ref List<AvancoCaminho[]> resultados)
        {
            bool temCaminhoPossivel = true;
            bool movimentou;
            int idAtual = atual.Id;
            int start = 0;
            bool[] passou = new bool[matrizCaminhos.GetLength(0)];

            while (temCaminhoPossivel)
            {
                passou[idAtual] = true;
                movimentou = false;
                for (int j = start; j < matrizCaminhos.GetLength(0); j++)
                {
                    AvancoCaminho ac = matrizCaminhos[idAtual, j];
                    if (ac != null && !passou[j])
                    {
                        if (j == destino.Id)
                        {
                            caminhoEncontrado.Push(matrizCaminhos[idAtual, j]);
                            resultados.Add(caminhoEncontrado.ToArray());
                            caminhoEncontrado.Pop();
                        }
                        else
                        {
                            caminhoEncontrado.Push(matrizCaminhos[idAtual, j]);
                            idAtual = j;
                            movimentou = true;
                            start = 0;
                            break;
                        }
                    }
                }

                if (!movimentou)
                {
                    if (caminhoEncontrado.Count() == 0)
                        temCaminhoPossivel = false;
                    else
                    {
                        passou[idAtual] = false;
                        start = idAtual + 1;
                        idAtual = caminhoEncontrado.Pop().Origem.Id;
                    }
                }
            }
        }

        static public AvancoCaminho[] BuscarMelhorCaminhoDistancia(List<AvancoCaminho[]> caminhos)
        {
            var distancias = new List<int>();

            for (int i = 0; i < caminhos.Count; i++)
            {
                int distancia = 0;

                for (int j = 0; j < caminhos[i].Length; j++)
                {
                    distancia += caminhos[i][j].Caminho.Distancia;                    
                }

                distancias.Add(distancia);
            }

            return caminhos[distancias.IndexOf(distancias.Min())];
        }

        static public AvancoCaminho[] BuscarMelhorCaminhoTempo(List<AvancoCaminho[]> caminhos)
        {
            var tempos = new List<int>();

            for (int i = 0; i < caminhos.Count; i++)
            {
                int tempo = 0;

                for (int j = 0; j < caminhos[i].Length; j++)
                {
                    tempo += caminhos[i][j].Caminho.Tempo;
                }

                tempos.Add(tempo);
            }

            return caminhos[tempos.IndexOf(tempos.Min())];
        }

        static public AvancoCaminho[] BuscarMelhorCaminhoCusto(List<AvancoCaminho[]> caminhos)
        {
            var custos = new List<int>();

            for (int i = 0; i < caminhos.Count; i++)
            {
                int custo = 0;

                for (int j = 0; j < caminhos[i].Length; j++)
                {
                    custo += caminhos[i][j].Caminho.Custo;
                }

                custos.Add(custo);
            }

            return caminhos[custos.IndexOf(custos.Min())];
        }
    }
}
