

namespace ConsoleTestApp;

public class Program
{
    static void Main(string[] args)
    {
        var a = new A();

        var b = new B()
        {
            First = "A",
            Second = "B",
            Third = "C",
        };
    }
}

public class A
{
    public string First { get; set; }
    public string Second { get; set; }
}

public class B : A
{
    public string Third { get; set; }
}