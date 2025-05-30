using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace OnlineMuhasebeserver.Persistance
{
    public static class AssemblyReference
    {
        public static readonly Assembly assembly = typeof(Assembly).Assembly;
    }
}
