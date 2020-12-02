//Eduardo Migueis - 19167 e Rodrigo Smith - 19197

using System;

namespace apCaminhosMarte.Data
{
    class NoArvore<T> : IComparable<NoArvore<T>> where T : IComparable<T>
    {
        //Propriedades
        public T Info { get; set; }
        public NoArvore<T> Esq { get; set; }
        public NoArvore<T> Dir { get; set; }
        public int Altura { get; set; }
        public bool EstaMarcadoParaMorrer { get; set; }

        /// <summary>
        /// Constrói uma instância de NoArvore com valor armazenado. Porém, sem descendentes.
        /// </summary>
        /// <param name="info">Valor a ser armazenado no nó</param>
        public NoArvore(T info)
        {
            this.Info = info;
            this.Esq = this.Dir = null;
        }

        public NoArvore(T info, NoArvore<T> esq, NoArvore<T> dir, int altura, bool estaMarcadoParaMorrer)
        {
            this.Info = info;
            this.Esq = esq;
            this.Dir = dir;
            this.Altura = altura;
            this.EstaMarcadoParaMorrer = estaMarcadoParaMorrer;
        }

        public int CompareTo(NoArvore<T> o)
        {
            return Info.CompareTo(o.Info);
        }

        public bool Equals(NoArvore<T> o)
        {
            return this.Info.Equals(o.Info);
        }
    }
}
