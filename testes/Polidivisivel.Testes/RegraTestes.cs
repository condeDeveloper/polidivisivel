using System.Numerics;
using Conde.Polidivisivel;
using Xunit;

namespace Conde.Polidivisivel.Testes;

public class RegraTestes
{
    private static int[] Algarismos(string texto) => texto.Select(c => c - '0').ToArray();

    [Theory]
    [InlineData("1")]
    [InlineData("9")]
    [InlineData("12")]
    [InlineData("123")]
    [InlineData("1236")]
    [InlineData("3608528850368400786036725")]
    public void NumerosQueValem(string texto)
    {
        Assert.True(Regra.Vale(Algarismos(texto)));
        Assert.Equal(0, Regra.OndeFalha(Algarismos(texto)));
    }

    [Theory]
    [InlineData("13", 2)]
    [InlineData("124", 3)]
    [InlineData("1234", 4)]
    [InlineData("01", 1)]
    public void NumerosQueNaoValemEOndeFalham(string texto, int onde)
    {
        Assert.False(Regra.Vale(Algarismos(texto)));
        Assert.Equal(onde, Regra.OndeFalha(Algarismos(texto)));
    }

    [Fact]
    public void ZeroAEsquerdaNaoVale()
    {
        Assert.False(Regra.Vale([0]));
        Assert.False(Regra.Vale([0, 2]));
        Assert.False(Regra.Vale([]));
    }

    [Fact]
    public void TodoPrefixoDeUmPolidivisivelTambemEPolidivisivel()
    {
        var inteiro = Algarismos("3608528850368400786036725");
        for (var k = 1; k <= inteiro.Length; k++)
        {
            Assert.True(Regra.Vale(inteiro[..k]));
        }
    }

    [Fact]
    public void OSegundoAlgarismoFormaSempreUmNumeroPar()
    {
        foreach (var camada in Geracao.PorComprimento())
        {
            foreach (var numero in camada.Where(n => n.Length >= 2))
            {
                Assert.Equal(0, ((numero[0] * 10) + numero[1]) % 2);
            }
        }
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("7", 7)]
    [InlineData("123", 123)]
    [InlineData("3608528850368400786036725", 0)]
    public void OValorEONumeroMontado(string texto, long esperado)
    {
        var valor = Regra.Valor(Algarismos(texto));
        if (esperado != 0 || texto == "0")
        {
            Assert.Equal(new BigInteger(esperado), valor);
        }
        else
        {
            Assert.Equal(BigInteger.Parse(texto), valor);
        }
    }

    [Fact]
    public void OTextoUsaLetrasAPartirDoDez()
    {
        Assert.Equal("fae0", Regra.Texto([15, 10, 14, 0]));
        Assert.Equal("123", Regra.Texto([1, 2, 3]));
    }

    [Fact]
    public void EmBaseDoisPoucoSobrevive()
    {
        Assert.True(Regra.Vale([1], 2));
        Assert.True(Regra.Vale([1, 0], 2));
        Assert.False(Regra.Vale([1, 1], 2));
    }

    [Fact]
    public void ArgumentosInvalidosNaoSaoAceitos()
    {
        Assert.Throws<ArgumentNullException>(() => Regra.Vale(null!));
        Assert.Throws<ArgumentNullException>(() => Regra.OndeFalha(null!));
        Assert.Throws<ArgumentNullException>(() => Regra.Valor(null!));
        Assert.Throws<ArgumentNullException>(() => Regra.Texto(null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => Regra.Vale([1], 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Regra.Vale([1, 9], 5));
    }
}
