﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetCharm.Image.Addins;

namespace InternalFilters
{
    public partial class ResizeForm : Form
    {
        internal AddinHost host;
        private IAddin addin;

        private int _width = 0;
        public ParamItem ParamWidth
        {
            get
            {
                ParamItem pi = new ParamItem();
                pi.Name = "Width";
                pi.DisplayName = AddinUtils._( addin, pi.Name );
                pi.Type = Convert.ToInt32( edWidth.Value ).GetType();
                pi.Value = Convert.ToInt32( edWidth.Value );
                return ( pi );
            }
            internal set
            {
                _width = (int) value.Value;
                edWidth.Value = Convert.ToDecimal( value.Value );
            }
        }

        private int _height = 0;
        public ParamItem ParamHeight
        {
            get
            {
                ParamItem pi = new ParamItem();
                pi.Name = "Height";
                pi.DisplayName = AddinUtils._( addin, pi.Name );
                pi.Type = Convert.ToInt32( edHeight.Value ).GetType();
                pi.Value = Convert.ToInt32( edHeight.Value );
                return ( pi );
            }
            internal set
            {
                _height = (int) value.Value;
                edHeight.Value = Convert.ToDecimal( value.Value );
            }
        }

        private bool _aspect = true;
        public ParamItem ParamAspect
        {
            get
            {
                ParamItem pi = new ParamItem();
                pi.Name = "Mode";
                pi.DisplayName = AddinUtils._( addin, pi.Name );
                pi.Type = chkAspect.Checked.GetType();
                pi.Value = chkAspect.Checked;
                return ( pi );
            }
            internal set
            {
                _aspect = (bool) value.Value;
                chkAspect.Checked = (bool) value.Value;
            }
        }

        private int _method = 0;
        public ParamItem ParamMethod
        {
            get
            {
                ParamItem pi = new ParamItem();
                pi.Name = "Method";
                pi.DisplayName = AddinUtils._( addin, pi.Name );
                pi.Type = cbResizeMethod.SelectedIndex.GetType();
                pi.Value = cbResizeMethod.SelectedIndex;
                return ( pi );
            }
            internal set
            {
                if ( 0 <= Convert.ToInt32( value.Value ) && Convert.ToInt32( value.Value ) < cbResizeMethod.Items.Count )
                {
                    _method = (int) value.Value;
                    cbResizeMethod.SelectedIndex = Convert.ToInt32( value.Value );
                }
                else cbResizeMethod.SelectedIndex = 0;
            }
        }

        public ResizeForm()
        {
            InitializeComponent();
        }

        public ResizeForm( AddinHost host )
        {
            this.host = host;
            InitializeComponent();
        }

        public ResizeForm( IAddin filter )
        {
            this.addin = filter;
            InitializeComponent();

            cbResizeMethod.Items.Clear();
            cbResizeMethod.Items.Add( AddinUtils.T( "Bicubic" ) );
            cbResizeMethod.Items.Add( AddinUtils.T( "Bilinear" ) );
            cbResizeMethod.Items.Add( AddinUtils.T( "Nearest" ) );
        }

        private void ResizeForm_Load( object sender, EventArgs e )
        {
            chkAspect.Checked = _aspect;
            cbResizeMethod.SelectedIndex = _method;
        }

        private void edWidth_ValueChanged( object sender, EventArgs e )
        {
            if ( (this.Focused || edWidth.Focused || edHeight.Focused) && 
                addin is IAddin && 
                chkAspect.Checked )
            {
                var w = addin.ImageData.Width;
                var h = addin.ImageData.Height;
                double factor_o = w / (float)h;

                int wn = Convert.ToInt32(edWidth.Value);
                int hn = Convert.ToInt32(edHeight.Value);

                if ( sender == edWidth )
                {
                    hn = (int) Math.Round( wn / factor_o );
                    edHeight.Value = Convert.ToDecimal( hn );
                }
                else if ( sender == edHeight )
                {
                    wn = (int) Math.Round( hn * factor_o );
                    edWidth.Value = Convert.ToDecimal( wn );
                }
            }
        }
    }
}
