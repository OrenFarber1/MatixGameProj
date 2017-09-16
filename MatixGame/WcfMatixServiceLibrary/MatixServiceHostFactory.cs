using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Activation;
using System.ServiceModel;

namespace WcfMatixServiceLibrary
{
    public class MatixServiceHostFactory : ServiceHostFactory
    {
        /// <summary>
        /// Reference to the MatixBuisnessInterface
        /// </summary>
        private readonly IMatixBusinessInterface matixBuisnessInterface = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="buisnessInterface"></param>
        public MatixServiceHostFactory(IMatixBusinessInterface buisnessInterface)
        {
            matixBuisnessInterface = buisnessInterface;
        }

        /// <summary>
        /// Override method to create a MatixServiceHost instance
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="baseAddresses"></param>
        /// <returns>A new ServiceHost instance</returns>
        protected override ServiceHost CreateServiceHost(Type serviceType,  Uri[] baseAddresses)
        {
            return new MatixServiceHost(matixBuisnessInterface, serviceType, baseAddresses);
        }

    }
}
