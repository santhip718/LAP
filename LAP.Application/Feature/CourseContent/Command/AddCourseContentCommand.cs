using System.IO;
using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Course;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using CourseContentEntity = LAP.Domain.Entity.CourseContent;

namespace LAP.Application.Feature.CourseContent.Command;

/// <summary>
/// Command for adding content to a course.
/// </summary>
/// <param name="Dto">The course content creation request.</param>
public record AddCourseContentCommand(CreateCourseContentRequestDto Dto)
    : IRequest<SuccessResponse>;

/// <summary>
/// Validates the add course content command.
/// </summary>
public class AddCourseContentValidator : AbstractValidator<AddCourseContentCommand>
{
    /// <summary>
    /// Initializes validation rules for adding course content.
    /// </summary>
    public AddCourseContentValidator()
    {
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
/// Handles requests to add content under a course meta topic.
/// </summary>
public class AddCourseContentHandler : IRequestHandler<AddCourseContentCommand, SuccessResponse>
{
    private readonly IMapper _mapper;
    private readonly ICustomLogger<AddCourseContentHandler> _logger;
    private readonly ITransactionService _transactionService;
    private readonly IFileService _fileService;
    private readonly IRequestContext _requestContext;
    private readonly ICourseContentService _courseContentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddCourseContentHandler"/> class.
    /// </summary>
    /// <param name="mapper">Mapper for converting DTOs to entities.</param>
    /// <param name="logger">Application logger.</param>
    /// <param name="transactionService">Transaction service for database transactions.</param>
    /// <param name="fileService">File storage service for PDF uploads.</param>
    /// <param name="requestContext">Current request context.</param>
    /// <param name="courseContentService">Service for course content operations.</param>
    public AddCourseContentHandler(
        IMapper mapper,
        ICustomLogger<AddCourseContentHandler> logger,
        ITransactionService transactionService,
        IFileService fileService,
        IRequestContext requestContext,
        ICourseContentService courseContentService
    )
    {
        _mapper = mapper;
        _logger = logger;
        _transactionService = transactionService;
        _fileService = fileService;
        _requestContext = requestContext;
        _courseContentService = courseContentService;
    }

    /// <summary>
    /// Adds course content and returns the created content identifier.
    /// </summary>
    /// <param name="request">The add course content command.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A success response containing the created content id.</returns>
    public async Task<SuccessResponse> Handle(
        AddCourseContentCommand request,
        CancellationToken cancellationToken
    )
    {
        CreateCourseContentRequestDto dto = request.Dto;

        _logger.LogInfo(
            "Initiating course content creation for course {CourseId}, meta topic {MetaTopic}, and title {Title}.",
            dto.CourseId,
            dto.MetaTopic,
            dto.Title
        );

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

        int contentSequenceOrder = await ResolveContentSequenceOrderAsync(
            metaTopic.Id,
            dto,
            cancellationToken
        );

        _logger.LogInfo(
            "Resolved content sequence order to {SequenceOrder} for topic {TopicId}.",
            contentSequenceOrder,
            metaTopic.Id
        );

        return await _transactionService.ExecuteInTransactionAsync<SuccessResponse>(
            async () =>
            {
                if (isNewMetaTopic)
                {
                    await _courseContentService.AddMetaTopicAsync(metaTopic, cancellationToken);

                    _logger.LogInfo(
                        "Meta topic created successfully with id {MetaTopicId}.",
                        metaTopic.Id
                    );
                }

                CourseContentEntity courseContent = _mapper.Map<CourseContentEntity>(dto);
                courseContent.MetaTopicId = metaTopic.Id;
                courseContent.SequenceOrder = contentSequenceOrder;
                courseContent.Id = Guid.NewGuid();

                if (dto.PdfFile is not null && dto.PdfFile.Length > 0)
                {
                    _logger.LogInfo(
                        "Uploading PDF file '{FileName}' with size {FileSize} bytes.",
                        dto.PdfFile.FileName,
                        dto.PdfFile.Length
                    );

                    courseContent.PdfFilePath = await _fileService.SaveFileAsync(
                        dto.PdfFile,
                        courseContent.Id.ToString(),
                        cancellationToken
                    );

                    _logger.LogInfo(
                        "PDF uploaded successfully to {PdfPath}.",
                        courseContent.PdfFilePath
                    );
                }

                await _courseContentService.AddAsync(courseContent, cancellationToken);

                _logger.LogInfo(
                    "Course content created successfully with id {ContentId} for course {CourseId}.",
                    courseContent.Id,
                    dto.CourseId
                );

                return new SuccessResponse
                {
                    Id = courseContent.Id,
                    Message = "Course content added successfully",
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
        CreateCourseContentRequestDto dto,
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

    /// <summary>
    /// Resolves the content sequence order from the request or the next available course order.
    /// </summary>
    /// <param name="metaTopicId">The meta topic identifier.</param>
    /// <param name="dto">The course content creation request.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The resolved content sequence order.</returns>
    private async Task<int> ResolveContentSequenceOrderAsync(
        Guid metaTopicId,
        CreateCourseContentRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Resolving content sequence order for meta topic {MetaTopicId}.",
            metaTopicId
        );

        if (dto.SequenceOrder is not null && dto.SequenceOrder >= 0)
        {
            return dto.SequenceOrder.Value;
        }

        int largestSequenceOrder =
            await _courseContentService.GetLargestContentSequenceOrderByMetaTopicAsync(
                metaTopicId,
                cancellationToken
            );

        return largestSequenceOrder + 1;
    }
}
