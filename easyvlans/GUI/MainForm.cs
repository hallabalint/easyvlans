﻿using easyvlans.GUI;
using easyvlans.GUI.Helpers;
using easyvlans.GUI.Helpers.DropDowns;
using easyvlans.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace easyvlans.GUI
{
    public partial class MainForm : Form
    {

        private Config config;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(Config config)
        {
            this.config = config;
            Load += loadConfig;
            InitializeComponent();
        }

        private void loadConfig(object sender, EventArgs e)
        {

            if (config == null)
            {
                // Todo...
                return;
            }

            showPorts();
            showSwitches();

        }

        private void showPorts()
        {

            if (config.Ports.Count == 0)
            {
                // Todo...
                return;
            }

            int portRow = 0;
            int portRowHeight = (int)portTable.RowStyles[1].Height;
            foreach (Port port in config.Ports)
            {

                int portTableRow = portRow + 1;
                PortRowControls thisPortRowControls = getPortRowControls(portRow);
                portRowControls.Add(thisPortRowControls);
                portAssociatedRowControls.Add(port, thisPortRowControls);

                if (portRow > 0)
                {
                    portTable.RowCount++;
                    portTable.RowStyles.Add(new RowStyle(SizeType.Absolute, portRowHeight));
                    portTable.Controls.Add(thisPortRowControls.PortLabel, 0, portTableRow);
                    portTable.Controls.Add(thisPortRowControls.Switch, 1, portTableRow);
                    portTable.Controls.Add(thisPortRowControls.PortId, 2, portTableRow);
                    portTable.Controls.Add(thisPortRowControls.CurrentVlan, 3, portTableRow);
                    portTable.Controls.Add(thisPortRowControls.SetVlanTo, 4, portTableRow);
                    portTable.Controls.Add(thisPortRowControls.Set, 5, portTableRow);
                    portTable.Controls.Add(thisPortRowControls.State, 6, portTableRow);
                    Size = new Size(Size.Width, Size.Height + portRowHeight);
                }

                thisPortRowControls.PortLabel.Text = port.Label;
                thisPortRowControls.Switch.Text = port.Switch.Label;
                thisPortRowControls.PortId.Text = port.Index;
                thisPortRowControls.CurrentVlan.Text = CURRENT_VLAN_UNKNOWN;
                thisPortRowControls.SetVlanTo.Tag = port;
                thisPortRowControls.Set.Tag = port;
                thisPortRowControls.Set.Enabled = false;
                thisPortRowControls.State.Text = "";

                portRow++;

            }

            portRow = 0;
            foreach (Port port in config.Ports)
                portRowControls[portRow++].SetVlanTo.CreateAdapterAsDataSource(port.Vlans, vlanToStr, true, "");

            foreach (Port port in config.Ports)
            {
                port.CurrentVlanChanged += portsCurrentVlanChangedHandler;
                PortRowControls rowControls = portAssociatedRowControls[port];
                rowControls.SetVlanTo.SelectedIndexChanged += portsSetVlanToSelectedIndexChangedHandler;
                rowControls.Set.Click += portsSetButtonClickHandler;
            }

        }

        private void portsCurrentVlanChangedHandler(Port port, Vlan newValue)
        {
            PortRowControls rowControls = portAssociatedRowControls[port];
            rowControls.CurrentVlan.Text = vlanToStr(newValue);
            rowControls.SetVlanTo.SelectedIndex = 0;
        }

        private void portsSetVlanToSelectedIndexChangedHandler(object sender, EventArgs e)
        {
            ComboBox typedSender = sender as ComboBox;
            Port port = typedSender?.Tag as Port;
            if (port == null)
                return;
            portAssociatedRowControls[port].Set.Enabled = (typedSender.SelectedIndex > 0);
        }

        private void portsSetButtonClickHandler(object sender, EventArgs e)
        {
            Button typedSender = sender as Button;
            Port port = typedSender?.Tag as Port;
            if (port == null)
                return;
            Vlan selectedVlan = portAssociatedRowControls[port].SetVlanTo.SelectedValue as Vlan;
            port.SetVlanTo(selectedVlan);
        }

        private void showSwitches()
        {

            if (config.Switches.Count == 0)
            {
                // Todo...
                return;
            }

            int switchRow = 0;
            int switchRowHeight = (int)switchTable.RowStyles[1].Height;
            foreach (Switch @switch in config.Switches.Values)
            {

                int switchTableRow = switchRow + 1;
                SwitchRowControls thisSwitchRowControls = getSwitchRowControls(switchRow);
                switchRowControls.Add(thisSwitchRowControls);
                switchAssociatedRowControls.Add(@switch, thisSwitchRowControls);

                if (switchRow > 0)
                {
                    switchTable.RowCount++;
                    switchTable.RowStyles.Add(new RowStyle(SizeType.Absolute, switchRowHeight));
                    switchTable.Controls.Add(thisSwitchRowControls.SwitchName, 0, switchTableRow);
                    switchTable.Controls.Add(thisSwitchRowControls.PendingChanges, 1, switchTableRow);
                    switchTable.Controls.Add(thisSwitchRowControls.PersistChanges, 2, switchTableRow);
                    Size = new Size(Size.Width, Size.Height + switchRowHeight);
                }

                thisSwitchRowControls.SwitchName.Text = @switch.Label;
                thisSwitchRowControls.PendingChanges.Text = "no ports changed";
                thisSwitchRowControls.PersistChanges.Tag = @switch;
                thisSwitchRowControls.PersistChanges.Enabled = false;
                thisSwitchRowControls.PersistChanges.Click += switchesPersistChangesButtonClickHandler;
                @switch.PortsWithPendingChangeCountChanged += switchesPortsWithPendingChangeCountChangedHandler;

                switchRow++;

            }

        }

        private void switchesPortsWithPendingChangeCountChangedHandler(Switch @switch, int newValue)
        {
            SwitchRowControls thisSwitchRowControls = switchAssociatedRowControls[@switch];
            string newText = "no ports changed";
            if (newValue == 0)
            {
                thisSwitchRowControls.PendingChanges.Text = newText;
                thisSwitchRowControls.PendingChanges.ForeColor = COLOR_NO_PENDING_CHANGES;
                thisSwitchRowControls.PersistChanges.Enabled = false;
            }
            else
            {
                newText = (newValue > 1) ? $"{newValue} ports changed" : "1 port changed";
                thisSwitchRowControls.PendingChanges.Text = newText;
                thisSwitchRowControls.PendingChanges.ForeColor = COLOR_HAS_PENDING_CHANGES;
                thisSwitchRowControls.PersistChanges.Enabled = true;
            }
        }

        private void switchesPersistChangesButtonClickHandler(object sender, EventArgs e)
        {
            Button typedSender = sender as Button;
            Switch @switch = typedSender?.Tag as Switch;
            @switch?.PersistChanges();
        }

        private string vlanToStr(Vlan vlan) => $"{vlan.ID} - {vlan.Name}";

        public class PortRowControls
        {
            public Label PortLabel { get; init; }
            public Label Switch { get; init; }
            public Label PortId { get; init; }
            public Label CurrentVlan { get; init; }
            public ComboBox SetVlanTo { get; init; }
            public Button Set { get; init; }
            public Label State { get; init; }
        }

        public class SwitchRowControls
        {
            public Label SwitchName { get; init; }
            public Label PendingChanges { get; init; }
            public Button PersistChanges { get; init; }
        }

        private List<PortRowControls> portRowControls = new List<PortRowControls>();
        private Dictionary<Port, PortRowControls> portAssociatedRowControls = new Dictionary<Port, PortRowControls>();

        private List<SwitchRowControls> switchRowControls = new List<SwitchRowControls>();
        private Dictionary<Switch, SwitchRowControls> switchAssociatedRowControls = new Dictionary<Switch, SwitchRowControls>();

        private T cloneOrOriginal<T>(T originalControl, int row)
            where T : Control
            => (row == 0) ? originalControl : originalControl.Clone();

        private PortRowControls getPortRowControls(int portRow)
        {
            return new PortRowControls()
            {
                PortLabel = cloneOrOriginal(rowPortPortLabel, portRow),
                Switch = cloneOrOriginal(rowPortSwitch, portRow),
                PortId = cloneOrOriginal(rowPortPortId, portRow),
                CurrentVlan = cloneOrOriginal(rowPortCurrentVlan, portRow),
                SetVlanTo = cloneOrOriginal(rowPortSetVlanTo, portRow),
                Set = cloneOrOriginal(rowPortSet, portRow),
                State = cloneOrOriginal(rowPortState, portRow)
            };
        }

        private SwitchRowControls getSwitchRowControls(int switchRow)
        {
            return new SwitchRowControls()
            {
                SwitchName = cloneOrOriginal(rowSwitchSwitchName, switchRow),
                PendingChanges = cloneOrOriginal(rowSwitchPendingChanges, switchRow),
                PersistChanges = cloneOrOriginal(rowSwitchPersistChanges, switchRow)
            };
        }

        private const string CURRENT_VLAN_UNKNOWN = "unknown";
        private Color COLOR_NO_PENDING_CHANGES = SystemColors.ControlDark;
        private Color COLOR_HAS_PENDING_CHANGES = Color.DarkRed;

    }
}