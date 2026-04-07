using MifareOneTool.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MifareOneTool
{
    /// <summary>
    /// 主题管理类，用于处理暗色/亮色主题切换
    /// </summary>
    public static class ThemeManager
    {
        // 暗色主题颜色
        public static readonly Color DarkBackColor = Color.FromArgb(45, 45, 48);
        public static readonly Color DarkForeColor = Color.FromArgb(220, 220, 220);
        public static readonly Color DarkControlBackColor = Color.FromArgb(30, 30, 30);
        public static readonly Color DarkButtonBackColor = Color.FromArgb(62, 62, 64);
        public static readonly Color DarkGroupBoxBackColor = Color.FromArgb(45, 45, 48);
        public static readonly Color DarkTabBackColor = Color.FromArgb(45, 45, 48);
        public static readonly Color DarkTextBoxBackColor = Color.FromArgb(30, 30, 30);
        public static readonly Color DarkGridBackColor = Color.FromArgb(30, 30, 30);
        public static readonly Color DarkGridAlternatingBackColor = Color.FromArgb(40, 40, 40);
        public static readonly Color DarkMenuBackColor = Color.FromArgb(45, 45, 48);
        public static readonly Color DarkStatusBackColor = Color.FromArgb(45, 45, 48);

        // 亮色主题颜色
        public static readonly Color LightBackColor = SystemColors.Control;
        public static readonly Color LightForeColor = SystemColors.ControlText;
        public static readonly Color LightControlBackColor = SystemColors.Window;
        public static readonly Color LightButtonBackColor = SystemColors.Control;
        public static readonly Color LightGroupBoxBackColor = SystemColors.Control;
        public static readonly Color LightTabBackColor = SystemColors.Control;
        public static readonly Color LightTextBoxBackColor = SystemColors.Window;
        public static readonly Color LightGridBackColor = SystemColors.Window;
        public static readonly Color LightGridAlternatingBackColor = SystemColors.ControlLight;
        public static readonly Color LightMenuBackColor = SystemColors.Control;
        public static readonly Color LightStatusBackColor = SystemColors.Control;

        private static bool _isDarkTheme = false;

        public static bool IsDarkTheme
        {
            get { return _isDarkTheme; }
            set
            {
                _isDarkTheme = value;
                Properties.Settings.Default.DarkTheme = value;
                Properties.Settings.Default.Save();
            }
        }

        public static void Initialize()
        {
            _isDarkTheme = Properties.Settings.Default.DarkTheme;
        }

        public static void ApplyTheme(Form form)
        {
            if (_isDarkTheme)
            {
                ApplyDarkTheme(form);
            }
            else
            {
                ApplyLightTheme(form);
            }
        }

        private static void ApplyDarkTheme(Control control)
        {
            if (control is Form form)
            {
                form.BackColor = DarkBackColor;
                form.ForeColor = DarkForeColor;
            }
            else if (control is Button btn)
            {
                // 跳过特殊样式的按钮（如 DodgerBlue 背景的按钮）
                if (btn.BackColor == Color.DodgerBlue || btn.BackColor == Color.FromArgb(0, 122, 204))
                {
                    // 保持原样，只改前景色
                    if (btn.ForeColor != Color.White)
                    {
                        btn.ForeColor = Color.White;
                    }
                }
                else
                {
                    btn.BackColor = DarkButtonBackColor;
                    btn.ForeColor = DarkForeColor;
                }
            }
            else if (control is TextBox txt)
            {
                txt.BackColor = DarkTextBoxBackColor;
                txt.ForeColor = DarkForeColor;
            }
            else if (control is RichTextBox rtxt)
            {
                // 保持 CLI 区域的现有颜色设置
                if (rtxt.BackColor != Color.Black)
                {
                    rtxt.BackColor = DarkTextBoxBackColor;
                    rtxt.ForeColor = DarkForeColor;
                }
            }
            else if (control is ComboBox cmb)
            {
                cmb.BackColor = DarkControlBackColor;
                cmb.ForeColor = DarkForeColor;
            }
            else if (control is ListBox lb)
            {
                lb.BackColor = DarkControlBackColor;
                lb.ForeColor = DarkForeColor;
            }
            else if (control is CheckBox chk)
            {
                chk.BackColor = DarkBackColor;
                chk.ForeColor = DarkForeColor;
            }
            else if (control is RadioButton rb)
            {
                rb.BackColor = DarkBackColor;
                rb.ForeColor = DarkForeColor;
            }
            else if (control is GroupBox gb)
            {
                gb.BackColor = DarkGroupBoxBackColor;
                gb.ForeColor = DarkForeColor;
            }
            else if (control is TabControl tc)
            {
                tc.BackColor = DarkTabBackColor;
                tc.ForeColor = DarkForeColor;
            }
            else if (control is TabPage tp)
            {
                tp.BackColor = DarkTabBackColor;
                tp.ForeColor = DarkForeColor;
            }
            else if (control is DataGridView dgv)
            {
                dgv.BackgroundColor = DarkGridBackColor;
                dgv.BackColor = DarkGridBackColor;
                dgv.ForeColor = DarkForeColor;
                dgv.DefaultCellStyle.BackColor = DarkGridBackColor;
                dgv.DefaultCellStyle.ForeColor = DarkForeColor;
                dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(62, 62, 64);
                dgv.DefaultCellStyle.SelectionForeColor = Color.White;
                dgv.AlternatingRowsDefaultCellStyle.BackColor = DarkGridAlternatingBackColor;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = DarkControlBackColor;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = DarkForeColor;
                dgv.RowHeadersDefaultCellStyle.BackColor = DarkControlBackColor;
                dgv.RowHeadersDefaultCellStyle.ForeColor = DarkForeColor;
                dgv.GridColor = Color.FromArgb(60, 60, 60);
            }
            else if (control is MenuStrip menu)
            {
                menu.BackColor = DarkMenuBackColor;
                menu.ForeColor = DarkForeColor;
                foreach (ToolStripMenuItem item in menu.Items)
                {
                    ApplyDarkThemeToMenuItem(item);
                }
            }
            else if (control is ToolStrip toolStrip)
            {
                toolStrip.BackColor = DarkStatusBackColor;
                toolStrip.ForeColor = DarkForeColor;
            }
            else if (control is StatusStrip status)
            {
                status.BackColor = DarkStatusBackColor;
                status.ForeColor = DarkForeColor;
            }
            else if (control is Label lbl)
            {
                lbl.BackColor = Color.Transparent;
                lbl.ForeColor = DarkForeColor;
            }
            else if (control is Panel panel)
            {
                panel.BackColor = DarkBackColor;
                panel.ForeColor = DarkForeColor;
            }
            else if (control is FlowLayoutPanel flp)
            {
                flp.BackColor = DarkBackColor;
                flp.ForeColor = DarkForeColor;
            }
            else if (control is TableLayoutPanel tlp)
            {
                tlp.BackColor = DarkBackColor;
                tlp.ForeColor = DarkForeColor;
            }
            else if (control is PictureBox pb)
            {
                pb.BackColor = DarkBackColor;
            }
            else
            {
                control.BackColor = DarkBackColor;
                control.ForeColor = DarkForeColor;
            }

            // 递归处理子控件
            foreach (Control child in control.Controls)
            {
                ApplyDarkTheme(child);
            }
        }

        private static void ApplyDarkThemeToMenuItem(ToolStripMenuItem item)
        {
            item.BackColor = DarkMenuBackColor;
            item.ForeColor = DarkForeColor;
            foreach (ToolStripItem subItem in item.DropDownItems)
            {
                if (subItem is ToolStripMenuItem subMenuItem)
                {
                    ApplyDarkThemeToMenuItem(subMenuItem);
                }
                else
                {
                    subItem.BackColor = DarkMenuBackColor;
                    subItem.ForeColor = DarkForeColor;
                }
            }
        }

        private static void ApplyLightTheme(Control control)
        {
            if (control is Form form)
            {
                form.BackColor = LightBackColor;
                form.ForeColor = LightForeColor;
            }
            else if (control is Button btn)
            {
                if (btn.BackColor == Color.DodgerBlue || btn.BackColor == Color.FromArgb(0, 122, 204))
                {
                    btn.ForeColor = Color.White;
                }
                else
                {
                    btn.BackColor = LightButtonBackColor;
                    btn.ForeColor = LightForeColor;
                }
            }
            else if (control is TextBox txt)
            {
                txt.BackColor = LightTextBoxBackColor;
                txt.ForeColor = LightForeColor;
            }
            else if (control is RichTextBox rtxt)
            {
                if (rtxt.BackColor != Color.Black)
                {
                    rtxt.BackColor = LightTextBoxBackColor;
                    rtxt.ForeColor = LightForeColor;
                }
            }
            else if (control is ComboBox cmb)
            {
                cmb.BackColor = LightControlBackColor;
                cmb.ForeColor = LightForeColor;
            }
            else if (control is ListBox lb)
            {
                lb.BackColor = LightControlBackColor;
                lb.ForeColor = LightForeColor;
            }
            else if (control is CheckBox chk)
            {
                chk.BackColor = LightBackColor;
                chk.ForeColor = LightForeColor;
            }
            else if (control is RadioButton rb)
            {
                rb.BackColor = LightBackColor;
                rb.ForeColor = LightForeColor;
            }
            else if (control is GroupBox gb)
            {
                gb.BackColor = LightGroupBoxBackColor;
                gb.ForeColor = LightForeColor;
            }
            else if (control is TabControl tc)
            {
                tc.BackColor = LightTabBackColor;
                tc.ForeColor = LightForeColor;
            }
            else if (control is TabPage tp)
            {
                tp.BackColor = LightTabBackColor;
                tp.ForeColor = LightForeColor;
            }
            else if (control is DataGridView dgv)
            {
                dgv.BackgroundColor = LightGridBackColor;
                dgv.BackColor = LightGridBackColor;
                dgv.ForeColor = LightForeColor;
                dgv.DefaultCellStyle.BackColor = LightGridBackColor;
                dgv.DefaultCellStyle.ForeColor = LightForeColor;
                dgv.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                dgv.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                dgv.AlternatingRowsDefaultCellStyle.BackColor = LightGridAlternatingBackColor;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = LightControlBackColor;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = LightForeColor;
                dgv.RowHeadersDefaultCellStyle.BackColor = LightControlBackColor;
                dgv.RowHeadersDefaultCellStyle.ForeColor = LightForeColor;
                dgv.GridColor = SystemColors.ControlLight;
            }
            else if (control is MenuStrip menu)
            {
                menu.BackColor = LightMenuBackColor;
                menu.ForeColor = LightForeColor;
                foreach (ToolStripMenuItem item in menu.Items)
                {
                    ApplyLightThemeToMenuItem(item);
                }
            }
            else if (control is ToolStrip toolStrip)
            {
                toolStrip.BackColor = LightStatusBackColor;
                toolStrip.ForeColor = LightForeColor;
            }
            else if (control is StatusStrip status)
            {
                status.BackColor = LightStatusBackColor;
                status.ForeColor = LightForeColor;
            }
            else if (control is Label lbl)
            {
                lbl.BackColor = Color.Transparent;
                lbl.ForeColor = LightForeColor;
            }
            else if (control is Panel panel)
            {
                panel.BackColor = LightBackColor;
                panel.ForeColor = LightForeColor;
            }
            else if (control is FlowLayoutPanel flp)
            {
                flp.BackColor = LightBackColor;
                flp.ForeColor = LightForeColor;
            }
            else if (control is TableLayoutPanel tlp)
            {
                tlp.BackColor = LightBackColor;
                tlp.ForeColor = LightForeColor;
            }
            else if (control is PictureBox pb)
            {
                pb.BackColor = LightBackColor;
            }
            else
            {
                control.BackColor = LightBackColor;
                control.ForeColor = LightForeColor;
            }

            foreach (Control child in control.Controls)
            {
                ApplyLightTheme(child);
            }
        }

        private static void ApplyLightThemeToMenuItem(ToolStripMenuItem item)
        {
            item.BackColor = LightMenuBackColor;
            item.ForeColor = LightForeColor;
            foreach (ToolStripItem subItem in item.DropDownItems)
            {
                if (subItem is ToolStripMenuItem subMenuItem)
                {
                    ApplyLightThemeToMenuItem(subMenuItem);
                }
                else
                {
                    subItem.BackColor = LightMenuBackColor;
                    subItem.ForeColor = LightForeColor;
                }
            }
        }
    }
}
