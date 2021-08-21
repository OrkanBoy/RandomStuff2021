//all namespaces used
using System; using System.Collections.Generic; using System.Linq; using System.Text; using System.Threading; using System.Collections; using System.Diagnostics;
using LearningCSharpPart2.Tools; using LearningCSharpPart2.Classes;

//all aliases used
using Int2D = System.ValueTuple<int, int>;

//all static classes used
using static System.Console; using static System.MathF; using static System.Array; using static System.Convert;
using static LearningCSharpPart2.Tools.GenTools; using static LearningCSharpPart2.Tools.MathTools; using static LearningCSharpPart2.Tools.HuffTools; using static LearningCSharpPart2.Tools.TypeTools; using static LearningCSharpPart2.Tools.Tools;
namespace LearningCSharpPart2.Tools
{
    
    static class HuffTools {

        public static string ToHuffCode<T>(this IEnumerable<T> iEnum)
        {
            Dictionary<T, string> huffDict = iEnum.GetHuffDict();
            string huffStr = "";
            iEnum.ForEach(elem => huffStr += huffDict[elem]);
            return huffStr;
        }

        public static Dictionary<T, string> ToHuffDict<T>(this Tree<T> huffedTree) {
            Dictionary<T, string> dictionary= new ();
            huffedTree.AllLeaves.ForEach(Leaf => {
                int[] LeafArrIdx = huffedTree.ArrIndexOf(Leaf);
                string LeafHuffKey = "";
                LeafArrIdx.ForEach(leftOrRight => LeafHuffKey += leftOrRight);
                dictionary.Add(Leaf.Conts, LeafHuffKey);
            });
            return dictionary;
        }

        public static Dictionary<T, string> GetHuffDict<T>(this IEnumerable<T> iEnum) => iEnum.GetHuffTree().ToHuffDict();

        public static Tree<T> GetHuffTree<T>(this IEnumerable<T> iEnum)
        {
            SetList<HuffTree<T>> setList = new();
            HuffTree<T> huffTree = null;
            iEnum.ForEach(elem => setList.Add(new HuffTree<T>(elem, iEnum.FreqOf(elem))));
            setList.SortByFreq();
            while (setList.Count > 0)
            {
                huffTree = new HuffTree<T>(setList.Count > 1 ? new[] { setList[0], setList[1] } : new[] { setList[0] });
                setList.RemoveRange(0, huffTree.Children.Length);
                if (setList.Count > 0)
                    setList.Add(huffTree);
                setList.SortByFreq();
            }
            return huffTree;
        }

        internal static void SortByFreq<T>(this List<HuffTree<T>> huffTree)
        {
            int swaps;
            do
            {
                swaps = 0;
                for (int i = 1; i < huffTree.Count; i++)
                    if (huffTree[i].Freq < huffTree[i - 1].Freq)
                    {
                        HuffTree<T> temp = huffTree[i - 1];
                        huffTree[i - 1] = huffTree[i];
                        huffTree[i] = temp;
                        swaps++;
                    }

            } while (swaps > 0);
        }
    }

    static class MathTools {
        public static int CloserTo(this int myInt, int intA, int intB) => Dst(myInt, intA) < Dst(myInt, intB) ? intA : intB;

        public static bool InBoundsInc(this float myVal, float bound1, float bound2) => myVal <= bound1 && myVal >= bound2 || myVal >= bound1 && myVal <= bound2;

        public static bool InBoundsExc(this float myVal, float bound1, float bound2) => myVal < bound1 && myVal > bound2 || myVal > bound1 && myVal < bound2;

        public static float Dst(dynamic pointA, dynamic pointB)
        {
            if (((object)pointA).IsMultNumb() && ((object)pointB).IsMultNumb())
                return MultDst((float[])pointA, (float[])pointB);
            if (((object)pointA).IsNumb() && ((object)pointB).IsNumb())
                return SingDst((float)pointA, (float)pointB);
            throw new InvalidOperationException();
        }

        private static float SingDst(float numbA, float numbB) => Abs(numbA - numbB);

