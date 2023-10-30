using System.ComponentModel.DataAnnotations;

namespace LoginAndRegistration.Models;
public class EmailUnico : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("El correo es requerido");
        }
        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        if (_context.users.Any(e => e.Email == value.ToString()))
        {
            return new ValidationResult("El correo debe ser unico");
        }
        else
        {
            return ValidationResult.Success;
        }
    }
}