namespace LibraryOfUsefulClasses.Extensions;

/// <summary>
/// Метод расширения для обработки открытых свойств типа.
/// Осуществляется проход по всем открытым свойствам типа.
/// Обновляются все открытые свойства, значения которых 
/// отличны от <c>null</c> или <c>string.Empty</c>
/// </summary>
public static class IgnoringEmptyParameters
{
    public static object IgnorEmptyParam(this object newObj, object oldObj)
    {
        var propNewObj = newObj.GetType().GetProperties();
        var propOldObj = newObj.GetType().GetProperties();

        for (int i = 0; i < propNewObj.Length; i++)
        {
            //string newPropertyName = propNewObj[i].Name;
            var newPropertyValue = propNewObj[i].GetValue(newObj);
            var oldPropertyValue = propOldObj[i].GetValue(oldObj);

            if (propNewObj[i].PropertyType == typeof(string) && string.IsNullOrEmpty((string)newPropertyValue!) && propNewObj[i].CanWrite)
            {
                propNewObj[i].SetValue(newObj, oldPropertyValue);
            }
        }

        return newObj;
    }
}
