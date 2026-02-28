using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace main.Interfaces
{
    public interface IMapper<in TIn,out TOut>
    {
        public TOut? Map(TIn source);
    }
}
