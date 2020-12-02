using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cristales_pva
{
    public partial class datagridviewNE : DataGridView
    {
        public datagridviewNE()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
            }
            catch (Exception)
            {
                this.Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            try
            {
                base.OnMouseLeave(e);
            }
            catch (Exception)
            {
                this.Invalidate();
            }
        }
    }
}
