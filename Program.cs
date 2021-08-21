//all namespaces used
using System; using System.Collections.Generic; using System.Runtime.CompilerServices; using System.Linq;
using LearningCSharpPart2.Tools; using LearningCSharpPart2.Classes;

//all aliases used
using Int2D = System.ValueTuple<int, int>;

//all static classes used
using static System.Console; using static System.MathF; using static System.Array; using static System.Convert;
using static LearningCSharpPart2.Tools.GenTools; using static LearningCSharpPart2.Tools.MathTools; using static LearningCSharpPart2.Tools.HuffTools; using static LearningCSharpPart2.Tools.TypeTools; using static LearningCSharpPart2.Tools.Tools;
namespace LearningCSharpPart2
{
    
    public class Sample {
        public string Name { get; set; }
        public int age;
        [MyMethod]
        public void Method() { }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class MyClassAttribute : Attribute { }


    [AttributeUsage(AttributeTargets.Method)]
    public class MyMethodAttribute : Attribute { }


    class Program
    {
        
        static void Main(string[] args)
        {
            
            Tree<string> tree = new("Apple");
            tree.Split(3);
            tree[0][0] = "Mango";
            print( "ABDABBDCCDC".GetHuffDict().ToStringSeq());
            print($"The Huff Code of \"ABDABBDCCDC\" is {"ABDABBDCCDC".ToHuffCode()}");
        }
        /*public static int?[] SortGame() {
            int?[] numbs = RandArrNull(9);

            while (!numbs.IsInAscOrder())
            {

            }

            return numbs;
            
        }

        public static bool IsValid<TArr>(TArr[] vals, object valA, object valB) {
            if ((valA == null || valB == null) &&
                (Abs(vals.IndexOf(valA) - vals.IndexOf(valB)) == 1 || Abs(vals.IndexOf(valA) - vals.IndexOf(valB)) == 3))
                return true;
            return false;
        }

        public static int?[] RandArrNull(dynamic len) {
            int?[] numbs = new int?[len];
            for (int cyc = 0; cyc < len; cyc++) {
                bool copyB;
                do {
                    int? randNumb = rand.Next(0, len);
                    randNumb = randNumb == 0 ? null : randNumb;
                    copyB = false;
                    for (int idx = 0; idx < cyc; idx++)
                        if (numbs[idx] == randNumb && idx != cyc)
                        {
                            copyB = true;
                            break;
                        }
                    numbs[cyc] = randNumb;
                } while (copyB);
            }
            return numbs;
        }*/



        public static long Part(int amount, int partLim, Dictionary<Int2D, long> dictionary = null)
        {
            Int2D dictKey = (amount, partLim);
            dictionary ??= new Dictionary<Int2D, long>();

            if (dictionary.ContainsKey(dictKey))
                return dictionary[dictKey];
            if (amount == 0) return 1;
            if (partLim == 0 || amount < 0) return 0;

            long result = Part(amount - partLim, partLim, dictionary) + Part(amount, partLim - 1, dictionary);
            dictionary.Add(dictKey, result);
            return result;
        }

        public static long Min(long x, long y) => x < y ? x : y;

        public static long Max(long x, long y) => x > y ? x : y;

        public static int[] RandArr(dynamic len)
        {
            int[] numbs = new int[len];
            for (int cyc = 0; cyc < len; cyc++)
            {
                bool copyB;
                do
                {
                    int randNumb = rand.Next(0, len);
                    copyB = false;
                    for (int idx = 0; idx < cyc; idx++)
                        if (numbs[idx] == randNumb && idx != cyc)
                        {
                            copyB = true;
                            break;
                        }
                    numbs[cyc] = randNumb;
                } while (copyB);
            }
            return numbs;
        }

        public static float Sqr(float x) => (float)Pow(x, 2);

        public static long FibMemo(long n, Dictionary<long, long> memo = null)
        {               
            memo ??= new();
            
            if (n == 0 || n == 1) return 1;
            if (memo.ContainsKey(n)) return memo[n];
            long memoPiece = FibMemo(n - 2, memo) + FibMemo(n - 1, memo);
            memo.Add(n, memoPiece);

            return memoPiece;
        }

        public static IEnumerable<int> Power(int number, int exponent)
        {
            int result = 1;
            
            for (int i = 0; i < exponent; i++, result *= number)
                yield return result;
        }

    }


}

