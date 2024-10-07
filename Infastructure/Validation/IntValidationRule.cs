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
    /// Класс валидации числовых значений.
    /// </summary>
    public class IntValidationRule : ValidationRule
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public bool IsNullorEmpty { get; set; }
        public IntValidationRule()
        {
            MinValue = 0;
            MaxValue = DateTime.Now.Year; ;
            IsNullorEmpty = false;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            if (IsNullorEmpty == false && (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input)))
            {
                return new ValidationResult(false, "Поле не может быть пустым!");
            }

            int inputInt;
            if (int.TryParse(input, out inputInt))
            {
                if (inputInt < MinValue || inputInt > MaxValue)
                    return new ValidationResult(false, $"Число должно быть от {MinValue}" +
                        $" и до {MaxValue}");
                else return ValidationResult.ValidResult;

            }

            return new ValidationResult(false, "Ошибка ввода!");
        }
    }
}
