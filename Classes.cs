//all namespaces used
using System; using System.Collections.Generic; using System.Linq; using System.Text; using System.Threading; using System.Collections; using System.Diagnostics; using System.Runtime.CompilerServices;
using LearningCSharpPart2.Tools;

//all aliases used
using Int2D = System.ValueTuple<int, int>;

//all static classes used
using static System.Console; using static System.MathF; using static System.Array; using static System.Convert;
using static LearningCSharpPart2.Tools.GenTools; using static LearningCSharpPart2.Tools.MathTools; using static LearningCSharpPart2.Tools.HuffTools; using static LearningCSharpPart2.Tools.TypeTools; using static LearningCSharpPart2.Tools.Tools;

namespace LearningCSharpPart2.Classes
{
    using static LearningCSharpPart2.Tools.Tools;



    public class SetList<T> : List<T> {

        public SetList() { new List<T>(); }

        public SetList(IEnumerable<T> collection) { new List<T>(collection); }

        public SetList(int capacity) => new List<T>(capacity);


        public new void Add(T addition) { if (!Contains(addition)) base.Add(addition); }

        public new T this[int index] {

            get => base[index];
            set { if (!Contains(value)) base[index] = value; }
        }

    }


    public class Person<T> {


        public T conts;

        public string ContsStr => conts.ToVisString();

        private SetList<Person<T>> children;
        public SetList<Person<T>> Children {
            get => children;
            set {
                //parent cannot be child
                if (parent != null && value.Contains(parent)) throw new ArgumentException();
                //sets child relation
                children = value;
                if (children != null) children.ForEach(child => child.parent = this);
            }
        }
        private Person<T> parent;
        public Person<T> Parent
        {
            get => parent;
            set
            {
                //child cannot be parent
                if (children != null && children.Contains(value)) throw new ArgumentException();
                //sets parent relation
                //A = this node, B = value or parent
                //B becomes A's parent
                parent = value;
                if (parent != null) {
                    //is parents.children was null you wouldnt be able to add any members
                    if (parent.children == null) parent.children = new SetList<Person<T>>();
                    //A is B's child
                    parent.children.Add(this);
                }
            }
        }




        public Person<T> this[int index] => Children[index];
        public bool IsSuccessor(Person<T> targetPredecessor) {
            if (parent == targetPredecessor) return true;
            //asks the parent if the targetPredecessor is their parent(parent's parent...)
            if (parent != null) return parent.IsSuccessor(targetPredecessor);
            return false;
        }

        public bool IsPredecessor(Person<T> targetSuccessor) => targetSuccessor.IsSuccessor(this);

        public static explicit operator T(Person<T> node) => node.conts;



        public IEnumerable<Person<T>> Neighbours => children.union(parent);


        public Person<T> AbsAnscestor {
            get {
                Person<T> curNode = this;
                while (curNode.parent != null)
                    curNode = curNode.parent;
                return curNode;
            }
        }
        public static implicit operator Person<T>(T info) => new Person<T>(info);

        public static explicit operator string(Person<T> node) => node.conts.ToString();
        public void ShowTree() => print(AbsAnscestor);

        public string ToStringShort() {
            if (this == null) return null;
            string childrenStr = children == null ? "null" : children.Count == 0 ? "" : "...";
            return "{" + $"conts: {conts.ToVisString()}\nchildren: {childrenStr}" + "}";
        }
        public void ShowNeighbours() {
            string nghsStr = "{";
            foreach (Person<T> neighbour in Neighbours)
            {
                if (nghsStr != "{") nghsStr += ",\n" + neighbour.ToStringShort();
                else nghsStr += neighbour.ToStringShort();
            } nghsStr += "}";
            print("{" + $"conts: {conts.ToVisString()}\nneighbours: {nghsStr.Indent()}" + "}");
        }
        public override string ToString() => $"conts: {conts.ToVisString()}\nchildren: {children.ToStringSeq()}" ?? null;

        public event EventHandler<NodeEventArgs<T>> NodeCreated;

        protected virtual void OnNodeCreated(Person<T> node)
        {
            if (NodeCreated != null) NodeCreated(this, new NodeEventArgs<T> { Node = node });
        }

