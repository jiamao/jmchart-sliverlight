using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Reflection.Emit;

namespace Silverlight.Common.Reflection
{
    /// <summary>
    /// 类帮助
    /// </summary>
    public static class DynamicHelper
    {
        /// <summary>
        /// Holds previously generated DTO types
        /// </summary>
        private static System.Collections.Generic.Dictionary<string, Type> _cachedTypes = new System.Collections.Generic.Dictionary<string,Type>();

        /// <summary>
        /// Creates DTO property
        /// </summary>
        /// <param name="typeBuilder">Type builder</param>
        /// <param name="propertyName">Property name</param>
        private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, new Type[0]);

            // Define "get"
            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    propertyType, Type.EmptyTypes);

            // Get MethodInfo for base class (DataRow)
            var getmname = "GetValue";
            var setmname = "SetValue";
            switch (propertyType.FullName)
            {
                case "System.Decimal":
                    {
                        getmname = "GetDecimalValue";
                        setmname = "SetDecimalValue";
                        break;
                    }
                case "System.DateTime":
                    {
                        getmname = "GetDateTimeValue";
                        setmname = "SetDateTimeValue";
                        break;
                    }
            }
            MethodInfo getMethodInfo = typeof(BindableObject).GetMethod(getmname, new Type[] { typeof(string) });

            // Generate content
            ILGenerator getILGenerator = getMethodBuilder.GetILGenerator();
            getILGenerator.Emit(OpCodes.Ldarg_0); // Load BindableObject on stack
            getILGenerator.Emit(OpCodes.Ldstr, propertyName); // Load property name
            getILGenerator.Emit(OpCodes.Call, getMethodInfo); // Call GetValue(string) method            
            getILGenerator.Emit(OpCodes.Ret); // Return

            // Define set
            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new Type[] { propertyType });

            // Get MethodInfo for base class (DataRow)
            MethodInfo setMethodInfo = null;
            //if (propertyType == typeof(decimal))
            //{
            //    setMethodInfo = typeof(OAPBindableObject).GetMethod("SetDecimalValue", new Type[] { typeof(string), propertyType });
            //}
            //else
            //{
            setMethodInfo = typeof(BindableObject).GetMethod(setmname, new Type[] { typeof(string), propertyType });
            //}

            // Generate content
            ILGenerator setILGenerator = setMethodBuilder.GetILGenerator();
            setILGenerator.Emit(OpCodes.Ldarg_0); // Load BindableObject on stack
            setILGenerator.Emit(OpCodes.Ldstr, propertyName); // Load property name
            setILGenerator.Emit(OpCodes.Ldarg_1); // Load value on stack
            setILGenerator.Emit(OpCodes.Call, setMethodInfo); // Call set_Item(string, string) method
            setILGenerator.Emit(OpCodes.Ret);

            // Assign "get" and "set" to "property"
            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }

        /// <summary>
        /// Creates DTO property
        /// </summary>
        /// <param name="typeBuilder">Type builder</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="supportSuffix">Support property suffix</param>
        /// <param name="propertyType">Property type</param>
        /// <param name="createGetAccessor">True if Get accessor need to be created</param>
        /// <param name="createSetAccessor">True if Set accessor need to be created</param>
        private static void CreateSupportProperty(TypeBuilder typeBuilder, string propertyName, string supportSuffix, Type propertyType, bool createGetAccessor, bool createSetAccessor)
        {
            // Generate DTO "property"
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName + supportSuffix, PropertyAttributes.None, propertyType, new Type[0]);

            if (createGetAccessor)
            {
                // Define "get"
                MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName + supportSuffix,
                        MethodAttributes.Public |
                        MethodAttributes.SpecialName |
                        MethodAttributes.HideBySig,
                        propertyType, Type.EmptyTypes);

                // Get MethodInfo for base class (DataRow)
                MethodInfo getMethodInfo = typeof(BindableObject).GetMethod("Get" + supportSuffix, BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);

                // Generate content
                ILGenerator getILGenerator = getMethodBuilder.GetILGenerator();
                getILGenerator.Emit(OpCodes.Ldarg_0); // Load this on stack
                if (string.IsNullOrEmpty(propertyName))
                {
                    getILGenerator.Emit(OpCodes.Ldstr, supportSuffix); // Load property name
                }
                else
                {
                    getILGenerator.Emit(OpCodes.Ldstr, propertyName); // Load property name
                }
                getILGenerator.Emit(OpCodes.Call, getMethodInfo); // Call GetValue(string) method
                getILGenerator.Emit(OpCodes.Ret); // Return

                // Assign "get" to "property"
                propertyBuilder.SetGetMethod(getMethodBuilder);
            }

            if (createSetAccessor)
            {
                // Define set
                MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName + supportSuffix,
                      MethodAttributes.Public |
                      MethodAttributes.SpecialName |
                      MethodAttributes.HideBySig,
                      null, new Type[] { propertyType });

                // Get MethodInfo for base class (DataRow)
                MethodInfo setMethodInfo = typeof(BindableObject).GetMethod("Set" + supportSuffix, BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string), propertyType }, null);

                // Generate content
                ILGenerator setILGenerator = setMethodBuilder.GetILGenerator();
                setILGenerator.Emit(OpCodes.Ldarg_0); // Load BindableObject on stack
                setILGenerator.Emit(OpCodes.Ldstr, propertyName); // Load property name
                setILGenerator.Emit(OpCodes.Ldarg_1); // Load value on stack
                setILGenerator.Emit(OpCodes.Call, setMethodInfo); // Call set_Item(string, string) method
                setILGenerator.Emit(OpCodes.Ret);

                // Assign "get" and "set" to "property"
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }
        }

        /// <summary>
        /// Creates DTO type (class)
        /// </summary>
        /// <param name="data">data as a data source</param>
        /// <param name="addSupportProperties">Tru for generating properties with suffixes Required, Error, and HasError</param>
        /// <returns>Newly created type</returns>
        private static Type CreateType(DataCollection data, bool addSupportProperties, string dtoTypeFullName)
        {
            // Get type builder from newly created assembly
            // Define dynamic assembly
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = dtoTypeFullName + ".dll";
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            // Define type
            TypeBuilder typeBuilder = moduleBuilder.DefineType(dtoTypeFullName
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , typeof(BindableObject));

            // Get ConstructorInfo for base class (BindableObject)
            ConstructorInfo constructorInfo = typeof(BindableObject).GetConstructor(new Type[] { typeof(DataCollection) });

            // Define DTO constructor
            //var defaultConstructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            Type[] constructorArguments = new Type[] { typeof(DataCollection) };
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorArguments);

            // Generate DTO constructor
            ILGenerator getILGenerator = constructorBuilder.GetILGenerator();
            getILGenerator.Emit(OpCodes.Ldarg_0); // Load BindableObject on stack
            getILGenerator.Emit(OpCodes.Ldarg_1); // Load DataRow on stack
            getILGenerator.Emit(OpCodes.Call, constructorInfo); // Call BindableObject constructor
            getILGenerator.Emit(OpCodes.Ret);

            //// Generate DTO constructor
            //var defaultgenerator = defaultConstructor.GetILGenerator();
            //defaultgenerator.Emit(OpCodes.Ldarg_0); // Load BindableObject on stack
            //defaultgenerator.Emit(OpCodes.Ldarg_1); // Load DataRow on stack    
            ////defaultgenerator.Emit(OpCodes.Call); // Call BindableObject constructor
            //defaultgenerator.Emit(OpCodes.Ret);

            foreach (var dc in data.Items)
            {
                Type datatype = typeof(string);

                if (Data.TypeHelper.IsNumber(dc.DataType))
                {
                    datatype = typeof(decimal);
                }
                else if (typeof(DateTime) == dc.DataType)
                {
                    datatype = typeof(DateTime);
                }

                CreateProperty(typeBuilder, dc.Key, datatype);
                //if (addSupportProperties)
                //{
                //    CreateSupportProperty(typeBuilder, dc.ColumnName, "Required", Type.GetType("System.Boolean"), true, false);
                //    CreateSupportProperty(typeBuilder, dc.ColumnName, "HasError", Type.GetType("System.Boolean"), true, false);
                //    CreateSupportProperty(typeBuilder, dc.ColumnName, "Error", Type.GetType("System.String"), true, false);
                //    CreateSupportProperty(typeBuilder, dc.ColumnName, "DataSourceNullValue", Type.GetType("System.String"), true, true);
                //}
            }

            //if (addSupportProperties)
            //{
            //    CreateSupportProperty(typeBuilder, null, "HasNoErrors", Type.GetType("System.Boolean"), true, false);
            //}

            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Generates bindable object
        /// </summary>
        /// <param name="dataRow">DataRow instance</param>
        /// <param name="addSupportProperties">True for generating properties with suffixes Required, Error, and HasError</param>
        /// <returns>Generated bindable object</returns>
        public static BindableObject GetBindableData(DataCollection data, Type dtoType)
        {
            var obj = Activator.CreateInstance(dtoType, data);//, new object[] { dataRow }

            return (BindableObject)obj;
        }

        /// <summary>
        /// 转换为列表
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static System.Collections.IList ConvertToList(System.Collections.Generic.IEnumerable<DataCollection> datas, string format = "yyyy-MM-dd HH:mm:ss")
        {
            string dtoTypeFullName;
            var addSupportProperties = false;
            if (addSupportProperties)
            {
                dtoTypeFullName = GetTypeId(datas) + "1";
            }
            else
            {
                dtoTypeFullName = GetTypeId(datas) + "0";
            }

            Type dtoType = null;
            if (_cachedTypes.ContainsKey(dtoTypeFullName))
            {
                // Get type from cache
                dtoType = _cachedTypes[dtoTypeFullName];
            }
            else
            {
                // Cache new type
                dtoType = CreateType(datas.ElementAt<DataCollection>(0), addSupportProperties, dtoTypeFullName);
                _cachedTypes.Add(dtoTypeFullName, dtoType);
            }

            var dtoCollection = new System.Collections.ObjectModel.ObservableCollection<BindableObject>();
            foreach(var d in datas)
            {
                dtoCollection.Add(GetBindableData(d, dtoType));
            }

            return dtoCollection;
        }

        /// <summary>
        /// Gets generated type id
        /// </summary>
        public static string GetTypeId(System.Collections.Generic.IEnumerable<DataCollection> datas)
        {

            var stringBuilder = new System.Text.StringBuilder();
            foreach (var dc in datas)
            {
                var arr = (from p in dc.Items
                           select p.ToString()).ToArray<string>();
                stringBuilder.Append(string.Join(",", arr));
               
            }

            string typeId = stringBuilder.ToString();
            int hashCode = typeId.GetHashCode();
            if (hashCode < 0)
            {
                hashCode = -1 * hashCode;
                return "T0" + hashCode.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return "T1" + hashCode.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}
