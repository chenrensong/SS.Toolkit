using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace SS.Toolkit.Extensions
{
    public static class TypeExtension
    {

        // http://rogeralsing.com/2008/02/28/linq-expressions-creating-objects/
        // As an laternative just call: dynamic badge = Activator.CreateInstance(badgeMapping.BadgeClass);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public delegate T ObjectActivator<out T>(params object[] args);


        public static ObjectActivator<T> GetActivator<T>(this ConstructorInfo ctor)
        {
            var paramsInfo = ctor.GetParameters();

            // Create a single param of type object[]
            var param = Expression.Parameter(typeof(object[]), "args");

            var argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array and create a typed expression of them
            for (var i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp = Expression.ArrayIndex(param, index);

                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            // Make a NewExpression that calls the //ctor with the args we just created
            var newExp = Expression.New(ctor, argsExp);

            // Create a lambda with the New expression as body and our param object[] as arg
            var lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            // Compile it
            var compiled = (ObjectActivator<T>)lambda.Compile();

            return compiled;
        }

        public static IEnumerable<T> GetTypes<T>(this Assembly assembly)
        {
            var result = new List<T>();

            var types = assembly.GetTypes()
                .Where(t => t.GetTypeInfo().IsClass && typeof(T).IsAssignableFrom(t))
                .ToList();

            foreach (var type in types)
            {
                var instance = (T)Activator.CreateInstance(type);
                result.Add(instance);
            }

            return result;
        }

        // FindInterfaces
        // This method will filter the interfaces supported the class
        public static Type[] FindInterfaces(this Type type, TypeFilter filter, Object filterCriteria)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            Contract.EndContractBlock();
            Type[] c = type.GetInterfaces();
            int cnt = 0;
            for (int i = 0; i < c.Length; i++)
            {
                if (!filter(c[i], filterCriteria))
                    c[i] = null;
                else
                    cnt++;
            }
            if (cnt == c.Length)
                return c;

            Type[] ret = new Type[cnt];
            cnt = 0;
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] != null)
                    ret[cnt++] = c[i];
            }
            return ret;
        }
    }
}
