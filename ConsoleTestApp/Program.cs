using LibraryOfUsefulClasses.Transformations;

namespace ConsoleTestApp;

public class Program
{
    static void Main(string[] args)
    {
        var testObj = new
        {
            FirstName = "Viktor",
            Surname = "Testovich",
            Age = 15,
            Patronymic = "",
            City = ""
        };

        TestObg pers = new("Viktor", "Petrovich", 15, "", "");
        pers.EmptyToNull();
    }
}

public class TestObg
{
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public int Age { get; set; }
    public string Patronymic { get; set; }
    public string City { get; set; }

    public TestObg(string firstName, string surname, int age, string patronymic, string city)
    {
        FirstName = firstName;
        Surname = surname;
        Age = age;
        Patronymic = patronymic;
        City = city;
    }
}