using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Utilities
{
    public enum NavigationEnum
    {
        ObjectFound = 1,
        InvalidArguments = 2,
        ListChildNavigationRequired = 4,
        NavigationBlockedByUninstantiatedObject = 8,
        NavigationNotPossible = 16
    }

    public interface IObjectHierarchyNavigator
    {
        TExpectedType FindObjectFieldOrProperty<TExpectedType>(object instance, string propertyOrFieldSearchExpression);
        bool SetObjectFieldOrProperty<TType>(object instance, string propertyOrFieldSearchExpression, TType value);
        bool SetObjectFieldOrProperty<TType>(object instance, string propertyOrFieldSearchExpression, TType value, bool forceInstantiate);
        bool InstantiatePropertyOrField(object instance, string propertyOrFieldSearchExpression);
    }

    public class ObjectHierarchyNavigator : IObjectHierarchyNavigator
    {
        static IObjectHierarchyNavigator _instance;
        const char DELIMITER = '.';

        private ObjectHierarchyNavigator() { }

        public static IObjectHierarchyNavigator GetInstance()
        {
            if (_instance == null)
                _instance = new ObjectHierarchyNavigator();

            return _instance;
        }


        void SetMemberValue(MemberInfo currentObjectMemberInfo, object currentObject, object value)
        {
            if ((currentObjectMemberInfo == null) || (currentObject == null))
                return;

            bool convertToEnum = (((PropertyInfo)currentObjectMemberInfo).PropertyType.IsEnum)
                                    && ((value.GetType() == typeof(string)) || (value.GetType() == typeof(int)));

            object localValue = (convertToEnum) ? (Enum.Parse(((PropertyInfo)currentObjectMemberInfo).PropertyType, value.ToString())) : value;

            /*Set value to object instance of proper type*/
            if (currentObjectMemberInfo.MemberType == MemberTypes.Field)
                ((FieldInfo)currentObjectMemberInfo).SetValue(currentObject, localValue);
            else if (currentObjectMemberInfo.MemberType == MemberTypes.Property)
                ((PropertyInfo)currentObjectMemberInfo).SetValue(currentObject, localValue);
        }

        object GetMemberValue(MemberInfo currentObjectMemberInfo, object currentObject)
        {
            Type currentObjectType = null;

            return GetMemberValue(currentObjectMemberInfo, currentObject, out currentObjectType);
        }

        object GetMemberValue(MemberInfo currentObjectMemberInfo, object currentObject, out Type currentObjectType)
        {
            currentObjectType = null;

            if ((currentObjectMemberInfo == null) || (currentObject == null))
                return null;

            object memberValue = null;

            /*Get object instance from proper type*/
            if (currentObjectMemberInfo.MemberType == MemberTypes.Field)
            {
                currentObjectType = ((FieldInfo)currentObjectMemberInfo).FieldType;
                memberValue = ((FieldInfo)currentObjectMemberInfo).GetValue(currentObject);
            }
            else if (currentObjectMemberInfo.MemberType == MemberTypes.Property)
            {
                currentObjectType = ((PropertyInfo)currentObjectMemberInfo).PropertyType;
                memberValue = ((PropertyInfo)currentObjectMemberInfo).GetValue(currentObject, null);
            }

            return memberValue;
        }

        MemberInfo GetObjectPropertiesAndFields(Type objectType, string fieldOrPropertyToSearch)
        {
            if ((objectType == null) || (string.IsNullOrEmpty(fieldOrPropertyToSearch)))
                return null;

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            IDictionary<string, MemberInfo> FieldPropertyCollection = new Dictionary<string, MemberInfo>();

            foreach (var field in objectType.GetFields(flags))    //Get immediate public and non-public object fields
            {
                FieldPropertyCollection.Add(field.Name, field);
            }

            foreach (var property in objectType.GetProperties(flags))    //Get immediate public and non-public object properties
            {
                FieldPropertyCollection.Add(property.Name, property);
            }

            MemberInfo foundMemberInfo = null;

            try
            {
                foundMemberInfo = FieldPropertyCollection[fieldOrPropertyToSearch];
            }
            catch (KeyNotFoundException) { /*Skip process here*/}
            catch (Exception) { throw; }    //For everything else, elevate error handling to caller

            return foundMemberInfo;
        }

        /// Search field or property (Public or non-public) instance within the object graph recursively
        /// </summary>
        /// <typeparam name="TExpectedType">Type used to cast found object</typeparam>
        /// <param name="instance">Object instance containing nested object graphs where the search is applied</param>
        /// <param name="propertyOrFieldSearchExpression">Search expression used to locate a field or properties in an object.
        /// E.g.: object_1stDegree.object_2ndDegree...object_NthDegree.object_Leaf
        /// </param>
        /// <returns>Return object found from a search graph</returns>
        public TExpectedType FindObjectFieldOrProperty<TExpectedType>(object instance, string propertyOrFieldSearchExpression)
        {
            TExpectedType returnedInstance = default(TExpectedType);

            if ((instance == null) || string.IsNullOrEmpty(propertyOrFieldSearchExpression))
                return returnedInstance;

            object locatedObject = null;
            MemberInfo currentObjectMemberInfo = null;
            string searchExpression = propertyOrFieldSearchExpression;
            Type currentType = null;

            NavigateFieldOrProperty(ref searchExpression, instance, out currentType, out currentObjectMemberInfo, out locatedObject);

            if ((currentObjectMemberInfo != null) && (locatedObject != null))
                returnedInstance = (TExpectedType)GetMemberValue(currentObjectMemberInfo, locatedObject);

            return returnedInstance;
        }

        NavigationEnum NavigateFieldOrProperty(ref string propertyOrFieldSearchExpression, object navigatedInstance, out Type locatedType,
            out MemberInfo locatedInstanceMemberInfo, out object locatedObject)
        {
            locatedInstanceMemberInfo = null;
            locatedObject = null;
            locatedType = null;

            if ((navigatedInstance == null) || string.IsNullOrEmpty(propertyOrFieldSearchExpression))
                return NavigationEnum.InvalidArguments;


            string[] linearObjectHierarchy = propertyOrFieldSearchExpression.Split(DELIMITER);
            MemberInfo currentMemberInfo = GetObjectPropertiesAndFields(navigatedInstance.GetType(), linearObjectHierarchy[0]);

            propertyOrFieldSearchExpression = string.Empty; //Reset search expression;

            if (currentMemberInfo != null)
            {
                //Determines the hierarchy and degree of recursion from object graph
                int searchDegreeCount = linearObjectHierarchy.Length;
                object currentMemberObject = GetMemberValue(currentMemberInfo, navigatedInstance, out locatedType);

                if (searchDegreeCount > 1) //Continue recursive search until last search degree
                {
                    int newSearchDegreeCount = searchDegreeCount - 1;
                    string[] newMembers = new string[newSearchDegreeCount];

                    //Construct new search expression, omitting current object searched
                    Array.Copy(linearObjectHierarchy, 1, newMembers, 0, newSearchDegreeCount);

                    if (currentMemberObject == null)
                    {
                        //Navigation blocked, even-though found but was uninstantiated
                        //Pass back to caller, let it decide what to do
                        propertyOrFieldSearchExpression = string.Join(DELIMITER.ToString(), linearObjectHierarchy);
                        locatedInstanceMemberInfo = currentMemberInfo;
                        locatedObject = null;

                        return NavigationEnum.NavigationBlockedByUninstantiatedObject;
                    }

                    propertyOrFieldSearchExpression = string.Join(DELIMITER.ToString(), newMembers);

                    if (InstanceIsCollectionType(currentMemberObject))
                    {
                        //Stop here since collection navigation is not possible
                        locatedObject = currentMemberObject;

                        return NavigationEnum.ListChildNavigationRequired;
                    }

                    //Search object graph recursively
                    return NavigateFieldOrProperty(ref propertyOrFieldSearchExpression, currentMemberObject,
                        out locatedType, out locatedInstanceMemberInfo, out locatedObject);
                }
                else //Stop search since current degree is last degree
                {
                    locatedInstanceMemberInfo = currentMemberInfo;
                    locatedObject = navigatedInstance;

                    return NavigationEnum.ObjectFound;
                }
            }

            return NavigationEnum.NavigationNotPossible;
        }

        bool InstanceIsCollectionType(object instance)
        {
            return (instance != null) && (TypeIsCollectionType(instance.GetType()));
        }

        bool TypeIsCollectionType(Type instanceType)
        {
            return (typeof(ICollection<>).IsAssignableFrom(instanceType) ||
                   typeof(ICollection).IsAssignableFrom(instanceType));
        }

        bool TypeIsListType(Type instanceType)
        {
            return ((typeof(IList<>).Name == instanceType.Name) || (typeof(IList).Name == instanceType.Name));
        }

        public bool SetObjectFieldOrProperty<TType>(object instance, string propertyOrFieldSearchExpression, TType value)
        {
            if ((instance == null) || (string.IsNullOrEmpty(propertyOrFieldSearchExpression)))
                return false;

            return SetObjectFieldOrProperty(instance, propertyOrFieldSearchExpression, value, false);
        }

        public bool SetObjectFieldOrProperty<TType>(object instance, string propertyOrFieldSearchExpression, TType value, bool forceInstantiate)
        {
            if ((instance == null) || (string.IsNullOrEmpty(propertyOrFieldSearchExpression)))
                return false;

            object locatedObject = null;
            MemberInfo currentObjectMemberInfo = null;
            string searchExpression = propertyOrFieldSearchExpression;
            Type currentType = null;

            NavigationEnum navigationResult = NavigateFieldOrProperty(ref searchExpression, instance, out currentType, out currentObjectMemberInfo, out locatedObject);

            if (navigationResult == NavigationEnum.ListChildNavigationRequired)     //Object found but in a list element
            {
                //Reflecting changes against collection (Array, List, etc.)
                IEnumerator enumerator = ((ICollection)locatedObject).GetEnumerator();
                bool listElementsModified = false;      //Whenever list has no element

                while (enumerator.MoveNext())
                {
                    object element = enumerator.Current;

                    SetObjectFieldOrProperty(element, searchExpression, value);

                    listElementsModified = true;
                }

                return listElementsModified;
            }
            else if (navigationResult == NavigationEnum.NavigationNotPossible)      //Object not found, thus exiting function
                return false;
            else if (navigationResult == NavigationEnum.ObjectFound)
                SetMemberValue(currentObjectMemberInfo, locatedObject, value);
            else if (navigationResult == NavigationEnum.NavigationBlockedByUninstantiatedObject)
            {
                if (!forceInstantiate)
                    return false;

                if (!InstantiatePropertyOrField(instance, propertyOrFieldSearchExpression))
                    return false;

                //Iterate the newly instantiated property/field
                return SetObjectFieldOrProperty(instance, propertyOrFieldSearchExpression, value, forceInstantiate);
            }

            return true;
        }

        public bool InstantiatePropertyOrField(object instance, string propertyOrFieldSearchExpression)
        {
            bool instantiated = false;
            string fieldToInstantiate = propertyOrFieldSearchExpression.Split(DELIMITER)[0];
            object locatedObject = null;
            MemberInfo currentObjectMemberInfo = null;
            Type currentType = null;

            NavigateFieldOrProperty(ref fieldToInstantiate, instance, out currentType, out currentObjectMemberInfo, out locatedObject);

            try
            {
                object instantiatedObject = null;

                if (TypeIsListType(currentType))
                    instantiatedObject = DynamicListManipulation.GetInstance().CreateGenericListFromType(currentType.GenericTypeArguments[0]);
                else
                    instantiatedObject = Activator.CreateInstance(currentType);


                if (instantiatedObject != null)
                {
                    SetMemberValue(currentObjectMemberInfo, locatedObject, instantiatedObject);

                    instantiated = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return instantiated;
        }
    }
}
