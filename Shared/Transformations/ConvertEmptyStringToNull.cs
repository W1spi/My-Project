using System.Linq;

namespace LibraryOfUsefulClasses.Transformations;

/// <summary>
/// Метод расширения для обработки открытых свойств типа.
/// Осуществляется проход по всем открытым свойствам типа,
/// после чего всем свойствам, принимающим пустую строку 
/// (<c>string.IsNullOrEmtpy(propValue)</c>),
/// присваивается значение <c>null</c>
/// </summary>
public static class ConvertEmptyStringToNull
{
    public static object EmptyToNull(this object obj)
    {
        var prop = obj.GetType().GetProperties();

        for (int i = 0; i < prop.Length; i++)
        {
            string propertyName = prop[i].Name;
            var propertyValue = prop[i].GetValue(obj);

            if (prop[i].PropertyType == typeof(string) && string.IsNullOrEmpty((string)propertyValue!) && prop[i].CanWrite)
            {
                prop[i].SetValue(obj, null);
            }
        }

        return obj;
    }
}
