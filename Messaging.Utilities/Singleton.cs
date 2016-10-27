using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Utilities
{
    public abstract class Singleton<TConcrete, TInterface>
        where TConcrete : TInterface
    {
        static TConcrete _instance = InstantiatePrivateConstructor(new Type[] { }, new object[] { });       //Very thread-safe but not very lazy instantiated

        public static TInterface GetInstance()
        {
            return _instance;
        }

        public static TConcrete GetInstance(ref TConcrete instance, bool alwaysCreateNew)
        {
            TConcrete newInstance = instance;

            Type[] expectedConstructorParameterTypes = new Type[] { };
            object[] expectedConstructorParameterValues = new object[] { };

            if ((instance == null) || (alwaysCreateNew))
                newInstance = InstantiatePrivateConstructor(expectedConstructorParameterTypes, expectedConstructorParameterValues);

            instance = newInstance;

            return newInstance;
        }

        static TConcrete InstantiatePrivateConstructor(
            Type[] constructureParameterTypes,
            object[] constructureParameterValues
            )
        {
            Type concreteType = typeof(TConcrete);
            Type[] parameterTypes = constructureParameterTypes;

            ConstructorInfo constructor = concreteType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                parameterTypes,
                null
                );

            TConcrete instance = (TConcrete)constructor.Invoke(constructureParameterValues);

            return instance;
        }
    }
}
