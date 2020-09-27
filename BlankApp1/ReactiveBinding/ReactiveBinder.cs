using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;

namespace BlankApp1.ReactiveBinding
{
    public class ReactiveBinder<TSource, TTarget>
        where TTarget : INotifyPropertyChanged
    {
        private readonly static Dictionary<string, Action<TSource, TTarget>> binderActions = new Dictionary<string, Action<TSource, TTarget>>();
        private readonly static Dictionary<string, Action<TSource, TTarget>> observableBinderActions = new Dictionary<string, Action<TSource, TTarget>>();

        private readonly static Type sourceType = typeof(TSource);
        private readonly static Type targetType = typeof(TTarget);

        static ReactiveBinder()
        {
            if(sourceType.GetInterface(nameof(INotifyPropertyChanged)) != null)
            {
                BuildObservableBinders();
            }
            else
            {
                BuildBinders();
            }
        }

        public void Bind(TSource source, TTarget target)
        {
            foreach (var action in binderActions)
            {
                action.Value.Invoke(source, target);
            }
        }
        public void BindObservable(TSource source, TTarget target)
        {
            foreach (var action in observableBinderActions)
            {
                action.Value.Invoke(source, target);
            }
        }

        private static void BuildBinders()
        {
            MethodInfo fromObject = GetBinderMethod(targetType);

            var sourceParameter = Expression.Parameter(sourceType, "$source");
            var targetParameter = Expression.Parameter(targetType, "$destination");
            var modeParameter = Expression.Constant(ReactivePropertyMode.Default);
            var ignoreValidationErrorValue = Expression.Constant(false);

            var targetProperties = targetType.GetProperties()
                .Where(p => p.GetCustomAttribute<AutoBindingPropertyAttribute>() != null);
            foreach (var property in targetProperties)
            {
                var sourceProperty = sourceType.GetProperty(property.Name);
                var sourcePropertyAccessor = Expression.Property(
                    Expression.Convert(sourceParameter, sourceType),
                    property.Name);

                var propertySelector = Expression.Lambda(sourcePropertyAccessor, sourceParameter);

                var genericMethod = fromObject.MakeGenericMethod(new Type[] { sourceType, sourceProperty.PropertyType });
                var binderMethodCall = Expression.Call(genericMethod, sourceParameter, propertySelector, modeParameter, ignoreValidationErrorValue);

                var bindingProperty = Expression.Property(
                    targetParameter,
                    property);

                var assignment = Expression.Assign(bindingProperty, binderMethodCall);

                var lambda = Expression.Lambda<Action<TSource, TTarget>>(
                    assignment,
                    sourceParameter,
                    targetParameter
                    );

                binderActions.Add(property.Name, lambda.Compile());
            }
        }
        private static void BuildObservableBinders()
        {
            MethodInfo binderMethod = GetObservableBinderMethod(targetType);

            var sourceParameter = Expression.Parameter(sourceType, "$source");
            var targetParameter = Expression.Parameter(targetType, "$destination");
            var modeParameter = Expression.Constant(ReactivePropertyMode.Default);
            var ignoreValidationErrorValue = Expression.Constant(false);

            var targetProperties = targetType.GetProperties()
                .Where(p => p.GetCustomAttribute<AutoBindingPropertyAttribute>() != null);
            foreach (var property in targetProperties)
            {
                var sourceProperty = sourceType.GetProperty(property.Name);
                var sourcePropertyAccessor = Expression.Property(
                    Expression.Convert(sourceParameter, sourceType),
                    property.Name);

                var propertySelector = Expression.Lambda(sourcePropertyAccessor, sourceParameter);

                var genericMethod = binderMethod.MakeGenericMethod(new Type[] { sourceType, sourceProperty.PropertyType });
                var binderMethodCall = Expression.Call(genericMethod, sourceParameter, propertySelector, modeParameter, ignoreValidationErrorValue);

                var bindingProperty = Expression.Property(
                    targetParameter,
                    property);

                var assignment = Expression.Assign(bindingProperty, binderMethodCall);

                var lambda = Expression.Lambda<Action<TSource, TTarget>>(
                    assignment,
                    sourceParameter,
                    targetParameter
                    );

                observableBinderActions.Add(property.Name, lambda.Compile());
            }
        }
        private static MethodInfo GetBinderMethod(Type targetType)
        {
            MethodInfo fromObject = null;
            Type[] paramTypes = new Type[] {
                targetType,
                typeof(int),
                typeof(ReactivePropertyMode),
                typeof(bool)
            };
            var flags = BindingFlags.Static | BindingFlags.Public;
            var methods = typeof(ReactiveProperty).GetMethods(flags)
           .Where(x => x.Name == "FromObject" &&
               x.IsGenericMethod &&
               x.IsGenericMethodDefinition &&
               x.ContainsGenericParameters);
            foreach (MethodInfo method in methods)
            {
                if (method.Name == "FromObject" && method.GetParameters().Length == paramTypes.Length)
                {
                    fromObject = method;
                    break;
                }
            }
            return fromObject;
        }
        private static MethodInfo GetObservableBinderMethod(Type targetType)
        {
            MethodInfo targetMethod = null;
            var parameterTypes = new Type[] { typeof(TSource), typeof(Func<,>), typeof(ReactivePropertyMode), typeof(bool) };
            var flags = BindingFlags.Static | BindingFlags.Public;
            var methods = typeof(INotifyPropertyChangedExtensions).GetMethods(flags)
                .Where(x => x.Name == "ToReactivePropertyAsSynchronized" && 
                    x.IsGenericMethod && 
                    x.IsGenericMethodDefinition && 
                    x.ContainsGenericParameters);

            foreach (MethodInfo method in methods)
            {

                if (method.GetParameters().Length == parameterTypes.Length)
                {
                    targetMethod = method;
                    break;
                }
            }
            return targetMethod;
        }

    }
}
