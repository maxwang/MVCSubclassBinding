using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SubclassBinding.Models;
using System.Reflection;

namespace SubclassBinding.Controllers
{
    public class Properties
    {
        public string ClassType { get; set; }
        public IList<string> MyProperties { get; set; }

        public Properties()
        {
            this.MyProperties = new List<string>();
        }
    }

    public class ProductBinder : IModelBinder
    {
        private string _ProductType = "ProductType";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            object model = bindingContext.Model;
            Type modelType = bindingContext.ModelType;

            if (modelType.Equals(typeof(IEnumerable<Product>)))
            {
                bool emptyPrefix = false;
                if (!String.IsNullOrEmpty(bindingContext.ModelName) && !bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
                {
                    if (bindingContext.FallbackToEmptyPrefix)
                    {
                        emptyPrefix = true;
                    }
                    else
                    {
                        return null;
                    }
                }

                IList<Product> products = new List<Product>();

                string[] formKeys = controllerContext.HttpContext.Request.Form.AllKeys.ToArray();

                Dictionary<int, List<string>> items = ConvertItemValue(formKeys, bindingContext.ModelName, emptyPrefix);

                Product product = null;
                foreach (var item in items)
                {
                    product = GetProduct(item.Key, item.Value, bindingContext, bindingContext.ModelName, emptyPrefix);
                    if (product != null)
                    {
                        products.Add(product);
                    }

                }

                return products;
            }
            return null;
        }

        private Product GetProduct(int index, List<string> keys, ModelBindingContext bindingContext, string prefix, bool emptyPrefix)
        {
            if (!keys.Contains(_ProductType)) return null;

            var key = emptyPrefix
                          ? string.Format("[{0}].{1}", index, _ProductType)
                          : string.Format("{0}[{1}].{2}", prefix, index, _ProductType);

            var valueProvider = bindingContext.ValueProvider;

            var productType = valueProvider.GetValue(key).AttemptedValue;

            var type = Type.GetType(productType);

            if (type == null) return null;

            if (!type.IsSubclassOf(typeof(Product))) return null;

            var result = Activator.CreateInstance(type);
            PropertyInfo propertyInfo = null;
            foreach (var itemKey in keys)
            {
                if (itemKey.Equals(_ProductType)) continue;
                propertyInfo = type.GetProperty(itemKey);
                propertyInfo.SetValue(result,
                                      valueProvider.GetValue(emptyPrefix
                                                                 ? string.Format("[{0}].{1}", index, itemKey)
                                                                 : string.Format("{0}[{1}].{2}", prefix, index, itemKey))
                                          .ConvertTo(propertyInfo.PropertyType),
                                      null);
            }

            return (Product)result;

        }

        private Dictionary<int, List<string>> ConvertItemValue(string[] formKeys, string prefix, bool emptyPrefix)
        {
            Dictionary<int, List<string>> results = new Dictionary<int, List<string>>();
            int index = 0;

            int startIndex = 0;
            int endIndex = 0;

            foreach (var key in formKeys)
            {
                if( ! emptyPrefix && ! key.StartsWith(prefix)) continue;
                
                var keys = key.Split(new char[] { '.' });

                //if it is array in array
                var itemIndex = keys.Count() - 2;

                startIndex = keys[itemIndex].IndexOf('[') + 1;
                endIndex = keys[itemIndex].IndexOf(']');

                index = int.Parse(keys[itemIndex].Substring(startIndex, endIndex - startIndex));

                if (results.ContainsKey(index))
                {
                    results[index].Add(keys[itemIndex + 1]);
                }
                else
                {
                    results.Add(index, new List<string> { keys[itemIndex + 1] });
                }
            }
            return results;
        }
    }
}