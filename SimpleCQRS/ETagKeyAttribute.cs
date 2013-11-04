using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleCQRS
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ETagKeyAttribute : Attribute
    {

    }
}