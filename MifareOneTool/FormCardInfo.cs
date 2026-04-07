using MifareOneTool.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MifareOneTool
{
    public partial class FormCardInfo : Form
    {
        private S50 card = new S50();
        private string filename = "";

        public FormCardInfo()
        {
            InitializeComponent();
        }

        public void LoadCard(string file)
        {
            if (!File.Exists(file))
            {
                MessageBox.Show(Resources.加载的文件不存在, Resources.打开出错, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                card = new S50();
                card.LoadFromMfd(file);
                filename = file;
                this.Text = Resources.卡片信息预览 + " - " + Path.GetFileName(file);
                RefreshInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.打开出错, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadCard(S50 s50)
        {
            card = s50;
            this.Text = Resources.卡片信息预览;
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            // 显示 UID 和 BCC
            byte[] block0 = card.Sectors[0].Block[0];
            string uid = string.Format("{0:X2} {1:X2} {2:X2} {3:X2}", block0[0], block0[1], block0[2], block0[3]);
            byte bcc = block0[4];
            byte calcBcc = (byte)(block0[0] ^ block0[1] ^ block0[2] ^ block0[3]);

            labelUID.Text = Resources.UID + ": " + uid;
            labelBCC.Text = Resources.BCC + ": " + bcc.ToString("X2");
            labelBCCStatus.Text = bcc == calcBcc ? "✓ " + Resources.正常 : "✗ " + Resources.异常;
            labelBCCStatus.ForeColor = bcc == calcBcc ? Color.Green : Color.Red;

            // 刷新扇区列表
            RefreshSectorList();
        }

        private void RefreshSectorList()
        {
            dataGridViewSectors.Rows.Clear();

            for (int i = 0; i < 16; i++)
            {
                Sector sec = card.Sectors[i];
                int verify = sec.Verify();

                // 解析访问控制位
                byte[] acbits = Utils.ReadAC(sec.Block[3].Skip(6).Take(4).ToArray());
                string acInfo = FormatAccessBits(acbits, i);

                string keyA = Utils.Hex2Str(sec.KeyA);
                string keyB = Utils.Hex2Str(sec.KeyB);

                int rowIndex = dataGridViewSectors.Rows.Add();
                DataGridViewRow row = dataGridViewSectors.Rows[rowIndex];

                row.Cells[0].Value = i.ToString();
                row.Cells[1].Value = keyA;
                row.Cells[2].Value = keyB;
                row.Cells[3].Value = acInfo;
                row.Cells[4].Value = verify == 0 ? Resources.正常 : Resources.异常;

                // 根据状态设置颜色
                if (verify != 0)
                {
                    row.Cells[4].Style.ForeColor = Color.Red;
                }
                else
                {
                    row.Cells[4].Style.ForeColor = Color.Green;
                }

                // 检查是否为空扇区
                bool isEmpty = IsSectorEmpty(sec, i == 0);
                if (isEmpty)
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }

        private bool IsSectorEmpty(Sector sec, bool isSector0)
        {
            for (int b = 0; b < 3; b++)
            {
                if (isSector0 && b == 0) continue; // 跳过块0
                for (int i = 0; i < 16; i++)
                {
                    if (sec.Block[b][i] != 0x00)
                        return false;
                }
            }
            return true;
        }

        private string FormatAccessBits(byte[] acbits, int sector)
        {
            StringBuilder sb = new StringBuilder();

            // acbits[0-2] 是数据块0-2的访问控制
            // acbits[3] 是扇区尾的访问控制
            for (int i = 0; i < 4; i++)
            {
                if (i > 0) sb.Append(" ");
                sb.Append(DecodeAccessValue(acbits[i], i == 3));
            }

            return sb.ToString();
        }

        private string DecodeAccessValue(byte value, bool isSectorTrailer)
        {
            if (isSectorTrailer)
            {
                // 扇区尾访问控制
                switch (value)
                {
                    case 0: return "KAB";
                    case 1: return "KAB*";
                    case 2: return "K-A";
                    case 3: return "K-B";
                    case 4: return "KAB";
                    case 5: return "R/W";
                    case 6: return "R/W";
                    case 7: return "N/A";
                    default: return "???";
                }
            }
            else
            {
                // 数据块访问控制
                switch (value)
                {
                    case 0: return "AB ";
                    case 1: return "AB*";
                    case 2: return "A  ";
                    case 3: return "B  ";
                    case 4: return "AB ";
                    case 5: return "B  ";
                    case 6: return "AB ";
                    case 7: return "---";
                    default: return "???";
                }
            }
        }

        private void FormCardInfo_Load(object sender, EventArgs e)
        {
            InitDataGridView();
        }

        private void InitDataGridView()
        {
            dataGridViewSectors.Columns.Clear();
            dataGridViewSectors.Columns.Add("Sector", Resources.扇区);
            dataGridViewSectors.Columns.Add("KeyA", Resources.密钥A);
            dataGridViewSectors.Columns.Add("KeyB", Resources.密钥B);
            dataGridViewSectors.Columns.Add("AC", Resources.访问控制);
            dataGridViewSectors.Columns.Add("Status", Resources.状态);

            dataGridViewSectors.Columns[0].Width = 50;
            dataGridViewSectors.Columns[1].Width = 120;
            dataGridViewSectors.Columns[2].Width = 120;
            dataGridViewSectors.Columns[3].Width = 100;
            dataGridViewSectors.Columns[4].Width = 60;

            dataGridViewSectors.AllowUserToAddRows = false;
            dataGridViewSectors.AllowUserToDeleteRows = false;
            dataGridViewSectors.RowHeadersVisible = false;
            dataGridViewSectors.AllowUserToResizeRows = false;
        }

        private void dataGridViewSectors_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < 16)
            {
                ShowBlockDetail(e.RowIndex);
            }
        }

        private void ShowBlockDetail(int sectorIndex)
        {
            Sector sec = card.Sectors[sectorIndex];
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format(Resources.扇区0, sectorIndex));
            sb.AppendLine(new string('-', 40));

            for (int i = 0; i < 4; i++)
            {
                sb.AppendLine(string.Format("Block {0}: {1}", i, Utils.Hex2StrWithSpan(sec.Block[i])));
            }

            sb.AppendLine();
            sb.AppendLine(Resources.密钥A + ": " + Utils.Hex2Str(sec.KeyA));
            sb.AppendLine(Resources.密钥B + ": " + Utils.Hex2Str(sec.KeyB));
            sb.AppendLine(Resources.访问控制 + ": " + Utils.Hex2Str(sec.ACBits));

            richTextBoxDetail.Text = sb.ToString();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
