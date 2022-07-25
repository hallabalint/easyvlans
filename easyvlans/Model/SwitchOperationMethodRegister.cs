﻿using System.Xml;

namespace easyvlans.Model
{
    internal sealed class SwitchOperationMethodCollectionRegister : MethodRegisterBase<ISwitchOperationMethodCollection, ISwitchOperationMethodCollection.IFactory>
    {
        public static SwitchOperationMethodCollectionRegister Instance { get; } = new();
        private SwitchOperationMethodCollectionRegister() { }
        public ISwitchOperationMethodCollection GetMethodInstance(XmlNode configNode, Switch @switch) => getFactory(configNode.LocalName)?.GetInstance(configNode, @switch);
        protected override ISwitchOperationMethodCollection.IFactory[] KnownFactories { get; } = new ISwitchOperationMethodCollection.IFactory[]
        {
            new SnmpV2SwitchOperationMethodCollection.Factory()
        };
    }
}