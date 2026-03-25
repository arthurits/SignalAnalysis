using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace $safeprojectname$.Services;

public class PaginatedList<T> : List<T>
{
    public int PageIndex
    {
        get; private set;
    }

    public int PageCount
    {
        get; private set;
    }

    private PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageCount = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        int count = source.Count();
        if (count == 0)
        {
            // No results -> return page 0.
            return new PaginatedList<T>(new List<T>(), 0, 0, pageSize);
        }

        List<T> items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        if (items.Count == 0)
        {
            // Requested page is out of range -> return last page.
            pageIndex = (int)Math.Ceiling(count / (double)pageSize);
            items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}