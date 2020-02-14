/*
参考文献

C# | Wikipedia
https://ja.wikipedia.org/wiki/C_Sharp
C# の歴史 - C# ガイド | Microsoft Docs
https://docs.microsoft.com/ja-jp/dotnet/csharp/whats-new/csharp-version-history
C# 7 の新機能 - C# によるプログラミング入門 | ++C++; // 未確認飛行 C
https://ufcpp.net/study/csharp/cheatsheet/ap_ver7/
C# 7.1 の新機能 - C# によるプログラミング入門 | ++C++; // 未確認飛行 C
https://ufcpp.net/study/csharp/cheatsheet/ap_ver7_1/
C# 7.2 の新機能 - C# によるプログラミング入門 | ++C++; // 未確認飛行 C
https://ufcpp.net/study/csharp/cheatsheet/ap_ver7_2/
C# 7.3 の新機能 - C# によるプログラミング入門 | ++C++; // 未確認飛行 C
https://ufcpp.net/study/csharp/cheatsheet/ap_ver7_3/
C# 8.0 の新機能 - C# によるプログラミング入門 | ++C++; // 未確認飛行 C
https://ufcpp.net/study/csharp/cheatsheet/ap_ver8/
今日からできる! 簡単 .NET 高速化 Tips | slideshare
https://www.slideshare.net/xin9le/dotnetperformancetips-170268354
foreach の掛け方いろいろ | ++C++; // 未確認飛行 C ブログ
https://ufcpp.net/blog/2018/12/howtoenumerate/
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NewFeatures7 // C# 7 - 2017/3 - Visual Studio 2017
{
    static class Extension
    {
        public static void ForEach<TElement>(this IEnumerable<TElement> @this, Action<TElement> action)
        {
            foreach (var element in @this)
                action(element);
        }

        public static void Times(this int @this, Action action)
        {
            for (var index = 0; index < @this; index++)
                action();
        }
    }

    class StopwatchViewer : IDisposable
    {
        Stopwatch stopwatch = new Stopwatch();

        public Action<string> WriteLine = Console.WriteLine;
        public StopwatchViewer() => stopwatch.Start();

        public void Dispose()
        {
            stopwatch.Stop();
            ShowResult();
        }

        void ShowResult() => WriteLine?.Invoke($"({stopwatch.ElapsedMilliseconds / 1000.0}s.)");
    }

    static class TupleSample
    {
        // タプル

        // Tuple
        static Tuple<int, int> TallyOld(this IEnumerable<int> @this)
            => new Tuple<int, int>(@this.Count(), @this.Sum());
        static Tuple<int, int> DivRemOld(int divident, int divisor)
            => new Tuple<int, int>(Math.DivRem(divident, divisor, out var reminder), reminder);

        // ValueTuple
        static (int count, int sum) Tally(this IEnumerable<int> @this)
            => (@this.Count(), @this.Sum());
        static (int quotient, int reminder) DivRem(int divident, int divisor)
            => (Math.DivRem(divident, divisor, out var reminder), reminder);

        // Deconstruct
        static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> @this, out TKey key, out TValue value)
        {
            key   = @this.Key  ;
            value = @this.Value;
        }

        static void Sample()
        {
            var dividend              = 10;
            var divisor               =  3;
            var answer                = (dividend / divisor, dividend % divisor);
            var (quotient, remainder) = answer;
        }

        public static void Run()
        {
            // Tuple
            Sample();

            var tally     = new[] { 0, 1, 1, 2, 3, 5, 8, 13 }.TallyOld();
            var count     = tally.Item1;
            var sum       = tally.Item2;

            var divRem    = DivRemOld(3533, 821);
            var quotient0 = divRem.Item1;
            var reminder0 = divRem.Item2;

            switch (DivRemOld(3533, 821)) {
                case Tuple<int, int> dr when dr.Item1 == 0 && dr.Item2 == 0:
                    // ...
                    break;
                default:
                    break;
            }

            // ValueTuple
            // 分解
            var (count1, sum1)     = new[] { 0, 1, 1, 2, 3, 5, 8, 13 }.Tally();
            (var count2, var sum2) = new[] { 0, 1, 1, 2, 3, 5, 8, 13 }.Tally();
            // 値の破棄
            var (quotient, _) = DivRem(3533, 821);
            (_, var reminder) = DivRem(3533, 821);

            switch (DivRem(3533, 821)) {
                case (0, 0):
                    // ...
                    break;
                default:
                    break;
            }

            var (key1, value1) = new KeyValuePair<string, int>("one", 1);
            // 出力変数宣言
            new KeyValuePair<string, int>("two", 2).Deconstruct(out var key2, out var value2);
        }
    }

    static class DeconstructSample
    {
        public struct Point
        {
            public readonly double x, y;

            public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public void Deconstruct(out double x, out double y)
            {
                x = this.x;
                y = this.y;
            }
        }

        public struct Person
        {
            public readonly int id;
            public readonly string name;

            public Person(int id, string name)
            {
                this.id = id;
                this.name = name;
            }
        }

        public static void Deconstruct(this Person @this, out int id, out string name)
        {
            id = @this.id;
            name = @this.name;
        }

        public static void Run()
        {
            var point = new Point(x: 100.0, y: -200.0);
            var (x, y) = point;

            var staff = new Person(id: 100, name: "志垣太郎");
            var (id, name) = staff;
        }
    }

    static class TypeSwitchSample
    {
        static string ToString(object item)
        {
            // 型スイッチ
            switch (item) {
                case 0                           : return "零"                         ;
                case int number when number <   0: return $"負の整数{number}"          ;
                case int number when number <  10: return $"一桁の正の整数{number}"    ;
                case int number when number < 100: return $"二桁の正の整数{number}"    ;
                case int number                  : return $"三桁以上の正の整数{number}";
                case string text                 : return text                         ;
                default                          : throw new ArgumentException()       ;
            }
        }

        public static void Run()
        {
            Enumerable.Range(-10, 120)
                      .Select(FizzBuzz)
                      .Select(ToString)
                      .ForEach(Console.WriteLine);
        }

        static object FizzBuzz(int value)
            => value % 3 == 0 && value % 5 == 0 ? "FizzBuzz"
                                                : value % 3 == 0 ? "Fizz"
                                                                 : value % 5 == 0 ? "Buzz"
                                                                                  : (object)value;
    }

    static class ValueTaskSample
    {
        static async Task<int> Taskを返す例Async(int value)
        {
            switch (value) {
                case int number when number % 100 == 0:
                    await Task.Delay(1);
                    return 0;
                default:
                    return value;
            }
        }

        static async ValueTask<int> ValueTaskを返す例Async(int value)
        {
            switch (value) {
                case int number when number % 100 == 0:
                    await Task.Delay(1);
                    return 0;
                default:
                    return value;
            }
        }

        public static async Task Run()
        {
            const int times = 1;
            var values = Enumerable.Range(1, 100);

            await values.Taskを返す例Asyncのパフォーマンステスト(times);
            await values.ValueTaskを返す例Asyncのパフォーマンステスト(times);
            await values.ValueTaskを返す例Asyncのパフォーマンステスト(times);
            await values.Taskを返す例Asyncのパフォーマンステスト(times);
        }

        static async Task Taskを返す例Asyncのパフォーマンステスト(this IEnumerable<int> @this, int times)
        {
            var sum = 0;
            Console.Write($"{nameof(Taskを返す例Asyncのパフォーマンステスト)}: {times}times - ");
            using (var _ = new StopwatchViewer()) {
                for (var count = 0; count < times; count++) {
                    foreach (var element in @this)
                        sum += await Taskを返す例Async(element);
                }
            }
        }

        static async Task ValueTaskを返す例Asyncのパフォーマンステスト(this IEnumerable<int> @this, int times)
        {
            var sum = 0;
            Console.Write($"{nameof(ValueTaskを返す例Asyncのパフォーマンステスト)}: {times}times - ");
            using (var _ = new StopwatchViewer()) {
                for (var count = 0; count < times; count++) {
                    foreach (var element in @this)
                        sum += await ValueTaskを返す例Async(element);
                }
            }
        }
    }

    static class OtherSample
    {
        static int Count<TElement>(this IEnumerable<TElement> @this)
        {
            // is での変数宣言
            if (@this is ICollection<TElement> collection1)
                return collection1.Count;
            if (@this is ICollection collection2)
                return collection2.Count;

            var count = 0;
            using (var enumerator = @this.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    count = checked(count + 1);
                return count;
            }
        }

        // 参照戻り値
        static ref double Square(this ref double @this)
        {
            @this *= @this;
            return ref @this;
        }

        static ref double Round(this ref double @this)
        {
            // 参照ローカル変数
            ref var result = ref @this;
            result = Math.Floor(result + 0.5);
            return ref result;
        }

        static int ToDecimal(this IEnumerable<int> @this)
        {
            var number = 0;
            // ローカル関数 (number をキャプチャー)
            void Add(int digit) => number = number * 10 + digit;
            @this.ForEach(Add);
            return number;
        }

        //static int ToDecimal2(this IEnumerable<int> @this)
        //    => @this.Aggregate((number, digit) => number * 10 + digit);

        public static void Run()
        {
            var count = new[] { 0, 1, 1, 2, 3, 5, 8, 13 }.Count();
            var value = Math.PI;
            value.Square().Round();

            var number = new[] { 1, 1, 2, 3, 5 }.ToDecimal();

            // 数字区切り文字
            uint value1 = 0xffff_0000;
            // 2進数リテラル
            byte value2 = 0b0000_1111;
        }
    }

    class Samples
    {
        public static async Task Run()
        {
            TupleSample         .Run();
            DeconstructSample    .Run();
            TypeSwitchSample     .Run();
            await ValueTaskSample.Run();
            OtherSample          .Run();
        }
    }
}

namespace NewFeatures7_1 // C#7.1 - 2017/8 - Visual Studio 2017 Update 3 (15.3)
{
    class DefaultExpressionSample
    {
        // default 式
        static void WriteLine<T>(T item = default)
        {
            Console.WriteLine(item);
        }

        enum SampleEnum
        {
            Value0, Value1, Value2
        }

        public static void Run()
        {
            WriteLine<string    >();
            WriteLine<int       >();
            WriteLine<SampleEnum>();
        }
    }

    class タプル要素名の推論Sample
    {
        public static void Run()
        {
            //var x = 100;
            //var y = 200;
            //var point = (x: x, y: y);
            //Console.WriteLine($"({point.x}, {point.y})");

            // タプル要素名の推論
            var x = 100;
            var y = 200;
            var point = (x, y);
            Console.WriteLine($"({point.x}, {point.y})");
        }
    }

    class TypeSwitchSample
    {
        static string ToString<T>(T item)
        {
            // 型スイッチ
            switch (item) {
                case 0                           : return "零"                         ;
                case int number when number <   0: return $"負の整数{number}"          ;
                case int number when number <  10: return $"一桁の正の整数{number}"    ;
                case int number when number < 100: return $"二桁の正の整数{number}"    ;
                case int number                  : return $"三桁以上の正の整数{number}";
                case string text                 : return text                         ;
                default                          : throw new ArgumentException()       ;
            }
        }

        public static void Run()
        {
            Console.WriteLine(ToString("A"));
            Console.WriteLine(ToString(100));
        }
    }

    class Samples
    {
        public static void Run()
        {
            DefaultExpressionSample .Run();
            タプル要素名の推論Sample.Run();
            TypeSwitchSample        .Run();
        }
    }
}

namespace NewFeatures7_2 // C#7.2 - 2017/12 - Visual Studio 2017 15.5
{
    using NewFeatures7;

    struct inを使わないVector3D
    {
        public double X;
        public double Y;
        public double Z;

        public static inを使わないVector3D operator +(inを使わないVector3D v1, inを使わないVector3D v2)
            => new inを使わないVector3D { X = v1.X + v2.X, Y = v1.Y + v2.Y, Z = v1.Z + v2.Z };

        public static bool operator <(inを使わないVector3D v1, inを使わないVector3D v2) => v1.Absolute() < v2.Absolute();
        public static bool operator >(inを使わないVector3D v1, inを使わないVector3D v2) => v1.Absolute() > v2.Absolute();
        public double Absolute() => Math.Sqrt(X * X + Y * Y + Z * Z);
    }

    struct inを使うVector3D
    {
        public double X;
        public double Y;
        public double Z;

        // ref readonly
        // 演算子のin引数
        public static inを使うVector3D operator +(in inを使うVector3D v1, in inを使うVector3D v2)
            => new inを使うVector3D { X = v1.X + v2.X, Y = v1.Y + v2.Y, Z = v1.Z + v2.Z };

        public static bool operator <(in inを使うVector3D v1, in inを使うVector3D v2) => v1.Absolute() < v2.Absolute();
        public static bool operator >(in inを使うVector3D v1, in inを使うVector3D v2) => v1.Absolute() > v2.Absolute();
        public double Absolute() => Math.Sqrt(X * X + Y * Y + Z * Z);
    }

    static class Vector3DExtensions
    {
        // 参照渡しの拡張メソッド
        public static void Increment(ref this inを使うVector3D v)
        {
            v.X++;
            v.Y++;
            v.Z++;
        }

        public static string ToText(in this inを使うVector3D v)
            => $"(X: {v.X}, Y: {v.Y}, Z: {v.Z})";
    }

    // readonly struct
    readonly struct Point
    {
        public readonly double X;
        public readonly double Y;

        public Point(double x, double y) => (X, Y) = (x, y);
    }

    class ReferenceSample
    {
        static inを使わないVector3D inを使わないMax(inを使わないVector3D v1, inを使わないVector3D v2)
        {
            if (v1 > v2)
                return v1;
            return v2;
        }

        // ref readonly
        static ref readonly inを使うVector3D inを使うMax(in inを使うVector3D v1, in inを使うVector3D v2)
        {
            //// ref readonly
            //ref readonly var rv1 = ref v1;
            //ref readonly var rv2 = ref v2;

            //if (rv1 > rv2)
            //    return ref rv1;
            //return ref rv2;

            if (v1 > v2)
                return ref v1;
            return ref v2;
        }

        public static void Run()
        {
            int a = 1;
            int b = 2;
            // 条件演算子での ref 利用
            (a < b ? ref a : ref b)++;

            var v = new inを使うVector3D { X = 1.0, Y = 2.0, Z = -3.0 } + new inを使うVector3D { X = 3.0, Y = -1.0, Z = 2.0 };
            v.Increment();
            var text = v.ToText();

            var point = new Point(1.0, -2.0);

            const int times = 10000000;
            inを使わない方のパフォーマンステスト(times);
            inを使う方のパフォーマンステスト(times);
            inを使う方のパフォーマンステスト(times);
            inを使わない方のパフォーマンステスト(times);
        }

        static void inを使わない方のパフォーマンステスト(int times)
        {
            var v1 = new inを使わないVector3D { X = 1.0, Y = 2.0, Z = -3.0 };
            var v2 = new inを使わないVector3D { X = 3.0, Y = -1.0, Z = 2.0 };
            Console.Write($"{nameof(inを使わない方のパフォーマンステスト)}: {times}times - ");
            using (var _ = new StopwatchViewer()) {
                for (var count = 0; count < times; count++) {
                    var v3 = inを使わないMax(v1, v2);
                }
            }
        }

        static void inを使う方のパフォーマンステスト(int times)
        {
            var v1 = new inを使うVector3D { X = 1.0, Y =  2.0, Z = -3.0 };
            var v2 = new inを使うVector3D { X = 3.0, Y = -1.0, Z =  2.0 };
            Console.Write($"{nameof(inを使う方のパフォーマンステスト)}: {times}times - ");
            using (var _ = new StopwatchViewer()) {
                for (var count = 0; count < times; count++) {
                    var v3 = inを使うMax(v1, v2);
                }
            }
        }
    }

    class SpanSample
    {
        const int bufferSize = 256;

        static void newを使う例()
        {
            var buffer = new int[bufferSize];
            for (var index = 0; index < bufferSize; index++)
                buffer[index] = index;
        }

        static void stackallocとSpanを使う例()
        {
            Span<int> buffer = stackalloc int[bufferSize];
            for (var index = 0; index < bufferSize; index++)
                buffer[index] = index;
        }

        public static void Run()
        {
            const int times = 100000;
            newを使うパフォーマンステスト(times);
            stackallocとSpanを使うパフォーマンステスト(times);
            stackallocとSpanを使うパフォーマンステスト(times);
            newを使うパフォーマンステスト(times);
        }

        static void newを使うパフォーマンステスト(int times)
        {
            Console.Write($"{nameof(newを使うパフォーマンステスト)}: {times}times - ");
            using (var _ = new StopwatchViewer()) {
                for (var count = 0; count < times; count++)
                    newを使う例();
            }
        }

        static void stackallocとSpanを使うパフォーマンステスト(int times)
        {
            Console.Write($"{nameof(stackallocとSpanを使うパフォーマンステスト)}: {times}times - ");
            using (var _ = new StopwatchViewer()) {
                for (var count = 0; count < times; count++)
                    stackallocとSpanを使う例();
            }
        }
    }

    class Samples
    {
        static int Sum(int x, int y, int z) => x + y + z;

        public static void Run()
        {
            // 数字区切り文字
            uint value1 = 0x_ffff_0000;
            // 2進数リテラル
            byte value2 = 0b_0000_1111;

            // 非末尾名前付き引数
            Sum(x: 1, 2, 3);

            ReferenceSample.Run();
            SpanSample     .Run();
        }
    }
}

namespace NewFeatures7_3 // C#7.3 - 2018/5 - Visual Studio 2017 15.7/.NET Core 2.1
{
    class Samples
    {
        public static void Run()
        {
            // タプルの ==, != 比較
            var x = (1, "Apple");
            var y = (1, "Grape");
            if (x == y)
                ;

            var array = new[] { 1, 2, 3 };
            int index = 0;
            // ref再代入
            // for/foreach のループ変数を参照に
            for (ref var r = ref array[index]; ; r = ref array[index]) {
                r *= r;
                if (++index == array.Length)
                    break;
            }
        }
    }
} 

namespace NewFeatures8 // C#8 - 2019/9 - Visual Studio 2019 16.3/.NET Core 3.0/.NET Standard 2.1
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int x = 0, int y = 0) => (X, Y) = (x, y);
        public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
    }

    static class Samples
    {
        static string NotNull() => "";
        static string? MaybeNull() => null;

        static int M(string s)
        {
            var notNull = NotNull();
            var maybeNull = MaybeNull();

            // null チェックをしていないので、以下の行の notNull のところに警告が出る。
            return s.Length + notNull.Length + maybeNull.Length;
        }

        // switch 式
        // 再帰パターン
        static int M(object item) => item switch {
            0 => 1,
            int n => 2,
            Point(1, _) => 4,
            Point { X: 2, Y: var y } => y,
            _ => 0
        };

        // switch 文
        static int Compare0(int? x, int? y)
        {
            switch ((x, y)) {
                case (int value1, int value2):
                    return value1.CompareTo(value2);
                case ({}        , null      ):
                case (null      , {}        ):
                case (null      , null      ):
                    return 0;
            }
        }

        // switch 式
        static int Compare(int? x, int? y)
            => (x, y) switch {
                (int value1, int value2) => value1.CompareTo(value2),
                ({}  , null) => 0,
                (null, {}  ) => 0,
                (null, null) => 0
            };

        static bool IsNullOrSpace(string? text)
            => text switch {
                    null                               => true ,
                    string {  Length: 0 }              => true , // プロパティ
                    string s when s.Trim().Length == 0 => true ,
                    _                                  => false
                };


        // インターフェイスのデフォルト実装
        interface IEnumerable改<TElement> : IEnumerable<TElement>
        {
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class MyCollection : IEnumerable改<int>
        {
            public IEnumerator<int> GetEnumerator()
            {
                yield return 0;
                yield return 1;
                yield return 1;
                yield return 2;
                yield return 3;
            }
        }

        // 非同期ストリーム
        static async IAsyncEnumerable<string> GetData()
        {
            int n = 0;

            const string dataFileName = "data.txt";
            PrepareDataFile(dataFileName);

            using var reader = new StreamReader(dataFileName);
            for (; ;) {
                var text = await reader.ReadLineAsync();
                if (text == null)
                    break;
                yield return text;
            }

            //// 非同期イテレーター
            //yield return 1;
            //await Task.Delay(1);
            //yield return 2;
            //await Task.Delay(1);
            //yield return 3;
        }

        static Random random = new Random();

        static void PrepareDataFile(string dataFileName)
        {
            //if (!File.Exists(dataFileName)) {
            using var writer = new StreamWriter(dataFileName);
            Enumerable.Range(0, 10)
                      .Select(_ => $"{DateTime.Now.Ticks:#,0} - {random.Next():#,0}円")
                      .ForEach(async text => await writer.WriteLineAsync(text));
            //}
        }

        static async IAsyncEnumerable<TResult> SelectAsync<TElement, TResult>(this IAsyncEnumerable<TElement> @this, Func<TElement, TResult> selector)
        {
            // 非同期foreach
            await foreach (var item in @this)
                // 非同期イテレーター
                yield return selector(item);
        }

        static void ForEach<TElement>(this IEnumerable<TElement> @this, Action<TElement> action)
        {
            // 同期foreach
            foreach (var item in @this)
                action(item);
        }

        static async ValueTask ForEachAsync<TElement>(this IAsyncEnumerable<TElement> @this, Action<TElement> action)
        {
            // 非同期foreach
            await foreach (var item in @this)
                action(item);
        }

        readonly struct DefferredMessage : IDisposable
        {
            readonly string message;

            public DefferredMessage(string message) => this.message = message;

            public void Dispose() => Console.WriteLine(message);
        }

        // ref 構造体 (スタック領域のみ)
        ref struct RefDisposable
        {
            public void Dispose() {}
        }

        static void F(int value)
        {
            // ローカル関数
            int GetMultiple(int multiplier) => value * multiplier;

            // 静的ローカル関数
            static int Square(int x) => x * x;
        }

        readonly struct Point2
        {
            public readonly double X;
            public readonly double Y;

            public Point2(double x, double y) => (X, Y) = (x, y);

            // readonly 関数メンバー
            public double AbsoluteValue => Math.Sqrt(X * X + Y * Y);
            public readonly double DotProduct(Point another) => X * another.X + Y * another.Y;
        }

        public static async Task Run()
        {
            var numbers = new[] { 100, 200, 300, 400, 500 };
            // 範囲アクセス
            var middleNumbers1 = numbers[1..^1]; // 200, 300, 400
            var middleNumbers2 = numbers[1..4]; // 200, 300, 400

            await GetData().SelectAsync(item => $"{item}_")
                           .ForEachAsync(Console.WriteLine);

            // using 変数宣言
            using var defferredMessage = new DefferredMessage("bye!");
            Console.WriteLine("Hello.");

            // パターン ベースな using
            using (new RefDisposable()) {
            }

            string? s = null;
            // null 合体代入
            s ??= "default string";
        }
    }
}

namespace NewFeatures
{
    class Program
    {
        // 非同期Main (C#7.1)
        static async Task Main()
        {
            await NewFeatures7.Samples.Run();
            NewFeatures7_1    .Samples.Run();
            NewFeatures7_2    .Samples.Run();
            NewFeatures7_3    .Samples.Run();
            await NewFeatures8.Samples.Run();
        }
    }
}
