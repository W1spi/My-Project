using static System.Console;

namespace ConsoleTestApp;

public class Program
{
    static void Main(string[] args)
    {
        var a = new A();
        WriteLine(a);
        WriteLine(typeof(A));
        WriteLine(sizeof(int));
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