using EventManager.WebAPI.Model;
using FluentValidation;

namespace EventManager.WebAPI.Validators
{
    /// <summary>
    ///     Fluent validation logic for <see cref="EventJobRequest" /> objects.
    /// </summary>
    public class EventJobRequestValidator : AbstractValidator<EventJobRequest>
    {
        /// <summary>
        ///     Rules
        /// </summary>
        public EventJobRequestValidator()
        {
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.UserName).NotEmpty();
            RuleFor(e => e.Data).NotEmpty();
        }
    }
}