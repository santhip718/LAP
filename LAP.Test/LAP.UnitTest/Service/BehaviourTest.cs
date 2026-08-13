using FluentValidation;
using LAP.Application.Behaviors;
using LAP.Application.Interface;
using LAP.Shared.Exceptions;
using MediatR;
using Moq;

namespace LAP.UnitTest.Service;

public class BehaviourTest
{
    [Fact]
    public async Task LoggingBehavior_ShouldLogAndReturnResponse()
    {
        var loggerMock = new Mock<ICustomLogger<LoggingBehavior<TestRequest, TestResponse>>>();
        var behavior = new LoggingBehavior<TestRequest, TestResponse>(loggerMock.Object);
        var request = new TestRequest();
        Task<TestResponse> Next() => Task.FromResult(new TestResponse { Value = "done" });

        var result = await behavior.Handle(request, Next, CancellationToken.None);

        Assert.Equal("done", result.Value);
        loggerMock.Verify(
            x => x.LogDebug(It.IsAny<string>(), It.IsAny<object?[]>()),
            Times.Exactly(2)
        );
        loggerMock.Verify(
            x => x.LogDebug("Handling request: {RequestName}", It.IsAny<object?[]>()),
            Times.Once
        );
        loggerMock.Verify(
            x => x.LogDebug(It.Is<string>(s => s.Contains("Completed")), It.IsAny<object?[]>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ValidationBehavior_ShouldCallNext_WhenNoValidators()
    {
        var loggerMock = new Mock<ICustomLogger<ValidationBehavior<TestRequest, TestResponse>>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(
            Enumerable.Empty<IValidator<TestRequest>>(),
            loggerMock.Object
        );
        var request = new TestRequest();
        Task<TestResponse> Next() => Task.FromResult(new TestResponse { Value = "done" });

        var result = await behavior.Handle(request, Next, CancellationToken.None);

        Assert.Equal("done", result.Value);
    }

    [Fact]
    public async Task ValidationBehavior_ShouldPass_WhenValidationSucceeds()
    {
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v =>
                v.ValidateAsync(
                    It.IsAny<ValidationContext<TestRequest>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var loggerMock = new Mock<ICustomLogger<ValidationBehavior<TestRequest, TestResponse>>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>([validatorMock.Object], loggerMock.Object);
        var request = new TestRequest();

        var result = await behavior.Handle(
            request,
            () => Task.FromResult(new TestResponse { Value = "done" }),
            CancellationToken.None
        );

        Assert.Equal("done", result.Value);
    }

    [Fact]
    public async Task ValidationBehavior_ShouldThrow_WhenValidationFails()
    {
        var validatorMock = new Mock<IValidator<TestRequest>>();
        var failures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Name", "Name is required"),
        };
        validatorMock
            .Setup(v =>
                v.ValidateAsync(
                    It.IsAny<ValidationContext<TestRequest>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(failures));
        var loggerMock = new Mock<ICustomLogger<ValidationBehavior<TestRequest, TestResponse>>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>([validatorMock.Object], loggerMock.Object);
        var request = new TestRequest();

        await Assert.ThrowsAsync<BadRequestException>(() =>
            behavior.Handle(
                request,
                () => Task.FromResult(new TestResponse { Value = "done" }),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task ValidationBehavior_ShouldRunAllValidators()
    {
        var validator1Mock = new Mock<IValidator<TestRequest>>();
        validator1Mock
            .Setup(v =>
                v.ValidateAsync(
                    It.IsAny<ValidationContext<TestRequest>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var validator2Mock = new Mock<IValidator<TestRequest>>();
        validator2Mock
            .Setup(v =>
                v.ValidateAsync(
                    It.IsAny<ValidationContext<TestRequest>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var loggerMock2 = new Mock<ICustomLogger<ValidationBehavior<TestRequest, TestResponse>>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>([
            validator1Mock.Object,
            validator2Mock.Object,
        ], loggerMock2.Object);
        var request = new TestRequest();

        var result = await behavior.Handle(
            request,
            () => Task.FromResult(new TestResponse { Value = "done" }),
            CancellationToken.None
        );

        Assert.Equal("done", result.Value);
        validator1Mock.Verify(
            v =>
                v.ValidateAsync(
                    It.IsAny<ValidationContext<TestRequest>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        validator2Mock.Verify(
            v =>
                v.ValidateAsync(
                    It.IsAny<ValidationContext<TestRequest>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}

public record TestRequest : IRequest<TestResponse>;

public record TestResponse
{
    public string Value { get; set; } = string.Empty;
}
