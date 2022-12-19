using ExternalSort.TournamentTree;
using NUnit.Framework;

namespace ExternalSort.Tests;

public class TournamentTreeTest
{
    [Test]
    public void Test1()
    {
        var exampleInput1 = new[] {5, 1, 9, 3};
        var tree1 = new TournamentTree<int>(exampleInput1);

        Assert.That(tree1.PopRoot(), Is.EqualTo(1));

        tree1.InsertLeaf(2);
        Assert.That(tree1.PopRoot(), Is.EqualTo(2));

        tree1.InsertLeaf(int.MaxValue);
        Assert.That(tree1.PopRoot(), Is.EqualTo(3));

        tree1.InsertLeaf(1);
        Assert.That(tree1.PopRoot(), Is.EqualTo(1));

        tree1.InsertLeaf(10);
        Assert.That(tree1.PopRoot(), Is.EqualTo(5));
    }

    [Test]
    public void Test2()
    {
        var exampleInput2 = new[] {"4. a", "3. a", "1. a", "2. a", "5. a"};
        var solution2 = new[] {"1. a", "2. a", "3. a", "4. a", "5. a"};
        var calculatedSolution2 = new TournamentTree<string>(exampleInput2, FastStringComparer.Instance).SortRest();

        Assert.That(calculatedSolution2, Is.EqualTo(solution2));
    }
}
