﻿using easyvlans.Model.Remote;
using EmberPlusProviderClassLib;
using EmberPlusProviderClassLib.EmberHelpers;
using Lextm.SharpSnmpLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace easyvlans.Model.Remote.EmberPlus
{
    class MyEmberPlusProvider : IRemoteMethod
    {
        private int _port;
        private EmberPlusProvider _tree;
        private string _identity;
        private bool _autoPersist;
        public string Code => "ember";
        protected Dictionary<int, Vlan> _vlans;
        protected Dictionary<int, Port> _ports;
        protected Dictionary<int, Switch> _switch;
        public Dictionary<int, Vlan> Vlans { get{ return _vlans; } }
        public Dictionary<int, Port> Ports { get { return _ports; } }
        public Dictionary<int, Switch> Switchs { get { return _switch; } }
        public bool AutoPersist { get { return _autoPersist; } }

        public void Start() {
            _tree = new EmberPlusProvider(
                _port,
                "EasyVLANs",
                "EasyVLANs");

            _tree.CreateIdentityNode(
                1,
                _identity ?? "EasyVLANs",
                "EasyVLANs",
                "Komlós Boldizsár",
                "v1.0.0");

            EmberNode matricesNode = _tree.AddChildNode(2, "matrices");
            _ = new VlanToPortMatrix(this, 1, "vlan2port", matricesNode, _tree);
            EmberNode switchesNode = _tree.AddChildNode(3, "switches");
            _ = new SwitchPersist(this, switchesNode, _tree);

        }

        public MyEmberPlusProvider(int port, string identity, bool autoPersist)
        {
            _port = port;
            _identity = identity;
             _autoPersist = autoPersist;
        }
           
        public void MeetConfig(Config config)
        {
            _vlans = new();
            _switch = new();
            _ports = new();
            foreach (var vlan in config.Vlans)
            {
                _vlans.Add(vlan.Value.ID, vlan.Value);
            }
            foreach (var @switch in config.Switches) {
                if (@switch.Value.RemoteIndex != null)
                {
                    _switch.Add(@switch.Value.RemoteIndex.Value, @switch.Value);
                }
            }
            foreach (var port in config.Ports)
            {
                if (port.RemoteIndex != null)
                {
                    _ports.Add(port.RemoteIndex.Value, port);
                }
            }
        }

    }
}
