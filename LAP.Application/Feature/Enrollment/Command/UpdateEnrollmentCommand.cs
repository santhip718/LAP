using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Enrollment;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;
using EnrollmentEntity = LAP.Domain.Entity.Enrollment;

namespace LAP.Application.Feature.Enrollment.Command;

/// <summary>
/// Command for updating an existing enrollment with the provided details.
/// </summary>
/// <param name="Id">The identifier of the enrollment to update.</param>
/// <param name="Dto">The enrollment update request data transfer object.</param>
public record UpdateEnrollmentCommand(Guid Id, UpdateEnrollmentRequestDto Dto)
    : IRequest<SuccessResponse>;

/// <summary>
/// Validates the <see cref="UpdateEnrollmentCommand"/> before it is handled.
/// </summary>
public class UpdateEnrollmentCommandValidator : AbstractValidator<UpdateEnrollmentCommand>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="UpdateEnrollmentCommand"/>.
    /// </summary>
    public UpdateEnrollmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Enrollment ID is required");
    }
}

/// <summary>
/// Handles the <see cref="UpdateEnrollmentCommand"/> by updating the matching enrollment entity.
/// </summary>
public class UpdateEnrollmentHandler : IRequestHandler<UpdateEnrollmentCommand, SuccessResponse>
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly ITransactionService _transactionService;
    private readonly ICustomLogger<UpdateEnrollmentHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateEnrollmentHandler"/> class.
    /// </summary>
    /// <param name="enrollmentService">Service used to retrieve and update enrollment data.</param>
    /// <param name="transactionService">Service used to manage transactional operations.</param>
    /// <param name="logger">Custom application logger.</param>
    public UpdateEnrollmentHandler(
        IEnrollmentService enrollmentService,
        ITransactionService transactionService,
        ICustomLogger<UpdateEnrollmentHandler> logger
    )
    {
        _enrollmentService = enrollmentService;
        _transactionService = transactionService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the enrollment update request and sets the new enrollment status.
    /// </summary>
    /// <param name="request">The <see cref="UpdateEnrollmentCommand"/> containing the enrollment identifier and update data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="SuccessResponse"/> containing the updated enrollment ID and a confirmation message.
    /// </returns>
    public async Task<SuccessResponse> Handle(
        UpdateEnrollmentCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Updating enrollment {EnrollmentId}.", request.Id);

        EnrollmentEntity? enrollment = await _enrollmentService.GetEnrollmentByIdWithDetailAsync(
            request.Id,
            cancellationToken
        );

        if (enrollment is null)
        {
            _logger.LogError(
                "Enrollment update failed because enrollment {EnrollmentId} was not found.",
                request.Id
            );

            throw new NotFoundException(
                "Enrollment not found",
                $"No enrollment found with id {request.Id}"
            );
        }

        enrollment.EnrollmentStatus = request.Dto.EnrollmentStatus;

        _enrollmentService.UpdateEnrollment(enrollment);
        await _transactionService.SaveChangesAsync(cancellationToken);

        _logger.LogInfo(
            "Enrollment {EnrollmentId} updated with status {Status}.",
            request.Id,
            enrollment.EnrollmentStatus
        );

        return new SuccessResponse { Id = request.Id, Message = "Enrollment updated successfully" };
    }
}
