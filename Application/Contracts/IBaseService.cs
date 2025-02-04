namespace Application.Contracts;

public interface IBaseService
{
    /// <summary>
    /// Used to throw a 400 BadRequest exception
    /// that is handled by ExceptionMiddleware.
    /// </summary>
    /// <param name="detail">Detailed info describing why exception occur.</param>
    /// <param name="title"></param>
    void BadRequest(string detail = "");


    /// <summary>
    /// Used to throw a 409 Conflict exception
    /// that is handled by ExceptionMiddleware.
    /// </summary>
    /// <param name="detail">Detailed info describing why exception occur.</param>
    /// <param name="title"></param>
    void Conflict(string detail = "");

    /// <summary>
    /// Used to throw a 403 Forbidden exception
    /// that is by ExceptionMiddleware.
    /// </summary>
    /// <param name="detail">Detailed info describing why exception occur.</param>
    /// <param name="title"></param>
    void Forbidden(string detail = "");

    /// <summary>
    /// Used to throw a 404 NotFound exception
    /// that is handled by ExceptionMiddleware.
    /// </summary>
    /// <param name="detail">Detailed info describing why exception occur.</param>
    /// <param name="title"></param>
    void NotFound(string detail = "");

    /// <summary>
    /// Used to throw a 401 Unauthorized exception
    /// that is handled by ExceptionMiddleware.
    /// </summary>
    /// <param name="detail">Detailed info describing why exception occur.</param>
    /// <param name="title"></param>
    void Unauthorized(string detail = "");
}
