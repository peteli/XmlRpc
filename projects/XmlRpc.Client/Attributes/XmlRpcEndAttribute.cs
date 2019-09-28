using System;

namespace XmlRpc.Client.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class XmlRpcEndAttribute : Attribute
    {
        public XmlRpcEndAttribute()
        {
        }

        public XmlRpcEndAttribute(string method)
        {
            this.method = method;
        }

        public string Method
        {
            get
            { return method; }
        }

        public bool IntrospectionMethod
        {
            get { return introspectionMethod; }
            set { introspectionMethod = value; }
        }

        public override string ToString()
        {
            string value = "Method : " + method;
            return value;
        }

        public string Description = "";
        public bool Hidden = false;
        string method = "";
        bool introspectionMethod = false;
    }
}

