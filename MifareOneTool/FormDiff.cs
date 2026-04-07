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
    public partial class FormDiff : Form
    {
        public FormDiff()
        {
            InitializeComponent();
        }

        private S50 sa = new S50();
        private S50 sb = new S50();
        private string fa = "";
        private string fb = "";

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = Resources.MFD文件_mfd_dump;
            ofd.Title = Resources.请选择需要打开的MFD文件_比较A;
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fa = ofd.FileName;
            }
            else
            {
                return;
            }
            sa = new S50();
            try
            {
                sa.LoadFromMfd(fa);
                button1.Text = "A=" + ofd.SafeFileName;
            }
            catch (IOException ioe)
            {
                MessageBox.Show(ioe.Message, Resources.打开出错, MessageBoxButtons.OK, MessageBoxIcon.Error);
                sa = new S50();
                return;
            }
        }

        private void FormDiff_Load(object sender, EventArgs e)
        {
            // 初始化 DataGridView 列
            InitDataGridView();
        }

        private void InitDataGridView()
        {
            dataGridViewDiff.Columns.Clear();
            dataGridViewDiff.Columns.Add("Sector", Resources.扇区);
            dataGridViewDiff.Columns.Add("Block", Resources.块);
            dataGridViewDiff.Columns.Add("DataA", "A");
            dataGridViewDiff.Columns.Add("DataB", "B");
            dataGridViewDiff.Columns.Add("Diff", Resources.差异);

            dataGridViewDiff.Columns[0].Width = 50;
            dataGridViewDiff.Columns[1].Width = 50;
            dataGridViewDiff.Columns[2].Width = 320;
            dataGridViewDiff.Columns[3].Width = 320;
            dataGridViewDiff.Columns[4].Width = 60;

            dataGridViewDiff.Columns[0].ReadOnly = true;
            dataGridViewDiff.Columns[1].ReadOnly = true;
            dataGridViewDiff.Columns[2].ReadOnly = true;
            dataGridViewDiff.Columns[3].ReadOnly = true;
            dataGridViewDiff.Columns[4].ReadOnly = true;

            dataGridViewDiff.AllowUserToAddRows = false;
            dataGridViewDiff.AllowUserToDeleteRows = false;
            dataGridViewDiff.RowHeadersVisible = false;
            dataGridViewDiff.AllowUserToResizeRows = false;
        }

        private void logAppend(string msg)
        {
            richTextBox1.AppendText(msg + "\n");
            richTextBox1.ScrollToCaret();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = Resources.MFD文件_mfd_dump;
            ofd.Title = Resources.请选择需要打开的MFD文件_比较B;
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fb = ofd.FileName;
            }
            else
            {
                return;
            }
            sb = new S50();
            try
            {
                sb.LoadFromMfd(fb);
                button2.Text = "B=" + ofd.SafeFileName;
            }
            catch (IOException ioe)
            {
                MessageBox.Show(ioe.Message, Resources.打开出错, MessageBoxButtons.OK, MessageBoxIcon.Error);
                sb = new S50();
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (File.Exists(fa) && File.Exists(fb))
            {
                richTextBox1.Clear();
                CompareVisual();
            }
            else
            {
                logAppend(Resources.AB文件中一个或两个无效);
            }
        }

        private void CompareVisual()
        {
            dataGridViewDiff.Rows.Clear();
            int diffCount = 0;
            int diffBytes = 0;

            for (int i = 0; i < 16; i++)
            {
                for (int a = 0; a < 4; a++)
                {
                    byte[] blockA = sa.Sectors[i].Block[a];
                    byte[] blockB = sb.Sectors[i].Block[a];

                    bool hasDiff = !blockA.SequenceEqual(blockB);
                    if (hasDiff)
                    {
                        diffCount++;
                        for (int b = 0; b < 16; b++)
                        {
                            if (blockA[b] != blockB[b])
                            {
                                diffBytes++;
                            }
                        }
                    }

                    int rowIndex = dataGridViewDiff.Rows.Add();
                    DataGridViewRow row = dataGridViewDiff.Rows[rowIndex];

                    row.Cells[0].Value = i.ToString();
                    row.Cells[1].Value = a.ToString();
                    row.Cells[2].Value = Utils.Hex2StrWithSpan(blockA);
                    row.Cells[3].Value = Utils.Hex2StrWithSpan(blockB);
                    row.Cells[4].Value = hasDiff ? Resources._有差异 : Resources._相同;

                    if (hasDiff)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightPink;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }

            // 更新统计信息
            richTextBox1.Text = string.Format(Resources.对比完成_共_0_个块不同_共_1_个字节不同, diffCount, diffBytes);
        }

        private string Compare()
        {
            StringBuilder stb = new StringBuilder();
            int diffCount = 0;
            for (int i = 0; i < 16; i++)
            {
                stb.AppendLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                stb.AppendLine(Resources.扇区0 + i.ToString());
                for (int a = 0; a < 4; a++)
                {
                    string res = "";
                    for (int b = 0; b < 16; b++)
                    {
                        if (sa.Sectors[i].Block[a][b] == sb.Sectors[i].Block[a][b])
                        {
                            res += "-- ";
                        }
                        else
                        {
                            res += "## ";
                        }
                    }
                    stb.AppendLine("A: " + Utils.Hex2StrWithSpan(sa.Sectors[i].Block[a]));
                    stb.AppendLine("B: " + Utils.Hex2StrWithSpan(sb.Sectors[i].Block[a]));
                    stb.AppendLine("   " + res);
                    if (res.Contains("##"))
                    {
                        diffCount++;
                    }
                }

            }
            return Resources.共找到 + diffCount.ToString() + Resources._个块不同 + stb.ToString();
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewDiff_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 颜色在添加行时已设置，这里保留以防需要更细粒度的控制
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 1 && File.Exists(fa) && File.Exists(fb))
            {
                // 切换到可视化视图时重新加载
                CompareVisual();
            }
        }
    }
}