﻿using Lextm.SharpSnmpLib;

namespace easyvlans.Model.SwitchOperationMethods
{

    internal sealed class SnmpPersistChangesHpBasicConfigMethod : ISnmpPersistChangesMethod
    {

        public const string CODE = "hpbasicconfig";

        public class Factory : ISnmpPersistChangesMethod.IFactory
        {
            public string Code => CODE;
            public ISnmpPersistChangesMethod GetInstance(string @params, ISnmpSwitchOperationMethodCollection parent)
                => new SnmpPersistChangesHpBasicConfigMethod(@params, parent);
        }

        private ISnmpSwitchOperationMethodCollection _parent;

        public SnmpPersistChangesHpBasicConfigMethod(string @params, ISnmpSwitchOperationMethodCollection parent)
            => _parent = parent;

        public string Code => CODE;
        public string DetailedCode => $"{_parent.Code}[{CODE}]";

        async Task IPersistChangesMethod.DoAsync()
        {
            await _parent.SnmpConnection.SetAsync(new List<Variable>() {
                new Variable(new ObjectIdentifier($"{OID_HP_SAVECONFIG}"), new Integer32(2))
            });
        }

        private const string OID_HP_SAVECONFIG = "1.3.6.1.4.1.11.2.14.11.5.1.7.1.29.1.1";

    }

}