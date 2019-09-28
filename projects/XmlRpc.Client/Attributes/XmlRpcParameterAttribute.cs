﻿using System;

namespace XmlRpc.Client.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class XmlRpcParameterAttribute : Attribute
    {
        public XmlRpcParameterAttribute()
        {
        }

        public XmlRpcParameterAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public override string ToString()
        {
            string value = "Description : " + description;
            return value;
        }

        string name = "";
        string description = "";
    }
}