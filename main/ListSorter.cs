using System;
using System.Collections.Generic;
using System.Text;
using main.Models;

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
