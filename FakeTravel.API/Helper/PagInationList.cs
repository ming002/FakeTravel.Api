using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeTravel.API.Helper
{
    public class PagInationList<T>:List<T>
    {
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage<TotalPages;

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public PagInationList(int totalCount,int currentPage,int pageSize,List<T>items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            AddRange(items);
            TotalCount = totalCount;
            TotalPages=(int)Math.Ceiling(totalCount/(double)pageSize);
        }
        /// <summary>
        /// 工厂模式创建分页模组
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static async Task<PagInationList<T>> CreateAsync(
            int currentPage, int pageSize, IQueryable<T> result)
        {
            var totalCount=await result.CountAsync();
            //分页
            //1.跳过一定量的数据
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            //2.以pagesize为标准显示一定量的数据
            result = result.Take(pageSize);

            var items= await result.ToListAsync();
            return new PagInationList<T>(totalCount,currentPage, pageSize, items);
        }
    }
}
