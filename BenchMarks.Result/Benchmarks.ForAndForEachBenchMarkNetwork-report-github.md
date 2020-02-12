``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-7500U CPU 2.70GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]   : .NET Framework 4.8 (4.8.4121.0), X64 RyuJIT
  ShortRun : .NET Framework 4.8 (4.8.4121.0), X64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
|                                          Method |  Size |          Mean |          Error |        StdDev |
|------------------------------------------------ |------ |--------------:|---------------:|--------------:|
|                              **StructのArrayをForする** |   **100** |      **73.79 ns** |       **6.834 ns** |      **0.375 ns** |
|                          StructのArrayをForEachする |   100 |      68.24 ns |      10.660 ns |      0.584 ns |
|                               ClassのArrayをForする |   100 |      68.19 ns |       4.266 ns |      0.234 ns |
|                 ClassのArrayをLengthを変数にしてからForする |   100 |      66.26 ns |      11.937 ns |      0.654 ns |
|                       ClassのArrayをIListとしてForする |   100 |     714.70 ns |      65.731 ns |      3.603 ns |
|                           ClassのArrayをForEachする |   100 |      48.13 ns |       5.393 ns |      0.296 ns |
|         ClassのArrayをstructのEnumeratorでForEachする |   100 |      55.42 ns |       3.296 ns |      0.181 ns |
|          ClassのArrayをclassのEnumeratorでForEachする |   100 |     432.24 ns |      43.267 ns |      2.372 ns |
|             ClassのArrayをIEnumerableとしてForEachする |   100 |     525.94 ns |     110.310 ns |      6.046 ns |
|            ClassのArrayをIEnumerableとしてLinqのSumする |   100 |     703.06 ns |      77.244 ns |      4.234 ns |
|      ClassのArrayをIEnumerableとしてLinqのAggregateする |   100 |     904.63 ns |      31.463 ns |      1.725 ns |
|                                ClassのListをForする |   100 |     129.99 ns |      24.155 ns |      1.324 ns |
|                   ClassのListをCountを変数にしてからForする |   100 |     101.64 ns |       1.073 ns |      0.059 ns |
|                        ClassのListをIListとしてForする |   100 |     508.60 ns |      94.545 ns |      5.182 ns |
|                            ClassのListをForEachする |   100 |     790.89 ns |      16.970 ns |      0.930 ns |
|              ClassのListをIEnumerableとしてForEachする |   100 |     872.73 ns |      99.739 ns |      5.467 ns |
|             ClassのListをIEnumerableとしてLinqのSumする |   100 |   1,095.02 ns |     113.617 ns |      6.228 ns |
|       ClassのListをIEnumerableとしてLinqのAggregateする |   100 |   1,293.88 ns |     168.906 ns |      9.258 ns |
|                      ClassのLinkedListをForEachする |   100 |     520.29 ns |      85.065 ns |      4.663 ns |
|        ClassのLinkedListをIEnumerableとしてForEachする |   100 |   1,125.41 ns |     213.632 ns |     11.710 ns |
|       ClassのLinkedListをIEnumerableとしてLinqのSumする |   100 |   1,946.09 ns |     478.802 ns |     26.245 ns |
| ClassのLinkedListをIEnumerableとしてLinqのAggregateする |   100 |   2,095.66 ns |     171.170 ns |      9.382 ns |
|              ClassのReadOnlyCollectionをForEachする |   100 |     525.42 ns |     119.863 ns |      6.570 ns |
|                              **StructのArrayをForする** | **10000** |   **6,453.47 ns** |     **292.089 ns** |     **16.010 ns** |
|                          StructのArrayをForEachする | 10000 |   5,873.84 ns |     437.294 ns |     23.970 ns |
|                               ClassのArrayをForする | 10000 |   8,801.67 ns |   1,267.650 ns |     69.484 ns |
|                 ClassのArrayをLengthを変数にしてからForする | 10000 |   6,893.42 ns |     528.031 ns |     28.943 ns |
|                       ClassのArrayをIListとしてForする | 10000 |  70,175.49 ns |  10,113.918 ns |    554.378 ns |
|                           ClassのArrayをForEachする | 10000 |   6,650.37 ns |     242.032 ns |     13.267 ns |
|         ClassのArrayをstructのEnumeratorでForEachする | 10000 |   6,963.43 ns |     504.784 ns |     27.669 ns |
|          ClassのArrayをclassのEnumeratorでForEachする | 10000 |  41,098.03 ns |   7,368.181 ns |    403.875 ns |
|             ClassのArrayをIEnumerableとしてForEachする | 10000 |  49,768.24 ns |   3,802.246 ns |    208.414 ns |
|            ClassのArrayをIEnumerableとしてLinqのSumする | 10000 |  64,229.13 ns |   8,532.903 ns |    467.717 ns |
|      ClassのArrayをIEnumerableとしてLinqのAggregateする | 10000 |  84,298.20 ns |   8,636.783 ns |    473.411 ns |
|                                ClassのListをForする | 10000 |  13,307.49 ns |   2,222.305 ns |    121.812 ns |
|                   ClassのListをCountを変数にしてからForする | 10000 |  11,106.81 ns |   1,261.346 ns |     69.139 ns |
|                        ClassのListをIListとしてForする | 10000 |  49,547.58 ns |   5,231.245 ns |    286.742 ns |
|                            ClassのListをForEachする | 10000 |  35,573.84 ns |   7,244.966 ns |    397.121 ns |
|              ClassのListをIEnumerableとしてForEachする | 10000 |  84,992.79 ns |  10,725.170 ns |    587.883 ns |
|             ClassのListをIEnumerableとしてLinqのSumする | 10000 | 103,047.72 ns |   6,276.928 ns |    344.060 ns |
|       ClassのListをIEnumerableとしてLinqのAggregateする | 10000 | 122,063.88 ns |  12,958.177 ns |    710.281 ns |
|                      ClassのLinkedListをForEachする | 10000 |  49,567.57 ns |   2,622.164 ns |    143.730 ns |
|        ClassのLinkedListをIEnumerableとしてForEachする | 10000 | 108,081.54 ns |   4,091.658 ns |    224.278 ns |
|       ClassのLinkedListをIEnumerableとしてLinqのSumする | 10000 | 365,100.84 ns | 222,168.323 ns | 12,177.795 ns |
| ClassのLinkedListをIEnumerableとしてLinqのAggregateする | 10000 | 386,191.88 ns | 211,390.282 ns | 11,587.015 ns |
|              ClassのReadOnlyCollectionをForEachする | 10000 |  86,352.87 ns | 341,743.392 ns | 18,732.108 ns |
