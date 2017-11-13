using Entitas;

[Game]
public sealed class PositionComponent : IComponent {

    public float x;
    public float y;
    public float z;

    public override string ToString() {
        return "Position(" + x + ", " + y + "," + z +")";
    }
}

[Game]
public sealed class TestComponent : IComponent
{
    public bool isTesting;

    public override string ToString()
    {
        return isTesting.ToString();
    }
}