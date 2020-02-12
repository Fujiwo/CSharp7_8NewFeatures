namespace structを使うときの注意点
{
    using System;
    using System.Collections.Generic;

    class TestProgram
    {
        class ClassCounter
        {
            public int Number { get; set; }
            public void Increment() { Number++; }
        }

        struct StructCounter
        {
            public int Number { get; set; }
            public void Increment() { Number++; } // struct に内部の状態を変えるようなメソッドを持たすときは要注意
        }

        // 参照型 (class) のメンバーと値型 (struct) のメンバー挙動が異なることがある
        class Foo
        {
            public ClassCounter ClassCounter { get; set; } = new ClassCounter();
            public StructCounter StructCounter { get; set; }
        }

        // 参照型 (class) のメンバーと値型 (struct) のメンバー挙動が異なることがある
        class StructMemberTest
        {
            Foo foo = new Foo();

            public void Run()
            {
                foo.ClassCounter.Number = 0; // OK: 変更できる
                // foo.StructCounter.Number = 1; // NG: 変更できない

                foo.ClassCounter.Increment(); // ClassCounter をインクリメントしている
                Console.WriteLine($"{nameof(StructMemberTest)} - class: {foo.ClassCounter.Number}"); // 結果は 1
                foo.StructCounter.Increment(); // StructCounter のコピーをインクリメントしている
                Console.WriteLine($"{nameof(StructMemberTest)} - struct: {foo.StructCounter.Number}"); // 結果は 0
            }
        }

        // struct では、readonly なときとそうでないときで、内部の状態を変えるようなメソッドを呼んだときの挙動が異なるので要注意
        // 勘違いから思わぬバグの元になることも
        class ReadOnlyStructTest
        {
            // class を使った例
            ClassCounter classCounter = new ClassCounter();
            readonly ClassCounter readOnlyClassCounter = new ClassCounter();

            // struct を使った例
            StructCounter structCounter;
            readonly StructCounter readOnlyStructCounter;

            public void Run()
            {
                // クラスの場合はどっちも結果は同じなので、違いを特に意識しなくても大丈夫
                classCounter.Increment();
                Console.WriteLine($"{nameof(ReadOnlyStructTest)} - {nameof(classCounter)}: {classCounter.Number}"); // 結果は 1
                readOnlyClassCounter.Increment();
                Console.WriteLine($"{nameof(ReadOnlyStructTest)} - {nameof(readOnlyClassCounter)}: {readOnlyClassCounter.Number}"); // 結果は 1

                // struct の場合は、readonly なときとそうでないときで、要素の内部の状態を変えるようなメソッドを呼んだときの挙動が異なる
                structCounter.Increment();
                Console.WriteLine($"{nameof(ReadOnlyStructTest)} - {nameof(structCounter)}: {structCounter.Number}");  // 結果は 1 - structCounter そのものにアクセスしている
                readOnlyStructCounter.Increment();
                Console.WriteLine($"{nameof(ReadOnlyStructTest)} - {nameof(readOnlyStructCounter)}: {readOnlyStructCounter.Number}");  // 結果は 0 - readOnlyStructCounter そのものではなく、コピーにアクセスしている
            }
        }

        // struct の配列とリストでは、要素の内部の状態を変えるようなメソッドを呼んだときの挙動が異なるので要注意
        // 勘違いから思わぬバグの元になることも
        static void StructArrayTest()
        {
            // class の場合はどっちも結果は同じなので、違いを特に意識しなくても大丈夫
            var classArray = new ClassCounter[] { new ClassCounter() };
            classArray[0].Increment();
            Console.WriteLine($"{nameof(StructArrayTest)} - {nameof(classArray)}: {classArray[0].Number}"); // 結果は 1

            var classList = new List<ClassCounter>() { new ClassCounter() };
            classList[0].Increment();
            Console.WriteLine($"{nameof(StructArrayTest)} - {nameof(classList)}: {classList[0].Number}"); // 結果は 1

            // struct の場合は、配列とリストでインデクサ経由で要素の内部の状態を変えるようなメソッドを呼んだときの挙動が異なる
            var structArray = new StructCounter[] { new StructCounter() };
            structArray[0].Increment();
            Console.WriteLine($"{nameof(StructArrayTest)} - {nameof(structArray)}: {structArray[0].Number}"); // 結果は 1 - structArray のインデクサを通じて、内部のアイテムに直にアクセスできる

            var structList = new List<StructCounter>() { new StructCounter() };
            structList[0].Increment();
            Console.WriteLine($"{nameof(StructArrayTest)} - {nameof(structList)}: {structList[0].Number}"); // 結果は 0 - structList のインデクサは、内部のアイテムのコピーを返す
        }

        static void Main()
        {
            new StructMemberTest().Run();
            new ReadOnlyStructTest().Run();
            StructArrayTest();
        }
    }
}
