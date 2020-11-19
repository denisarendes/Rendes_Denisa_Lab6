using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rendes_Denisa_Lab6
{
    //Validare pt camp required
    public class StringNotEmpty : ValidationRule {
        //se mosteneste din clasa ValidationRule; se suprascrie metoda Validate ce returneaza un Validation Result
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) {
            string aString = value.ToString();
            if (aString == "")
                return new ValidationResult(false, "String cannot be empty");
            return new ValidationResult(true, null);
        }
    }

    //Validare pentru lungime minima string
    public class StringMinLenghtValidator : ValidationRule {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string aString = value.ToString();
            if (aString.Length < 3)
                return new ValidationResult(false, "String must have at least 3 characters!");
            return new ValidationResult(true, null);
        }
    }
}
