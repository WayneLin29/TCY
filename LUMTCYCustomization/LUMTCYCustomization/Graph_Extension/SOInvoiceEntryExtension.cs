using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.PO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.CM.Extensions;

namespace PX.Objects.SO
{
    public class SOInvoiceEntryExtension : PXGraphExtension<SOInvoiceEntry>
    {
        [PXCopyPasteHiddenView]
        public SelectFrom<SOOrderShipment>
               .InnerJoin<SOOrder>.On<SOOrderShipment.orderType.IsEqual<SOOrder.orderType>
                                 .And<SOOrderShipment.orderNbr.IsEqual<SOOrder.orderNbr>>>
               .OrderBy<Asc<SOOrderShipment.orderNbr>, Asc<SOOrderShipment.shipmentNbr>, Asc<SOOrderShipment.shipmentType>>.View shipmentlist;

        [PXOverride]
		public virtual IEnumerable sHipmentlist()
		{
			PXSelectBase<ARTran> cmd = new PXSelect<ARTran,
				Where<ARTran.sOShipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>,
				And<ARTran.sOShipmentType, Equal<Current<SOOrderShipment.shipmentType>>,
				And<ARTran.sOOrderType, Equal<Current<SOOrderShipment.orderType>>,
				And<ARTran.sOOrderNbr, Equal<Current<SOOrderShipment.orderNbr>>,
				And<ARTran.sOOrderLineNbr, IsNotNull,
				And<ARTran.canceled, NotEqual<True>,
				And<ARTran.isCancellation, NotEqual<True>>>>>>>>>(Base);

			var list = new InvoiceList(Base);
			CurrencyInfo info = Base.currencyinfo.Select();
			list.Add(Base.Document.Current, Base.SODocument.Select(), info);

			bool newInvoice = (Base.Transactions.SelectSingle() == null);
			bool curyRateNotDefined = (info.CuryRate ?? 0m) == 0m;
			var invoiceSearchValues = new List<FieldLookup>();

			HashSet<SOOrderShipment> selectedShipments = new HashSet<SOOrderShipment>(shipmentlist.Cache.GetComparer());

			foreach (SOOrderShipment shipment in shipmentlist.Cache.Updated)
			{
				selectedShipments.Add(shipment);
			}

			foreach (PXResult<SOOrderShipment, SOOrder, SOOrderType> order in
				PXSelectJoin<SOOrderShipment,
					InnerJoin<SOOrder, On<SOOrderShipment.FK.Order>,
					InnerJoin<SOOrderType, On<SOOrderShipment.FK.OrderType>,
					InnerJoin<SOShipment, On<SOShipment.shipmentNbr, Equal<SOOrderShipment.shipmentNbr>,
						And<SOShipment.shipmentType, Equal<SOOrderShipment.shipmentType>>>>>>,
					Where<SOOrderShipment.customerID, Equal<Current<ARInvoice.customerID>>,
						And<SOOrderShipment.hold, Equal<boolFalse>,
						And<SOOrderShipment.confirmed, Equal<boolTrue>,
						And<SOOrderType.aRDocType, Equal<Current<ARInvoice.docType>>,
						And<Where<SOOrderShipment.invoiceNbr, IsNull,
							Or<SOOrderShipment.invoiceNbr, Equal<Current<ARInvoice.refNbr>>>>>>>>>>
				.Select(Base)
				.Concat(
				PXSelectJoin<SOOrderShipment,
					InnerJoin<SOOrder, On<SOOrderShipment.FK.Order>,
					InnerJoin<SOOrderType, On<SOOrderShipment.FK.OrderType>,
					InnerJoin<POReceipt, On<POReceipt.receiptNbr, Equal<SOOrderShipment.shipmentNbr>,
						And<POReceipt.receiptType, Equal<POReceiptType.poreceipt>>>>>>,
					Where<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>,
						And<SOOrderShipment.customerID, Equal<Current<ARInvoice.customerID>>,
						And<SOOrderType.aRDocType, Equal<Current<ARInvoice.docType>>,
						And<Where<SOOrderShipment.invoiceNbr, IsNull,
							Or<SOOrderShipment.invoiceNbr, Equal<Current<ARInvoice.refNbr>>>>>>>>>
				.Select(Base)))
			{
				SOOrderShipment soOrderShipment = order;
				if (cmd.View.SelectSingleBound(new object[] { soOrderShipment }) != null)
					continue;

				SOOrder soOrder = order;
				SOOrderType soOrderType = order;
				bool copyCuryInfoFromSO = (curyRateNotDefined || newInvoice && soOrderType.UseCuryRateFromSO == true);

				if (!newInvoice)
				{
					invoiceSearchValues.Add(new FieldLookup<ARInvoice.customerID>(soOrder.CustomerID));
					invoiceSearchValues.Add(new FieldLookup<SOInvoice.billAddressID>(soOrder.BillAddressID));
					invoiceSearchValues.Add(new FieldLookup<SOInvoice.billContactID>(soOrder.BillContactID));
					invoiceSearchValues.Add(new FieldLookup<ARInvoice.termsID>(soOrder.TermsID));
					invoiceSearchValues.Add(new FieldLookup<ARInvoice.hidden>(false));
				}
				if (!copyCuryInfoFromSO)
				{
					invoiceSearchValues.Add(new FieldLookup<ARInvoice.curyID>(soOrder.CuryID));
				}

				if (list.Find(invoiceSearchValues.ToArray()) != null)
				{
					selectedShipments.Remove(soOrderShipment);
					yield return order;
				}

				invoiceSearchValues.Clear();
			}

			foreach (var notListed in selectedShipments)
			{
				notListed.Selected = false;
			}
		}
	}
}
