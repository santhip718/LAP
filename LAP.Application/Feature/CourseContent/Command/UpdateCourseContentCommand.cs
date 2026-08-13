using System.IO;
using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Course;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using CourseContentEntity = LAP.Domain.Entity.CourseContent;

namespace LAP.Application.Feature.CourseContent.Command;

/// <summary>
/// Command for updating an existing course content with the provided details.
/// </summary>
/// <param name="Id">The identifier of the course content to update.</param>
/// <param name="Dto">The course content update request data transfer object.</param>
public record UpdateCourseContentCommand(Guid Id, UpdateCourseContentRequestDto Dto)
    : IRequest<SuccessResponse>;

/// <summary>
/// Validates the <see cref="UpdateCourseContentCommand"/> before it is handled.
/// </summary>
public class UpdateCourseContentValidator : AbstractValidator<UpdateCourseContentCommand>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="UpdateCourseContentCommand"/>.
    /// </summary>
    public UpdateCourseContentValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Content ID is required");
        RuleFor(x => x.Dto.CourseId).NotEmpty().WithMessage("Course ID is required");
        RuleFor(x => x.Dto.MetaTopic)
            .NotEmpty()
            .WithMessage("Meta topic is required")
            .MaximumLength(200)
            .WithMessage("Meta topic cannot exceed 200 characters");
        RuleFor(x => x.Dto.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");
        RuleFor(x => x.Dto.ContentTypeId).NotEmpty().WithMessage("Content type is required");
        RuleFor(x => x.Dto.SequenceOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Sequence order must be greater than or equal to 0");
    }
}

