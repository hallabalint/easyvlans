﻿using easyvlans.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace easyvlans.GUI
{
    public partial class PortDataDisplay : UserControl
    {

        public PortDataDisplay()
        {
            InitializeComponent();
            displayStyle(ST_UNKNOWN);
        }

        private readonly ToolTip toolTip = new();

        protected Port _port;
        public Port Port
        {
            get => _port;
            set
            {
                if (value == _port)
                    return;
                if (_port != null)
                    unsubscribeEvents();
                _port = value;
                if (_port != null)
                    subscribeEvents();
            }
        }

        protected /*abstract*/ virtual void subscribeEvents() { }
        protected /*abstract*/ virtual void unsubscribeEvents() { }

        protected void update() => displayStyle(getStyleFromData());

        protected /*abstract*/ virtual StatusStyle getStyleFromData() => ST_UNKNOWN;

        bool tooltipShown = true;

        private void label_MouseEnter(object sender, EventArgs e)
        {
            tooltipShown = true;
            showTooltip();
        }

        private void label_MouseLeave(object sender, EventArgs e)
        {
            tooltipShown = false;
            toolTip.Hide(label);
        }

        protected void showTooltip() => toolTip.Show(getTooltipText(), label);

        protected void reshowTooltip()
        {
            if (!tooltipShown)
                return;
            showTooltip();
        }

        protected /*abstract*/ virtual string getTooltipText() => string.Empty;

        protected record StatusStyle(Color Background, Color Foreground, Func<string> StrFunc);
        protected static readonly StatusStyle ST_UNKNOWN = new(Color.Silver, Color.Black, () => "unknw");

        private void displayStyle(StatusStyle statusStyle)
        {
            label.BackColor = statusStyle.Background;
            label.ForeColor = statusStyle.Foreground;
            label.Text = statusStyle.StrFunc();
        }

    }
}