        public Person(object givenConts = null, Person<T> givenParent = null, object givenChidren = null)
        {
            conts = (T)givenConts;


            Parent = givenParent;

            if (givenChidren.IsSeq()) Children = (SetList<Person<T>>)givenChidren;
            else if (givenChidren != null) Children = new SetList<Person<T>>() { (Person<T>)givenChidren };

            //NodeCreated += mailservice.OnNodeCreated;
            //OnNodeCreated(this);

        }

    }
    public class NodeEventArgs<T> : EventArgs
    {
        public Person<T> Node { get; set; }
    }

    public class Video
    {
        public string Title { get; set; }
    }
    public class VideoEventArgs : EventArgs
    {
        public Video video { get; set; }
    }
    public class VideoEncoder
    {
        //public delegate void VideoEncodedEventHandler(object source, VideoEventArgs args);
        //public event VideoEncodedEventHandler VideoEncoded;


        public event EventHandler<VideoEventArgs> VideoEncoded;


        public void Encode(Video video)
        {
            print("Encoding the video named: " + video.Title);
            Thread.Sleep(2000);
            VideoEncoded += mailServ.OnVideoEncoded;
            OnVideoEncoded(video);
        }

        protected virtual void OnVideoEncoded(Video encodedVideo)
        {
            if (VideoEncoded != null) VideoEncoded(this, new VideoEventArgs { video = encodedVideo });
        }
    }
    public class MailService
    {

        public Action action;

        public void OnNodeCreated<T>(object source, NodeEventArgs<T> args)
        {
            print("Sending mail... to: " + args.Node.ToStringShort());
        }
        public void OnVideoEncoded(object source, VideoEventArgs args)
        {
            print("Sending mail... Requested from sent video: " + args.video.Title);
        }
    }


    public class Tree<T>
    {
        protected object conts;

        public readonly byte splitSize;

        public virtual Tree<T>[] Children
        {

            get => IsParent() ? (Tree<T>[])conts : null;
            set => conts = value;
        }

        public T Conts
        {
            get => (T)conts;
            set => conts = value;
        }


        //Indexers:

        //Standard Indexing
        public virtual Tree<T> this[int index] {
            get => Children.Length > 0 ? Children[index] : null;
            set => Children[index] = value;
        }
        //Array Indexing
        public virtual Tree<T> this[int[] depthPath, int curDepth = 0]
        {
            get {
                if (depthPath.Length == 0) return this;
                if (depthPath.Length > Depth) return null;
                if (depthPath.Length == curDepth + 1) return this[depthPath[curDepth]];
                return this[depthPath, curDepth + 1][depthPath[curDepth]];
            }
            set {
                if (depthPath.Length == curDepth + 1) this[depthPath[curDepth]] = value;
                this[depthPath, curDepth + 1][depthPath[curDepth]] = value;
            }
        }
        //Tuple Indexing
        public virtual Tree<T> this[ITuple depthPath, int curDepth = 0] {
            get
            {
                if (depthPath.Length == 0) return this;
                if (depthPath.Length > Depth) return null;
                if (depthPath.Length == curDepth + 1) return this[(int)depthPath[curDepth]];
                return this[depthPath, curDepth + 1][(int)depthPath[curDepth]];
            }
            set {
                if (depthPath.Length == curDepth + 1) this[(int)depthPath[curDepth]] = value;
                this[depthPath, curDepth + 1][(int)depthPath[curDepth]] = value;
            }
        }

        //Constructors:
        public Tree(object conts = null, byte splitSize = 2)
        { 
            this.conts = conts;
            this.splitSize = splitSize;
        }

        public bool IsParent() => conts.Is<IEnumerable<Tree<T>>>(); //``Children != null`` doesnt work, investigate why

        public void Split(byte splitDepth = 1, object storedConts = null)
        {
            storedConts ??= conts;
            if (splitDepth == 0)
                conts = storedConts;
            else {
                Children = new Tree<T>[splitSize];
                for (int i = 0; i < Children.Length; i++) {
                    Children[i] = new Tree<T>(default, splitSize);
                    Children[i].Split((byte)(splitDepth - 1), storedConts);
                }
            }
        }

        //Bottotm to Top Retracting
        public void Retract(int unsplitDepth = -1) {
            if (IsParent()) {
                //base case:
                if (Children.IsPlain()) {
                    conts = GoToChild(Depth < unsplitDepth ? Depth : unsplitDepth).conts;
                    return;
                }

                //recursive case:
                Children.ForEach(Child => Child.Retract(unsplitDepth));
            }
        }

