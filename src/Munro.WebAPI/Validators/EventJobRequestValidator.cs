using EventManager.WebAPI.Model;
using FluentValidation;

namespace EventManager.WebAPI.Validators
{
    /// <summary>
    /// Fluent validation logic for <see cref="EventJobRequest"/> objects.
    /// </summary>
    public class EventJobRequestValidator : AbstractValidator<EventJobRequest>
    {
        /// <summary>
        /// Rules
        /// </summary>
        public EventJobRequestValidator()
        {
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.UserName).NotEmpty();
            RuleFor(e => e.Data).NotEmpty();
        }
    }

    /// <summary>
    /// Fluent validation logic for <see cref="EventJob"/> objects.
    /// </summary>
    public class EventJobValidator : AbstractValidator<EventJob>
    {
        /// <summary>
        /// Rules
        /// </summary>
        public EventJobValidator()
        {
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.UserName).NotEmpty();
            RuleFor(e => e.Data).NotEmpty();
        }
    }
}
