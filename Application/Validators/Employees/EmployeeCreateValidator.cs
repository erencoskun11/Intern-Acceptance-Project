using Application.DTOs.Employee;
using FluentValidation;

namespace Application.Validators.Employees
{
    public class EmployeeCreateValidator : AbstractValidator<EmployeeCreateDto>
    {
        public EmployeeCreateValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad alanı boş olamaz.")
                .Length(2, 50).WithMessage("Ad 2 ile 50 karakter arasında olmalıdır.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad alanı boş olamaz.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");


            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("İşe başlama tarihi boş geçilemez.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("İşe başlama tarihi gelecekte olamaz.");


            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Telefon numarası zorunludur.")
                .Matches(@"^\d+$").WithMessage("Telefon sadece rakamlardan oluşmalıdır.")
                .MinimumLength(10).WithMessage("Telefon en az 10 haneli olmalıdır.");
        }
    }
}