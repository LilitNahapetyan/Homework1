using System.ComponentModel.DataAnnotations;

namespace Homework1
{
    public class PastDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date < DateTime.Today;
            }
            return false;
        }
    }

}
