using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRewriteMeWebService
    {
        Task<HttpRequestResult<IEnumerable<FileItem>>> GetFileItemsAsync(int? minimumVersion = 0);
    }
}
