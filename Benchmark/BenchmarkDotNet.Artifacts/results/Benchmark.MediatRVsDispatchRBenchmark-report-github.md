```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
13th Gen Intel Core i7-13700H, 1 CPU, 20 logical and 14 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method                  | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Gen2   | Allocated | Alloc Ratio |
|------------------------ |----------:|---------:|---------:|------:|--------:|-------:|-------:|-------:|----------:|------------:|
| SendPing_With_DispatchR |  87.02 ns | 1.795 ns | 2.794 ns |  0.79 |    0.07 | 0.0082 | 0.0001 | 0.0001 |         - |        0.00 |
| SendPing_With_MediatR   | 111.45 ns | 3.167 ns | 9.188 ns |  1.01 |    0.12 | 0.0111 |      - |      - |     360 B |        1.00 |
