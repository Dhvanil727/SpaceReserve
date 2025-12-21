namespace SpaceReserve.Infrastructure.Extensions;

public static  class IQueryableExtension
{
    public static IQueryable<T> GetPaginated<T> (this IQueryable<T> query , int pageNo  , int pageSize)
    {
        if (pageSize <= 0)
        {
            return query;
        }

        if (pageNo <= 0)
        {
            pageNo = 1;
        }
      
       return  query.Skip((pageNo - 1) * pageSize).Take(pageSize);
    }
}
