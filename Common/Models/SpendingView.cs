using System;

namespace Common.Models
{
    /// <summary>
    /// نموذج عرض سندات الصرف
    /// </summary>
    public class SpendingView
    {
        // الحقول الأساسية - الترتيب الجديد
        public long ID { get; set; } // ID
        public long? TheNumber { get; set; } // IDالرقم المرجعي
        public long? الرقم { get; set; } // الرقم
        public DateTime? التاريخ { get; set; } // التاريخ
        public string طريقة_الصرف { get; set; } // طريقة الصرف
        public decimal? المبلغ { get; set; } // المبلغ
        public string العملة { get; set; } // العملة
        public long? AccountID { get; set; } // الصندوق
        public decimal? ExchangeAmount { get; set; } // مبلغ الحساب
        public long? ExchangeCurrencyID { get; set; } // عملة الحساب
        public long? ExchangeAccountID { get; set; } // اسم الحساب
        public string ملاحظات { get; set; } // ملاحظات
        public string RefernceNumber { get; set; } // رقم المرجع
        public string Delivery { get; set; } // مناولة
        public long? CostCenterID { get; set; } // مركز التكلفة
        public long? UserID { get; set; } // المستخدم
        public string المستخدم { get; set; } // المستخدم (اسم)
        public long? BranchID { get; set; } // الفرع
        public DateTime? EnterTime { get; set; } // وقت الإدخال
        public int? Prints { get; set; } // الطبعات
        public string SpecialChequeNumber { get; set; } // رقم الشيك الخاص
        public long? CommissionerID { get; set; } // اسم المفوض
        public bool? IsCommissioner { get; set; } // الصرف للمفوض
        public bool? IsDepend { get; set; } // معتمد
        public long? DependUserID { get; set; } // المعتمد
        public long? ChequeID { get; set; } // الفئات

        // حقول إضافية للعرض
        public string عملة_الحساب { get; set; } // عرض عملة الحساب
        public bool? معتمد { get; set; } // معتمد (للعرض فقط)
    }
}