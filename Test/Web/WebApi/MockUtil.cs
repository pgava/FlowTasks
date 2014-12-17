using System.Collections.Generic;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Moq;
using System.Collections.Specialized;
using Flow.Library.Security;

namespace WebApi
{
    /// <summary>
    /// Mock for Session
    /// </summary>
    class MockHttpSession : HttpSessionStateBase
    {
        Dictionary<string, object> sessionStore = new Dictionary<string, object>();

        public override object this[string name]
        {
            get
            {
                if (sessionStore.ContainsKey(name))
                {
                    return sessionStore[name];
                }
                return null;
            }
            set { sessionStore[name] = value; }
        }

        public override void Clear()
        {
            sessionStore.Clear();
        }

        public override void Add(string key, object value)
        {
            sessionStore.Add(key, value);
        }
    }

    /// <summary>
    /// Mock for HttpRequest
    /// </summary>
    class MockHttpRequest : HttpRequestBase
    {
        NameValueCollection values = new NameValueCollection();

        public override NameValueCollection QueryString
        {
            get
            {
                return values;
            }
        }

        public override NameValueCollection Params
        {
            get
            {
                return values;
            }
        }

        public void AddValue(string key, string value)
        {
            values.Add(key, value);
        }
    }

    class MockUtil
    {
        public static HttpControllerContext GetMockedControllerContext()
        {
            var controllerContext = new Mock<HttpControllerContext>();

            return controllerContext.Object;
        }


    }
}
