using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.IO;

namespace MVC.Utilities
{
    public static class GlobalCommonExtensionMethod
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
        public static IEnumerable<string> KeysFor(this DbContext context, Type entityType)
        {
            Contract.Requires(context != null);
            Contract.Requires(entityType != null);

            entityType = ObjectContext.GetObjectType(entityType);

            var metadataWorkspace =
                ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;
            var objectItemCollection =
                (ObjectItemCollection)metadataWorkspace.GetItemCollection(DataSpace.OSpace);

            var ospaceType = metadataWorkspace
                .GetItems<EntityType>(DataSpace.OSpace)
                .SingleOrDefault(t => objectItemCollection.GetClrType(t) == entityType);

            if (ospaceType == null)
            {
                throw new ArgumentException(
                    string.Format(
                        "The type '{0}' is not mapped as an entity type.",
                        entityType.Name),
                    "entityType");
            }

            return ospaceType.KeyMembers.Select(k => k.Name);
        }
        public static object[] KeyValuesFor(this DbContext context, object entity)
        {
            Contract.Requires(context != null);
            Contract.Requires(entity != null);

            var entry = context.Entry(entity);
            return context.KeysFor(entity.GetType())
                .Select(k => entry.Property(k).CurrentValue)
                .ToArray();
        }
        /// <summary>
        /// Convert DataTable To List Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns>List Data</returns>
        private static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        /// <summary>
        /// Convert List Data To DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns>DataTable</returns>
        public static DataTable ConvertListToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static DataTable ToDataTable<T>(this List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        /// <summary>
        /// Determines whether the specified collection has any elements in the sequence.
        /// This method also checks for a null collection.
        /// </summary>
        /// <param name="items">The ICollection of items to check.</param>
        public static bool HasElements(this ICollection items)
        {
            return items != null && items.Count > 0;
        }
        public static DataSet AsDataSet<T>(this List<T> list)
        {
            //list is nothing or has nothing, return nothing (or add exception handling)
            if (list == null || list.Count == 0) { return null; }

            //get the type of the first obj in the list
            var obj = list[0].GetType();

            //now grab all properties
            var properties = obj.GetProperties();

            //make sure the obj has properties, return nothing (or add exception handling)
            if (properties.Length == 0) { return null; }

            //it does so create the dataset and table
            var dataSet = new DataSet();
            var dataTable = new DataTable();

            //now build the columns from the properties
            var columns = new DataColumn[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                columns[i] = new DataColumn(properties[i].Name, properties[i].PropertyType);
            }

            //add columns to table
            dataTable.Columns.AddRange(columns);

            //now add the list values to the table
            foreach (var item in list)
            {
                //create a new row from table
                var dataRow = dataTable.NewRow();

                //now we have to iterate thru each property of the item and retrieve it's value for the corresponding row's cell
                var itemProperties = item.GetType().GetProperties();

                for (int i = 0; i < itemProperties.Length; i++)
                {
                    dataRow[i] = itemProperties[i].GetValue(item, null);
                }

                //now add the populated row to the table
                dataTable.Rows.Add(dataRow);
            }

            //add table to dataset
            dataSet.Tables.Add(dataTable);

            //return dataset
            return dataSet;
        }
        /// <summary>
        /// Appends an item to this sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="newItem">The item to append.</param>
        /// <returns>A new sequence with the items of <paramref name="source"/> and <paramref name="newItem"/>.</returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T newItem)
        {
            foreach (var item in source)
            {
                yield return item;
            }
            yield return newItem;
        }
        // <summary>
        /// Returns all elements which are not of a specific type or there derivates..
        /// </summary>
        /// <typeparam name="TRemove">The type that should not be included in the result.</typeparam>
        /// <param name="source">The source list.</param>
        /// <returns>A list of all items from <paramref name="source"/> which type is not <typeparamref name="TRemove"/>.</returns>
        public static IEnumerable NotOfType<TRemove>(this IEnumerable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            foreach (var item in source)
            {
                if (!(item is TRemove))
                {
                    yield return item;
                }
            }
        }

        //C# 6 syntax with LINQ

        /// <summary>
        /// Validates a MD5 hash string.
        /// </summary>
        /// <param name="md5">The string to test.</param>
        /// <returns><c>true</c>, in case <paramref name="md5"/> is valid; otherwise <c>false</c>.</returns>
        public static bool IsValidMd5(string md5) => md5 != null && md5.Length == 32 && md5.All(x => (x >= '0' && x <= '9') || (x >= 'a' && x <= 'f') || (x >= 'A' && x <= 'F'));

        //old C# syntax

        /// <summary>
        /// Validates a MD5 hash string.
        /// </summary>
        /// <param name="md5">The string to test.</param>
        /// <returns><c>true</c>, in case <paramref name="md5"/> is valid; otherwise <c>false</c>.</returns>
        //public static bool IsValidMD5(string md5)
        //{
        //    if (md5 == null || md5.Length != 32) return false;
        //    foreach (var x in md5)
        //    {
        //        if ((x < '0' || x > '9') && (x < 'a' || x > 'f') && (x < 'A' || x > 'F'))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
    }

    public class EnumDescription : Attribute
    {
        public string Text { get; private set; }

        public EnumDescription(string text)
        {
            this.Text = text;
        }
    }

    /**
     * Convert DataTable To List using LINQ
     * */
    //List<Student> studentList = new List<Student>();
    //studentList = (from DataRow dr in dt.Rows
    //select new Student()
    //{
    //StudentId = Convert.ToInt32(dr["StudentId"]),  
    //StudentName = dr["StudentName"].ToString(),  
    //Address = dr["Address"].ToString(),  
    //MobileNo = dr["MobileNo"].ToString()
    //}).ToList();
    public static class Html
    {
        /// <summary>
        /// Returns an input-element as a button with the given content. It navigates with JavaScript to the given action.
        /// </summary>
        public static MvcHtmlString ActionButton(this HtmlHelper helper, string content, string actionName)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            return LinkButton(helper, content, urlHelper.Action(actionName));
        }
        /// <summary>
        /// Returns an input-element as a button with the given content. It navigates with JavaScript to the given action in the given controller.
        /// </summary>
        public static MvcHtmlString ActionButton(this HtmlHelper helper, string content, string actionName, string controllerName)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            return LinkButton(helper, content, urlHelper.Action(actionName, controllerName));
        }

        /// <summary>
        /// Returns an input-element as a button with the given content. It navigates with JavaScript to the given URL.
        /// </summary>
        public static MvcHtmlString LinkButton(this HtmlHelper helper, string content, string url)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            var sb = new StringBuilder();
            sb.Append("<input type=\"button\" onclick=\"window.location.href='");
            sb.Append(urlHelper.Content(url));
            sb.Append("';\" value=\"");
            sb.Append(content);
            sb.Append("\"/>");

            return new MvcHtmlString(sb.ToString());
        }

        public static string Label(this HtmlHelper helper, string target, string text)
        {
            return String.Format("<label for='{0}'>{1}</label>", target, text);
        }
    }
}