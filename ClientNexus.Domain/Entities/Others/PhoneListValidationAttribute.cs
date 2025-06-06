using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClientNexus.Domain.Entities.Others
{
    public class PhoneListValidationAttribute : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var phoneNumbers = value as List<string>;

            if (phoneNumbers == null || phoneNumbers.Count == 0)
                return ValidationResult.Success;

            var regex = new System.Text.RegularExpressions.Regex(@"^\d{11}$");

            foreach (var phone in phoneNumbers)
            {
                if (string.IsNullOrWhiteSpace(phone) || !regex.IsMatch(phone))
                    return new ValidationResult($"The phone number '{phone}' is not valid.");
            }

            return ValidationResult.Success;
        }

    }
}
