using Conde.Polidivisivel;
using Xunit;

namespace Conde.Polidivisivel.Testes;

/// <summary>
/// Os números do README presos aqui. Só entra o que eu vi o medidor imprimir.
/// </summary>
public class MedidasTestes
{
    [Fact]
    public void OTotalEVinteMilQuatrocentosECinquentaESeis()
    {
        Assert.Equal(20456, Medidas.Total());
    }

    [Fact]
    public void OMaiorTemVinteECincoAlgarismosEEUnico()
    {
        var (comprimento, quantos, maior) = Medidas.Maior();
        Assert.Equal(25, comprimento);
        Assert.Equal(1, quantos);
        Assert.Equal("3608528850368400786036725", maior);
        Assert.True(Regra.Vale(maior.Select(c => c - '0').ToArray()));
    }

    [Theory]
    [InlineData(1, 9, 9.0, 1.0)]
    [InlineData(2, 45, 45.0, 1.0)]
    [InlineData(3, 150, 150.0, 1.0)]
    [InlineData(4, 375, 375.0, 1.0)]
    [InlineData(5, 750, 750.0, 1.0)]
    [InlineData(6, 1200, 1250.0, 0.96)]
    [InlineData(7, 1713, 1785.7, 0.959)]
    [InlineData(8, 2227, 2232.1, 0.998)]
    [InlineData(9, 2492, 2480.2, 1.005)]
    [InlineData(10, 2492, 2480.2, 1.005)]
    [InlineData(15, 770, 688.2, 1.119)]
    [InlineData(20, 44, 37.0, 1.189)]
    [InlineData(24, 3, 1.5, 2.068)]
    [InlineData(25, 1, 0.6, 1.723)]
    public void ATabelaPorComprimento(int k, long quantos, double previsto, double razao)
    {
        var l = Medidas.PorComprimento().Single(x => x.Comprimento == k);
        Assert.Equal(quantos, l.Quantos);
        Assert.Equal(previsto, l.Previsto);
        Assert.Equal(razao, l.Razao);
    }

    [Fact]
    public void APrevisaoIngenuaEExataAteOCincoENuncaMais()
    {
        var linhas = Medidas.PorComprimento();
        for (var k = 1; k <= 5; k++)
        {
            Assert.Equal(1.0, linhas[k - 1].Razao);
        }

        Assert.All(linhas.Skip(5), l => Assert.NotEqual(1.0, l.Razao));
    }

    [Fact]
    public void DoDozeEmDianteAPrevisaoSempreSubestima()
    {
        foreach (var l in Medidas.PorComprimento().Where(x => x.Comprimento >= 12))
        {
            Assert.True(l.Razao > 1.0);
        }
    }

    [Fact]
    public void OPicoEUmPlatoDeDois()
    {
        var linhas = Medidas.PorComprimento();
        var maximo = linhas.Max(l => l.Quantos);
        Assert.Equal(2492, maximo);
        var noPico = linhas.Where(l => l.Quantos == maximo).Select(l => l.Comprimento).ToList();
        Assert.Equal([9, 10], noPico);
    }

    [Fact]
    public void ASomaDaTabelaEOTotal()
    {
        Assert.Equal(20456, Medidas.PorComprimento().Sum(l => l.Quantos));
    }

    [Theory]
    [InlineData(2, 2, 2, "10")]
    [InlineData(3, 6, 15, "200220")]
    [InlineData(4, 7, 37, "2220301")]
    [InlineData(5, 10, 127, "4022042200")]
    [InlineData(6, 11, 323, "52323000003")]
    [InlineData(7, 18, 1067, "222255000033402606")]
    [InlineData(8, 17, 2568, "56147660642432202")]
    [InlineData(9, 22, 8380, "8831805708264668401762")]
    [InlineData(10, 25, 20456, "3608528850368400786036725")]
    [InlineData(12, 28, 148058, "606890346850ba6800b036206464")]
    public void ATabelaPorBase(int baseNumerica, int comprimento, long total, string maior)
    {
        var l = Medidas.PorBase([baseNumerica])[0];
        Assert.Equal(comprimento, l.MaiorComprimento);
        Assert.Equal(total, l.Total);
        Assert.Equal(maior, l.Maior);
    }

    [Fact]
    public void ABaseSeteVaiMaisLongeQueABaseOito()
    {
        var sete = Medidas.PorBase([7])[0];
        var oito = Medidas.PorBase([8])[0];
        Assert.True(sete.MaiorComprimento > oito.MaiorComprimento);
        Assert.True(sete.Total < oito.Total);
    }

    [Theory]
    [InlineData(1, 9, 10)]
    [InlineData(4, 375, 10000)]
    [InlineData(6, 1200, 1000000)]
    [InlineData(7, 1713, 10000000)]
    public void OConfrontoComAForcaBruta(int k, long quantos, long candidatos)
    {
        var (crescimento, bruta, varridos) = Medidas.Confronto(k);
        Assert.Equal(quantos, crescimento);
        Assert.Equal(quantos, bruta);
        Assert.Equal(candidatos, varridos);
    }

    [Fact]
    public void OCrescimentoTestaMuitoMenosCandidatos()
    {
        foreach (var l in Medidas.PorComprimento().Where(x => x.Comprimento >= 7))
        {
            Assert.True(l.Candidatos < Math.Pow(10, l.Comprimento));
        }
    }
}
