using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportPanel
{
    class SearchCriteria
    {
        /*List<Filters> filters;
        List<SearchCriteria> searchCriteria;*/

        Filters[] filters;
        SearchCriteria[] searchCriteria;
    }

    struct Filters
    {
        public string Field;
        public string Value;
        public ConditionalTypes ConditionalType;
    }

    enum ConditionalTypes
    {
        eq,
        lt,
        gt
    }
}
