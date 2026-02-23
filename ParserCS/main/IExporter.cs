using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace main
{
    public interface IExporter<in T>
    {
        Task ExportAsync(IEnumerable<T> products);
    }
}
