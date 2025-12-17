using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Employee;
using Application.Validators.Employees;
using FluentValidation.TestHelper;


namespace Stajyer_Projesi.Tests.Systems.Validations
{
    public class EmployeeValidationTests
    {
        private readonly EmployeeCreateValidator _validator;

        public EmployeeValidationTests()
        {
            _validator = new EmployeeCreateValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new EmployeeCreateDto { Email = "hatali mail" };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(employee => employee.Email);
        }

        [Fact]
        public void Should_Have_Error_When_FistName_Is_Empty()
        {
            var model = new EmployeeCreateDto { FirstName = "" };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                .WithErrorMessage("Ad alanı boş olamaz."); 
        }

        [Fact]
        public void Should_Have_Error_When_Data_Is_Valid()
        {
            var model = new EmployeeCreateDto
            {
                FirstName = "Ali",
                LastName = "Veli",
                Email = "ali@test.com",
                Phone = "5551112233"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
