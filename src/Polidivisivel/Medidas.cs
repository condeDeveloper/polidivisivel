namespace Conde.Polidivisivel;

/// <summary>
/// As tabelas do README. Cada número sai de um método daqui e está preso num
/// teste; só entra aqui o que eu vi o programa imprimir.
/// </summary>
public static class Medidas
{
    public readonly record struct LinhaComprimento(int Comprimento, long Quantos, double Previsto, double Razao, long Candidatos);

    public readonly record struct LinhaBase(int Base_, int MaiorComprimento, long Total, string Maior);

    /// <summary>
    /// Quantos números poli-divisíveis existem de cada comprimento, contra a
    /// previsão ingênua.
    ///
    /// A previsão: existem <c>(b−1)·b^(k−1)</c> números de <c>k</c> algarismos,
    /// e se cada condição de divisibilidade fosse um sorteio independente com
    /// chance <c>1/j</c>, sobrariam <c>(b−1)·b^(k−1) / k!</c>. Nada garante que
    /// as condições sejam independentes, e a tabela mede o quanto não são.
    /// </summary>
    public static List<LinhaComprimento> PorComprimento(int baseNumerica = 10)
    {
        var camadas = Geracao.PorComprimento(baseNumerica);
        var linhas = new List<LinhaComprimento>();

        double previsto = baseNumerica - 1;
        for (var k = 1; k <= camadas.Count; k++)
        {
            if (k > 1)
            {
                previsto = previsto * baseNumerica / k;
            }

            // Os candidatos de um comprimento sao os sobreviventes do anterior
            // vezes a base: e isso que a geracao por crescimento chega a testar,
            // e nao todos os numeros daquele tamanho.
            var anteriores = k == 1 ? baseNumerica - 1 : (long)camadas[k - 2].Count * baseNumerica;

            linhas.Add(new LinhaComprimento(
                k,
                camadas[k - 1].Count,
                Math.Round(previsto, 1),
                Math.Round(camadas[k - 1].Count / previsto, 3),
                anteriores));
        }

        return linhas;
    }

    /// <summary>O total de números poli-divisíveis numa base.</summary>
    public static long Total(int baseNumerica = 10)
    {
        long total = 0;
        foreach (var camada in Geracao.PorComprimento(baseNumerica))
        {
            total += camada.Count;
        }

        return total;
    }

    /// <summary>O maior de todos, e quantos empatam com ele.</summary>
    public static (int Comprimento, int Quantos, string Maior) Maior(int baseNumerica = 10)
    {
        var camadas = Geracao.PorComprimento(baseNumerica);
        var ultima = camadas[^1];
        var maior = ultima.OrderBy(a => Regra.Valor(a, baseNumerica)).Last();
        return (camadas.Count, ultima.Count, Regra.Texto(maior));
    }

    /// <summary>
    /// O mesmo problema em outras bases. Em base 2 e 3 a brincadeira acaba
    /// quase imediatamente; a pergunta é se o comprimento máximo acompanha a
    /// base de algum jeito previsível.
    /// </summary>
    public static List<LinhaBase> PorBase(IEnumerable<int> bases)
    {
        var linhas = new List<LinhaBase>();
        foreach (var b in bases)
        {
            var (comprimento, _, maior) = Maior(b);
            linhas.Add(new LinhaBase(b, comprimento, Total(b), maior));
        }

        return linhas;
    }

    /// <summary>
    /// A geração por crescimento contra a força bruta, no maior comprimento em
    /// que a força bruta ainda cabe.
    /// </summary>
    public static (long PorCrescimento, long ForcaBruta, long Candidatos) Confronto(int comprimento, int baseNumerica = 10)
    {
        var camadas = Geracao.PorComprimento(baseNumerica);
        var crescimento = camadas[comprimento - 1].Count;
        var bruta = Geracao.PorForcaBruta(comprimento, baseNumerica).Count;

        long candidatos = 1;
        for (var i = 0; i < comprimento; i++)
        {
            candidatos *= baseNumerica;
        }

        return (crescimento, bruta, candidatos);
    }
}
