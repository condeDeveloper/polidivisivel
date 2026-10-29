using System.Numerics;

namespace Conde.Polidivisivel;

/// <summary>
/// A regra, pela definição e nada mais: um número é poli-divisível quando o
/// pedaço formado pelos seus <c>k</c> primeiros algarismos é divisível por
/// <c>k</c>, para TODO <c>k</c> de 1 até o comprimento.
///
/// O primeiro algarismo é sempre divisível por 1, os dois primeiros têm que
/// formar um número par, os três primeiros um múltiplo de 3, e assim por
/// diante. Zero à esquerda não vale: o número tem o comprimento que tem.
///
/// Esta classe monta o número inteiro para conferir, sem truque nenhum. É a
/// resposta de referência contra a qual a geração esperta é conferida.
/// </summary>
public static class Regra
{
    public static bool Vale(IReadOnlyList<int> algarismos, int baseNumerica = 10)
    {
        ArgumentNullException.ThrowIfNull(algarismos);
        ArgumentOutOfRangeException.ThrowIfLessThan(baseNumerica, 2);

        if (algarismos.Count == 0 || algarismos[0] == 0)
        {
            return false;
        }

        var pedaco = BigInteger.Zero;
        for (var k = 1; k <= algarismos.Count; k++)
        {
            var algarismo = algarismos[k - 1];
            if (algarismo < 0 || algarismo >= baseNumerica)
            {
                throw new ArgumentOutOfRangeException(nameof(algarismos), "algarismo fora da base");
            }

            pedaco = (pedaco * baseNumerica) + algarismo;
            if (pedaco % k != 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>O primeiro <c>k</c> em que a regra falha, ou zero se ela valer.</summary>
    public static int OndeFalha(IReadOnlyList<int> algarismos, int baseNumerica = 10)
    {
        ArgumentNullException.ThrowIfNull(algarismos);
        if (algarismos.Count == 0 || algarismos[0] == 0)
        {
            return 1;
        }

        var pedaco = BigInteger.Zero;
        for (var k = 1; k <= algarismos.Count; k++)
        {
            pedaco = (pedaco * baseNumerica) + algarismos[k - 1];
            if (pedaco % k != 0)
            {
                return k;
            }
        }

        return 0;
    }

    public static BigInteger Valor(IReadOnlyList<int> algarismos, int baseNumerica = 10)
    {
        ArgumentNullException.ThrowIfNull(algarismos);
        var valor = BigInteger.Zero;
        foreach (var algarismo in algarismos)
        {
            valor = (valor * baseNumerica) + algarismo;
        }

        return valor;
    }

    public static string Texto(IReadOnlyList<int> algarismos)
    {
        ArgumentNullException.ThrowIfNull(algarismos);
        return string.Concat(algarismos.Select(a => a < 10 ? (char)('0' + a) : (char)('a' + a - 10)));
    }
}
