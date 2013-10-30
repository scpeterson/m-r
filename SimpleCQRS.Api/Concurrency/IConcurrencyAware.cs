using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleCQRS.Api.Concurrency
{
    public interface IConcurrencyAware
    {
        string ConcurrencyVersion { get; set; }
    }
}