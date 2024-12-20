using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using De1.DataB;

namespace De1
{
    public partial class Form1 : Form
    {
        private Model1 dbContext;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dbContext = new Model1();
            LoadSinhVien();
            LoadLopHoc();
            SetButtonState(false);
            CustomizeDataGridViewColumns();
            txtTim.TextChanged += TxtTim_TextChanged;

        }
        private void LoadSinhVien()
        {
            var sinhVienList = dbContext.Sinhviens
        .Join(dbContext.Lops,
              sv => sv.MaLop,
              l => l.MaLop,
              (sv, l) => new
              {
                  sv.MaSV,
                  sv.HoTenSV,
                  sv.NgaySinh,
                  TenLop = l.TenLop
              })
        .ToList();

            dataGridView1.DataSource = sinhVienList;
        }

        private void LoadLopHoc()
        {
            var lopList = dbContext.Lops.Select(l => new
            {
                l.MaLop,
                l.TenLop
            }).ToList();

            cbLop.DataSource = lopList;
            cbLop.DisplayMember = "TenLop";
            cbLop.ValueMember = "MaLop";
        }
        private void CustomizeDataGridViewColumns()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                var selectedRow = dataGridView1.CurrentRow;
                txtMSSV.Text = selectedRow.Cells["MaSV"].Value.ToString();
                txtTen.Text = selectedRow.Cells["HoTenSV"].Value.ToString();
                dateTimePicker1.Value = (DateTime)selectedRow.Cells["NgaySinh"].Value;
                cbLop.SelectedValue = selectedRow.Cells["MaLop"].Value.ToString();
                SetButtonState(false);
            }
        }
        private void SetButtonState(bool isEditing)
        {
            btnLuu.Enabled = isEditing;
            btnKLuu.Enabled = isEditing;
            btnThem.Enabled = !isEditing;
            btnXoa.Enabled = !isEditing;
            btnSua.Enabled = !isEditing;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMSSV.Text))
            {
                txtTen.Enabled = true;
                dateTimePicker1.Enabled = true;
                cbLop.Enabled = true;

                SetButtonState(true);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnKLuu_Click(object sender, EventArgs e)
        {
            dataGridView1_SelectionChanged(sender, e);
            SetButtonState(false);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string maSV = txtMSSV.Text;
            var sinhVien = dbContext.Sinhviens.SingleOrDefault(sv => sv.MaSV == maSV);
            if (sinhVien != null)
            {
                sinhVien.HoTenSV = txtTen.Text;
                sinhVien.NgaySinh = dateTimePicker1.Value;
                sinhVien.MaLop = cbLop.SelectedValue.ToString();
                dbContext.SaveChanges();
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không tìm thấy sinh viên để cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            LoadSinhVien();
            SetButtonState(false);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("chắc chắn muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            var newSinhVien = new Sinhvien
            {
                MaSV = txtMSSV.Text,
                HoTenSV = txtTen.Text,
                NgaySinh = dateTimePicker1.Value,
                MaLop = cbLop.SelectedValue.ToString()
            };

            dbContext.Sinhviens.Add(newSinhVien);
            dbContext.SaveChanges();
            MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadSinhVien();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMSSV.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maSV = txtMSSV.Text;
            var sinhVien = dbContext.Sinhviens.SingleOrDefault(sv => sv.MaSV == maSV);
            if (sinhVien != null)
            {
                var confirmResult = MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    dbContext.Sinhviens.Remove(sinhVien);
                    dbContext.SaveChanges();
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadSinhVien();
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy sinh viên để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtMSSV.Text = row.Cells["MaSV"].Value.ToString();
                txtTen.Text = row.Cells["HoTenSV"].Value.ToString();
                dateTimePicker1.Value = (DateTime)row.Cells["NgaySinh"].Value;

                string tenLop = row.Cells["TenLop"].Value.ToString();
                cbLop.SelectedValue = dbContext.Lops.FirstOrDefault(l => l.TenLop == tenLop)?.MaLop;
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string searchValue = txtTim.Text.Trim();
            if (string.IsNullOrEmpty(searchValue))
            {
                LoadSinhVien(); 
                return;
            }

            var searchResults = dbContext.Sinhviens
                .Where(sv => sv.MaSV.Contains(searchValue) || sv.HoTenSV.Contains(searchValue))
                .Select(sv => new
                {
                    sv.MaSV,
                    sv.HoTenSV,
                    sv.NgaySinh,
                    sv.MaLop
                }).ToList();

            if (searchResults.Any())
            {
                dataGridView1.DataSource = searchResults;
            }
            else
            {
                MessageBox.Show("Không tìm thấy kết quả phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadSinhVien();
            }
        }
        private void TxtTim_TextChanged(object sender, EventArgs e)
        {
                if (string.IsNullOrWhiteSpace(txtTim.Text))
        {
            LoadSinhVien(); 
        }
            }
        }
}
