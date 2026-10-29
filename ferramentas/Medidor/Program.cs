using Conde.Polidivisivel;

if (args.Length == 0)
{
    Console.WriteLine("uso: medidor <comprimentos|bases|confronto|maior>");
    return 1;
}

switch (args[0])
{
    case "comprimentos":
        Comprimentos();
        break;
    case "bases":
        Bases();
        break;
    case "confronto":
        Confronto();
        break;
    case "maior":
        Maior();
        break;
    default:
        Console.WriteLine($"comando desconhecido: {args[0]}");
        return 1;
}

return 0;

static void Comprimentos()
{
    Console.WriteLine("Quantos numeros poli-divisiveis tem cada comprimento (base 10)");
    Console.WriteLine("k   quantos  candidatos  previsto   medido/previsto");
    foreach (var l in Medidas.PorComprimento())
    {
        Console.WriteLine($"{l.Comprimento,-4}{l.Quantos,-9}{l.Candidatos,-12}{l.Previsto,-11}{l.Razao}");
    }

    Console.WriteLine();
    Console.WriteLine($"total: {Medidas.Total()}");
}

static void Bases()
{
    Console.WriteLine("O mesmo problema em outras bases");
    Console.WriteLine("base  maior comprimento  total   o maior de todos");
    foreach (var l in Medidas.PorBase([2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 16]))
    {
        Console.WriteLine($"{l.Base_,-6}{l.MaiorComprimento,-18}{l.Total,-8}{l.Maior}");
    }
}

static void Confronto()
{
    Console.WriteLine("A geracao por crescimento contra a forca bruta (base 10)");
    Console.WriteLine("k   crescimento  forca bruta  numeros varridos pela bruta");
    foreach (var k in new[] { 1, 2, 3, 4, 5, 6, 7 })
    {
        var (crescimento, bruta, candidatos) = Medidas.Confronto(k);
        Console.WriteLine($"{k,-4}{crescimento,-13}{bruta,-13}{candidatos}");
    }
}

static void Maior()
{
    var (comprimento, quantos, maior) = Medidas.Maior();
    Console.WriteLine($"O maior numero poli-divisivel tem {comprimento} algarismos");
    Console.WriteLine($"no comprimento {comprimento} existem {quantos}");
    Console.WriteLine($"o maior deles: {maior}");
}
