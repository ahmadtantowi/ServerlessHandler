using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace ServerlessHandler.Extensions
{
    public static class HttpRequestExtension
    {
        private static readonly ConcurrentDictionary<string, PropertyInfo[]> _cachedProperties = new ConcurrentDictionary<string, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<string, CultureInfo> _cachedCultureInfos = new ConcurrentDictionary<string, CultureInfo>();

        public static TModel GetParams<TModel>(this HttpRequest request) where TModel : class, new()
        {
            var param = new TModel();
            var modelName = typeof(TModel).FullName ?? typeof(TModel).Name;

            // cache model properties
            _cachedProperties.TryAdd(modelName, typeof(TModel).GetProperties());

            foreach (var propInfo in _cachedProperties[modelName])
            {
                if (request.Query.TryGetValue(propInfo.Name, out var valString) && !string.IsNullOrEmpty(valString.FirstOrDefault()))
                {
                    object? valType;
                    var propType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                    if (propType.IsEnum)
                    {
                        Enum.TryParse(propType, valString.First(), true, out valType);
                    }
                    else if (typeof(DateTime).IsAssignableFrom(propType))
                    {
                        valType = ConvertToDateTime(valString.First());
                    }
                    else if (propType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propType))
                    {
                        var collectionInstance = Activator.CreateInstance(typeof(List<>).MakeGenericType(propType.GenericTypeArguments[0])) as IList;
                        var typeConverter = TypeDescriptor.GetConverter(propType.GenericTypeArguments[0]);
                        
                        foreach (var item in valString.First().Split(','))
                        {
                            var value = typeof(DateTime).IsAssignableFrom(propType.GenericTypeArguments[0])
                                ? ConvertToDateTime(item)
                                : typeConverter.ConvertFromString(item);
                            collectionInstance!.Add(value);
                        }
                        
                        valType = collectionInstance;
                    }
                    else
                    {
                        valType = TypeDescriptor.GetConverter(propType).ConvertFromString(valString.First());
                    }
                    
                    propInfo.SetValue(param, valType);
                }
            }

            DateTime ConvertToDateTime(string value)
            {
                var acceptLanguage = request.GetAcceptLanguages().FirstOrDefault();
                if (acceptLanguage != null)
                    _cachedCultureInfos.TryAdd(acceptLanguage, new CultureInfo(acceptLanguage));
                
                return acceptLanguage is null
                    ? DateTime.Parse(value)
                    : DateTime.Parse(value, _cachedCultureInfos[acceptLanguage]);
            }

            return param;
        }

        public static string[] GetAcceptLanguages(this HttpRequest request)
        {
            return request.GetTypedHeaders()
                .AcceptLanguage?
                .OrderByDescending(x => x.Quality ?? 1)
                .Select(x => x.Value.ToString())
                .ToArray() ?? Array.Empty<string>();
        }
    }
}