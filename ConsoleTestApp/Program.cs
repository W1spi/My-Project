using static System.Console;

namespace ConsoleTestApp;

public class Program
{
    static void Main(string[] args)
    {
        double latitude = Convert.ToDouble(ReadLine()); // географическая широта
        double longitude = Convert.ToDouble(ReadLine()); // географическая долгота

        // Преобразование в прямоугольную проекцию Меркатора
        var mercator = MercatorProjection(latitude, longitude);
        WriteLine($"Меркатор: x = {mercator.Item1}, y = {mercator.Item2}");

        // Преобразование в прямоугольную проекцию Гаусса-Крюгера
        var gaussKrueger = GaussKruegerProjection(latitude, longitude);
        WriteLine($"Гаусс-Крюгер: x = {gaussKrueger.Item1}, y = {gaussKrueger.Item2}");
    }

    static Tuple<double, double> MercatorProjection(double latitude, double longitude)
    {
        double x = longitude;
        double y = Math.Log(Math.Tan(latitude * Math.PI / 180.0 / 2 + Math.PI / 4));

        return new Tuple<double, double>(x, y);
    }

    static Tuple<double, double> GaussKruegerProjection(double latitude, double longitude)
    {
        double lon0 = Math.Floor(longitude) + 3; // долгота центрального меридиана
        double x = Math.Floor((longitude - lon0) * 1000000); // координата x в метрах
        double y = Math.Floor(latitude * 1000000); // координата y в метрах

        return new Tuple<double, double>(x, y);
    }
}