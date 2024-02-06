using System.Collections.Generic;
using tabuleiro;

namespace xadres
{
    class PartidaDeXadrez
    {
        public Tabuleiro tab { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }
        public bool xeque { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;
        public Peca vulneravelEnPassant { get; private set; }


        public PartidaDeXadrez()
        {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            terminada = false;
            xeque = false;
            vulneravelEnPassant = null;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public Peca executarMovimento(Posicao origem, Posicao destino)
        {
            Peca p = tab.retiraPeca(origem);
            p.incrementarMovimentos();
            Peca pecaCapturada = tab.retiraPeca(destino);
            tab.colocarPeca(p, destino);
            if (pecaCapturada != null)
            {
                capturadas.Add(pecaCapturada); //aqui ficara armazenadas todas as peças capituradas
            }

            //#jogada Especial Roque pequeno

            if (p is Rei && destino.coluna == origem.coluna + 2)
            {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna+3);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retiraPeca(origemTorre);
                T.incrementarMovimentos();
                tab.colocarPeca(T, destinoTorre);


            }

            //#jogada Especial Roque grande

            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna -1);
                Peca T = tab.retiraPeca(origemTorre);
                T.incrementarMovimentos();
                tab.colocarPeca(T, destinoTorre);


            }

            //#Jagada Especial en Passant
            if(p is Peao)
            {
                if(origem.coluna !=destino.coluna && pecaCapturada == null)
                {
                    Posicao posPeao;
                    if (p.cor == Cor.Branca)
                    {
                        posPeao = new Posicao(destino.linha + 1, destino.coluna);
                    }
                    else
                    {
                        posPeao = new Posicao(destino.linha - 1, destino.coluna);
                    }
                    pecaCapturada = tab.retiraPeca(posPeao);
                    capturadas.Add(pecaCapturada);
                }

            }

            return pecaCapturada;

        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tab.retiraPeca(destino);
            p.decrementarMovimentos();

            if (pecaCapturada != null)
            {
                tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }

            //#jogada Especial Roque pequeno

            if (p is Rei && destino.coluna == origem.coluna + 2)
            {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retiraPeca(destinoTorre);
                T.decrementarMovimentos();
                tab.colocarPeca(T, origemTorre);


            }
            //#jogada Especial Roque grande

            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = tab.retiraPeca(destinoTorre);
                T.decrementarMovimentos();
                tab.colocarPeca(T, origemTorre);


            }

            //#Jagada Especial en Passant
            if (p is Peao)
            {
                if (origem.coluna != destino.coluna && pecaCapturada == vulneravelEnPassant)
                {
                    Peca peaoRetira = tab.retiraPeca(destino);
                    Posicao posPeao;
                    if (p.cor == Cor.Branca)
                    {
                        posPeao = new Posicao(3, destino.coluna);
                    }
                    else
                    {
                        posPeao = new Posicao(4, destino.coluna);
                    }
                    tab.colocarPeca(peaoRetira, posPeao);
                }

            }

