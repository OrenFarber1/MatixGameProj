using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfMatixServiceLibrary
{
    public class MatixServiceHost : ServiceHost
    {

        public MatixServiceHost(IMatixBuisnessInterface buisnessInterface, Type serviceType, params Uri[] baseAddresses)
        : base(serviceType, baseAddresses)
        {
            if (buisnessInterface == null)
            {
                throw new ArgumentNullException("IMatixBuisnessInterface");
            }

            foreach (var cd in this.ImplementedContracts.Values)
            {
                cd.Behaviors.Add(new MatixInstanceProvider(buisnessInterface));
            }
        }

    }
}