        private static float MultDst(float[] multNumbA, float[] multNumbB)
        {

            if (!multNumbA.IsMultNumb() || !multNumbB.IsMultNumb() || (multNumbA.Length() != multNumbB.Length()) || (multNumbA.Dim() > 1) || (multNumbB.Dim() > 1))
                throw new InvalidOperationException();
            float[] result = Minus(multNumbA, multNumbB).ToArray();
            for (int i = 0; i < multNumbA.Length; i++)
                result[i] = (float)Pow(result[i], 2);
            return (float)Sqrt(result.Sum());
        }

        public static bool InBounds(this float myVal, float bound1, float bound2) => myVal <= bound1 && myVal >= bound2 || myVal >= bound1 && myVal <= bound2;

    }

    static class GenTools {
        public static Random rand = new Random();

        public static MailService mailServ = new MailService();

        public static void print(object output = null) => WriteLine($"{output}");

        public static string input(object message = null)
        {
            Write($"{message}");
            return ReadLine();
        }

        public static string mult(this string myStr, int rep)
        {
            string output = "";
            for (int cyc = 0; cyc < rep; cyc++) output += myStr;
            return output;
        }

        public static string Indent(this string myStr, string indent = "    ")
        {
            string indentedStr = "\n" + indent;
            for (int cyc = 0; cyc < myStr.Length; cyc++)
            {
                char myChar = myStr[cyc];
                indentedStr += myChar;
                if (myChar == '\n')
                    indentedStr += indent;
            }
            return indentedStr;
        }
    }

    static class Tools
    {
        public static IEnumerable<T> Cleaned<T>(this IEnumerable<T> iEnum) => iEnum.Where(elem => elem != null);

        public static IEnumerable Cleaned(this IEnumerable iEnum) {
            foreach (var elem in iEnum)
                if (elem != null)
                    yield return elem;
        }

        public static object At(this IEnumerable iEnumerable, int index)
        {
            IEnumerator iEnumerator = iEnumerable.GetEnumerator();
            while (index > 0)
                iEnumerator.MoveNext();
            return iEnumerator.Current;
        }

        public static int FreqOf<T>(this IEnumerable<T> iEnum, T target)
        {
            int freq = 0;
            foreach (T elem in iEnum)
                if (target.equals(elem))
                    freq++;
            return freq;
        }

        public static int FreqOf(this IEnumerable iEnum, object target) {
            int freq = 0;
            foreach (object elem in iEnum)
                if (target.equals(elem))
                    freq++;
            return freq;
        }

        public static IEnumerable<float> Minus(float[] arr1, float[] arr2)
        {
            for (int i = 0; i < arr1.Length; i++)
                yield return arr1[i] - arr2[i];
        }

        public static IEnumerable<float> Plus(float[] arr1, float[] arr2) {
            for (int i = 0; i < arr1.Length; i++)
                yield return arr1[i] + arr2[i];
        }

        public static int Length(this object obj) => obj.IsSeq() ? (int)((IEnumerable)obj).Size() : 0;

        //meh, kinda working but shittily :\
        public static dynamic MakeDeepDefault(int[] dims, object defVal, byte layer = 0)
        {
            dynamic result;
            if (layer == dims.Length)
                result = defVal;
            else
            {
                result = new object[dims[layer]];
                for (int i = 0; i < result.Length; i++)
                    result[i] = MakeDeepDefault(dims, defVal, (byte)(layer + 1));
            }
            return result;
        }

        public static T[][] ToJag<T>(this T[,] multArr) {

            T[][] jagArr = new T[multArr.GetLength((0))][];
            for (int x = 0; x < multArr.GetLength(0); x++)
            {
                jagArr[x] = new T[multArr.GetLength((1))];
                for (int y = 0; y < multArr.GetLength(1); y++)
                {
                    jagArr[x][y] = multArr[x, y];
                }
            }
            return jagArr;
        }

        public static T Last<T>(this IEnumerable<T> sequence)
        {
            T[] arr = (T[])sequence;
            return arr[^1];
        }

