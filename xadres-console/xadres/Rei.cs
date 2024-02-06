using tabuleiro;

namespace xadres
{
    class Rei : Peca
    {

        private PartidaDeXadrez partida;
        public Rei(Tabuleiro tab, Cor cor, PartidaDeXadrez partida) : base(tab, cor)
        {
            this.partida = partida;
        }

        public override string ToString()
        {
            return "R";
        }

        private bool podeMover(Posicao pos)
        {
            Peca p = tab.peca(pos);

            return p == null || p.cor != cor;
        }

        private bool testeTorreParaRoque(Posicao pos)
        {
            Peca p = tab.peca(pos);

            return p != null && p is Torre && p.cor == cor && p.qtdMovimento == 0;

        }

       


        public override bool[,] movimentosPossiveis()
        {
            bool[,] mat = new bool[tab.linhas, tab.colunas]; // MATRIZ TEMPORARIA
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    int novaLinha = posicao.linha + i;
                    int novaColuna = posicao.coluna + j;

                    Posicao pos = new Posicao(novaLinha, novaColuna);

                    if (tab.posicaoValida(pos) && podeMover(pos))
                    {
                        mat[pos.linha, pos.coluna] = true;
                    }
                }
            }
            //#Jogada Especial Roque

            if (qtdMovimento==0&& !partida.xeque)
            {
                //#jogada especial Roque pequeno
               Posicao posTorre1=new Posicao(posicao.linha, posicao.coluna+3);

                if (testeTorreParaRoque(posTorre1))
                {
                    Posicao p1 = new Posicao(posicao.linha, posicao.coluna + 1);
                    Posicao p2 = new Posicao(posicao.linha, posicao.coluna + 2);

                    if(tab.peca(p1)==null && tab.peca(p2) == null)
                    {
                        mat[posicao.linha, posicao.coluna + 2]=true;
                    }

                }

                //#jogada especial Roque Grande
                Posicao posTorre2 = new Posicao(posicao.linha, posicao.coluna - 4);

                if (testeTorreParaRoque(posTorre2))
                {
                    Posicao p1 = new Posicao(posicao.linha, posicao.coluna - 1);
                    Posicao p2 = new Posicao(posicao.linha, posicao.coluna - 2);
                    Posicao p3 = new Posicao(posicao.linha, posicao.coluna - 3);
                    if (tab.peca(p1) == null && tab.peca(p2) == null && tab.peca(p3) == null)
                    {
                        mat[posicao.linha, posicao.coluna - 2] = true;
                    }

                }
            }
            return mat;
        }
    }
}
