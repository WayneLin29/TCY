using PX.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.SO
{
    public class SOShipmentEntryExtension : PXGraphExtension<SOShipmentEntry>
    {
        public delegate bool SetShipmentFieldsFromOrderDelegate(SOOrder order, SOShipment shipment,
             int? siteID, DateTime? shipDate, string operation, SOOrderTypeOperation orderOperation,
             bool newlyCreated);
        [PXOverride]
        public virtual bool SetShipmentFieldsFromOrder(SOOrder order, SOShipment shipment,
            int? siteID, DateTime? shipDate, string operation, SOOrderTypeOperation orderOperation,
            bool newlyCreated, SetShipmentFieldsFromOrderDelegate baseMethod)
        {
            if (newlyCreated)
                shipment.OwnerID = order.OwnerID;
            return baseMethod(order, shipment, siteID, shipDate, operation, orderOperation, newlyCreated);
        }
    }
}
