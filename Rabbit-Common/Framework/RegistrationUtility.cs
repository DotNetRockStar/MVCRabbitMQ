using Jerrod.RabbitCommon.Framework.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework
{
    public delegate object ResolveControllerDependencyHandler(Type type, Type controller);
    public delegate IRulesRepository ResolveRulesHandler(RabbitController controller);

    /// <summary>
    /// The utility class used to register RabbitController's and cleanup RabbitController's and their registered methods.
    /// </summary>
    public static class RegistrationUtility
    {
        /// <summary>
        /// Invoked when rules are being requested.
        /// </summary>
        public static ResolveRulesHandler ResolveRules { get; set; }
        /// <summary>
        /// (IoC) Invoked when a controller dependency is being requested.
        /// </summary>
        public static ResolveControllerDependencyHandler ResolveControllerDependency { get; set; }
        static RegistrationUtility()
        {
            ResolveControllerDependency = (type, controller) =>
            {
                return Activator.CreateInstance(type);
            };

            ResolveRules = (controller) =>
            {
                return new AllowAllRulesRepository();
               // return new DBRulesRepository();
            };
        }

        internal static List<MethodContainer> _methodContainers = new List<MethodContainer>();

        /// <summary>
        /// Register a RabbitController by specifying the specific instantiated controller.
        /// </summary>
        /// <typeparam name="T">The type of the RabbitController to be registered.</typeparam>
        /// <param name="controller">The instantiated RabbitController object to register.</param>
        public static void RegisterController<T>(T controller) where T : RabbitController
        {
            var exists = _methodContainers.FirstOrDefault(d => d.Controller.GetType().FullName == typeof(T).FullName);
            if (exists != null)
                throw new Exception("Unable to register controller " + typeof(T).FullName + " because it has already been registered.");

            try
            {
                var methods = typeof(T).GetMethods();
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(true);
                    foreach (var attribute in attributes)
                    {
                        if (attribute is RabbitMethodAttribute)
                        {
                            // found one
                            RegisterMethod(method, controller, attribute as RabbitMethodAttribute);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UnRegisterController<T>();
                throw;
            }
        }

        /// <summary>
        /// Register a RabbitController by specifying the type.  Any dependencies that the RabbitController has will be requested by invoking the ResolveControllerDependency delegate.
        /// </summary>
        /// <typeparam name="T">The type of the RabbitController to register.</typeparam>
        public static void RegisterController<T>() where T : RabbitController
        {
            var constructors = typeof(T).GetConstructors();
            if (constructors != null && constructors.Count() > 1)
            {
                throw new Exception("Unable to register controller " + typeof(T).FullName + " because there are multiple constructors.  Make sure there is 1 constructor and try again.");
            }

            var ctor = constructors.First();
            var inputParams = ctor.GetParameters();
            List<object> inputParamInstances = new List<object>();
            if (inputParams != null && inputParams.Any())
            {
                if (ResolveControllerDependency == null)
                    throw new Exception("Unable to resolve input parameters for controller " + typeof(T).FullName + " because RegistrationUtility.ResolveControllerDependency has not been set.");

                foreach (var param in inputParams)
                {
                    inputParamInstances.Add(ResolveControllerDependency.Invoke(param.ParameterType, typeof(T)));
                }
            }

            T service = (T)Activator.CreateInstance(typeof(T), inputParamInstances.ToArray());
            RegisterController<T>(service);
        }

        /// <summary>
        /// Unregister a RabbitController by specifying the type.  This will make it so that the controller is no longer invoked when a message comes in.
        /// </summary>
        /// <typeparam name="T">The type of controller that is to be unregistered.</typeparam>
        /// <returns>Returns true if the controller was unregistered and false if it was not.</returns>
        public static bool UnRegisterController<T>() where T : RabbitController
        {
            var target = _methodContainers.FirstOrDefault(d => d.Controller.GetType().FullName == typeof(T).FullName);
            if (target != null)
            {
                target.Listener.Dispose();
                _methodContainers.Remove(target);
                return true;
            }

            return false;
        }

        private static void RegisterMethod(MethodInfo methodInfo, RabbitController controller, RabbitMethodAttribute rabbitMethodAttr)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo");

            var inputParameters = methodInfo.GetParameters();
            var paramCount = inputParameters.Count();

            if (paramCount == 0)
                throw new Exception("Unable to register RabbitMethod " + methodInfo.Name + " because there are no input parameters.  Ensure that the message that is being listened for is in the input parameter list.");
            else if (paramCount > 1)
                throw new Exception("Unable to register RabbitMethod " + methodInfo.Name + " because there are more than 1 input parameters for the method.  Make sure there is 1 and only 1 input parameter which is the message that the method is expecting.");

            ParameterInfo messageParam = inputParameters.First();

            var type = messageParam.ParameterType;
            if (!type.IsAssignableFrom(typeof(MessageBase)) && !typeof(MessageBase).IsAssignableFrom(type))
            {
                throw new Exception("Unable to register RabbitMethod " + methodInfo.Name + " because the input parameter for this method does not inherit " + typeof(MessageBase).FullName + ". Make sure the input parameter inherits this class.");
            }

            var messageType = messageParam.ParameterType;
            var returnType = methodInfo.ReturnType;
            if (returnType.FullName == "System.Void")
                returnType = null;

            string routingKey = rabbitMethodAttr.RoutingKey;
            if (string.IsNullOrWhiteSpace(routingKey))
                routingKey = messageType.FullName;

            var existingItem = _methodContainers.FirstOrDefault(d => 
                d.MessageType.FullName == messageType.FullName && d.RoutingKey == routingKey);

            if (existingItem != null)
                throw new ArgumentNullException("There is already a method listeneing for message of type " + messageType.FullName + ".  Existing Method: " + existingItem.Controller.GetType().Name + "." + existingItem.Method.Name + "(" + messageType.FullName + " message)");

            Type methodInvokerType = typeof(RabbitMethodContainer<,>);
            if (returnType == null)
                methodInvokerType = methodInvokerType.MakeGenericType(new Type[] { messageType, typeof(object) });
            else
                methodInvokerType = methodInvokerType.MakeGenericType(new Type[] { messageType, returnType });

            var instance = Activator.CreateInstance(methodInvokerType, controller, methodInfo);

            Type listenerType = typeof(Server<,>);

            if (returnType == null)
                listenerType = listenerType.MakeGenericType(new Type[] { messageType, typeof(object) });
            else
            {
                if (messageType.FullName == returnType.FullName)
                    throw new Exception("Unable to register " + methodInfo.Name + " method for " + controller.GetType().FullName + " controller.  Method cannot return the same type as the input as this would result in a runaway pub/sub cycle.");

                listenerType = listenerType.MakeGenericType(new Type[] { messageType, returnType });
            }

            // The way that I am doing this requires the constructors input parameters to be in a certain order...  Fix later?
            List<object> inputParametersForServer = new List<object>();
            inputParametersForServer.Add(instance);

            if (controller.Exchange != null)
                inputParametersForServer.Add(controller.Exchange);

            if (!string.IsNullOrWhiteSpace(rabbitMethodAttr.RoutingKey))
            {
                inputParametersForServer.Add(new Settings() { RoutingKey = rabbitMethodAttr.RoutingKey });
            }

            inputParametersForServer.Add(controller.Settings);

            var rules = ResolveRules(controller);
            object listener = null;

            try
            {
                listener = Activator.CreateInstance(listenerType, inputParametersForServer.ToArray());
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw new ApplicationException("Unable to register controller " + controller.GetType().FullName + ".", ex.InnerException);
                }
                else
                    throw;
            }

            _methodContainers.Add(new MethodContainer()
            {
                MessageType = messageType,
                Method = methodInfo,
                ReturnType = returnType,
                Controller = controller,
                Listener = listener as IDisposable,
                RoutingKey = string.IsNullOrWhiteSpace(rabbitMethodAttr.RoutingKey) ? messageType.FullName : rabbitMethodAttr.RoutingKey
            });
        }

        /// <summary>
        /// Cleanup.  Dispose and unregister all controllers currently registered.
        /// </summary>
        /// <param name="forceExitAfterDispose">If set to true, this will call Environment.Exit(1) causing the app pool to force quit ensuring all threads are closed.</param>
        public static void DisposeAndExit()
        {
            foreach (var container in _methodContainers)
                container.Listener.Dispose();
            _methodContainers.Clear();

            Environment.Exit(1);
        }

        public static void RegisterAllControllers()
        {
            List<Assembly> allAssemblies = new List<Assembly>();
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            RegisterAllControllers(path);
        }

        public static void RegisterAllControllers(string path)
        {
            List<Assembly> allAssemblies = new List<Assembly>();
            foreach (string dll in Directory.GetFiles(path, "*.dll"))
                allAssemblies.Add(Assembly.LoadFile(dll));
            foreach (string dll in Directory.GetFiles(path, "*.exe"))
                allAssemblies.Add(Assembly.LoadFile(dll));

            foreach (var assembly in allAssemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsAbstract && (type.IsAssignableFrom(typeof(RabbitController)) || typeof(RabbitController).IsAssignableFrom(type)))
                    {
                        var method = typeof(RegistrationUtility).GetMethods().FirstOrDefault(d => d.Name == "RegisterController" && d.GetParameters().Length == 0);
                        if (method != null)
                        {
                            MethodInfo genericMethod = method.MakeGenericMethod(type);
                            genericMethod.Invoke(null, null);
                        }
                    }
                }
            }
        }
    }
}