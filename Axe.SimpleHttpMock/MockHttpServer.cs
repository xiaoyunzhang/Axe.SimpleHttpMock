﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Axe.SimpleHttpMock
{
    public class MockHttpServer : HttpMessageHandler
    {
        readonly List<IRequestHandler> m_handlers = new List<IRequestHandler>(32);

        public void AddHandler(IRequestHandler handler)
        {
            m_handlers.Add(handler);
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            IRequestHandler matchedHandler = m_handlers.FirstOrDefault(m => m.IsMatch(request));
            if (matchedHandler == null)
            {
                return Task.Factory.StartNew(
                    () => new HttpResponseMessage(HttpStatusCode.NotFound), cancellationToken);
            }

            return Task.Factory.StartNew(
                () => matchedHandler.Handle(request, cancellationToken),
                cancellationToken);
        }
    }
}