        //Top to Bottom Unsplitting
        public void UnSplit(sbyte unsplitDepth = -1) {

            if ((unsplitDepth < 0 || unsplitDepth > 0) && IsParent() && Children.IsPlain())
            {
                if (Children.Length == 0)
                    conts = null;
                else
                    conts = TreeTip.conts;
            }
            //recursive case:
            else if ((unsplitDepth > 1 || unsplitDepth < 0) && Children != null)
                Children.ForEach(Child => Child.UnSplit((sbyte)(unsplitDepth - 1)));
        }

        public virtual Tree<T> GoToChild(int curDepth = 1, int? targetDepth = null)
        {
            targetDepth ??= Depth;
            if (curDepth == 0 || !IsParent())
                return this;
            Tree<T> bestChild = null;
            if (curDepth <= targetDepth)
            {
                foreach (var Child in Children)
                {
                    bestChild ??= Child;
                    if (curDepth.CloserTo((int)targetDepth - Child.Depth, (int)targetDepth - bestChild.Depth) == targetDepth - Child.Depth)
                        bestChild = Child.GoToChild(curDepth - 1, targetDepth);
                }
            }
            return bestChild;
        }

        public static explicit operator T(Tree<T> tree) => tree.Conts;

        public static implicit operator Tree<T>(T val) => new(val);

        protected Tree<T> TreeTip => GoToChild(-1);

        public IEnumerable<T> AllLeavesConts {
            get
            {
                if (!IsParent()) yield return Conts;
                else foreach (Tree<T> Child in Children)
                        foreach (T ChildLeaf in Child.AllLeavesConts)
                            yield return ChildLeaf;
            }
        }

        public virtual IEnumerable<Tree<T>> All {
            get {
                yield return this;
                if (IsParent()) foreach (Tree<T> Child in Children)
                        foreach (Tree<T> ChildPred in Child.All)
                            yield return ChildPred;
            }
        }

        public virtual IEnumerable<Tree<T>> AllLeaves {
            get {
                if (!IsParent()) yield return this;
                else foreach (Tree<T> Child in Children)
                        foreach (Tree<T> ChildLeaf in Child.AllLeaves)
                            yield return ChildLeaf;
            }
        }

        public int Depth {
            get
            {
                if (!IsParent())
                    return 0;
                int deepestDepth = 0;
                foreach (var Child in Children)
                {
                    int curDim = 1 + Child.Depth;
                    if (curDim > deepestDepth)
                        deepestDepth = curDim;
                }
                return deepestDepth;
            }

        }

        public int LeafCount => !IsParent() ? 1 : Children.Sum(Child => Child.LeafCount);

        public int[] ArrIndexOf(Tree<T> target) {
            for (int i = 0; i < Children.Length; i++) {
                Tree<T> Child = Children[i];
                if (Child.Contains(target))
                {
                    if (Child.Depth == 0)
                        return new []{ i };
                    else
                        return Child.ArrIndexOf(target).Concat(new int[] { i }).ToArray();
                }
            }
            return null;
        }

        public bool Contains(Tree<T> target) => AllLeaves.Contains(target); //should check the time/space complexities

        public override string ToString() => "conts: " + (Children != null ? "\n" : "") + $"{conts.ToVisString(true)}" ?? null;

        public override bool Equals(object otherObj)
        {
            try
            {
                Tree<T> otherTree = (Tree<T>)otherObj;
                if (!otherTree.IsParent() && !IsParent())
                    return otherTree.conts.equals(conts);
                else
                    return Children.sequenceEqual(otherTree.Children);
            }
            catch (Exception) { return false; }
        }

    }
    sealed class HuffTree<T> : Tree<T>{
        public new HuffTree<T>[] Children
        {
            get => IsParent() ? (HuffTree<T>[])conts : null;
            set => conts = value;
        }

        private readonly int freq;

        public int Freq => IsParent() ? Children.Sum(Child => Child.freq) : freq;

        public HuffTree(object conts = null, int freq = -1) : base(conts) {
            this.freq = freq == -1 ? Freq : freq;
        }

    }


}
