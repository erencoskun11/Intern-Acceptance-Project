using Application.DTOs.Assignment;
using FluentValidation;

namespace Application.Validators.Assignments
{
    public class AssignmentCreateValidator : AbstractValidator<AssignmentCreateDto>
    {
        public AssignmentCreateValidator()
        {
            RuleFor(x => x.InventoryItemId)
                .GreaterThan(0).WithMessage("Lütfen geçerli bir ürün seçiniz.");

         
            RuleFor(x => x)
                .Must(x => x.InternId.HasValue && !x.EmployeeId.HasValue || !x.InternId.HasValue && x.EmployeeId.HasValue)
                .WithMessage("Zimmet işlemi için SADECE BİR KİŞİ (Stajyer veya Personel) seçilmelidir.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Not alanı en fazla 500 karakter olabilir.");
        }
    }
}