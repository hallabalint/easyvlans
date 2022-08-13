﻿using B.XmlDeserializer.Exceptions;
using System.Xml;

namespace B.XmlDeserializer.Attributes;

public class MandatoryAttributeNotFoundException : DeserializationException
{

    public string AttributeName { get; }

    public MandatoryAttributeNotFoundException(XmlNode ownerNode, string attributeName)
        : base($"Mandatory attribute [{attributeName}] not found.", ownerNode)
        => AttributeName = attributeName;

}
