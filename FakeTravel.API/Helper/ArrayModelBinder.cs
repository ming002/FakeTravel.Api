using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FakeTravel.API.Helper
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //是否允许列表类型
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result=ModelBindingResult.Failed();
                return Task.CompletedTask;
            }
            //获取绑定的值
            var value=bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName).ToString();
            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }
            //如果数值不为空，将字符串转为相应的数据类型
            //获取泛型参数类型
            var elementType=bindingContext.ModelType.GetTypeInfo().GenericTypeArguments.GetType();
            //进行类型转换
            var converter = TypeDescriptor.GetConverter(elementType);
            //将每一个字符串通过类型转换工具转为对应的数据类型
            var values=value.Split(new[] {"," },System.StringSplitOptions.RemoveEmptyEntries)
                .Select(x=>converter.ConvertFromString(x.Trim()))
                .ToArray();
            //通过反射赋值到新的数组中
            var typeValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typeValues, 0);
            bindingContext.Model = typeValues;

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
