using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PX.Objects.GL
{
    public class JournalEntryExtension : PXGraphExtension<JournalEntry>
    {

        public virtual void _(Events.RowSelected<Batch> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            var row = e.Row;
            if (row != null && !string.IsNullOrEmpty(row.BatchNbr))
            {
                var GUINbr = SelectFrom<ARRegister>
                             .Where<ARRegister.batchNbr.IsEqual<P.AsString>>
                             .View.Select(Base, row?.BatchNbr)
                             .TopFirst.GetExtension<ARRegisterExt>()?.UsrGUINbr;
                var TaxAccountInfo = SelectFrom<Account>
                                    .Where<Account.accountCD.IsEqual<P.AsString>>
                                    .View.Select(Base, "220012").TopFirst;
                var detail = SelectFrom<GLTran>
                            .Where<GLTran.batchNbr.IsEqual<P.AsString>
                              .And<GLTran.accountID.IsEqual<P.AsInt>>>
                            .View.Select(Base, row?.BatchNbr, TaxAccountInfo?.AccountID).TopFirst;
                if (detail != null && detail.TranDesc != GUINbr && !string.IsNullOrEmpty(GUINbr))
                {
                    PXDatabase.Update<GLTran>(
                        new PXDataFieldAssign<GLTran.tranDesc>(GUINbr),
                        new PXDataFieldRestrict<GLTran.batchNbr>(row.BatchNbr),
                        new PXDataFieldRestrict<GLTran.lineNbr>(detail.LineNbr));
                }
            }
        }
    }
}
