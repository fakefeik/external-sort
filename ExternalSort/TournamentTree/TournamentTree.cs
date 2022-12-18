using System.Collections.Generic;
using System.Linq;

namespace ExternalSort.TournamentTree;

public class TournamentTree<T>
{
    private readonly IComparer<T?> comparer;
    private readonly List<List<TreeNode<T>>> nodes;
    private int missingLeafIndex = -1;

    public TournamentTree(T[] data, IComparer<T?>? comparer = null)
    {
        this.comparer = comparer ?? Comparer<T?>.Default;
        nodes = new List<List<TreeNode<T>>> {data.Select(x => TreeNode.Create(x, -1)).ToList()};

        for (var level = 0; nodes[level].Count > 1; level++)
        {
            nodes.Add(new List<TreeNode<T>>());
            for (var i = 0; i < nodes[level].Count; i += 2)
            {
                var left = nodes[level][i].Value;
                if (nodes[level].Count <= i + 1)
                {
                    nodes[level + 1].Add(TreeNode.Create(left, i));
                    continue;
                }

                var right = nodes[level][i + 1].Value;
                nodes[level + 1].Add(
                    this.comparer.Compare(left, right) < 0
                        ? TreeNode.Create(left, i)
                        : TreeNode.Create(right, i + 1)
                );
            }
        }
    }

    public T? PopRoot()
    {
        var rootNodeValue = nodes[^1][0].Value;

        var level = nodes.Count - 1;
        var i = 0;
        var currentNode = nodes[level][i];

        while (level != 0)
        {
            i = currentNode.IndexAtBelowLevel;
            currentNode = nodes[--level][i];
        }

        missingLeafIndex = i;
        return rootNodeValue;
    }

    public void InsertLeaf(T? value)
    {
        nodes[0][missingLeafIndex] = TreeNode.Create(value, -1);

        var index1 = missingLeafIndex;
        for (var level = 0; level < nodes.Count - 1; level++)
        {
            var index2 = index1 - 1;
            var nextLevelIndex = index2 / 2;
            if (index1 % 2 == 0)
            {
                index2 = index1 + 1;
                nextLevelIndex = index1 / 2;
            }

            var element1 = nodes[level][index1].Value;
            if (nodes[level].Count <= index2)
            {
                nodes[level + 1][nextLevelIndex] = TreeNode.Create(element1, index1);
                index1 = nextLevelIndex;
                continue;
            }

            var element2 = nodes[level][index2].Value;
            nodes[level + 1][nextLevelIndex] =
                comparer.Compare(element1, element2) < 0
                    ? TreeNode.Create(element1, index1)
                    : TreeNode.Create(element2, index2);
            index1 = nextLevelIndex;
        }

        missingLeafIndex = -1;
    }

    public T?[] SortRest()
    {
        var sorted = new T?[nodes[0].Count];
        for (var i = 0; i < nodes[0].Count; i++)
        {
            sorted[i] = PopRoot();
            InsertLeaf(default);
        }

        return sorted;
    }
}
