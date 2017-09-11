using System;
using System.ServiceModel;


namespace WcfMatixServiceLibrary
{
    /// <summary>
    /// Implement a ServiceHost that receives IMatixBuisnessInterface reference on creation 
    /// </summary>
    public class MatixServiceHost : ServiceHost
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="buisnessInterface">Instanse of the buisness layer</param>
        /// <param name="serviceType"></param>
        /// <param name="baseAddresses"></param>
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
