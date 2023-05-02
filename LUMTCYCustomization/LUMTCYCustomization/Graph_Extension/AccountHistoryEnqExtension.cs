using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.GL
{
    public class AccountHistoryEnqExtension : PXGraphExtension<AccountHistoryEnq>
    {
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Curr. Beg. Balance")]
        public virtual void _(Events.CacheAttached<GLHistoryEnquiryResult.signCuryBegBalance> e) { }

        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Curr. Debit Total")]
        public virtual void _(Events.CacheAttached<GLHistoryEnquiryResult.curyPtdDebitTotal> e) { }

        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Curr. Credit Total")]
        public virtual void _(Events.CacheAttached<GLHistoryEnquiryResult.curyPtdCreditTotal> e) { }

        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Curr. Ending Balance")]
        public virtual void _(Events.CacheAttached<GLHistoryEnquiryResult.signCuryEndBalance> e) { }
    }
}
