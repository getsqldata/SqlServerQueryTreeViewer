﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlServerParseTreeViewer
{
    public partial class OptionsForm : Form
    {
        private UserControl _currentUserControl;

        public OptionsForm()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            List<OptionsPage> pages = new List<OptionsPage>()
            {
                new OptionsPage("Counters", new InfoTrackerUserControl()),
                new OptionsPage("Operator Colors", new OperatorColorsUserControl())
            };

            this.pagesListBox.Items.Clear();
            this.pagesListBox.Items.AddRange(pages.ToArray());

            this.pagesListBox.SelectedIndex = 0;
        }

        public class OptionsPage
        {
            public OptionsPage(string displayText, UserControl pageControl)
            {
                DisplayText = displayText;
                PageControl = pageControl;
            }

            public string DisplayText
            {
                get;
                private set;
            }

            public UserControl PageControl
            {
                get;
                private set;
            }

            public override string ToString()
            {
                return DisplayText;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ViewerSettings.CancelClone();
            DialogResult = DialogResult.Cancel;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            ViewerSettings.PromoteClone();
            ViewerSettings.Instance.Save();
            DialogResult = DialogResult.OK;
        }

        private void PagesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OptionsPage optionsPage = pagesListBox.SelectedItem as OptionsPage;
            UserControl page = optionsPage.PageControl;

            page.Left = pagesListBox.Left + pagesListBox.Width + 15;
            page.Top = pagesListBox.Top;
            page.Width = this.Width - page.Left - 15;
            page.Height = pagesListBox.Height;

            if (_currentUserControl != null)
            {
                this.Controls.Remove(_currentUserControl);
            }

            _currentUserControl = page;
            this.Controls.Add(_currentUserControl);
        }
    }
}
