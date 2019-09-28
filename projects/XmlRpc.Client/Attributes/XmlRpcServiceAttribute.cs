﻿using System;

namespace XmlRpc.Client.Attributes
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
  public class XmlRpcServiceAttribute : Attribute
  {
    public XmlRpcServiceAttribute()
    {
    }

    public bool AutoDocumentation
    {    
        get { return autoDocumentation; }
        set { autoDocumentation = value; }
    }

    public bool AutoDocVersion
    {    
      get { return autoDocVersion; }
      set { autoDocVersion = value; }
    }

    public string Description 
    {
      get { return description; }
      set { description = value; }
    }

    public int Indentation 
    {
      get { return indentation; }
      set { indentation = value; }
    }

    public bool Introspection
    {
      get { return introspection; }
      set { introspection = value; }
    }

    public string Name 
    {
      get { return name; }
      set { name = value; }
    }

    public bool UseIndentation
    {
      get { return useIndentation; }
      set { useIndentation = value; }
    }

    public bool UseIntTag
    {
      get { return useIntTag; }
      set { useIntTag = value; }
    }

    public bool UseStringTag
    {
      get { return useStringTag; }
      set { useStringTag = value; }
    }

    public string XmlEncoding
    {
      get { return xmlEncoding; }
      set { xmlEncoding = value; }
    }

    public override string ToString()
    {
      string value = "Description : " + description;
      return value;
    }
 
    string description = "";
    string xmlEncoding = null;
    int indentation = 2;
    bool introspection = false;
    bool autoDocumentation = true;
    bool autoDocVersion = true;
    string name = "";
    bool useStringTag = true;
    bool useIndentation = true;
    bool useIntTag = false;
  }
}