``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-7500U CPU 2.70GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.1.101
  [Host]   : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  ShortRun : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
|                                          Method |  Size |          Mean |         Error |       StdDev |
|------------------------------------------------ |------ |--------------:|--------------:|-------------:|
|                              **StructのArrayをForする** |   **100** |      **74.64 ns** |      **8.891 ns** |     **0.487 ns** |
|                          StructのArrayをForEachする |   100 |      68.44 ns |      4.538 ns |     0.249 ns |
|                               ClassのArrayをForする |   100 |      68.72 ns |     12.905 ns |     0.707 ns |
|                 ClassのArrayをLengthを変数にしてからForする |   100 |      66.44 ns |     12.763 ns |     0.700 ns |
|                       ClassのArrayをIListとしてForする |   100 |     685.09 ns |     99.062 ns |     5.430 ns |
|                           ClassのArrayをForEachする |   100 |      49.11 ns |      4.631 ns |     0.254 ns |
|         ClassのArrayをstructのEnumeratorでForEachする |   100 |      54.71 ns |      2.176 ns |     0.119 ns |
|          ClassのArrayをclassのEnumeratorでForEachする |   100 |     398.73 ns |      8.796 ns |     0.482 ns |
|             ClassのArrayをIEnumerableとしてForEachする |   100 |     433.47 ns |     19.424 ns |     1.065 ns |
|            ClassのArrayをIEnumerableとしてLinqのSumする |   100 |     736.27 ns |     62.358 ns |     3.418 ns |
|      ClassのArrayをIEnumerableとしてLinqのAggregateする |   100 |     901.14 ns |    147.204 ns |     8.069 ns |
|                    ClassのArrayをSpanとしてForEachする |   100 |      61.55 ns |      4.301 ns |     0.236 ns |
|            ClassのArrayをReadOnlySpanとしてForEachする |   100 |      49.55 ns |      2.930 ns |     0.161 ns |
|                                ClassのListをForする |   100 |     127.65 ns |     10.469 ns |     0.574 ns |
|                   ClassのListをCountを変数にしてからForする |   100 |      73.82 ns |     11.831 ns |     0.648 ns |
|                        ClassのListをIListとしてForする |   100 |     420.59 ns |      6.100 ns |     0.334 ns |
|                            ClassのListをForEachする |   100 |     367.72 ns |     25.571 ns |     1.402 ns |
|              ClassのListをIEnumerableとしてForEachする |   100 |     985.42 ns |    101.737 ns |     5.577 ns |
|             ClassのListをIEnumerableとしてLinqのSumする |   100 |   1,078.04 ns |    263.858 ns |    14.463 ns |
|       ClassのListをIEnumerableとしてLinqのAggregateする |   100 |   1,216.19 ns |    171.576 ns |     9.405 ns |
|                      ClassのLinkedListをForEachする |   100 |     547.50 ns |     11.953 ns |     0.655 ns |
|        ClassのLinkedListをIEnumerableとしてForEachする |   100 |   1,189.37 ns |    239.077 ns |    13.105 ns |
|       ClassのLinkedListをIEnumerableとしてLinqのSumする |   100 |   2,032.85 ns |    424.415 ns |    23.264 ns |
| ClassのLinkedListをIEnumerableとしてLinqのAggregateする |   100 |   2,047.10 ns |    307.892 ns |    16.877 ns |
|              ClassのReadOnlyCollectionをForEachする |   100 |     437.24 ns |     24.204 ns |     1.327 ns |
|                              **StructのArrayをForする** | **10000** |   **6,466.50 ns** |    **654.845 ns** |    **35.894 ns** |
|                          StructのArrayをForEachする | 10000 |   5,901.82 ns |    387.405 ns |    21.235 ns |
|                               ClassのArrayをForする | 10000 |   8,880.62 ns |  1,074.791 ns |    58.913 ns |
|                 ClassのArrayをLengthを変数にしてからForする | 10000 |   6,992.25 ns |    899.263 ns |    49.292 ns |
|                       ClassのArrayをIListとしてForする | 10000 |  67,302.95 ns | 14,025.878 ns |   768.806 ns |
|                           ClassのArrayをForEachする | 10000 |   6,698.47 ns |    489.645 ns |    26.839 ns |
|         ClassのArrayをstructのEnumeratorでForEachする | 10000 |   6,979.54 ns |    102.272 ns |     5.606 ns |
|          ClassのArrayをclassのEnumeratorでForEachする | 10000 |  35,122.60 ns |  5,251.916 ns |   287.875 ns |
|             ClassのArrayをIEnumerableとしてForEachする | 10000 |  44,304.22 ns |  3,897.634 ns |   213.642 ns |
|            ClassのArrayをIEnumerableとしてLinqのSumする | 10000 |  64,850.25 ns |  6,449.042 ns |   353.494 ns |
|      ClassのArrayをIEnumerableとしてLinqのAggregateする | 10000 |  87,102.04 ns | 15,744.461 ns |   863.007 ns |
|                    ClassのArrayをSpanとしてForEachする | 10000 |   6,780.76 ns |    398.579 ns |    21.847 ns |
|            ClassのArrayをReadOnlySpanとしてForEachする | 10000 |   6,706.99 ns |  1,118.435 ns |    61.305 ns |
|                                ClassのListをForする | 10000 |  13,453.22 ns |    253.283 ns |    13.883 ns |
|                   ClassのListをCountを変数にしてからForする | 10000 |   8,949.99 ns |    566.920 ns |    31.075 ns |
|                        ClassのListをIListとしてForする | 10000 |  39,973.75 ns |  4,314.559 ns |   236.496 ns |
|                            ClassのListをForEachする | 10000 |  41,222.33 ns |  4,914.723 ns |   269.393 ns |
|              ClassのListをIEnumerableとしてForEachする | 10000 |  90,215.21 ns |  9,733.683 ns |   533.536 ns |
|             ClassのListをIEnumerableとしてLinqのSumする | 10000 | 104,294.21 ns |  7,147.570 ns |   391.782 ns |
|       ClassのListをIEnumerableとしてLinqのAggregateする | 10000 | 121,577.07 ns |  5,766.028 ns |   316.055 ns |
|                      ClassのLinkedListをForEachする | 10000 |  56,364.57 ns | 25,679.257 ns | 1,407.567 ns |
|        ClassのLinkedListをIEnumerableとしてForEachする | 10000 | 122,022.07 ns | 18,178.928 ns |   996.448 ns |
|       ClassのLinkedListをIEnumerableとしてLinqのSumする | 10000 | 189,806.31 ns | 37,113.128 ns | 2,034.296 ns |
| ClassのLinkedListをIEnumerableとしてLinqのAggregateする | 10000 | 210,220.60 ns | 15,803.656 ns |   866.252 ns |
|              ClassのReadOnlyCollectionをForEachする | 10000 |  41,003.46 ns |  5,479.109 ns |   300.328 ns |
