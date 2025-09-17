using FluentResults;
using FluentValidation;
using MediatR;

namespace SmartAgroPlan.BLL.PipelineBehaviour;

public class ValidationBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResultBase
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var errors = failures
                    .Select(f => new Error(f.ErrorMessage)
                        .WithMetadata("PropertyName", f.PropertyName)
                        .WithMetadata("AttemptedValue", f.AttemptedValue))
                    .ToList();

                return (dynamic)Result.Fail(errors);
            }
        }

        return await next(cancellationToken);
    }
}
