﻿

namespace tabuleiro
{
    abstract class Peca
    {
        public Posicao posicao { get; set; }
        public Cor cor { get; protected set; }
        public int qtdMovimento { get; protected set; }
        public Tabuleiro tab { get; protected set; }

        public Peca(Tabuleiro tab, Cor cor)
        {
            this.posicao = null;
            this.tab = tab;
            this.cor = cor;
            this.qtdMovimento = 0;

        }

        public void incrementarMovimentos()
        {
            qtdMovimento++;
        }

        public void decrementarMovimentos()
        {
            qtdMovimento--;
        }

        public bool existeMovimentosPossiveis()
        {
            bool[,] mat = movimentosPossiveis(); // variavel temporaria
            for (int i = 0; i < tab.linhas; i++)
            {
                for (int j = 0; j < tab.colunas; j++)
                {
                    if (mat[i,j] == true)
                    {
                        return true;
                    }
                }
            }  
            return false;
        }

        public bool movimentoPossivel(Posicao pos) 
        { 
            return movimentosPossiveis()[pos.linha, pos.coluna]; 
        }

        public abstract bool[,] movimentosPossiveis();
        

        


    }
}
