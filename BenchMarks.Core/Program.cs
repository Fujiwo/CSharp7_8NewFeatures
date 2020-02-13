//#define Core

using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Columns;
//using BenchmarkDotNet.Configs;
//using BenchmarkDotNet.Diagnosers;
//using BenchmarkDotNet.Environments;
//using BenchmarkDotNet.Exporters;
//using BenchmarkDotNet.Jobs;
//using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using System;
using System.Collections;
//using BenchmarkDotNet.Toolchains.CsProj;
//using BenchmarkDotNet.Toolchains.DotNetCli;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Benchmarks
{
    [ShortRunJob]
    [HtmlExporter]
    [CsvExporter]
    public class StackAllocBenchMark
    {
        [Params(100, 10000)]
        public int Size = 10000;

        [Benchmark]
        public int 配列をnewする()
        {
            var array = new byte[Size];
            for (var index = 0; index < Size; index++)
                array[index] = 0;
            var sum = 0;
            for (var index = 0; index < array.Length; index++)
                sum += array[index];
            return sum;
        }

        [Benchmark]
        public unsafe int 配列をstackallocしてポインターで受ける()
        {
            unsafe {
                int* array = stackalloc int[Size];
                for (var index = 0; index < Size; index++)
                    array[index] = 0;
                var sum = 0;
                for (var index = 0; index < Size; index++)
                    sum += array[index];
                return sum;
            }
        }

#if Core
        [Benchmark]
        public int 配列をstackallocしてSpanで受ける()
        {
            Span<byte> array = stackalloc byte[Size];
            for (var index = 0; index < Size; index++)
                array[index] = 0;
            var sum = 0;
            for (var index = 0; index < array.Length; index++)
                sum += array[index];
            return sum;
        }
#endif // Core
    }

    [ShortRunJob]
    [HtmlExporter]
    [CsvExporter]
    public class ボックス化BenchMark
    {
        [GlobalSetup]
        public void Setup() {}

        [Params(100, 10000)]
        public int Size = 10000;

        interface IValuable
        {
            int GetValue();
        }

        struct FooStruct : IValuable
        {
            public int Id { get; set; }
            public int Value { get; set; }

            public int GetValue() => Value;
        }

        [Benchmark]
        public int 非ジェネリック版Collection()
        {
            var list = new ArrayList();
            for (var count = 0; count < Size; count++)
                list.Add(new FooStruct { Id = 0, Value = 0 });
            var sum = 0;
            for (var index = 0; index < list.Count; index++)
                sum += ((FooStruct)list[index]).Value;
            return sum;
        }

        [Benchmark]
        public int ジェネリック版Collection()
        {
            var list = new List<FooStruct>();
            for (var count = 0; count < Size; count++)
                list.Add(new FooStruct { Id = 0, Value = 0 });
            var sum = 0;
            for (var index = 0; index < list.Count; index++)
                sum += list[index].Value;
            return sum;
        }

        enum StoneState { None, White, Black }

        static int Enumを受け取る非ジェネリック版(Enum value) => Convert.ToInt32(value);
        static int Enumを受け取るジェネリック版<T>(T value) where T : Enum => Convert.ToInt32(value);

        [Benchmark]
        public int Enumを渡す非ジェネリック版()
        {
            var sum = 0;
            for (var count = 0; count < Size; count++)
                sum += Enumを受け取る非ジェネリック版(StoneState.None);
            return sum;
        }

        [Benchmark]
        public int Enumを渡すジェネリック版()
        {
            var sum = 0;
            for (var count = 0; count < Size; count++)
                sum += Enumを受け取るジェネリック版(StoneState.None);
            return sum;
        }

        static int interfaceを受け取る非ジェネリック版(IValuable item) => item.GetValue();
        static int interfaceを受け取るジェネリック版<T>(T item) where T : IValuable => item.GetValue();

        [Benchmark]
        public int interfaceを渡す非ジェネリック版()
        {
            var item = new FooStruct { Id = 0, Value = 0 };
            var sum = 0;
            for (var count = 0; count < Size; count++)
                sum += interfaceを受け取る非ジェネリック版(item);
            return sum;
        }

        [Benchmark]
        public int interfaceを渡すジェネリック版()
        {
            var item = new FooStruct { Id = 0, Value = 0 };
            var sum = 0;
            for (var count = 0; count < Size; count++)
                sum += interfaceを受け取るジェネリック版(item);
            return sum;
        }

#if DEBUG
        public static void Test()
        {
            var ボックス化BenchMark = new ボックス化BenchMark();
            ボックス化BenchMark.Setup();

            var tests = new Action[] {
                () => ボックス化BenchMark.非ジェネリック版Collection(),
                () => ボックス化BenchMark.ジェネリック版Collection(),
                () => ボックス化BenchMark.Enumを渡す非ジェネリック版(),
                () => ボックス化BenchMark.Enumを渡すジェネリック版(),
                () => ボックス化BenchMark.interfaceを渡す非ジェネリック版(),
                () => ボックス化BenchMark.interfaceを渡すジェネリック版()
            };
            foreach (var test in tests)
                test();
        }
#endif // DEBUG
    }

    [ShortRunJob]
    [HtmlExporter]
    [CsvExporter]
    public class ReadOnlyStructBenchMark
    {
        [GlobalSetup]
        public void Setup() {}

        const int 回数 = 1000;

        struct FooStruct
        {
            int id;
            int value;

            public int GetValue() => value;
        }

        readonly struct ReadOnlyFooStruct
        {
            readonly int id;
            readonly int value;

            public readonly int GetValue() => value;
        }

        int structを使うメソッド(in FooStruct foo) => foo.GetValue();
        int readonlyなstructを使うメソッド(in ReadOnlyFooStruct foo) => foo.GetValue();

        [Benchmark]
        public int structを使う()
        {
            var item = new FooStruct();
            var sum = 0;
            for (var count = 0; count < 回数; count++)
                sum += structを使うメソッド(item);
            return sum;
        }

        [Benchmark]
        public int readonlyなstructを使う()
        {
            var item = new ReadOnlyFooStruct();
            var sum = 0;
            for (var count = 0; count < 回数; count++)
                sum += readonlyなstructを使うメソッド(item);
            return sum;
        }
    }

#if Core
    [ShortRunJob]
    [HtmlExporter]
    [CsvExporter]
    public class StringCreateBenchMark
    {
        [GlobalSetup]
        public void Setup() { }

        const int 回数 = 1000;
        [Params(10, 100, 1000)]
        public int Length = 10;

        string バッファをnewして文字列を作る(char character)
        {
            var buffer = new char[Length];
            for (var index = 0; index < buffer.Length; index++)
                buffer[index] = (char)(character + index);
            return new string(buffer);
        }

        string バッファをnewしないで文字列を作る_キャプチャーされる(char character)
            =>  string.Create(Length, character, (buffer, state) => {
                for (var index = 0; index < buffer.Length; index++)
                    buffer[index] = (char)(character + index);
            });

        string バッファをnewしないで文字列を作る_キャプチャーされない(char character)
            => string.Create(Length, character, (buffer, state) => {
                for (var index = 0; index < buffer.Length; index++)
                    buffer[index] = (char)(state + index);
            });

        [Benchmark]
        public int バッファをnewして文字列を作るテスト()
        {
            var sum = 0;
            for (var count = 0; count < 回数; count++)
                sum += バッファをnewして文字列を作る('A').Length;
            return sum;
        }

        [Benchmark]
        public int バッファをnewしないで文字列を作る_キャプチャーされるテスト()
        {
            var sum = 0;
            for (var count = 0; count < 回数; count++)
                sum += バッファをnewしないで文字列を作る_キャプチャーされる('A').Length;
            return sum;
        }

        [Benchmark]
        public int バッファをnewしないで文字列を作る_キャプチャーされないテスト()
        {
            var sum = 0;
            for (var count = 0; count < 回数; count++)
                sum += バッファをnewしないで文字列を作る_キャプチャーされない('A').Length;
            return sum;
        }
    }
#endif // Core

    //[SimpleJob(RuntimeMoniker.Net48, baseline: true)]
    //[SimpleJob(RuntimeMoniker.NetCoreApp31)]
    //[SimpleJob(launchCount: 1, warmupCount: 3, targetCount: 5, invocationCount: 100, id: "QuickJob")]
    [ShortRunJob]
    [HtmlExporter]
    [CsvExporter]
#if Core
    public class ForAndForEachBenchMarkCore
#else // Core
    public class ForAndForEachBenchMarkNetwork
#endif // Core
    {
        public struct StructEnumeratorArray<T>
        {
            readonly T[] Array;
            public StructEnumeratorArray(T[] array) => Array = array;

            public Enumerator GetEnumerator() => new Enumerator(Array);

            public struct Enumerator : IEnumerator<T>
            {
                private readonly T[] array;
                private int index;

                internal Enumerator(T[] array) => (this.array, index) = (array, -1);

                public T Current => array[index];
                object? IEnumerator.Current => Current;
                public bool MoveNext() => ((uint)++index) < (uint)array.Length;
                public void Dispose() { }
                public void Reset() => index = -1;
            }
        }

        public struct ClassEnumeratorArray<T> : IEnumerable<T>
        {
            readonly T[] Array;
            public ClassEnumeratorArray(T[] array) => Array = array;

            public IEnumerator<T> GetEnumerator() => new Enumerator(Array);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public class Enumerator : IEnumerator<T>
            {
                private readonly T[] array;
                private int index;

                internal Enumerator(T[] array) => (this.array, index) = (array, -1);

                public T Current => array[index];
                object? IEnumerator.Current => Current;
                public bool MoveNext() => ((uint)++index) < (uint)array.Length;
                public void Dispose() { }
                public void Reset() => index = -1;
            }
        }

        class FooClass
        {
            public int Id { get; set; } = 0;
            public int Value { get; set; } = 1;
        }

        struct FooStruct
        {
            public int Id { get; set; }
            public int Value { get; set; }
        }

        // ■ StructのArray
        FooStruct[]? structArray = null;

        // ■ ClassのArray
        FooClass[]? classArray = null;
        IEnumerable<FooClass>? iEnumerableOfClassArray = null;
        IList<FooClass>? iListOfClassArray = null;
        StructEnumeratorArray<FooClass>? structEnumeratorClassArray = null;
        ClassEnumeratorArray<FooClass>? classEnumeratorClassArray = null;

        // ■ ClassのList
        List<FooClass>? classList = null;
        IEnumerable<FooClass>? iEnumerableOfClassList = null;
        IList<FooClass>? iListOfClassList = null;

        // ■ ClassのLinkedList
        LinkedList<FooClass>? classLinkedList = null;
        IEnumerable<FooClass>? iEnumerableOfClassLinkedList = null;

        // ■ ClassのReadOnlyCollection
        ReadOnlyCollection<FooClass>? classReadOnlyCollection = null;

        [Params(100, 10000)]
        public int Size = 10000;

        [GlobalSetup]
        public void Setup()
        {
            // ■ StructのArray
            structArray = Enumerable.Range(0, Size).Select(value => new FooStruct { Value = value }).ToArray();

            // ■ ClassのArray
            classArray = Enumerable.Range(0, Size).Select(value => new FooClass { Value = value }).ToArray();
            iEnumerableOfClassArray =
            iListOfClassArray = classArray;
            structEnumeratorClassArray = new StructEnumeratorArray<FooClass>(classArray);
            classEnumeratorClassArray = new ClassEnumeratorArray<FooClass>(classArray);

            // ■ ClassのList
            classList = classArray.ToList();
            iEnumerableOfClassList = iListOfClassList = classList;

            // ■ ClassのLinkedList
            classLinkedList = new LinkedList<FooClass>(classArray);
            iEnumerableOfClassLinkedList = classLinkedList;

            // ■ ClassのReadOnlyCollection
            classReadOnlyCollection = new ReadOnlyCollection<FooClass>(classArray);
        }

        // ■ StructのArray
        // ○ for

        [Benchmark]
        public int StructのArrayをForする()
        {
            var sum = 0;
            for (var index = 0; index < structArray.Length; index++)
                sum += structArray[index].Value;
            return sum;
        }

        // ○ foreach

        [Benchmark]
        public int StructのArrayをForEachする()
        {
            var sum = 0;
            foreach (var element in structArray)
                sum += element.Value;
            return sum;
        }

        // ■ ClassのArray
        // ○ for

        [Benchmark]
        public int ClassのArrayをForする()
        {
            var sum = 0;
            for (var index = 0; index < classArray.Length; index++)
                sum += classArray[index].Value;
            return sum;
        }

        [Benchmark]
        public int ClassのArrayをLengthを変数にしてからForする()
        {
            var sum = 0;
            var count = classArray.Length;
            for (var index = 0; index < count; index++)
                sum += classArray[index].Value;
            return sum;
        }

        [Benchmark]
        public int ClassのArrayをIListとしてForする()
        {
            var sum = 0;
            for (var index = 0; index < iListOfClassArray.Count; index++)
                sum += iListOfClassArray[index].Value;
            return sum;
        }

        // ○ foreach

        [Benchmark]
        public int ClassのArrayをForEachする()
        {
            var sum = 0;
            foreach (var element in classArray)
                sum += element.Value;
            return sum;
        }

        [Benchmark]
        public int ClassのArrayをstructのEnumeratorでForEachする()
        {
            var sum = 0;
            foreach (var element in structEnumeratorClassArray)
                sum += element.Value;
            return sum;
        }

        [Benchmark]
        public int ClassのArrayをclassのEnumeratorでForEachする()
        {
            var sum = 0;
            foreach (var element in classEnumeratorClassArray)
                sum += element.Value;
            return sum;
        }

        [Benchmark]
        public int ClassのArrayをIEnumerableとしてForEachする()
        {
            var sum = 0;
            foreach (var element in iEnumerableOfClassArray)
                sum += element.Value;
            return sum;
        }

        // ○ Linq

        [Benchmark]
        public int ClassのArrayをIEnumerableとしてLinqのSumする()
            => iEnumerableOfClassArray.Select(element => element.Value).Sum();

        [Benchmark]
        public int ClassのArrayをIEnumerableとしてLinqのAggregateする()
            => iEnumerableOfClassArray.Select(element => element.Value).Aggregate(0, (sum, next) => sum += next);

#if Core
        [Benchmark]
        public int ClassのArrayをSpanとしてForEachする()
        {
            var sum = 0;
            foreach (var element in (Span<FooClass>)classArray)
                sum += element.Value;
            return sum;
        }

        [Benchmark]
        public int ClassのArrayをReadOnlySpanとしてForEachする()
        {
            var sum = 0;
            foreach (var element in (ReadOnlySpan<FooClass>)classArray)
                sum += element.Value;
            return sum;
        }
#endif // Core

        // ■ ClassのList
        // ○ for

        [Benchmark]
        public int ClassのListをForする()
        {
            var sum = 0;
            for (var index = 0; index < classList.Count; index++)
                sum += classList[index].Value;
            return sum;
        }

        [Benchmark]
        public int ClassのListをCountを変数にしてからForする()
        {
            var sum = 0;
            var count = classList.Count;
            for (var index = 0; index < count; index++)
                sum += classList[index].Value;
            return sum;
        }

        [Benchmark]
        public int ClassのListをIListとしてForする()
        {
            var sum = 0;
            for (var index = 0; index < iListOfClassList.Count; index++)
                sum += iListOfClassList[index].Value;
            return sum;
        }

        // ○ foreach

        [Benchmark]
        public int ClassのListをForEachする()
        {
            var sum = 0;
            foreach (var element in classList)
                sum += element.Value;
            return sum;
        }

        [Benchmark]
        public int ClassのListをIEnumerableとしてForEachする()
        {
            var sum = 0;
            foreach (var element in iEnumerableOfClassList)
                sum += element.Value;
            return sum;
        }

        // ○ Linq

        [Benchmark]
        public int ClassのListをIEnumerableとしてLinqのSumする()
            => iEnumerableOfClassList.Select(element => element.Value).Sum();

        [Benchmark]
        public int ClassのListをIEnumerableとしてLinqのAggregateする()
            => iEnumerableOfClassList.Select(element => element.Value).Aggregate(0, (sum, next) => sum += next);

        // ■ ClassのLinkedList
        // ○ foreach

        [Benchmark]
        public int ClassのLinkedListをForEachする()
        {
            var sum = 0;
            foreach (var element in classLinkedList)
                sum += element.Value;
            return sum;
        }

        [Benchmark]
        public int ClassのLinkedListをIEnumerableとしてForEachする()
        {
            var sum = 0;
            foreach (var element in iEnumerableOfClassLinkedList)
                sum += element.Value;
            return sum;
        }

        // ○ Linq

        [Benchmark]
        public int ClassのLinkedListをIEnumerableとしてLinqのSumする()
            => iEnumerableOfClassLinkedList.Select(element => element.Value).Sum();

        [Benchmark]
        public int ClassのLinkedListをIEnumerableとしてLinqのAggregateする()
            => iEnumerableOfClassLinkedList.Select(element => element.Value).Aggregate(0, (sum, next) => sum += next);

        // ■ ClassのReadOnlyCollection
        // ○ foreach

        [Benchmark]
        public int ClassのReadOnlyCollectionをForEachする()
        {
            var sum = 0;
            foreach (var element in classReadOnlyCollection)
                sum += element.Value;
            return sum;
        }

#if DEBUG
        public static void Test()
        {
#if Core
            var forAndForEachBenchMark = new ForAndForEachBenchMarkCore();
#else // Core
            var forAndForEachBenchMark = new ForAndForEachBenchMarkNetwork();
#endif // Core
            forAndForEachBenchMark.Setup();

            var tests = new Func<int>[] {
                forAndForEachBenchMark.StructのArrayをForする,
                forAndForEachBenchMark.StructのArrayをForEachする,
                forAndForEachBenchMark.ClassのArrayをForする,
                forAndForEachBenchMark.ClassのArrayをLengthを変数にしてからForする,
                forAndForEachBenchMark.ClassのArrayをIListとしてForする,
                forAndForEachBenchMark.ClassのArrayをForEachする,
                forAndForEachBenchMark.ClassのArrayをstructのEnumeratorでForEachする,
                forAndForEachBenchMark.ClassのArrayをclassのEnumeratorでForEachする,
                forAndForEachBenchMark.ClassのArrayをIEnumerableとしてForEachする,
                forAndForEachBenchMark.ClassのArrayをIEnumerableとしてLinqのSumする,
                forAndForEachBenchMark.ClassのArrayをIEnumerableとしてLinqのAggregateする,
#if Core
                forAndForEachBenchMark.ClassのArrayをSpanとしてForEachする,
                forAndForEachBenchMark.ClassのArrayをReadOnlySpanとしてForEachする,
#endif // Core
                forAndForEachBenchMark.ClassのListをForする,
                forAndForEachBenchMark.ClassのListをCountを変数にしてからForする,
                forAndForEachBenchMark.ClassのListをIListとしてForする,
                forAndForEachBenchMark.ClassのListをForEachする,
                forAndForEachBenchMark.ClassのListをIEnumerableとしてForEachする,
                forAndForEachBenchMark.ClassのListをIEnumerableとしてLinqのSumする,
                forAndForEachBenchMark.ClassのListをIEnumerableとしてLinqのAggregateする,
                forAndForEachBenchMark.ClassのLinkedListをForEachする,
                forAndForEachBenchMark.ClassのLinkedListをIEnumerableとしてForEachする,
                forAndForEachBenchMark.ClassのLinkedListをIEnumerableとしてLinqのSumする,
                forAndForEachBenchMark.ClassのLinkedListをIEnumerableとしてLinqのAggregateする,
                forAndForEachBenchMark.ClassのReadOnlyCollectionをForEachする
            };
            var sums = tests.Select(test => test()).ToArray();
            var sumGroup = sums.GroupBy(value => value).ToArray();
            Debug.Assert(sumGroup.Count() == 1);
        }
#endif // DEBUG
    }




    [ShortRunJob]
    [HtmlExporter]
    [CsvExporter]
    public class 値型と参照型BenchMark
    {
        //[Params(100, 10000)]
        //public int Size = 10000;

        struct StructPoint8B
        {
            int x, y;

            public StructPoint8B(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public StructPoint8B 値渡し値返しPlus(StructPoint8B another) => new StructPoint8B(x + another.x, y + another.y);
            public StructPoint8B 参照渡し値返しPlus(in StructPoint8B another) => new StructPoint8B(x + another.x, y + another.y);

            public static StructPoint8B 値渡し値返しPlusEqual(ref StructPoint8B point1, StructPoint8B point2)
            {
                point1.x += point2.x;
                point1.y += point2.y;
                return point1;
            }
            public static StructPoint8B 参照渡し値返しPlusEqual(ref StructPoint8B point1, in StructPoint8B point2)
            {
                point1.x += point2.x;
                point1.y += point2.y;
                return point1;
            }
            public static ref StructPoint8B 参照渡し参照返しPlusEqual(ref StructPoint8B point1, in StructPoint8B point2)
            {
                point1.x += point2.x;
                point1.y += point2.y;
                return ref point1;
            }
        }

        readonly struct ReadOnlyStructPoint8B
        {
            readonly int x, y;

            public ReadOnlyStructPoint8B(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public ReadOnlyStructPoint8B 値渡し値返しPlus(ReadOnlyStructPoint8B another) => new ReadOnlyStructPoint8B(x + another.x, y + another.y);
            public ReadOnlyStructPoint8B 参照渡し値返しPlus(in ReadOnlyStructPoint8B another) => new ReadOnlyStructPoint8B(x + another.x, y + another.y);
        }

        class ClassPoint8B
        {
            int x, y;

            public ClassPoint8B(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public ClassPoint8B Plus(ClassPoint8B another) => new ClassPoint8B(x + another.x, y + another.y);

            public static ClassPoint8B PlusEqual(ClassPoint8B point1, ClassPoint8B point2)
            {
                point1.x += point2.x;
                point1.y += point2.y;
                return point1;
            }
        }

        struct StructPoint32B
        {
            decimal x, y;

            public StructPoint32B(decimal x, decimal y)
            {
                this.x = x;
                this.y = y;
            }

            public StructPoint32B 値渡し値返しPlus(StructPoint32B another) => new StructPoint32B(x + another.x, y + another.y);
            public StructPoint32B 参照渡し値返しPlus(in StructPoint32B another) => new StructPoint32B(x + another.x, y + another.y);

            public static StructPoint32B 値渡し値返しPlusEqual(ref StructPoint32B point1, StructPoint32B point2)
            {
                point1.x += point2.x;
                point1.y += point2.y;
                return point1;
            }
            public static StructPoint32B 参照渡し値返しPlusEqual(ref StructPoint32B point1, in StructPoint32B point2)
            {
                point1.x += point2.x;
                point1.y += point2.y;
                return point1;
            }
            public static ref StructPoint32B 参照渡し参照返しPlusEqual(ref StructPoint32B point1, in StructPoint32B point2)
            {
                point1.x += point2.x;
                point1.y += point2.y;
                return ref point1;
            }
        }

        readonly struct ReadOnlyStructPoint32B
        {
            readonly decimal x, y;

            public ReadOnlyStructPoint32B(decimal x, decimal y)
            {
                this.x = x;
                this.y = y;
            }

            public ReadOnlyStructPoint32B 値渡し値返しPlus(ReadOnlyStructPoint32B another) => new ReadOnlyStructPoint32B(x + another.x, y + another.y);
            public ReadOnlyStructPoint32B 参照渡し値返しPlus(in ReadOnlyStructPoint32B another) => new ReadOnlyStructPoint32B(x + another.x, y + another.y);
        }

        class ClassPoint32B
        {
            decimal x, y;

            public ClassPoint32B(decimal x, decimal y)
            {
                this.x = x;
                this.y = y;
            }

            public ClassPoint32B Plus(ClassPoint32B another) => new ClassPoint32B(x + another.x, y + another.y);

            public static ClassPoint32B PlusEqual(ClassPoint32B point1, ClassPoint32B point2)
            {
                point1.x += point2.x;
                point1.y += point2.y;
                return point1;
            }
        }

        StructPoint8B          structPoint8_1          = new StructPoint8B         (1, 2);
        StructPoint8B          structPoint8_2          = new StructPoint8B         (1, 2);
        StructPoint8B          structPoint8_3          = new StructPoint8B         (1, 2);
        StructPoint32B         structPoint32_1         = new StructPoint32B        (1, 2);
        StructPoint32B         structPoint32_2         = new StructPoint32B        (1, 2);
        StructPoint32B         structPoint32_3         = new StructPoint32B        (1, 2);
        ReadOnlyStructPoint8B  readOnlyStructPoint8_1  = new ReadOnlyStructPoint8B (1, 2);
        ReadOnlyStructPoint8B  readOnlyStructPoint8_2  = new ReadOnlyStructPoint8B (1, 2);
        ReadOnlyStructPoint8B  readOnlyStructPoint8_3  = new ReadOnlyStructPoint8B (1, 2);
        ReadOnlyStructPoint32B readOnlyStructPoint32_1 = new ReadOnlyStructPoint32B(1, 2);
        ReadOnlyStructPoint32B readOnlyStructPoint32_2 = new ReadOnlyStructPoint32B(1, 2);
        ReadOnlyStructPoint32B readOnlyStructPoint32_3 = new ReadOnlyStructPoint32B(1, 2);
        ClassPoint8B           classPoint8_1           = new ClassPoint8B          (1, 2);
        ClassPoint8B           classPoint8_2           = new ClassPoint8B          (1, 2);
        ClassPoint8B           classPoint8_3           = new ClassPoint8B          (1, 2);
        ClassPoint32B          classPoint32_1          = new ClassPoint32B         (1, 2);
        ClassPoint32B          classPoint32_2          = new ClassPoint32B         (1, 2);
        ClassPoint32B          classPoint32_3          = new ClassPoint32B         (1, 2);

        [GlobalSetup]
        public void Setup()
        {}

        [Benchmark]public void 値型8_値渡し値返しPlus() => structPoint8_3 = structPoint8_1.値渡し値返しPlus(structPoint8_2);
        [Benchmark] public void 値型8_参照渡し値返しPlus() => structPoint8_3 = structPoint8_1.参照渡し値返しPlus(structPoint8_2);
        [Benchmark] public void 値型8_値渡し値返しPlusEqual() => structPoint8_3 = StructPoint8B.値渡し値返しPlusEqual(ref structPoint8_1, structPoint8_2);
        [Benchmark] public void 値型8_参照渡し値返しPlusEqual() => structPoint8_3 = StructPoint8B.参照渡し値返しPlusEqual(ref structPoint8_1, structPoint8_2);
        [Benchmark] public void 値型8_参照渡し参照返しPlusEquall() => structPoint8_3 = StructPoint8B.参照渡し参照返しPlusEqual(ref structPoint8_1, structPoint8_2);

        [Benchmark] public void ReadOnly値型8_値渡し値返しPlus() => readOnlyStructPoint8_3 = readOnlyStructPoint8_1.値渡し値返しPlus(readOnlyStructPoint8_2);
        [Benchmark] public void ReadOnly値型8_参照渡し値返しPlus() => readOnlyStructPoint8_3 = readOnlyStructPoint8_1.参照渡し値返しPlus(readOnlyStructPoint8_2);

        [Benchmark] public void 参照型8_Plus() => classPoint8_3 = classPoint8_1.Plus(classPoint8_2);
        [Benchmark] public void 参照型8_PlusEqual() => classPoint8_3 = ClassPoint8B.PlusEqual(classPoint8_1, classPoint8_2);

        [Benchmark] public void 値型32_値渡し値返しPlus() => structPoint32_3 = structPoint32_1.値渡し値返しPlus(structPoint32_2);
        [Benchmark] public void 値型32_参照渡し値返しPlus() => structPoint32_3 = structPoint32_1.参照渡し値返しPlus(structPoint32_2);
        [Benchmark] public void 値型32_値渡し値返しPlusEqual() => structPoint32_3 = StructPoint32B.値渡し値返しPlusEqual(ref structPoint32_1, structPoint32_2);
        [Benchmark] public void 値型32_参照渡し値返しPlusEqual() => structPoint32_3 = StructPoint32B.参照渡し値返しPlusEqual(ref structPoint32_1, structPoint32_2);
        [Benchmark] public void 値型32_参照渡し参照返しPlusEquall() => structPoint32_3 = StructPoint32B.参照渡し参照返しPlusEqual(ref structPoint32_1, structPoint32_2);

        [Benchmark] public void ReadOnly値型32_値渡し値返しPlus() => readOnlyStructPoint32_3 = readOnlyStructPoint32_1.値渡し値返しPlus(readOnlyStructPoint32_2);
        [Benchmark] public void ReadOnly値型32_参照渡し値返しPlus() => readOnlyStructPoint32_3 = readOnlyStructPoint32_1.参照渡し値返しPlus(readOnlyStructPoint32_2);

        [Benchmark] public void 参照型32_Plus() => classPoint32_3 = classPoint32_1.Plus(classPoint32_2);
        [Benchmark] public void 参照型32_PlusEqual() => classPoint32_3 = ClassPoint32B.PlusEqual(classPoint32_1, classPoint32_2);

#if DEBUG
        public static void Test()
        {
            var 値型と参照型BenchMark = new 値型と参照型BenchMark();
            値型と参照型BenchMark.Setup();

            var tests = new Action[] {
                値型と参照型BenchMark.値型8_値渡し値返しPlus,
                値型と参照型BenchMark.値型8_参照渡し値返しPlus,
                値型と参照型BenchMark.値型8_値渡し値返しPlusEqual,
                値型と参照型BenchMark.値型8_参照渡し値返しPlusEqual,
                値型と参照型BenchMark.値型8_参照渡し参照返しPlusEquall,
                値型と参照型BenchMark.ReadOnly値型8_値渡し値返しPlus,
                値型と参照型BenchMark.ReadOnly値型8_参照渡し値返しPlus,
                値型と参照型BenchMark.参照型8_Plus,
                値型と参照型BenchMark.参照型8_PlusEqual,
                値型と参照型BenchMark.値型32_値渡し値返しPlus,
                値型と参照型BenchMark.値型32_参照渡し値返しPlus,
                値型と参照型BenchMark.値型32_値渡し値返しPlusEqual,
                値型と参照型BenchMark.値型32_参照渡し値返しPlusEqual,
                値型と参照型BenchMark.値型32_参照渡し参照返しPlusEquall,
                値型と参照型BenchMark.ReadOnly値型32_値渡し値返しPlus,
                値型と参照型BenchMark.ReadOnly値型32_参照渡し値返しPlus,
                値型と参照型BenchMark.参照型32_Plus,
                値型と参照型BenchMark.参照型32_PlusEqual
            };
            foreach (var test in tests)
                test();
        }
#endif // DEBUG
    }

    [ShortRunJob]
    [HtmlExporter]
    [CsvExporter]
    public class 並列化BenchMark
    {
        [Params(1000, 10_000, 100_000)]
        public int ループ回数 = 1000;

        double sum = 0.0;

        [GlobalSetup]
        public void Setup() => sum = 0.0;

        void 重い処理()
        {
            var sum = 0.0;
            for (var count = 0; count < ループ回数; count++)
                sum += Math.Log(count);
        }

        void 順次同期処理()
        {
            重い処理();
            重い処理();
            重い処理();
            重い処理();
        }

        async Task 順次非同期処理()
        {
            await Task.Run(重い処理);
            await Task.Run(重い処理);
            await Task.Run(重い処理);
            await Task.Run(重い処理);
        }

        async Task 並列処理()
        {
            var task1 = new Task(重い処理);
            var task2 = new Task(重い処理);
            var task3 = new Task(重い処理);
            var task4 = new Task(重い処理);
            await Task.WhenAll(task1, task2, task3, task4);
        }

        [Benchmark] public void 順次同期処理をする() => 順次同期処理();
        [Benchmark] public void 順次非同期処理をする() => 順次非同期処理().Wait();
        [Benchmark] public void 並列処理をする() => 並列処理().Wait();
    }

    class Program
    {
        static void RunStackAllocBenchMark() => BenchmarkRunner.Run<StackAllocBenchMark>();

        static void Runボックス化BenchMark()
        {
#if DEBUG
            ボックス化BenchMark.Test();
#else // DEBUG
            BenchmarkRunner.Run<ボックス化BenchMark>();
#endif // DEBUG
         }

        static void RunReadOnlyStructBenchMark() => BenchmarkRunner.Run<ReadOnlyStructBenchMark>();
#if Core
        static void RunStringCreateBenchMark() => BenchmarkRunner.Run<StringCreateBenchMark>();
#endif // Core

        static void RunForAndForEachBenchMark()
        {
#if DEBUG
#if Core
            ForAndForEachBenchMarkCore.Test();
#else // Core
            ForAndForEachBenchMarkNetwork.Test();
#endif // Core
#else // DEBUG
#if Core
            BenchmarkRunner.Run<ForAndForEachBenchMarkCore>();
#else // Core
            BenchmarkRunner.Run<ForAndForEachBenchMarkNetwork>();
#endif // Core
#endif // DEBUG
        }

        static void Run値型と参照型BenchMark()
        {
#if DEBUG
            値型と参照型BenchMark.Test();
#else // DEBUG
            BenchmarkRunner.Run<値型と参照型BenchMark>();
#endif // DEBUG
        }

        static void Run並列化BenchMark() => BenchmarkRunner.Run<並列化BenchMark>();

        static void Main()
        {
            //RunStackAllocBenchMark();
            //Runボックス化BenchMark();
            //RunReadOnlyStructBenchMark();
#if Core
            RunStringCreateBenchMark();
#endif // Core
            //RunForAndForEachBenchMark();
            //Run値型と参照型BenchMark();
            //Run並列化BenchMark();
        }
    }
}
