using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVC.Utilities
{
    public static class MyExtensionMethod
    {
        //public static MvcHtmlString EnumDropDownListFor<TModel, TProperty, TEnum>(this HtmlHelper<TModel> htmlHelper,
        //    Expression<Func<TModel, TProperty>> expression, TEnum selectedValue)
        //{
        //    IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        //    IEnumerable<SelectListItem> items = from value in values
        //        select new SelectListItem()
        //        {
        //            Text = value.ToString(),
        //            Value = value.ToString(),             
        //            Selected = (value.Equals(selectedValue))
        //        };
        //    return SelectExtensions.DropDownListFor(htmlHelper, expression, items);
        //}

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, TEnum selectedValue)
        {
            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>();

            IEnumerable<SelectListItem> items =
                from value in values
                select new SelectListItem
                {
                    Text = value.ToString(),
                    Value = value.ToString(),
                    Selected = (value.Equals(selectedValue))
                };

            return htmlHelper.DropDownList(
                name,
                items
            );
        }
        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, IEnumerable<SelectListItem> modelActionsList)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            TypeConverter converter = TypeDescriptor.GetConverter(enumType);

            IEnumerable<SelectListItem> items =
                from value in values
                select new SelectListItem
                {
                    Text = converter.ConvertToString(value),
                    Value = converter.ConvertToString(value),
                    Selected = value.Equals(metadata.Model)
                };

            if (metadata.IsNullableValueType)
            {
                items = SingleEmptyItem.Concat(items);
            }

            return htmlHelper.DropDownListFor(
                expression,
                items
            );
        }

        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }
        private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };

        public static SelectList ToSelectList(Type enumType)
        {
            return ToSelectList(enumType, String.Empty);
        }

        public static SelectList ToSelectList(Type enumType, string selectedItem)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var item in Enum.GetValues(enumType))
            {
                FieldInfo fi = enumType.GetField(item.ToString());
                var attribute = fi.GetCustomAttributes(typeof(EnumDescription), true).FirstOrDefault();
                var title = attribute == null ? item.ToString() : ((EnumDescription)attribute).Text;

                // uncomment to skip enums without attributes
                //if (attribute == null)
                //    continue;

                var listItem = new SelectListItem
                {
                    Value = ((int)item).ToString(),
                    Text = title,
                    Selected = selectedItem == ((int)item).ToString()
                };
                items.Add(listItem);
            }

            return new SelectList(items, "Value", "Text", selectedItem);
        }

        public static string ToDescription(this Enum enumeration)
        {
            Type type = enumeration.GetType();
            MemberInfo[] memInfo = type.GetMember(enumeration.ToString());

            if (null != memInfo && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(EnumDescription), false);
                if (null != attrs && attrs.Length > 0)
                    return ((EnumDescription)attrs[0]).Text;
            }
            else
            {
                return "0";
            }

            return enumeration.ToString();
        }

        public static Expression<Action<T, string>> GetAction<T>(string fieldName)
        {
            ParameterExpression targetExpr = Expression.Parameter(typeof(T), "Target");
            MemberExpression fieldExpr = Expression.Property(targetExpr, fieldName);
            ParameterExpression valueExpr = Expression.Parameter(typeof(string), "value");
            MethodCallExpression convertExpr = Expression.Call(typeof(Convert),
                "ChangeType", null, valueExpr, Expression.Constant(fieldExpr.Type));
            UnaryExpression valueCast = Expression.Convert(convertExpr, fieldExpr.Type);
            BinaryExpression assignExpr = Expression.Assign(fieldExpr, valueCast);
            return Expression.Lambda<Action<T, string>>(assignExpr, targetExpr, valueExpr);
        }
        public static SelectList ItemsPerPageList
        {
            get
            {
                //return (new SelectList(new List<int> { 5, 10, 25, 50, 100 }));
                return (new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10));
            }
        }
        public static IEnumerable<T> FindStringFromValue<T>(IEnumerable<T> aList, string searchItem)
        {
            List<T> bList = new List<T>();
            foreach (var akycinfo in aList)
            {
                var found = false;
                foreach (var prop in akycinfo.GetType().GetProperties())
                {
                    var propertyValue = prop.GetValue(akycinfo, null);
                    var value = "";
                    if (propertyValue != null)
                    {
                        value = propertyValue.ToString().Replace(" ", String.Empty).ToUpper();
                    }
                    var searchdata = searchItem.Replace(" ", String.Empty).ToUpper();

                    if (value != "" && value.Contains(searchdata))
                    {
                        found = true;
                        break;
                    }
                }
                if (found == true)
                    bList.Add(akycinfo);
            }
            return bList;
        }
        public static IEnumerable<T> WhereAtLeastOneProperty<T, PropertyType>(this IEnumerable<T> source, Predicate<PropertyType> predicate)
        {
            var properties = typeof(T).GetProperties().Where(prop => prop.CanRead && prop.PropertyType == typeof(PropertyType)).ToArray();
            return source.Where(item => properties.Any(prop => PropertySatisfiesPredicate(predicate, item, prop)));
        }

        private static bool PropertySatisfiesPredicate<T, PropertyType>(Predicate<PropertyType> predicate, T item, System.Reflection.PropertyInfo prop)
        {
            try
            {
                return predicate((PropertyType)prop.GetValue(item));
            }
            catch
            {
                return false;
            }
        }
    }

    public class EnumDescription : Attribute
    {
        public string Text { get; private set; }

        public EnumDescription(string text)
        {
            this.Text = text;
        }
    }
}