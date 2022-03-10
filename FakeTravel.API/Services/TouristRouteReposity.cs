using FakeTravel.API.Database;
using FakeTravel.API.Dtos;
using FakeTravel.API.Helper;
using FakeTravel.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeTravel.API.Services
{
    public class TouristRouteReposity : ITouristRouteReposity
    {
        private readonly AppDbContext _context;
        private readonly IPropertyMappingService _propertyMappingService;
        public TouristRouteReposity(
            AppDbContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if (touristRoute == null)
            {
                throw new ArgumentNullException(nameof(touristRoute));
            }
            _context.TouristRoutes.Add(touristRoute);
            //_context.SaveChanges();
        }

        public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if (touristRouteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            if (touristRoutePicture == null)
            {
                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            touristRoutePicture.TouristRouteId = touristRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);
        }

        public async Task<TouristRoutePicture> GetPictureAsync(int pictureId)
        { 
            return await _context.TouristRoutePictures.FirstOrDefaultAsync(x=>x.Id==pictureId);
        }

        public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristId)
        {
            return await _context.TouristRoutePictures.Where(x => x.TouristRouteId == touristId).ToListAsync();
        }

        public async Task<TouristRoute> GetTouristRouteAsync(Guid id)
        {
            return await _context.TouristRoutes.Include(x => x.TouristRoutePictures).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<PagInationList<TouristRoute>> GetTouristRoutesAsync(
            string keyword,
            string operatorType,
            int? raringValue,
            int pageSize,
            int pageNum,
            string orderBy)
        {
            IQueryable<TouristRoute> result = _context.
                TouristRoutes.
                Include(x => x.TouristRoutePictures);

            if (!string.IsNullOrEmpty(keyword))
            {
                result = result.Where(x => x.Title.Contains(keyword));
            }

            if (raringValue >= 0)
            {
                switch (operatorType)
                {
                    case "largerThan":
                        result = result.Where(t => t.Rating >= raringValue);
                        break;
                    case "lessThan":
                        result = result.Where(t => t.Rating <= raringValue);
                        break;
                    case "equalTo":
                        result = result.Where(t => t.Rating == raringValue);
                        break;
                    default:
                        break;
                }
            }

            if(!string.IsNullOrEmpty(orderBy))
            {
                if (orderBy.Equals("originalPrice", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderBy(t => t.OriginPrice);
                }
            }

            var touristRouteMappingDictionary = _propertyMappingService.
                GetPropertyMapping<TouristRouteDto,TouristRoute>();

            result.ApplySort

            return await PagInationList<TouristRoute>
                .CreateAsync(pageNum,pageSize,result);
        }

        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.TouristRoutes.Remove(touristRoute);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public async Task<bool> TouristRouteExitsAsync(Guid touristId)
        {
            return await _context.TouristRoutes.AnyAsync(x => x.Id == touristId);
        }

        public void DeleteTouristPictureRoute(TouristRoutePicture touristRoutePicture)
        {
            _context.TouristRoutePictures.Remove(touristRoutePicture);
        }

        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids)
        {
            return await _context.TouristRoutes.Where(x => ids.Contains(x.Id)).ToListAsync(); 
        }

        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }

        public async Task<ShoppingCart> GetShoppingCartByUserId(string userId)
        {
            return await _context.ShoppingCarts
                .Include(x => x.User)
                .Include(x => x.ShoppingCartItems)
                .ThenInclude(x=>x.TouristRoute)
                .Where(x=>x.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task CreateShoppingCart(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
        }

        public async Task CreateLineItem(LineItem lineItem)
        {
          await  _context.LineItems.AddAsync(lineItem);
        }

        public async Task<LineItem> GetShoppingCartItemByItem(int lineItemId)
        {
           return await _context.LineItems.FirstOrDefaultAsync(x => x.Id == lineItemId);
        }

        public void DeleteShoppingCartItem(LineItem lineItem)
        {
              _context.LineItems.Remove(lineItem);
        }

        public async Task<IEnumerable<LineItem>> GetShoppingCartItemsByIdListAsync(IEnumerable<int> ids)
        {
            return await _context.LineItems.Where(x=>ids.Contains(x.Id)).ToListAsync();
        }

        public void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems)
        {
            _context.LineItems.RemoveRange(lineItems);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<PagInationList<Order>> GetOrdersByUserId(
            string userId,int pageSize,int pageNum)
        {
            IQueryable<Order> result = _context.Orders
                .Where(x => x.UserId == userId);

            return await PagInationList<Order>
                .CreateAsync(pageNum, pageSize, result);
        }

        public Task<Order> GetOrderById(Guid orderId)
        {
            return _context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.TouristRoute)
                .FirstOrDefaultAsync(x => x.Id == orderId);
        }
    }
}
