using Conde.Polidivisivel;
using Xunit;

namespace Conde.Polidivisivel.Testes;

public class GeracaoTestes
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 6)]
    [InlineData(4, 12)]
    [InlineData(5, 60)]
    [InlineData(10, 2520)]
    [InlineData(25, 26771144400L)]
    public void OMinimoMultiploComum(int n, long esperado)
    {
        Assert.Equal(esperado, Geracao.MmcAte(n));
    }

    [Fact]
    public void OMmcDeVinteCincoCabeFolgadoEmSessentaEQuatroBits()
    {
        Assert.True(Geracao.MmcAte(25) < long.MaxValue / 100);
        Assert.Equal(0, Geracao.MmcAte(25) % 25);
        for (var k = 1; k <= 25; k++)
        {
            Assert.Equal(0, Geracao.MmcAte(25) % k);
        }
    }

    [Theory]
    [InlineData(12, 8, 4)]
    [InlineData(100, 75, 25)]
    [InlineData(17, 5, 1)]
    [InlineData(0, 7, 7)]
    public void OMaximoDivisorComum(long a, long b, long esperado)
    {
        Assert.Equal(esperado, Geracao.Mdc(a, b));
        Assert.Equal(esperado, Geracao.Mdc(b, a));
    }

    [Fact]
    public void OCrescimentoEAForcaBrutaAchamExatamenteOsMesmosNumeros()
    {
        var camadas = Geracao.PorComprimento();
        for (var k = 1; k <= 6; k++)
        {
            var bruta = Geracao.PorForcaBruta(k).Select(Regra.Texto).OrderBy(x => x).ToList();
            var crescimento = camadas[k - 1].Select(Regra.Texto).OrderBy(x => x).ToList();
            Assert.Equal(bruta, crescimento);
        }
    }

    [Fact]
    public void TodoNumeroGeradoPassaNaRegra()
    {
        foreach (var camada in Geracao.PorComprimento())
        {
            Assert.All(camada, n => Assert.True(Regra.Vale(n)));
        }
    }

    [Fact]
    public void CadaCamadaTemOComprimentoQueDiz()
    {
        var camadas = Geracao.PorComprimento();
        for (var k = 1; k <= camadas.Count; k++)
        {
            Assert.All(camadas[k - 1], n => Assert.Equal(k, n.Length));
        }
    }

    [Fact]
    public void NenhumNumeroSeRepete()
    {
        foreach (var camada in Geracao.PorComprimento())
        {
            var textos = camada.Select(Regra.Texto).ToList();
            Assert.Equal(textos.Count, textos.Distinct().Count());
        }
    }

    [Fact]
    public void TodoNumeroDeUmaCamadaTemSeuPrefixoNaAnterior()
    {
        var camadas = Geracao.PorComprimento();
        for (var k = 2; k <= camadas.Count; k++)
        {
            var anteriores = camadas[k - 2].Select(Regra.Texto).ToHashSet();
            foreach (var numero in camadas[k - 1])
            {
                Assert.Contains(Regra.Texto(numero[..(k - 1)]), anteriores);
            }
        }
    }

    [Fact]
    public void AListaSecaSozinhaEEIssoQueProvaQueElesAcabam()
    {
        var camadas = Geracao.PorComprimento();
        Assert.Equal(25, camadas.Count);
        Assert.Single(camadas[^1]);
    }

    [Theory]
    [InlineData(2, 2)]
    [InlineData(3, 6)]
    [InlineData(5, 10)]
    [InlineData(10, 25)]
    [InlineData(12, 28)]
    public void EmCadaBaseAListaTambemSeca(int baseNumerica, int comprimento)
    {
        Assert.Equal(comprimento, Geracao.PorComprimento(baseNumerica).Count);
    }

    [Fact]
    public void ArgumentosInvalidosNaoSaoAceitos()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Geracao.MmcAte(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Geracao.PorComprimento(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Geracao.PorForcaBruta(0));
    }
}
