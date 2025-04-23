```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
13th Gen Intel Core i7-13700H, 1 CPU, 20 logical and 14 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method                          | Mean     | Error   | StdDev   | Median   | Ratio | RatioSD | Gen0   | Gen1   | Gen2   | Allocated | Alloc Ratio |
|-------------------------------- |---------:|--------:|---------:|---------:|------:|--------:|-------:|-------:|-------:|----------:|------------:|
| SendPing_WithPipeline_DispatchR | 197.6 ns | 7.29 ns | 20.20 ns | 191.6 ns |  0.96 |    0.11 | 0.0100 |      - |      - |     480 B |          NA |
| SendPing_WithPipeline_MediatR   | 206.6 ns | 4.18 ns |  9.69 ns | 206.1 ns |  1.00 |    0.07 | 0.0191 | 0.0005 | 0.0005 |         - |          NA |
