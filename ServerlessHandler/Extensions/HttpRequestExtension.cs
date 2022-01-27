using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServerlessHandler.Extensions
{
    public static class HttpRequestExtension
    {
        public static TModel GetParams<TModel>(this HttpRequest request) where TModel : class, new()
        {
            var param = new TModel();

            foreach (var propInfo in typeof(TModel).GetProperties())
            {
                var propType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                if (request.Query.TryGetValue(propInfo.Name, out var valString) && !valString.Any(x => string.IsNullOrEmpty(x)))
                {
                    var valType = default(object);
                    
                    if (propType.IsEnum)
                    {
                        Enum.TryParse(propType, valString.ToString(), true, out valType);
                    }
                    else if (propType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propType))
                    {
                        var collectionInstance = Activator.CreateInstance(typeof(List<>).MakeGenericType(propType.GenericTypeArguments[0])) as IList;
                        
                        foreach (var item in valString.ToString().Split(','))
                            collectionInstance!.Add(Convert.ChangeType(item, propType.GenericTypeArguments[0]));
                        
                        valType = collectionInstance;
                    }
                    else
                    {
                        valType = Convert.ChangeType(valString.ToString(), propType);
                    }
                    
                    propInfo.SetValue(param, valType);
                }
            }

            return param;
        }
    }
}