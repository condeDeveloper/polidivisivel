namespace Conde.Polidivisivel;

/// <summary>
/// A geração por crescimento: todo prefixo de um número poli-divisível também
/// é poli-divisível, então os números de comprimento <c>k</c> saem dos de
/// comprimento <c>k−1</c> tentando acrescentar um algarismo.
///
/// Isso troca uma varredura sobre todos os números de <c>k</c> algarismos por
/// uma lista que, em cada passo, tem só o que sobreviveu — e é a diferença
/// entre caber e não caber.
///
/// <para>
/// O truque do resto: para testar "o pedaço de <c>k</c> algarismos é divisível
/// por <c>k</c>" não é preciso guardar o número, que chega a 25 algarismos.
/// Basta guardar o pedaço <b>módulo o mínimo múltiplo comum de 1 até o
/// comprimento máximo</b>, porque todo <c>k</c> divide esse mínimo. Para 25
/// algarismos ele vale 26.771.144.400, que cabe folgado num inteiro de 64 bits.
/// </para>
/// </summary>
public static class Geracao
{
    public readonly record struct Prefixo(int[] Algarismos, long Resto);

    /// <summary>O mínimo múltiplo comum de 1 até <paramref name="n"/>.</summary>
    public static long MmcAte(int n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);
        long mmc = 1;
        for (var i = 2; i <= n; i++)
        {
            mmc = mmc / Mdc(mmc, i) * i;
        }

        return mmc;
    }

    public static long Mdc(long a, long b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }

        return a < 0 ? -a : a;
    }

    /// <summary>
    /// Todos os números poli-divisíveis de cada comprimento, de 1 até onde a
    /// lista secar. A lista seca sozinha: é isso que prova que eles acabam.
    /// </summary>
    public static List<List<int[]>> PorComprimento(int baseNumerica = 10, int maximo = 40)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(baseNumerica, 2);
        var mmc = MmcAte(maximo);
        var camadas = new List<List<int[]>>();

        var atual = new List<Prefixo>();
        for (var a = 1; a < baseNumerica; a++)
        {
            atual.Add(new Prefixo([a], a % mmc));
        }

        var comprimento = 1;
        while (atual.Count > 0 && comprimento <= maximo)
        {
            camadas.Add(atual.Select(p => p.Algarismos).ToList());

            var seguinte = new List<Prefixo>();
            var k = comprimento + 1;
            foreach (var prefixo in atual)
            {
                for (var a = 0; a < baseNumerica; a++)
                {
                    var resto = ((prefixo.Resto * baseNumerica) + a) % mmc;
                    if (resto % k != 0)
                    {
                        continue;
                    }

                    var algarismos = new int[prefixo.Algarismos.Length + 1];
                    Array.Copy(prefixo.Algarismos, algarismos, prefixo.Algarismos.Length);
                    algarismos[^1] = a;
                    seguinte.Add(new Prefixo(algarismos, resto));
                }
            }

            atual = seguinte;
            comprimento++;
        }

        return camadas;
    }

    /// <summary>
    /// A força bruta: todos os números de <paramref name="comprimento"/>
    /// algarismos, conferidos um a um pela <see cref="Regra"/>. Só cabe em
    /// comprimentos pequenos, e é exatamente por isso que ela existe.
    /// </summary>
    public static List<int[]> PorForcaBruta(int comprimento, int baseNumerica = 10)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(comprimento, 1);
        var achados = new List<int[]>();
        var algarismos = new int[comprimento];

        Passo(0);
        return achados;

        void Passo(int posicao)
        {
            if (posicao == comprimento)
            {
                if (Regra.Vale(algarismos, baseNumerica))
                {
                    achados.Add((int[])algarismos.Clone());
                }

                return;
            }

            for (var a = 0; a < baseNumerica; a++)
            {
                algarismos[posicao] = a;
                Passo(posicao + 1);
            }
        }
    }
}
