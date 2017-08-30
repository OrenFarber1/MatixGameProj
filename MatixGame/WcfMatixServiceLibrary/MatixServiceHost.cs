using System;
using System.ServiceModel;


namespace WcfMatixServiceLibrary
{
    /// <summary>
    /// Implement a ServiceHost that receives IMatixBuisnessInterface reference on creation 
    /// </summary>
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
