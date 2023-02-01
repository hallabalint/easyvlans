﻿using easyvlans.Helpers;
using easyvlans.Logger;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace easyvlans.Model.SwitchOperationMethods
{
    internal class SnmpV2Connection : SnmpV1V2Connection
    {

        public SnmpV2Connection(string ip, int port, string communityStrings)
            : base(ip, port, communityStrings) { }

        protected override async Task<List<Variable>> DoWalkAsync(string objectIdentifierStr)
        {
            List<Variable> result = new();
            await Messenger.BulkWalkAsync(VersionCode.V2, _ipEndPoint, _readCommunityString, OctetString.Empty,
                new ObjectIdentifier(objectIdentifierStr), result, 5, WalkMode.WithinSubtree, null, null);
            return result;
        }

        protected override async Task DoSetAsync(List<Variable> variables)
            => await Messenger.SetAsync(VersionCode.V2, _ipEndPoint, _writeCommunityString, variables);

        protected override string VersionString => "SNMPv2";

    }
}
