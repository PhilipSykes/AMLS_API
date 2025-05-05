using Common.Constants;
using static Common.Models.Shared;

namespace Common.Models;

public static class Operations
{
    public class Request<T>
    {
        /// <summary>
        /// Main data of the request
        /// </summary>
        public T Data { get; set; }
        
        /// <summary>
        /// Details for sending notification emails
        /// </summary>
        public EmailDetails EmailDetails { get; set; }
        
        /// <summary>
        /// ReCaptcha verification data for secure operations
        /// </summary>
        public PayLoads.ReCaptchaData ReCaptchaVerification { get; set; }

        public List<Filter>? SearchFilters { get; set; }
    }

    public record Response<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public QueryResultCode StatusCode { get; set; } // Possible change this to enum in future
    }

    public record PaginatedResponse<T> : Response<T>
    {
        public long MatchCount { get; set; }
    }
}