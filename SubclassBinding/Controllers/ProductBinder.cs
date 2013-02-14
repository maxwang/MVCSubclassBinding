using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SubclassBinding.Models;
using System.Reflection;

namespace SubclassBinding.Controllers
{
    public class ProductBinder : IModelBinder
    {
        private string _namespace = "SubclassBinding.Models";
        private string _ProductType = "ProductType";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            object model = bindingContext.Model;
            Type modelType = bindingContext.ModelType;

            if (modelType.Equals(typeof(IEnumerable<Product>)))
            {
                //var ps = this.GetModelProperties(controllerContext, bindingContext);
                IList<Product> products = new List<Product>();

                string[] formKeys = controllerContext.HttpContext.Request.Form.AllKeys.ToArray();
                Dictionary<int, List<string>> items = ConvertItemValue(formKeys);

                Product product = null;
                foreach (var item in items)
                {
                    product = GetProduct(item.Key, item.Value, bindingContext);
                    if (product != null)
                    {
                        products.Add(product);
                    }

                }

                return products;
            }
            return null;
        }

        private Product GetProduct(int index, List<string> keys, ModelBindingContext bindingContext)
        {
            if (!keys.Contains(_ProductType)) return null;

            var key = string.Format("[{0}].{1}", index, _ProductType);

            var valueProvider = bindingContext.ValueProvider;

            var productType = valueProvider.GetValue(key).AttemptedValue;

            var type = Type.GetType(string.Format("{0}.{1}", _namespace, productType));

            if (type == null) return null;

            if (!type.IsSubclassOf(typeof(Product))) return null;

            var result = Activator.CreateInstance(type);
            PropertyInfo propertyInfo = null;
            foreach (var itemKey in keys)
            {
                if (itemKey.Equals(_ProductType)) continue;
                propertyInfo = type.GetProperty(itemKey);
                propertyInfo.SetValue(result,
                                      valueProvider.GetValue(string.Format("[{0}].{1}", index, itemKey)).ConvertTo(propertyInfo.PropertyType),
                                      null);
            }

            return (Product)result;

        }

        private Dictionary<int, List<string>> ConvertItemValue(string[] formKeys)
        {
            Dictionary<int, List<string>> results = new Dictionary<int, List<string>>();
            int index = 0;
            foreach (var key in formKeys)
            {
                var keys = key.Split(new char[] { '.' });
                index = int.Parse(keys[0].Substring(1, 1));
                if (results.ContainsKey(index))
                {
                    results[index].Add(keys[1]);
                }
                else
                {
                    results.Add(index, new List<string> { keys[1] });
                }
            }
            return results;
        }
    }
}