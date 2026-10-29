# Números poli-divisíveis

Um número é **poli-divisível** quando o pedaço formado pelos seus `k` primeiros
algarismos é divisível por `k`, para todo `k`. O primeiro algarismo é divisível por 1, os
dois primeiros formam um número par, os três primeiros são múltiplos de 3, e assim até o
fim.

`3608528850368400786036725` é um deles. Ele tem 25 algarismos, e **não existe nenhum
maior** — nem com 25 algarismos, nem com 26, nem com um milhão. São 20.456 no total, e
acabam.

```
dotnet test testes/Polidivisivel.Testes/Polidivisivel.Testes.csproj -c Release
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- maior
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- comprimentos
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- bases
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- confronto
```

## Como a lista seca sozinha

Todo prefixo de um número poli-divisível também é poli-divisível. Então a lista de
comprimento `k` sai da de comprimento `k−1` tentando acrescentar um algarismo, e a prova
de que eles acabam não é um argumento: é a lista do comprimento 26 **saindo vazia**.

Para testar "o pedaço de `k` algarismos é divisível por `k`" não é preciso guardar o
número, que chega a 25 algarismos. Basta guardar o pedaço **módulo o mínimo múltiplo
comum de 1 a 25**, porque todo `k` divide esse mínimo. Ele vale `26.771.144.400` e cabe
folgado num inteiro de 64 bits — o problema inteiro roda em aritmética de máquina, sem
número grande em lugar nenhum.

E a geração é conferida contra a força bruta, que monta cada número e aplica a definição.
Elas acham exatamente os mesmos 1.713 números de sete algarismos, com a diferença de que a
força bruta precisou olhar dez milhões para isso.

## Os achados

### 1. A previsão ingênua é exata nos cinco primeiros comprimentos e nunca mais

Existem `9 · 10^(k−1)` números de `k` algarismos. Se cada divisibilidade fosse um sorteio
independente de chance `1/j`, sobrariam `9 · 10^(k−1) / k!`.

```
k   quantos  previsto   medido/previsto
1   9        9          1
2   45       45         1
3   150      150        1
4   375      375        1
5   750      750        1
6   1200     1250       0.96
7   1713     1785.7     0.959
8   2227     2232.1     0.998
9   2492     2480.2     1.005
...
15  770      688.2      1.119
20  44       37         1.189
24  3        1.5        2.068
25  1        0.6        1.723
```

Até o comprimento 5 a conta acerta **exatamente** — 9, 45, 150, 375, 750 são os valores
certos, não aproximações. No 6 ela erra pela primeira vez, para menos. E a partir do 12 ela
passa a **subestimar sempre**, até prever metade do que existe no comprimento 24.

As condições de divisibilidade não são independentes, e a dependência não é neutra: ela
ajuda a sobrevivência. Um número que já passou por dez filtros tem mais chance de passar
pelo décimo primeiro do que um número qualquer teria.

### 2. O pico é um platô de dois

```
k     8     9     10    11
val   2227  2492  2492  2225
```

Os comprimentos 9 e 10 têm **o mesmo número** de poli-divisíveis: 2.492. Não é
arredondamento nem coincidência de tabela — são os dois iguais, e é ali que a quantidade
para de crescer. Depois disso cai sem voltar.

### 3. O maior é único, e a escassez chega de repente

No comprimento 24 existem **três** números. No 25, **um**. No 26, nenhum. O último
sobrevivente é `3608528850368400786036725`.

Não é um estreitamento suave: do comprimento 20 (44 números) ao 25 há um fator de 44, e
quase todo ele está nos últimos quatro passos.

### 4. A base 7 chega mais longe que a base 8

```
base  maior comprimento  total   o maior de todos
5     10                127     4022042200
6     11                323     52323000003
7     18                1067    222255000033402606
8     17                2568    56147660642432202
9     22                8380    8831805708264668401762
10    25                20456   3608528850368400786036725
12    28                148058  606890346850ba6800b036206464
16    39                8407789 fae06678c2e884607eb8b4e0b0a0f0603420342
```

O comprimento máximo não acompanha a base de forma monótona: a base 7 vai até 18
algarismos e a base 8 só até 17, mesmo tendo mais que o dobro de números
poli-divisíveis no total (2.568 contra 1.067).

Mais algarismos disponíveis é mais números, mas não necessariamente números mais
compridos. O que decide o comprimento é como os fatores da base se encaixam com os
divisores exigidos — e 8, sendo uma potência de 2, cobra uma divisibilidade por 8 muito
cedo e muito cara.

## O que as medidas me corrigiram

- **Eu ia guardar o número inteiro.** A primeira ideia foi carregar o valor em
  `BigInteger` e testar `valor % k`. Funciona e é lento à toa: guardando o pedaço módulo o
  mínimo múltiplo comum de 1 a 25 o problema inteiro cabe em `long`, porque todo divisor
  que eu preciso testar divide esse módulo. O número grande aparece uma vez só, no juiz,
  que é lento de propósito.

- **Eu esperava que a previsão errasse para cima.** Um filtro atrás do outro parece coisa
  que mata mais do que a conta diz. Mede-se o contrário: depois do comprimento 11 sobra
  sempre **mais** do que o previsto, até o dobro. Eu tinha a direção do erro errada, não só
  o tamanho.

- **"O pico está no 9" estava no rascunho.** Está no 9 e no 10, com o mesmo valor. Um pico
  que é um platô de dois é uma frase diferente, e eu só reparei conferindo a coluna duas
  vezes porque 2.492 repetido me pareceu erro de impressão.

- **Eu ia escrever que base maior vai sempre mais longe.** A base 7 passa da base 8. A
  tabela por base entrou no repositório por causa disso: era para ser uma curiosidade e
  virou o lugar onde a minha expectativa quebrou.

## Estrutura

```
src/Polidivisivel/
  Regra.cs     a definicao, montando o numero inteiro: a resposta de referencia
  Geracao.cs   o crescimento por prefixo, com o resto modulo o mmc, e a forca bruta
  Medidas.cs   as tabelas deste README
testes/Polidivisivel.Testes/
ferramentas/Medidor/
```

80 testes.
