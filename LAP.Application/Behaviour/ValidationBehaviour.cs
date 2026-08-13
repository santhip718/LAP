using FluentValidation;
using LAP.Application.Interface;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Behaviors;

/// <summary>Runs all FluentValidation validators for a request and throws on validation failure.</summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ICustomLogger<ValidationBehavior<TRequest, TResponse>> _logger;

    /// <summary>Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.</summary>
    /// <param name="validators">The collection of validators registered for the request type.</param>
    /// <param name="logger">Custom application logger.</param>
    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ICustomLogger<ValidationBehavior<TRequest, TResponse>> logger
    )
    {
        _validators = validators;
        _logger = logger;
    }

    /// <summary>Validates the request against all registered validators before passing it to the next pipeline step.</summary>
    /// <param name="request">The incoming MediatR request.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The response from the downstream pipeline.</returns>
    /// <exception cref="BadRequestException">Thrown when one or more validation rules fail.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                var description = string.Join(
                    "; ",
                    failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}")
                );

                _logger.LogError(
                    "Validation failed for request {RequestType}: {Description}",
                    typeof(TRequest).Name,
                    description
                );

                throw new BadRequestException("Validation failed.", description);
            }
        }

        return await next();
    }
}
