using Jerrod.RabbitCommon.Framework;
using Ninject;
using Ninject.Modules;
using Rabbit.Framework.Server.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Framework.Server
{
    public class NinjectConfig : NinjectModule
    {
        private static IKernel _staticContainer;
        public static IKernel StaticContainer
        {
            get
            {
                if (_staticContainer == null)
                    _staticContainer = BuildContainer();
                return _staticContainer;
            }
        }

        public override void Load()
        {
            Bind<IDataRepository>().To<FileSystemDataRepository>()
                .WithConstructorArgument<string>(ConfigurationManager.AppSettings["dataFileLocation"]);

            Bind<ILoggingRepository>().To<FileSystemLoggingRepository>()
                .WithConstructorArgument<string>(ConfigurationManager.AppSettings["loggingFileLocation"]);
        }

        public static IKernel BuildContainer()
        {
            return new StandardKernel(new NinjectConfig());
        }

        public static object RegisterContainer(Type typeToResolve, Type controllerType)
        {
            return StaticContainer.GetService(typeToResolve);
        }
    }
}
