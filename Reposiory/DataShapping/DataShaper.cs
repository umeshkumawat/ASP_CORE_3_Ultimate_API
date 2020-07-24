using Contracts;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Reposiory.DataShapping
{
    public class DataShaper<T> : IDataShaper<T>
    {
        public PropertyInfo[] Properties { get; set; }

        public DataShaper()
        {
            Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fields)
        {
            var props = GetRequiredProperties(fields);
            return FetchData(entities, props);
        }

        public ShapedEntity ShapeData(T entity, string fields)
        {
            var props = GetRequiredProperties(fields);
            return FetchDataForEntity(entity, props);
        }

        private IEnumerable<PropertyInfo> GetRequiredProperties(string fields)
        {
            var requiredProperties = new List<PropertyInfo>();
            if (!string.IsNullOrEmpty(fields))
            {
                var arrFields = fields.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var field in arrFields)
                {
                    var property = Properties.FirstOrDefault(p => p.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
                    if (property != null)
                        requiredProperties.Add(property);
                }
            }
            else
            {
                requiredProperties = Properties.ToList();
            }
            return requiredProperties;
        }

        private IEnumerable<ShapedEntity> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> properties)
        {
            var shapedData = new List<ShapedEntity>();
            foreach(var entity in entities)
            {
                shapedData.Add(FetchDataForEntity(entity, properties));
            }

            return shapedData;
        }

        private ShapedEntity FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new ShapedEntity();
            foreach (var prop in requiredProperties)
            {
                var propVal = prop.GetValue(entity);
                shapedObject.Entity.TryAdd(prop.Name, propVal);
            }

            var objectProp = entity.GetType().GetProperty("Id");
            shapedObject.Id = (Guid)objectProp.GetValue(entity);

            return shapedObject;
        }
    }
}
