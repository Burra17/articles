using System.Net;

namespace Blocks.Exceptions;

public class BadRequestException : HttpException
{
    public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }

    public BadRequestException(string message, Exception exception) : base(HttpStatusCode.BadRequest, message, exception) { }

}