            tab.colocarPeca(p, origem);
        }

        public void realizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = executarMovimento(origem, destino);
            if (estaEmXeque(jogadorAtual))
            {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Jogada Invalida: Você não pode colocar seu Rei em Xeque");
            }

            Peca p = tab.peca(destino);
            //#jogada especial promoção

            if (p is Peao)
            {
                if ((p.cor ==Cor.Branca && destino.linha == 0)|| (p.cor == Cor.Preta && destino.linha == 7))
                {
                    p = tab.retiraPeca(destino);
                    pecas.Remove(p);

                    Peca dama = new Dama(tab, p.cor);
                    tab.colocarPeca(dama, destino);
                    pecas.Add(dama);
                }

            }

            if (estaEmXeque(adversaria(jogadorAtual)))
            {
                xeque = true;
            }
            else
            {
                xeque = false;
            }
            if (testeXequemate(adversaria(jogadorAtual)))
            {
                terminada = true;
            }
            else
            {
                turno++;
                mudaJogador(); //Metado para alternnar o jogador
            }

           

            //#Jagada Especial en Passant

            if(p is Peao && (destino.linha == origem.linha -2 || destino.linha == origem.linha + 2))
            {
                vulneravelEnPassant = p;
            }
            else
            {
                vulneravelEnPassant = null;
            }


        }

        public void validarPosicaoDeOrigem(Posicao pos)
        {
            if (tab.peca(pos) == null)
            {
                throw new TabuleiroException("Jogada Invalida: Casa vazia. Tente novamente");
            }

            if (jogadorAtual != tab.peca(pos).cor)
            {

                throw new TabuleiroException("Jagada Invalida: Essa peça não é sua. Tente novamente ");
            }
            if (!tab.peca(pos).existeMovimentosPossiveis())
            {
                throw new TabuleiroException("Jogada Invalida: Não movimentos possivel dessa peça.  Tente novamente");
            }


        }

        public void validarPosicaoDeDestino(Posicao origem, Posicao destino)
        {
            if (!tab.peca(origem).movimentoPossivel(destino))
            {
                throw new TabuleiroException("Jogada Invalida: Posicação destino!");
            }
        }

        private void mudaJogador()
        {
            if (jogadorAtual == Cor.Branca)
            {
                jogadorAtual = Cor.Preta;
            }
            else
            {
                jogadorAtual = Cor.Branca;
            }
        }

        //Aqui vamos fazer metado onde sera separada as peças capturadas conforme a cor

        public HashSet<Peca> pecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();//variavel temporarias
            foreach (Peca x in capturadas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            return aux;

        }

        public HashSet<Peca> pecasEmJogo(Cor cor)
        {

            HashSet<Peca> aux = new HashSet<Peca>();//variavel temporarias
            foreach (Peca x in pecas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }

            aux.ExceptWith(pecasCapturadas(cor));//Aqui retiramos a pecas do tabuleiro
            return aux;

        }

        private Cor adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
            {
                return Cor.Preta;
            }
            else
            {
                return Cor.Branca;
            }
        }

        private Peca rei(Cor cor)
        {
            foreach (Peca x in pecasEmJogo(cor))
            {
                if (x is Rei)
                {
                    return x;
                }
            }
            return null;
        }

        public bool estaEmXeque(Cor cor)
        {
            Peca R = rei(cor); // cor do rei adversario
            if (R == null)
            {
                throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro");
            }
            foreach (Peca x in pecasEmJogo(adversaria(cor)))
            {
                bool[,] mat = x.movimentosPossiveis();
                if (mat[R.posicao.linha, R.posicao.coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca)
        {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        public bool testeXequemate(Cor cor)
        {
            if (!estaEmXeque(cor))
            {
                return false;
            }

            foreach (Peca x in pecasEmJogo(cor))
            {
                bool[,] mat = x.movimentosPossiveis();
                for (int i = 0; i < tab.linhas; i++)
                {
                    for (int j = 0; j < tab.colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = executarMovimento(origem, destino);
                            bool testXeque = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pecaCapturada);
                            if (!testXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;

        }


        private void colocarPecas()
        {

            colocarNovaPeca('a', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('b', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('c', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('d', 1, new Dama(tab, Cor.Branca));
            colocarNovaPeca('e', 1, new Rei(tab, Cor.Branca,this));
            colocarNovaPeca('f', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('g', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('h', 1, new Torre(tab, Cor.Branca));

            colocarNovaPeca('a', 2, new Peao(tab, Cor.Branca,this));
            colocarNovaPeca('b', 2, new Peao(tab, Cor.Branca,this));
            colocarNovaPeca('c', 2, new Peao(tab, Cor.Branca,this));
            colocarNovaPeca('d', 2, new Peao(tab, Cor.Branca,this));
            colocarNovaPeca('e', 2, new Peao(tab, Cor.Branca,this));
            colocarNovaPeca('f', 2, new Peao(tab, Cor.Branca,this));
            colocarNovaPeca('g', 2, new Peao(tab, Cor.Branca,this));
            colocarNovaPeca('h', 2, new Peao(tab, Cor.Branca,this));


            colocarNovaPeca('a', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('b', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('c', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('d', 8, new Dama(tab, Cor.Preta));
            colocarNovaPeca('e', 8, new Rei(tab, Cor.Preta,this));
            colocarNovaPeca('f', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('g', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('h', 8, new Torre(tab, Cor.Preta));

            colocarNovaPeca('a', 7, new Peao(tab, Cor.Preta,this));
            colocarNovaPeca('b', 7, new Peao(tab, Cor.Preta,this));
            colocarNovaPeca('c', 7, new Peao(tab, Cor.Preta,this));
            colocarNovaPeca('d', 7, new Peao(tab, Cor.Preta,this));
            colocarNovaPeca('e', 7, new Peao(tab, Cor.Preta,this));
            colocarNovaPeca('f', 7, new Peao(tab, Cor.Preta,this));
            colocarNovaPeca('g', 7, new Peao(tab, Cor.Preta,this));
            colocarNovaPeca('h', 7, new Peao(tab, Cor.Preta,this));

        }
    }
}
