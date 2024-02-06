using tabuleiro;

namespace xadres
{
    class Bispo : Peca
    {
        public Bispo(Tabuleiro tab, Cor cor) : base(tab, cor)
        {


        }

        public override string ToString()
        {
            return "B";
        }
        private bool podeMover(Posicao pos)
        {
            Peca p = tab.peca(pos);

            return p == null || p.cor != cor;
        }

        private void valores()
        {

        }
        public override bool[,] movimentosPossiveis()
        {
            bool[,] mat = new bool[tab.linhas, tab.colunas]; // MATRIZ TEMPORARIA
            Posicao pos = new Posicao(0, 0);


            //Difinido posição direta-acima↑ ↗ 
            pos.definirValores(posicao.linha - 1, posicao.coluna-1);
            while (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
                if (tab.peca(pos) != null && tab.peca(pos).cor != cor)
                {
                    break;
                }

                pos.definirValores(pos.linha - 1, pos.coluna - 1);
            }
            //Difinido posição  ↖ ↑esquerda-cima
            pos.definirValores(posicao.linha -1, posicao.coluna + 1);
            while (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
                if (tab.peca(pos) != null && tab.peca(pos).cor != cor)
                {
                    break;
                }

                pos.definirValores(pos.linha - 1, pos.coluna + 1);
            }

            //Difinido posição   direita-baixo↓ ↗ 
           
            pos.definirValores(posicao.linha + 1, posicao.coluna - 1);
            while (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
                if (tab.peca(pos) != null && tab.peca(pos).cor != cor)
                {
                    break;
                }

                pos.definirValores(pos.linha + 1, pos.coluna - 1);
            }

            //Difinido posição  ↖ ↓esquerda-baixo
            pos.definirValores(posicao.linha + 1, posicao.coluna + 1);
            while (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
                if (tab.peca(pos) != null && tab.peca(pos).cor != cor)
                {
                    break;
                }

                pos.definirValores(pos.linha + 1, pos.coluna + 1);
            }
            return mat;
        }
    }
}
