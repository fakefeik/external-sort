namespace ExternalSort.TournamentTree;

public static class TreeNode
{
    public static TreeNode<T> Create<T>(T? value, int index)
    {
        return new TreeNode<T>(value, index);
    }
}

public class TreeNode<T>
{
    public TreeNode(T? value, int index)
    {
        Value = value;
        IndexAtBelowLevel = index;
    }

    public T? Value { get; }
    public int IndexAtBelowLevel { get; }
}
