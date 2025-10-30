using System;

namespace Common.Models
{
    /// <summary>
    /// نموذج بيانات سند الصرف
    /// </summary>
    public class Spending
    {
        // الحقول الأساسية
        public long ID { get; set; }
        public long? TheNumber { get; set; }
        public DateTime? TheDate { get; set; }
        public long? TheMethod { get; set; } // 1=نقدي، 2=شيك، 3=تحويل بنكي
        public decimal? Amount { get; set; }
        public long? CurrencyID { get; set; } // 1=دينار، 2=دولار، 3=يورو
        public long? AccountID { get; set; }
        
        // الحقول الجديدة المطلوبة
        public decimal? ExchangeAmount { get; set; } // مبلغ الحساب
        public long? ExchangeCurrencyID { get; set; } // عملة الحساب
        public long? ExchangeAccountID { get; set; } // اسم الحساب
        public string Notes { get; set; }
        public string RefernceNumber { get; set; } // رقم المرجع
        public string Delivery { get; set; } // مناولة
        public long? CostCenterID { get; set; } // مركز التكلفة
        public long? UserID { get; set; }
        public long? BranchID { get; set; } // الفرع
        public DateTime? EnterTime { get; set; }
        public int? Prints { get; set; } // الطبعات
        public string SpecialChequeNumber { get; set; } // رقم الشيك الخاص
        public long? CommissionerID { get; set; } // اسم المفوض
        public bool? IsCommissioner { get; set; } // الصرف للمفوض
        public bool? IsDepend { get; set; } // معتمد
        public long? DependUserID { get; set; } // المعتمد
        public long? ChequeID { get; set; } // الفئات
    }
}