        public static string ToStringSeq(this IEnumerable sequence, string indent = "  ", int layer = 0) {
            if (sequence == null) return $"null";
            //seqQ is the result if something is a sequence or not, sequence-Question: seq-Q
            bool seqQ = false;
            string output = "{";
            layer++; 
            foreach (var element in sequence) {
                if (output != "{") output += ",";
                if (seqQ == true) output += "\n";
                if (element.IsSEQ())
                {
                    string stringedSubSeq = ToStringSeq((IEnumerable)element, indent.mult(layer), layer);
                    //prevent indentation on first elment
                    if (seqQ == false) output += stringedSubSeq;
                    else output += indent + stringedSubSeq;
                    seqQ = true;
                }
                else
                {
                    output += element.ToVisString();
                }
                
            }

            return output+"}";

            

        }

        public static bool sequenceEqual(this IEnumerable iE1, IEnumerable iE2) {
            //special cases where Nulls and IEnums with Length 0 are involved
            if (iE1 == null && iE2 == null) return true;
            if ((iE1 == null) != (iE2 == null)) return false;
            if (iE1.GetType().Equals(iE2.GetType()) && iE1.Size() == 0 && iE2.Size() == 0) return true;

            IEnumerator iEr1 = iE1.GetEnumerator();
            IEnumerator iEr2 = iE2.GetEnumerator();
            iEr1.MoveNext();
            iEr2.MoveNext();
            bool hasNext = true;

            while (hasNext)
            {
                if (!iEr1.Current.equals(iEr2.Current))
                    return false;
                hasNext = iEr1.MoveNext();
                if (hasNext != iEr2.MoveNext())//if ShadowEye were to directly compare iEr1.MoveNext() it would cause the enumerator to move to the next element, so instyead ShadowEye stored it
                    return false;
            }
            return true;
        }//credits to shadowEye

        public static bool equals(this object obj1, object obj2)
        {/*for null case*/
            if (obj1 == obj2)
                return true;
            else if (obj1 == null || obj2 == null)
                return false;
            else return (obj1.IsSEQ() && obj2.IsSEQ() ? ((IEnumerable)obj1).sequenceEqual((IEnumerable)obj2) : obj1.Equals(obj2));
        }

        public static bool IsPlain<T>(this IEnumerable<T> sequence) => sequence.Size() == 0 || !sequence.Any(element => !element.Equals(sequence.First())) || sequence == null; 

        public static bool IsSEQ(this object obj) => !obj.IsString() && obj.IsSeq();

        public static void ForEach<T>(this IEnumerable sequence, Action<T> action) { foreach (T element in sequence) action(element); }

        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action) { foreach (T element in sequence) action(element); }

        public static string ToVisString(this object obj, bool seqDetect = false) {
            if (obj == null) return $"null";
            if (seqDetect && obj.IsSEQ()) return ((IEnumerable)obj).ToStringSeq(); 
            if (obj.IsString()) return $"\"{obj}\"";
            if (obj.IsChar()) return $"\'{obj}\'";
            if (obj.ToString().Contains('\n')) return $"{obj.ToString().Indent()}";
            if (obj.Is<float>()) return $"{obj}f";
            if (obj.Is<long>()) return $"{obj}L";
            if (obj.Is<double>()) return $"{obj}M";
            return $"{obj}";
        }

        public static void ForAll<T>(this object obj, Action<T> action) {
            if (obj.Is<T>())
                action((T)obj);
            else
                foreach (object elem in (IEnumerable)obj)
                    ForAll(elem, action);
        }

        public static IEnumerable<T> intersect<T>(this IEnumerable<T> IE1, object IE2) => IE1 == null || IE2 == null ? null : IE2.IsSeq() ? IE1.Intersect((T[])IE2) : IE1.Intersect(new T[]{ (T)IE2 });

        public static IEnumerable<T> union<T>(this IEnumerable<T> IE1, object IE2) { 
            if (IE1 == null && IE2 == null) return null;
            else if (IE1 == null && IE2 != null)
            {
                if (IE2.IsSeq()) return (T[])IE2;
                else return new T[] { (T)IE2 };
            }
            else if (IE1 != null && IE2 == null) return IE1; 
            else if (IE2.IsSeq()) return IE1.Union((T[])IE2);
            return IE1.Union(new T[] { (T)IE2 });
        }

