using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Common.Models;
using System.Linq;
using DatabaseManager;
using Services;
using Utilities;
using Common.Enums;

namespace FormsLibrary
{
    public partial class PaymentVouchersForm : Form
    {
        private DataGridView dataGridViewSpending;
        private Panel containerPanel;
        private Label scrollStatusLabel;
        private Button btnNew, btnSave, btnRefresh, btnSearch, btnClose, btnDiagnostics, btnLoadByDate;
        private DateTimePicker datePickerFilter;
        private SpendingService _spendingService;
        private List<SpendingView> _spendingList;
        private DatabaseService _databaseService;
        private User _currentUser;

        public PaymentVouchersForm(DatabaseService databaseService, User currentUser = null)
        {
            _databaseService = databaseService;
            _currentUser = currentUser;
            _spendingService = new SpendingService(databaseService);
            InitializeComponent();
            SetupForm();
            AddControls();
            InitializeEmptyDataGridView();
        }

        private void SetupForm()
        {
            this.Text = "إدارة سندات الصرف";
            this.Size = new Size(1200, 700);
            this.MinimumSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.FromArgb(240, 245, 249);
            this.Font = new Font("Segoe UI", 9);
            this.AutoSize = false;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Resize += PaymentVouchersForm_Resize;
        }

        private void AddControls()
        {
            AddToolbar();
            AddDataGridViewWithScrollBars();
        }

        private void PaymentVouchersForm_Resize(object sender, EventArgs e)
        {
            if (containerPanel != null && dataGridViewSpending != null)
            {
                containerPanel.Location = new Point(10, 60);
                containerPanel.Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 100);

                int gridWidth = containerPanel.ClientSize.Width - 20;
                int gridHeight = containerPanel.ClientSize.Height - 50;

                dataGridViewSpending.Size = new Size(gridWidth, gridHeight);
                dataGridViewSpending.Location = new Point(10, 40);

                if (scrollStatusLabel != null)
                {
                    scrollStatusLabel.Location = new Point(10, this.ClientSize.Height - 30);
                    scrollStatusLabel.Width = this.ClientSize.Width - 20;
                }
            }
        }

        private void AddToolbar()
        {
            var toolbarPanel = new Panel();
            toolbarPanel.BackColor = Color.FromArgb(52, 152, 219);
            toolbarPanel.BorderStyle = BorderStyle.FixedSingle;
            toolbarPanel.Location = new Point(0, 0);
            toolbarPanel.Size = new Size(this.ClientSize.Width, 50);
            toolbarPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // ... (كود إضافة الأزرار والتاريخ كما في الإصدار السابق)

            this.Controls.Add(toolbarPanel);
        }

        private void AddDataGridViewWithScrollBars()
        {
            containerPanel = new Panel();
            containerPanel.Location = new Point(10, 60);
            containerPanel.Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 100);
            containerPanel.AutoScroll = true;
            containerPanel.BorderStyle = BorderStyle.FixedSingle;
            containerPanel.BackColor = Color.White;
            containerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            dataGridViewSpending = new DataGridView();
            dataGridViewSpending.Location = new Point(10, 10);
            dataGridViewSpending.Size = new Size(containerPanel.Width - 25, containerPanel.Height - 20);
            dataGridViewSpending.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            SetupModernDataGridView();

            containerPanel.Controls.Add(dataGridViewSpending);
            this.Controls.Add(containerPanel);

            AddScrollStatusLabel();
        }

        private void SetupModernDataGridView()
        {
            // ... (إعدادات DataGridView كما في الإصدار السابق)
        }

        private void AddScrollStatusLabel()
        {
            scrollStatusLabel = new Label();
            scrollStatusLabel.Location = new Point(10, this.ClientSize.Height - 30);
            scrollStatusLabel.Size = new Size(this.ClientSize.Width - 20, 25);
            scrollStatusLabel.Text = "لم يتم تحميل أي بيانات - اضغط على زر التحديث أو اختر تاريخاً";
            scrollStatusLabel.TextAlign = ContentAlignment.MiddleRight;
            scrollStatusLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            scrollStatusLabel.BackColor = Color.FromArgb(241, 196, 15);
            scrollStatusLabel.ForeColor = Color.FromArgb(52, 73, 94);
            scrollStatusLabel.BorderStyle = BorderStyle.FixedSingle;
            scrollStatusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.Controls.Add(scrollStatusLabel);
        }

        private void InitializeEmptyDataGridView()
        {
            dataGridViewSpending.Rows.Clear();
            UpdateStatusBar(0);
            scrollStatusLabel.Text = "لم يتم تحميل أي بيانات - اضغط على زر التحديث أو اختر تاريخاً";
        }

        private void LoadDataForToday()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                this.Text = "إدارة سندات الصرف - جاري تحميل بيانات اليوم...";
                scrollStatusLabel.Text = "جاري تحميل بيانات اليوم...";

                DateTime today = DateTime.Today;
                _spendingList = _spendingService.GetSpendingByDate(today);
                RefreshDataGridView(_spendingList);
                UpdateStatusBar(_spendingList.Count);

                this.Text = $"إدارة سندات الصرف - بيانات اليوم ({_spendingList.Count} سجل)";
                scrollStatusLabel.Text = $"تم تحميل {_spendingList.Count} سجل لليوم {today:yyyy/MM/dd}";
            }
            catch (Exception ex)
            {
                Logger.LogError($"خطأ في تحميل بيانات اليوم: {ex.Message}", ex);
                MessageBox.Show($"خطأ في تحميل بيانات اليوم: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Text = "إدارة سندات الصرف - خطأ في التحميل";
                scrollStatusLabel.Text = "خطأ في تحميل البيانات";
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void RefreshDataGridView(List<SpendingView> spendingList)
        {
            try
            {
                dataGridViewSpending.Rows.Clear();

                foreach (var spending in spendingList)
                {
                    string معتمدValue = "لا";
                    if (spending.معتمد.HasValue)
                    {
                        معتمدValue = spending.معتمد.Value ? "نعم" : "لا";
                    }

                    dataGridViewSpending.Rows.Add(
                        spending.ID,
                        spending.الرقم,
                        spending.التاريخ?.ToString("yyyy/MM/dd"),
                        spending.طريقة_الصرف,
                        spending.المبلغ,
                        spending.العملة,
                        spending.الصندوق,
                        spending.ملاحظات,
                        spending.المستخدم,
                        spending.الفرع,
                        معتمدValue
                    );
                }

                UpdateScrollStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحديث البيانات: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateScrollStatus()
        {
            if (scrollStatusLabel != null && dataGridViewSpending != null)
            {
                string status = $"السجلات: {dataGridViewSpending.Rows.Count}";

                if (dataGridViewSpending.Rows.Count > 0)
                {
                    int firstVisible = dataGridViewSpending.FirstDisplayedScrollingRowIndex;
                    int lastVisible = firstVisible + dataGridViewSpending.DisplayedRowCount(true) - 1;

                    status += $" | عرض من {firstVisible + 1} إلى {lastVisible + 1}";
                }

                scrollStatusLabel.Text = status;
            }
        }

        private void UpdateStatusBar(int recordCount)
        {
            if (scrollStatusLabel != null)
            {
                scrollStatusLabel.Text = $"تم تحميل {recordCount} سجل بنجاح";
            }
        }

        // ... باقي الدوال والأحداث كما في الإصدار السابق
    }
}