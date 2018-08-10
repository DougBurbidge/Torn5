using System;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;
 
namespace Torn
{
	/// <summary>
	/// WebServer class.
	/// 
	/// Copyright (c) 2013 David's Blog (www.codehosting.net)
	/// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
	/// associated documentation files (the "Software"), to deal in the Software without restriction, 
	/// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
	/// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
	/// furnished to do so, subject to the following conditions:
	/// The above copyright notice and this permission notice shall be included in all copies or 
	/// substantial portions of the Software.
	/// 
	/// All the work is done on background threads, which will be automatically cleaned up when the program quits.
	/// </summary>
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "HttpListener not supported on this platform. Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("URI prefixes are required.");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("Responder method required.");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
