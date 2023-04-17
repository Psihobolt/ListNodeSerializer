``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22000.1817/21H2/SunValley)
Intel Core i9-10900KF CPU 3.70GHz, 1 CPU, 20 logical and 10 physical cores
.NET SDK=7.0.202
  [Host]            : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2 [AttachedDebugger]
  ShortRun-.NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2

Job=ShortRun-.NET 6.0  Runtime=.NET 6.0  IterationCount=3  
LaunchCount=1  WarmupCount=3  

```
|                  Method | Depth |         Mean |        Error |     StdDev |       Rank |     Gen0 |     Gen1 |     Gen2 |  Allocated |
|------------------------ |------ |-------------:|-------------:|-----------:|-----------:|---------:|---------:|---------:|-----------:|
| **SerializeWithRandomNode** |    **10** |    **441.23 μs** |    **54.347 μs** |   **2.979 μs** |        ******* |   **3.9063** |   **1.9531** |        **-** |   **41.77 KB** |
| SerializeAndDeserialize |    10 |  1,008.70 μs |   296.453 μs |  16.250 μs |      ***** |   7.8125 |   3.9063 |        - |   87.63 KB |
|                DeepCopy |    10 |     34.49 μs |     2.410 μs |   0.132 μs |          * |   1.4648 |        - |        - |   15.21 KB |
|      DeepCopyWithRandom |    10 |     34.68 μs |     1.598 μs |   0.088 μs |          * |   1.4648 |        - |        - |   15.23 KB |
| **SerializeWithRandomNode** |   **100** |    **651.75 μs** |    **16.702 μs** |   **0.915 μs** |       ******** |  **17.5781** |   **8.7891** |        **-** |  **182.24 KB** |
| SerializeAndDeserialize |   100 |  1,537.71 μs |   172.009 μs |   9.428 μs |     ****** |  37.1094 |  17.5781 |        - |  381.88 KB |
|                DeepCopy |   100 |    147.27 μs |     9.911 μs |   0.543 μs |         ** |  12.9395 |   1.4648 |        - |  133.82 KB |
|      DeepCopyWithRandom |   100 |    147.80 μs |     7.977 μs |   0.437 μs |         ** |  12.6953 |   1.4648 |        - |  131.58 KB |
| **SerializeWithRandomNode** |  **1000** |  **7,561.32 μs** |   **246.415 μs** |  **13.507 μs** |  ************* | **109.3750** | **109.3750** | **109.3750** |  **1455.7 KB** |
| SerializeAndDeserialize |  1000 | 11,519.49 μs | 2,264.023 μs | 124.099 μs | ********** | 328.1250 | 203.1250 | 109.3750 | 3278.28 KB |
|                DeepCopy |  1000 |  6,203.27 μs |   203.009 μs |  11.128 μs |   ******** | 125.0000 |  39.0625 |        - | 1284.73 KB |
|      DeepCopyWithRandom |  1000 |  6,071.39 μs |   236.730 μs |  12.976 μs |    ******* | 117.1875 |  39.0625 |        - | 1263.67 KB |
