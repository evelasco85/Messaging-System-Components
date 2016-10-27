using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Utilities
{
    public interface IDynamicListManipulation
    {
        Type ConstructNewListTypeFromType(Type typeOfList);
        IList CreateGenericListFromType(Type typeOfList);
        void ConvertGenericListToTypedList<TEntity>(IList genericList, out IList<TEntity> convertedList);
        void ConvertTypedListToGenericList<TEntity>(IList<TEntity> concreteList, out IList genericList);
        object ConvertEntityToObject<TEntity>(Type type, TEntity entity);
        dynamic GetDynamicTypedList(IList genericList);
    } 

    public class DynamicListManipulation : Singleton<DynamicListManipulation, IDynamicListManipulation>, IDynamicListManipulation
    {
        private DynamicListManipulation() { }

        public Type ConstructNewListTypeFromType(Type typeOfList)
        {
            Type typeToInstantiate = typeOfList ?? typeof(object);
            Type genericListType = typeof(List<>);
            Type specificListType = genericListType.MakeGenericType(typeToInstantiate);

            return specificListType;
        }

        public IList CreateGenericListFromType(Type typeOfList)
        {
            Type specificListType = this.ConstructNewListTypeFromType(typeOfList);
            object list = Activator.CreateInstance(specificListType);
            dynamic objectListContainer = new ExpandoObject();

            objectListContainer.genericList = (IList)list;


            return objectListContainer.genericList;
        }

        public dynamic GetDynamicTypedList(IList genericList)
        {
            Type specificListType = this.ConstructNewListTypeFromType(genericList.GetType().GenericTypeArguments[0]);
            object list = Activator.CreateInstance(specificListType);
            dynamic concreteList = Convert.ChangeType(genericList, specificListType);

            return concreteList;
        }

        public void ConvertGenericListToTypedList<TEntity>(IList genericList, out IList<TEntity> convertedList)
        {
            IList list = genericList;
            Type[] allowableTypes = new Type[]
            {
                typeof(IList<TEntity>),
                typeof(List<TEntity>),
                typeof(IList<object>),
                typeof(List<object>)
            };

            if ((list == null) || (!allowableTypes.Contains(list.GetType())))
            {
                convertedList = new List<TEntity>();

                try
                {
                    list.Cast<TEntity>().ToList();
                }
                catch
                {
                    convertedList = new List<TEntity>();
                }

                return;
            }

            convertedList = list.Cast<TEntity>().ToList();
        }

        public void ConvertTypedListToGenericList<TEntity>(IList<TEntity> concreteList, out IList genericList)
        {
            if (concreteList == null)
            {
                genericList = ((IList)new List<object>());

                return;
            }

            genericList = (IList)concreteList;
        }

        public object ConvertEntityToObject<TEntity>(Type type, TEntity entity)
        {
            object entityObject = Convert.ChangeType(entity, type);

            return entityObject;
        }
    }
}
