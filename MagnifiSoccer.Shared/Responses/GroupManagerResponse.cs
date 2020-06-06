using System;
using System.Collections.Generic;
using System.Text;

namespace MagnifiSoccer.Shared.Responses
{
    public class GroupManagerResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