        public static bool IsInAscOrder(this int?[] seq)
        {
            if (seq.Length > 1)
            {
                int? prevValidVal = null;
                for (int i = 1; i < seq.Length; i++)
                {
                    prevValidVal = seq[i - 1] != null ? seq[i - 1] : prevValidVal;
                    if (seq[i] != null && seq[i] < prevValidVal)
                        return false;
                }
                return true;
            }
            return true;

        }

        public static int IndexOf(this IEnumerable sequence, object target) {
            int cyc = 0;
            foreach (var elem in sequence) {
                if (target.equals(elem))
                    return cyc;
                cyc++;
            }
            return -1;
        }

        public static seqT MakeDefaultArray<seqT>(int[] dimensions, object defaultVal, int start = 0)
        {
            //creates the nth dimension of elemnts
            object[] myArr = new object[dimensions[start]];

            //loops through each element to see if it has to be filled with more sequences or just the defualtVal
            for (int cyc = 0; cyc < myArr.Length; cyc++)
            {
                //singulairty has been reached
                if (start+1 == dimensions.Length)
                    myArr[cyc] = defaultVal; //fill with default object
                //more seq nesting required
                else
                    myArr[cyc] = MakeDefaultArray<object>(dimensions, defaultVal, start+1); //manage the (n+1)thdimesion for each of the elements
            }

            return (seqT)(object)myArr;
        }

        public static int? Size(this object sequence) => sequence.IsSEQ() ? ((ICollection)sequence).Count : null;

        public static int Dim(this object sequence, int layer = 1)
        {//assuming its a standard sequence and assuming all of its dimensions are properly filled and not a IEnumerable<object>
            if (sequence == null || !sequence.IsSEQ())
                return 0;
            int deepestDim = 0;
            foreach (object element in (IEnumerable)sequence)
            {
                try
                {
                    int curDim = Dim((IEnumerable)element, layer+1);
                    if (element.IsSEQ() && curDim > deepestDim)
                        deepestDim = curDim;
                }
                catch (Exception) { deepestDim = layer; }
            }
            return deepestDim;
        }
    }

    static class TypeTools {
        public static bool IsPoint(this object obj) => obj.IsMultNumb() || obj.IsNumb();

        public static bool IsMultNumb(this object obj)
        {
            return obj.Is<IEnumerable<byte>>() || obj.Is<IEnumerable<sbyte>>() || obj.Is<IEnumerable<short>>() || obj.Is<IEnumerable<ushort>>() ||
                obj.Is<IEnumerable<int>>() || obj.Is<IEnumerable<uint>>() || obj.Is<IEnumerable<long>>() || obj.Is<IEnumerable<ulong>>() ||
                obj.Is<IEnumerable<float>>() || obj.Is<IEnumerable<double>>() || obj.Is<IEnumerable<decimal>>();
        }

        public static bool IsNumb(this object obj) => obj.Is<byte>() || obj.Is<sbyte>() || obj.Is<short>() || obj.Is<ushort>() || obj.Is<int>() || obj.Is<uint>() || obj.Is<long>() || obj.Is<ulong>() || obj.Is<float>() || obj.Is<double>() || obj.Is<decimal>();

        public static bool Is(this object obj, Type typeObj) => typeObj.IsInstanceOfType(obj);

        public static bool Is<T>(this object obj) => typeof(T).IsInstanceOfType(obj);

        public static bool IsSeq(this object obj) => typeof(IEnumerable).IsInstanceOfType(obj);

        public static bool IsString(this object obj) => typeof(string).IsInstanceOfType(obj);

        public static bool IsChar(this object obj) => typeof(char).IsInstanceOfType(obj);

        public static bool IsDerivedFrom(this Type baseTypeObj, Type derivedTypeObj) => baseTypeObj.IsAssignableFrom(derivedTypeObj);

        public static bool IsDerivedFrom<BT, DT>() => typeof(BT).IsAssignableFrom(typeof(DT));

        public static bool IS<T>(this object obj) => obj.GetType() == typeof(T);

        public static bool IS(this object obj, Type typeObj) => obj.GetType() == typeObj;
    }
}
