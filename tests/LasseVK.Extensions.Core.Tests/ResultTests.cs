namespace LasseVK.Extensions.Core.Tests;

public class ResultTests
{
    [Test]
    public void IsSuccess_ForSuccessResult_ReturnsTrue()
    {
        Result<string, Exception> result = "some value";

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void IsSuccess_ForError_ReturnsFalse()
    {
        Result<string, Exception> result = new InvalidOperationException();

        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void Value_ForSuccessResult_ReturnsValue()
    {
        Result<string, Exception> result = "some value";

        Assert.That(result.Value, Is.EqualTo("some value"));
    }

    [Test]
    public void Value_ForError_ThrowsInvalidOperationException()
    {
        Result<string, Exception> result = new InvalidOperationException();

        Assert.Throws<InvalidOperationException>(() => { _ = result.Value; });
    }

    [Test]
    public void Error_ForSuccessResult_ThrowsInvalidOperationException()
    {
        Result<string, Exception> result = "some value";

        Assert.Throws<InvalidOperationException>(() => { _ = result.Error; });
    }

    [Test]
    public void Error_ForError_ReturnsException()
    {
        Result<string, Exception> result = new InvalidOperationException();

        Assert.That(result.Error, Is.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void CastToValue_ForSuccessResult_ReturnsValue()
    {
        Result<string, Exception> result = "some value";

        Assert.That((string)result, Is.EqualTo("some value"));
    }

    [Test]
    public void CastToValue_ForError_ThrowsInvalidOperationException()
    {
        Result<string, Exception> result = new InvalidOperationException();

        Assert.Throws<InvalidOperationException>(() => { _ = (string)result; });
    }

    [Test]
    public void CastToError_ForSuccessResult_ThrowsInvalidOperationException()
    {
        Result<string, Exception> result = "some value";

        Assert.Throws<InvalidOperationException>(() => { _ = (Exception)result; });
    }

    [Test]
    public void CastToError_ForError_ReturnsException()
    {
        Result<string, Exception> result = new InvalidOperationException();

        Assert.That((Exception)result, Is.TypeOf<InvalidOperationException>());
    }
}