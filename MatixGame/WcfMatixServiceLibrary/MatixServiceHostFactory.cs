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

        private  readonly IMatixBuisnessInterface matixBuisnessInterface = null;


        public MatixServiceHostFactory(IMatixBuisnessInterface buisnessInterface)
        {
            matixBuisnessInterface = buisnessInterface;
        }

        protected override ServiceHost CreateServiceHost(Type serviceType,  Uri[] baseAddresses)
        {
            return new MatixServiceHost(matixBuisnessInterface, serviceType, baseAddresses);
        }

    }
}
