using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiRequestLogs.Models
{
    public class RequestLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string LogId { get; set; }           /*Guid.NewGuid().ToString()*/
        public string Node { get; set; }            /*project name*/
        public string ClientIp { get; set; }
        public string TraceId { get; set; }         /*HttpContext TraceIdentifier*/
        public DateTime? RequestDateTimeUtc { get; set; }
        public DateTime? RequestDateTimeUtcActionLevel { get; set; }
        public string RequestPath { get; set; }
        public string RequestQuery { get; set; }
        public string RequestQueries { get; set; } // Save Json
        public string RequestMethod { get; set; }
        public string RequestScheme { get; set; }
        public string RequestHost { get; set; }
        public string RequestHeaders { get; set; } // Save Json
        public string RequestBody { get; set; }
        public string RequestContentType { get; set; }
        public DateTime? ResponseDateTimeUtc { get; set; }
        public DateTime? ResponseDateTimeUtcActionLevel { get; set; }
        public string ResponseStatus { get; set; }
        public string ResponseHeaders { get; set; } // Save Json
        public string ResponseBody { get; set; }
        public string ResponseContentType { get; set; }
        public bool? IsExceptionActionLevel { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
    }
}
