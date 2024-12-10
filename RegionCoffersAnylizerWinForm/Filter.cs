using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RegionCoffersAnylizerWinForm
{
    internal class Filter
    {
        public string columnName;

        public bool isNot;

        public string operate;

        public List<string> values;

        public Filter(string columnName, bool isNot, string operate, List<string> values)
        {
            this.columnName = columnName;
            this.isNot = isNot;
            this.operate = operate;
            this.values = values;
        }

        public Filter()
        {
        }


        public bool operating(string value)
        {
            switch (operate)
            {
                case "равно":
                    if (!isNot) {
                        return value.Equals(values[0]); }
                    else { 
                        return !value.Equals(values[0]); }
                case "входит":
                    if (!isNot) { 
                        return values.Contains(value); }
                    else { 
                        return !values.Contains(value); }
                case "начало":
                    if (!isNot){
                        return value.StartsWith(values[0]);}
                    else{
                        return !value.StartsWith(values[0]);}

                default: return false;
            }
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            // Получаем тип объекта
            Type type = obj.GetType();

            // Получаем информацию о свойстве
            PropertyInfo propertyInfo = type.GetProperty(propertyName);

            if (propertyInfo == null)
            {
                throw new ArgumentException($"Свойство '{propertyName}' не найдено в типе {type.Name}.");
            }

            // Получаем значение свойства
            return propertyInfo.GetValue(obj);
        }

        public bool isOperate(Models.Region region)
        {
            if(GetPropertyValue(region, columnName) == null)
            {
                return false;
            }
            return operating(GetPropertyValue(region, columnName).ToString());
        }

        
    }
}
