using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Library.Infastructure.Validation
{
     /// <summary>
     /// Класс правила валидации текста.
     /// </summary>
    public class TextValidationRule : ValidationRule
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public bool IsNullorEmpty { get; set; }

        public TextValidationRule()
        {
            this.MinLength = 1;
            this.MaxLength = 10;
            this.IsNullorEmpty = false;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;           

            if(string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                if (IsNullorEmpty) return ValidationResult.ValidResult;
                else return new ValidationResult(false, "Поле не может быть пустым!");
            }

            if (input.Length < MinLength || input.Length > MaxLength)
            {
                return new ValidationResult(false, $"Поле должно быть не короче {MinLength} " +
                    $"и не длиннее {MaxLength} символов!");
            }

            return ValidationResult.ValidResult;
        }
    }
}
