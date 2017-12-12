using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.BDD;
using Ninject;
using Ninject.Modules;

namespace GeneAutomate.BusinessLogic
{

    public class Binding : NinjectModule
    {
        public override void Load()
        {
            Bind<IBDDSolver>().To<BDDSharpSolver>();
        }
    }
    public static  class NinjectHelper
    {

        private static StandardKernel kernel;

        static NinjectHelper()
        {
            kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
        }

        public static T Get<T>()
        {
            return kernel.Get<T>();
        }
    }
}
