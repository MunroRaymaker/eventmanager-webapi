using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventManager.WebAPI.Model
{
    public class IntArrayToStringValueConverter : ValueConverter<int[], string>
    {
        public IntArrayToStringValueConverter() : base(le => ArrayToString(le), s => StringToArray(s))
        {
        }

        public static string ArrayToString(int[] value)
        {
            if (value == null || !value.Any()) return null;
            var s = string.Join(",", value);
            return s;
        }

        public static int[] StringToArray(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            var arr = value.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
            return arr;
        }
    }
}