using System;
using System.Collections.Generic;
using System.Linq;

namespace FakeTravel.API.Services
{
    public interface IPropertyMappingService
    {

        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        
    }
}