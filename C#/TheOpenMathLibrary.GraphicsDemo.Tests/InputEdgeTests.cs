namespace TheOpenMathLibrary.GraphicsDemo.Tests;

[TestClass]
public class InputEdgeTests
{
    [TestMethod]
    public void IsTriggered_OnlyReturnsTrueOnRisingEdge()
    {
        InputEdge edge = new();

        Assert.IsFalse(edge.IsTriggered(false));
        Assert.IsTrue(edge.IsTriggered(true));
        Assert.IsFalse(edge.IsTriggered(true));
        Assert.IsFalse(edge.IsTriggered(false));
        Assert.IsTrue(edge.IsTriggered(true));
    }
}

