using EventManager.WebAPI.Model;
using FluentValidation;

namespace EventManager.WebAPI.Validators
{
    /// <summary>
    /// Fluent validation logic for <see cref="Job"/> objects.
    /// </summary>
    public class JobValidator : AbstractValidator<Job>
    {
        /// <summary>
        /// Rules
        /// </summary>
        public JobValidator()
        {
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.UserName).NotEmpty();
            RuleFor(e => e.Data).NotEmpty();
        }
    }
}