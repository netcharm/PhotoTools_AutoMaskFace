﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetCharm.Image.Addins;

namespace InternalFilters.Actions
{
    using ParamList = Dictionary<string, ParamItem>;

    public partial class PinObjectForm : Form
    {
        internal AddinHost host;
        private IAddin addin;
        private Image thumb = null;
        private Image thumbBackup = null;

        private PinObjectMode mode = PinObjectMode.Picture;
        internal ParamItem ParamMode
        {
            get
            {
                ParamItem pi = new ParamItem();
                pi.Name = "PinObjectMode";
                pi.DisplayName = AddinUtils._( addin, pi.Name );
                pi.Type = mode.GetType();
                pi.Value = mode;
                return ( pi );
            }
            set { mode = (PinObjectMode) value.Value; }
        }

        private List<ListViewItem> effects = new List<ListViewItem>();
        //private List<Dictionary<string, ParamItem>> effectParams = new List<Dictionary<string, ParamItem>>();
        private List<ParamList> effectParams = new List<ParamList>();

        public PinObjectForm()
        {
            InitializeComponent();
        }

        public PinObjectForm( IAddin addin )
        {
            InitializeComponent();

            this.addin = addin;
            this.Text = this.addin.DisplayName;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            toolTip.ToolTipTitle = this.addin.DisplayName;
            AddinUtils.Translate( this.addin, this, toolTip );
        }

        private void PinObjectForm_Load( object sender, EventArgs e )
        {
            thumb = AddinUtils.CreateThumb( addin.ImageData, imgPreview.Size );
            imgPreview.Image = thumb;
        }

        private void lvFilters_RetrieveVirtualItem( object sender, RetrieveVirtualItemEventArgs e )
        {
            //lvFilters.VirtualListSize = effects.Count;
            try
            {
                if ( effects.Count > 0 && e.ItemIndex >= 0 && e.ItemIndex < effects.Count )
                {
                    e.Item = effects[e.ItemIndex];
                    if(lvFilters.View== View.Details)
                    {
                        e.Item.BackColor = ( e.ItemIndex % 2 == 1 ) ? Color.AliceBlue : e.Item.BackColor;
                    }
                }
                else
                {
                    e.Item = new ListViewItem( new string[] { "None" } );
                    e.Item.BackColor = Color.MediumAquamarine;
                }
            }
            catch ( Exception ex )
            {
                e.Item = new ListViewItem( new string[] { "Error" } );
                e.Item.BackColor = Color.LightPink;
            }
        }

        private void btnOriginal_Click( object sender, EventArgs e )
        {
            if ( btnOriginal.Checked )
            {
                thumbBackup = imgPreview.Image;
                imgPreview.Image = thumb;
            }
            else
            {
                if ( thumbBackup is Image )
                    imgPreview.Image = thumbBackup;
            }
        }

        private void btnEffectAdd_Click( object sender, EventArgs e )
        {
            lvFilters.BeginUpdate();
            foreach ( IAddin filter in AddinUtils.ShowAddinsDialog())
            {
                addin.Filters.Add( filter );
                ilLarge.Images.Add( filter.LargeIcon );
                ilSmall.Images.Add( filter.SmallIcon );
                effects.Add( new ListViewItem( filter.DisplayName ) );
                effects.Last().Selected = false;
                effects.Last().Tag = filter;
                effects.Last().ImageIndex = ilLarge.Images.Count;

                effectParams.Add( filter.Params as ParamList );
            }
            lvFilters.VirtualListSize = effects.Count;
            lvFilters.EndUpdate();
            //lvFilters.Update();
        }

        private void btnEffectRemove_Click( object sender, EventArgs e )
        {
            List<int> selected = new List<int>();
            foreach ( int i in lvFilters.SelectedIndices )
            {
                selected.Add( i );
            }
            selected.Reverse();
            lvFilters.BeginUpdate();
            foreach ( int i in selected )
            {
                ListViewItem item = effects[i];

                IAddin filter = item.Tag as IAddin;
                addin.Filters.Remove( filter );
                ilLarge.Images.RemoveAt( item.ImageIndex );
                ilSmall.Images.RemoveAt( item.ImageIndex );
                effects.Remove( item );
                effectParams.Remove( filter.Params as ParamList );
            }
            lvFilters.SelectedIndices.Clear();
            lvFilters.VirtualListSize = effects.Count;
            lvFilters.EndUpdate();
            //lvFilters.Update();
        }

        private void btnEffectUp_Click( object sender, EventArgs e )
        {
            lvFilters.BeginUpdate();
            foreach (int i in lvFilters.SelectedIndices )
            {
                if ( i <= 0 ) continue;
                var fi = effects[i];
                effects[i] = effects[i-1];
                effects[i - 1] = fi;
                lvFilters.Items[i - 1].Selected = true;
                lvFilters.Items[i].Selected = false;
            }
            lvFilters.EndUpdate();
            //lvFilters.Update();
        }

        private void btnEffectDown_Click( object sender, EventArgs e )
        {
            List<int> selected = new List<int>();
            foreach ( int i in lvFilters.SelectedIndices )
            {
                selected.Add( i );
            }
            selected.Reverse();
            lvFilters.BeginUpdate();
            foreach ( int i in selected )
            {
                if ( i >= effects.Count-1 ) continue;
                var fi = effects[i];
                effects[i] = effects[i + 1];
                effects[i + 1] = fi;
                lvFilters.Items[i + 1].Selected = true;
                lvFilters.Items[i].Selected = false;
            }
            lvFilters.EndUpdate();
            //lvFilters.Update();
        }

        private void lvFilters_DoubleClick( object sender, EventArgs e )
        {
            //lvFilters.GetItemAt();
            var filter = lvFilters.FocusedItem.Tag as IAddin;
            filter.ImageData = addin.ImageData;
            var pilist = effectParams[effectParams.IndexOf( filter.Params )];
            for (int i=0;i< pilist.Count;i++ )
            {
                var pi = pilist.ElementAt(i);
                filter.Params[pi.Key] = pi.Value;
            }
            filter.Show( this, true );
            effectParams[effectParams.IndexOf( filter.Params )] = filter.Params;
        }
    }
}
