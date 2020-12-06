//Eduardo Migueis - 19167 e Rodrigo Smith - 19197

using System;
using System.Collections.Generic;

namespace apCaminhosMarte.Data
{
    class ArvoreBinaria<T> where T : IComparable<T>
    {
        private int qtd = 0;
        public int Qtd { get => qtd; }

        public NoArvore<T> Raiz { get; set; }

        public ArvoreBinaria()
        { }

        public int AlturaArvore(NoArvore<T> atual, ref bool balanceada)
        {
            int alturaDireita, alturaEsquerda, result;
            if (atual != null && balanceada)
            {
                alturaEsquerda = 1 + AlturaArvore(atual.Esq, ref balanceada);
                alturaDireita = 1 + AlturaArvore(atual.Dir, ref balanceada);
                result = Math.Max(alturaEsquerda, alturaDireita);

                if (Math.Abs(alturaDireita - alturaEsquerda) > 1)
                    balanceada = false;
            }
            else
                result = 0;
            return result;
        }

        public int GetAltura(NoArvore<T> no)
        {
            if (no != null)
                return no.Altura;
            else
                return -1;
        }

        public void Incluir(T info)
        {
            if (this.Raiz == null)
            {
                this.Raiz = new NoArvore<T>(info);
                qtd++;
            }
            else
                IncluirRecBaleanceada(Raiz, info);
        }

        public NoArvore<T> IncluirRecBaleanceada(NoArvore<T> noAtual, T item)
        {
            if (noAtual == null)
            {
                noAtual = new NoArvore<T>(item);
                qtd++;
            }
            else
            {
                if (item.CompareTo(noAtual.Info) < 0)
                {
                    noAtual.Esq = IncluirRecBaleanceada(noAtual.Esq, item);
                    if (GetAltura(noAtual.Esq) - GetAltura(noAtual.Dir) == 2)
                        if (item.CompareTo(noAtual.Esq.Info) < 0)
                            noAtual = RotacaoSimplesComFilhoEsquerdo(noAtual);
                        else
                            noAtual = RotacaoDuplaComFilhoEsquerdo(noAtual);
                }
                else if (item.CompareTo(noAtual.Info) > 0)
                {
                    noAtual.Dir = IncluirRecBaleanceada(noAtual.Dir, item);
                    if (GetAltura(noAtual.Dir) - GetAltura(noAtual.Esq) == 2)
                        if (item.CompareTo(noAtual.Dir.Info) > 0)
                            noAtual = RotacaoSimplesComFilhoDireito(noAtual);
                        else
                            noAtual = RotacaoDuplaComFilhoDireito(noAtual);
                }

                noAtual.Altura = Math.Max(GetAltura(noAtual.Esq), GetAltura(noAtual.Dir)) + 1;
            }
            return noAtual;
        }

        private NoArvore<T> RotacaoSimplesComFilhoEsquerdo(NoArvore<T> no)
        {
            NoArvore<T> temp = no.Esq;
            no.Esq = temp.Dir;
            temp.Dir = no;
            if (Raiz.Equals(no))
                Raiz = temp;
            no.Altura = Math.Max(GetAltura(no.Esq), GetAltura(no.Dir)) + 1;
            temp.Altura = Math.Max(GetAltura(temp.Esq), GetAltura(no)) + 1;
            return temp;
        }

        private NoArvore<T> RotacaoSimplesComFilhoDireito(NoArvore<T> no)
        {
            NoArvore<T> temp = no.Dir;
            no.Dir = temp.Esq;
            temp.Esq = no;
            if (Raiz.Equals(no))
                Raiz = temp;
            no.Altura = Math.Max(GetAltura(no.Esq), GetAltura(no.Dir)) + 1;
            temp.Altura = Math.Max(GetAltura(temp.Dir), GetAltura(no)) + 1;
            return temp;
        }

        private NoArvore<T> RotacaoDuplaComFilhoEsquerdo(NoArvore<T> no)
        {
            no.Esq = RotacaoSimplesComFilhoDireito(no.Esq);
            return RotacaoSimplesComFilhoEsquerdo(no);
        }

        private NoArvore<T> RotacaoDuplaComFilhoDireito(NoArvore<T> no)
        {
            no.Dir = RotacaoSimplesComFilhoEsquerdo(no.Dir);
            return RotacaoSimplesComFilhoDireito(no);
        }

        public T Busca(T buscado)
        {
            return Achar(buscado, this.Raiz);
        }

        private T Achar(T buscado, NoArvore<T> atual)
        {
            if (atual == null)
                throw new Exception("Informação inexistente!");

            int comp = buscado.CompareTo(atual.Info);
            if (comp == 0)
                return atual.Info;
            if (comp < 0)
                return Achar(buscado, atual.Esq);
            else
                return Achar(buscado, atual.Dir);
        }

        public List<T> ToList()
        {
            List<T> result = new List<T>();
            Converter(this.Raiz, ref result);
            return result;
        }

        private void Converter(NoArvore<T> atual, ref List<T> result)
        {
            if (atual == null)
                return;

            result.Add(atual.Info);
            Converter(atual.Esq, ref result);
            Converter(atual.Dir, ref result);
        }

    }
}
