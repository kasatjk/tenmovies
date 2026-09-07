using System.ComponentModel.DataAnnotations;

namespace tenmovies.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MyYearAttribute : ValidationAttribute
    {

        public override bool IsValid(object? value)
        {
            if (value is int year)
            {
                return year >= 1888 && year <= DateTime.Now.Year;
            }
            return false;
        }
    }
}
