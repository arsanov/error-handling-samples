using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Input;

namespace Mvvm.Core
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public static class ViewModelTypeCreator
    {
        private static TypeBuilder GetTypeBuilder(Type viewModelType)
        {
            var assembly = viewModelType.Assembly;

            var typeSignature = $"{viewModelType.Name}_NotifyingViewModel";

            var an = new AssemblyName(assembly.GetName().Name + typeSignature);
            an.Version = assembly.GetName().Version;
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("ClassExtensionModule");
            var tb = moduleBuilder.DefineType(typeSignature, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, viewModelType);
            return tb;
        }

        public static Type CompileResultType(Type sourceType)
        {
            var properties = sourceType.GetProperties().Where(p => p.GetGetMethod().IsVirtual).ToList();

            if (properties.Count != 0)
            {
                var tb = GetTypeBuilder(sourceType);

                foreach (var property in properties)
                {
                    if (property.GetSetMethod() != null)
                        CreateProperty(tb, property);
                }

                var constructors = sourceType.GetConstructors();

                foreach (var constructor in constructors)
                {
                    CreateConstructor(tb, constructor);
                }

                var objectType = tb.CreateTypeInfo();
                return objectType.AsType();
            }
            else
            {
                return sourceType;
            }
        }

        private static void CreateProperty(TypeBuilder typeBuilder, PropertyInfo info)
        {
            var propertyBuilder = typeBuilder.DefineProperty(info.Name, PropertyAttributes.None, info.PropertyType, Type.EmptyTypes);

            var getterBuilder = typeBuilder.DefineMethod("get_" + info.Name, MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, info.PropertyType, Type.EmptyTypes);
            var getterBuilderGenerator = getterBuilder.GetILGenerator();

            getterBuilderGenerator.Emit(OpCodes.Ldarg_0);
            getterBuilderGenerator.Emit(OpCodes.Call, info.GetGetMethod());
            getterBuilderGenerator.Emit(OpCodes.Ret);

            var setterBuilder = typeBuilder.DefineMethod("set_" + info.Name, MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { info.PropertyType });
            var setterBuilderGenerator = setterBuilder.GetILGenerator();
            setterBuilderGenerator.Emit(OpCodes.Ldarg_0);
            setterBuilderGenerator.Emit(OpCodes.Ldarg_1);

            setterBuilderGenerator.Emit(OpCodes.Call, info.GetSetMethod());

            setterBuilderGenerator.Emit(OpCodes.Ldarg_0);
            setterBuilderGenerator.Emit(OpCodes.Ldstr, info.Name);

            setterBuilderGenerator.Emit(OpCodes.Call, typeof(ViewModelBase).GetMethod("OnPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic));

            setterBuilderGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(setterBuilder);
            propertyBuilder.SetGetMethod(getterBuilder);
        }

        private static void CreateConstructor(TypeBuilder tb, ConstructorInfo info)
        {
            var constructorBuilder = tb.DefineConstructor(info.Attributes, info.CallingConvention, info.GetParameters().Select(p => p.ParameterType).ToArray());

            var constructorBuilderGenerator = constructorBuilder.GetILGenerator();

            constructorBuilderGenerator.Emit(OpCodes.Ldarg_0);

            var parameters = info.GetParameters();

            for(var i = 0; i < parameters.Length; i++)
            {
                constructorBuilderGenerator.Emit(OpCodes.Ldarg, i+1);
            }

            constructorBuilderGenerator.Emit(OpCodes.Call, info);
            constructorBuilderGenerator.Emit(OpCodes.Ret);
        }
    }
}