/// <summary>
/// Handles the <see cref="UpdateCourseContentCommand"/> by updating the matching course content entity.
/// </summary>
public class UpdateCourseContentHandler
    : IRequestHandler<UpdateCourseContentCommand, SuccessResponse>
{
    private readonly ICustomLogger<UpdateCourseContentHandler> _logger;
    private readonly ITransactionService _transactionService;
    private readonly IFileService _fileService;
    private readonly IRequestContext _requestContext;
    private readonly ICourseContentService _courseContentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCourseContentHandler"/> class.
    /// </summary>
    /// <param name="logger">Custom application logger.</param>
    /// <param name="transactionService">Service used to manage transactional operations.</param>
    /// <param name="fileService">Service used to store uploaded PDF files.</param>
    /// <param name="requestContext">Context containing the current request user information.</param>
    /// <param name="courseContentService">Service for course content operations.</param>
    public UpdateCourseContentHandler(
        ICustomLogger<UpdateCourseContentHandler> logger,
        ITransactionService transactionService,
        IFileService fileService,
        IRequestContext requestContext,
        ICourseContentService courseContentService
    )
    {
        _logger = logger;
        _transactionService = transactionService;
        _fileService = fileService;
        _requestContext = requestContext;
        _courseContentService = courseContentService;
    }

    /// <summary>
    /// Handles the course content update request, including meta topic resolution, PDF upload, and entity persistence.
    /// </summary>
    /// <param name="request">The <see cref="UpdateCourseContentCommand"/> containing the content identifier and update data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="SuccessResponse"/> containing the updated content ID and a confirmation message.
    /// </returns>
    public async Task<SuccessResponse> Handle(
        UpdateCourseContentCommand request,
        CancellationToken cancellationToken
    )
    {
        UpdateCourseContentRequestDto dto = request.Dto;

        _logger.LogInfo(
            "Updating course content {ContentId} for course {CourseId}, meta topic {MetaTopic}, and title {Title}.",
            request.Id,
            dto.CourseId,
            dto.MetaTopic,
            dto.Title
        );

        CourseContentEntity? content = await _courseContentService.GetByIdAsync(
            request.Id,
            cancellationToken
        );

        if (content is null)
        {
            _logger.LogError(
                "Course content update failed because content {ContentId} was not found.",
                request.Id
            );

            throw new NotFoundException(
                "Course content not found",
                $"No course content found with id {request.Id}"
            );
        }

        string normalizedTopicName = dto.MetaTopic.Trim();

        _logger.LogInfo(
            "Searching for meta topic '{MetaTopic}' in course {CourseId}.",
            normalizedTopicName,
            dto.CourseId
        );

        CourseMetaTopic? metaTopic = await _courseContentService.GetMetaTopicByCourseAndNameAsync(
            dto.CourseId,
            normalizedTopicName,
            cancellationToken
        );

        bool isNewMetaTopic = false;

        if (metaTopic is null)
        {
            isNewMetaTopic = true;

            _logger.LogInfo(
                "Meta topic '{MetaTopic}' not found. Resolving sequence order for new meta topic.",
                normalizedTopicName
            );

            int metaTopicSequenceOrder = await ResolveMetaTopicSequenceOrderAsync(
                dto,
                cancellationToken
            );

            _logger.LogInfo(
                "Resolved meta topic sequence order to {SequenceOrder}.",
                metaTopicSequenceOrder
            );

            metaTopic = new CourseMetaTopic
            {
                Id = Guid.NewGuid(),
                CourseId = dto.CourseId,
                Name = normalizedTopicName,
                SequenceOrder = metaTopicSequenceOrder,
                DurationMinute = dto.MetaDurationMinute,
            };
        }

        return await _transactionService.ExecuteInTransactionAsync<SuccessResponse>(
            async () =>
            {
                if (isNewMetaTopic)
                {
                    await _courseContentService.AddMetaTopicAsync(metaTopic, cancellationToken);

                    _logger.LogInfo("Meta topic created with id {MetaTopicId}.", metaTopic.Id);
                }
                else
                {
                    metaTopic.DurationMinute = dto.MetaDurationMinute;

                    if (dto.MetaTopicOrder > 0)
                    {
                        metaTopic.SequenceOrder = dto.MetaTopicOrder.Value;
                    }

                    _logger.LogInfo(
                        "Updated meta topic {MetaTopicId} duration to {Duration}.",
                        metaTopic.Id,
                        dto.MetaDurationMinute
                    );
                }

                content.MetaTopicId = metaTopic.Id;
                content.Title = dto.Title;
                content.ContentTypeId = dto.ContentTypeId;
                content.VideoUrl = dto.VideoUrl?.ToString();

                if (dto.SequenceOrder is >= 0)
                {
                    content.SequenceOrder = dto.SequenceOrder.Value;
                }

                if (dto.PdfFile is not null && dto.PdfFile.Length > 0)
                {
                    _logger.LogInfo(
                        "Uploading PDF file '{FileName}' ({FileSize} bytes) for content {ContentId}.",
                        dto.PdfFile.FileName,
                        dto.PdfFile.Length,
                        request.Id
                    );

                    content.PdfFilePath = await _fileService.SaveFileAsync(
                        dto.PdfFile,
                        content.Id.ToString(),
                        cancellationToken
                    );

                    _logger.LogInfo(
                        "PDF uploaded successfully for content {ContentId} to storage path {PdfPath}.",
                        request.Id,
                        content.PdfFilePath
                    );
                }
                else
                {
                    _logger.LogInfo(
                        "No PDF file provided for course content {ContentId}. Keeping existing PDF.",
                        request.Id
                    );
                }

                _courseContentService.Update(content);

                _logger.LogInfo("Updated course content {ContentId}.", request.Id);

                return new SuccessResponse
                {
                    Id = request.Id,
                    Message = "Course content updated successfully",
                };
            },
            cancellationToken
        );
    }

    /// <summary>
    /// Resolves the meta topic sequence order from the request or the next available course order.
    /// </summary>
    /// <param name="dto">The course content creation request.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The resolved meta topic sequence order.</returns>
    private async Task<int> ResolveMetaTopicSequenceOrderAsync(
        UpdateCourseContentRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Resolving meta topic sequence order for course {CourseId}.",
            dto.CourseId
        );

        if (dto.MetaTopicOrder > 0)
        {
            return dto.MetaTopicOrder.Value;
        }

        int largestSequenceOrder =
            await _courseContentService.GetLargestMetaTopicSequenceOrderByCourseAsync(
                dto.CourseId,
                cancellationToken
            );

        return largestSequenceOrder + 1;
    }
}
