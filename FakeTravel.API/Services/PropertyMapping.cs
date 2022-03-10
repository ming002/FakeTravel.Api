﻿using System.Collections.Generic;

namespace FakeTravel.API.Services
{
    public class PropertyMapping<TSource,TDestination>:IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> _mappingDictionary { get; set; }
        public PropertyMapping(Dictionary<string,PropertyMappingValue> mappingDictionary)
        {
            _mappingDictionary = mappingDictionary;
        }
    }
}
