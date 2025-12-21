using System;
using System.Collections.Generic;
using System.Text;

namespace main
{
    class ListSorter
    {
        public static void SortByPriceAscending(List<Product> products)
        {
            products.Sort((a,b) => a.Price.CompareTo(b.Price));
        }
    }
}
