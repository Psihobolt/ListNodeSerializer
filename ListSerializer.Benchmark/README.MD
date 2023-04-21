``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22000.1817/21H2/SunValley)
Intel Core i9-10900KF CPU 3.70GHz, 1 CPU, 20 logical and 10 physical cores
.NET SDK=7.0.202
  [Host]            : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2 [AttachedDebugger]
  ShortRun-.NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2

Job=ShortRun-.NET 6.0  Runtime=.NET 6.0  IterationCount=3  
LaunchCount=1  WarmupCount=3  

```
|                  Method |  Depth |          Mean |       Error |     StdDev |    Rank |       Gen0 |      Gen1 |      Gen2 |    Allocated |
|------------------------ |------- |--------------:|------------:|-----------:|--------:|-----------:|----------:|----------:|-------------:|
| **SerializeWithRandomNode** |   **1000** |      **5.491 ms** |   **0.2870 ms** |  **0.0157 ms** |      ****** |    **85.9375** |   **39.0625** |         **-** |    **892.73 KB** |
| SerializeAndDeserialize |   1000 |      5.750 ms |   0.1596 ms |  0.0087 ms |    **** |   125.0000 |   39.0625 |         - |   1329.46 KB |
|                DeepCopy |   1000 |      5.235 ms |   0.2526 ms |  0.0138 ms |       * |   101.5625 |   31.2500 |         - |   1044.36 KB |
|      DeepCopyWithRandom |   1000 |      5.610 ms |   0.4302 ms |  0.0236 ms |     *** |   101.5625 |   31.2500 |         - |   1044.36 KB |
| **SerializeWithRandomNode** |  **10000** |    **500.754 ms** |  **71.0731 ms** |  **3.8958 ms** |  ********** |          **-** |         **-** |         **-** |   **9709.81 KB** |
| SerializeAndDeserialize |  10000 |    502.610 ms | 103.3094 ms |  5.6627 ms |  ****** |  1000.0000 | 1000.0000 | 1000.0000 |  14090.22 KB |
|                DeepCopy |  10000 |    498.559 ms |  36.5019 ms |  2.0008 ms |  ****** |          - |         - |         - |  10378.33 KB |
|      DeepCopyWithRandom |  10000 |    488.294 ms |  29.9724 ms |  1.6429 ms |   ***** |          - |         - |         - |  10378.33 KB |
| **SerializeWithRandomNode** | **100000** | **49,551.843 ms** | **187.6294 ms** | **10.2846 ms** | *********** |  **7000.0000** | **3000.0000** | **1000.0000** |  **91540.08 KB** |
| SerializeAndDeserialize | 100000 | 49,649.778 ms |  60.8943 ms |  3.3378 ms | ******* | 10000.0000 | 4000.0000 | 1000.0000 | 133867.05 KB |
|                DeepCopy | 100000 | 49,404.944 ms | 593.9345 ms | 32.5556 ms | ******* |  9000.0000 | 4000.0000 | 1000.0000 | 101370.62 KB |
|      DeepCopyWithRandom | 100000 | 49,519.110 ms | 250.1114 ms | 13.7094 ms | ******* |  9000.0000 | 4000.0000 | 1000.0000 | 101370.62 KB |
