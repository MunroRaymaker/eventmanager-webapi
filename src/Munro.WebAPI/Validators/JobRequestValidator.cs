using EventManager.WebAPI.Model;
using FluentValidation;

namespace EventManager.WebAPI.Validators
{
    /// <summary>
    /// Fluent validation logic for <see cref="JobRequest"/> objects.
    /// </summary>
    public class JobRequestValidator : AbstractValidator<JobRequest>
    {
        /// <summary>
        /// Rules
        /// </summary>
        public JobRequestValidator()
        {
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.UserName).NotEmpty();
            RuleFor(e => e.Data).NotEmpty();
        }
    }
}
