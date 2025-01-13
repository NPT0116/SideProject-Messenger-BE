using System.Net;

namespace backend.src.Exceptions.Example
{
public class ProductNotFoundException : BaseException
{
    public ProductNotFoundException(Guid id)
        : base($"product with id {id} not found", HttpStatusCode.NotFound)
    {
    }
}
}