using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.SO
{
    public abstract class SOCreatShipment_ProtectedExt : PXGraphExtension<SOCreateShipment>
    {
        [PXProtectedAccess(typeof(SOCreateShipment))]
        public abstract PXSelectBase<SOOrder> BuildCommandDefault();
    }

    public class SOCreatShipmentExtension : PXGraphExtension<SOCreatShipment_ProtectedExt, SOCreateShipment>
    {
        public delegate PXSelectBase<SOOrder> GetSelectCommandDelegate(SOOrderFilter filter);
        [PXOverride]
        public virtual PXSelectBase<SOOrder> GetSelectCommand(SOOrderFilter filter, GetSelectCommandDelegate baseMethod)
        {
            PXSelectBase<SOOrder> cmd;
            switch (filter.Action)
            {
                case "SO301000$createShipmentReceipt":
                    cmd = Base1.BuildCommandDefault();
                    cmd.Join<LeftJoin<SOOrderShipment, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>,
                                  And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>>>();
                    cmd.WhereAnd<Where<SOOrder.orderType,Equal<ROTypeAttr>,
                        Or<SOOrder.orderType,Equal<RNTypeAttr>>>>();
                    cmd.WhereAnd<Where<SOOrderShipment.orderNbr.IsNull>>();
                    break;
                default:
                    cmd = baseMethod(filter);
                    break;
            }
            return cmd;
        }
    }

    public class ROTypeAttr : PX.Data.BQL.BqlString.Constant<ROTypeAttr>
    {
        public ROTypeAttr() : base("RO") { }
    }
    public class RNTypeAttr : PX.Data.BQL.BqlString.Constant<RNTypeAttr>
    {
        public RNTypeAttr() : base("RN") { }
    }
}
