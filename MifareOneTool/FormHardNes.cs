using MifareOneTool.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MifareOneTool
{
    public partial class FormHardNes : Form
    {
        public FormHardNes()
        {
            InitializeComponent();
            checkBoxColOnly.CheckedChanged += (s, e) => UpdateTargetInputs();
            UpdateTargetInputs();
        }

        static int getBlock(int sector)
        {//可能有bug
            int trailer_block = 0;
            if (sector < 32)
            {
                trailer_block = sector * 4 + 3;
            }
            else
            {
                trailer_block = 128 + 16 * (sector - 32) + 15;
            }
            return trailer_block;
        }

        /// <summary>
        /// 目标扇区输入仅在"只采集不计算"模式下有意义（collect.exe 需要），破解模式隐藏
        /// </summary>
        private void UpdateTargetInputs()
        {
            bool collect = checkBoxColOnly.Checked;
            sector1.Visible = collect;
            label3.Visible = collect;
            label4.Visible = collect;
            radioKey1A.Visible = collect;
            radioKey1B.Visible = collect;
            tableLayoutPanel2.Visible = collect;
        }

        /// <summary>
        /// 生成 mfoc-hardnested 兼容的参数格式
        /// mfoc-hardnested 用法: -k <key> [-C] [-F] [-Z] [-P probnum] [-T tolerance] [-O output]
        /// </summary>
        public string GetArg()
        {
            return "-k " + keyEdit.Text.ToUpper() + " ";
        }

        /// <summary>
        /// 生成 collect.exe 兼容的参数格式
        /// collect.exe 用法: <known key> <for block> <A|B> <target block> <A|B>
        /// </summary>
        public string GetArgForCollect()
        {
            string key = keyEdit.Text.ToUpper();
            int knownBlock = getBlock(Convert.ToInt32(sector1.Text.Trim()));
            string knownType = radioKey1A.Checked ? "A" : "B";
            int targetBlock = getBlock(Convert.ToInt32(sector2.Text.Trim()));
            string targetType = radioKey2A.Checked ? "A" : "B";
            
            return string.Format("{0} {1} {2} {3} {4}", key, knownBlock, knownType, targetBlock, targetType);
        }

        public string GetFileAfter()
        {
            string a = "_";
            a += string.Format("{0:D3}", getBlock(Convert.ToInt32(sector2.Text.Trim())));
            a += radioKey2A.Checked ? "A" : "B";
            a += ".txt";
            return a;
        }

        public bool collectOnly()
        {
            return checkBoxColOnly.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool error = false;
            const string pattern = @"[0-9A-Fa-f]{12}";
            TextBox tb = keyEdit;
            string content = tb.Text.Trim();
            if (!(Regex.IsMatch(content, pattern) && content.Length == 12))
            {
                tb.BackColor = Color.Tomato;
                error = true;
            }
            else
            {
                tb.BackColor = Color.Aquamarine;
                tb.Text = content;
            }
            if (checkBoxColOnly.Checked)
            {
                int sec1, sec2;
                if (!int.TryParse(sector1.Text, out sec1))
                {
                    sector1.BackColor = Color.Tomato;
                    error = true;
                }
                else
                {
                    if (sec1 >= 0)
                    {
                        sector1.BackColor = Color.Aquamarine;
                    }
                    else
                    {
                        sector1.BackColor = Color.Tomato;
                        error = true;
                    }
                }
                if (!int.TryParse(sector2.Text, out sec2))
                {
                    sector2.BackColor = Color.Tomato;
                    error = true;
                }
                else
                {
                    if (sec2 >= 0)
                    {
                        sector2.BackColor = Color.Aquamarine;
                    }
                    else
                    {
                        sector2.BackColor = Color.Tomato;
                        error = true;
                    }
                }
            }
            if (error)
            {
                MessageBox.Show(Resources.设置错误_请修改);
                return;
            }
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
