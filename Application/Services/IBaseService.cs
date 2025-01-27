namespace Application.Services
{
    public interface IBaseService
    {
        void BadRequest(string detail = "", string title = "Bad Request");
        void Conflict(string detail = "", string title = "Conflict");
        void Forbidden(string detail = "", string title = "Forbidden");
        void NotFound(string detail = "", string title = "Not Found");
        void Unauthorized(string detail = "", string title = "Unauthorized");
    }
}
