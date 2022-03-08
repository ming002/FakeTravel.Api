﻿using FakeTravel.API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FakeTravel.API.Services
{
    public interface ITouristRouteReposity
    {
        Task<IEnumerable<TouristRoute>> GetTouristRoutesAsync(string keyword,string operatorType,int? raringValue);
        Task<TouristRoute> GetTouristRouteAsync(Guid id);
        Task<bool> TouristRouteExitsAsync(Guid touristId);
        Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristId);
        Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids);
        Task<TouristRoutePicture> GetPictureAsync(int pictureId);
        void AddTouristRoute(TouristRoute touristRoute);
        void AddTouristRoutePicture(Guid touristRouteId,TouristRoutePicture touristRoutePicture);
        void DeleteTouristRoute(TouristRoute touristRoute);
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
        void DeleteTouristPictureRoute(TouristRoutePicture touristRoutePicture);
        Task<ShoppingCart> GetShoppingCartByUserId(string userId);
        Task CreateShoppingCart(ShoppingCart shoppingCart);
        Task CreateLineItem(LineItem lineItem);
        Task<LineItem> GetShoppingCartItemByItem(int lineItemId);
        void DeleteShoppingCartItem(LineItem lineItem);
        Task<bool> SaveAsync();

    }
}
