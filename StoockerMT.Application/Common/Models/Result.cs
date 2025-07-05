using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Models
{
    public class Result
    {
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();

        public static Result Success() => new Result { Succeeded = true };
        public static Result Failure(params string[] errors) => new Result { Succeeded = false, Errors = errors };
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }

        public static Result<T> Success(T data) => new Result<T> { Succeeded = true, Data = data };
        public new static Result<T> Failure(params string[] errors) => new Result<T> { Succeeded = false, Errors = errors };
    }
}
