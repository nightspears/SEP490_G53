﻿using System.Net.Http.Headers;

namespace TCViettetlFC_Client.ApiServices
{
	public class AuthTokenHandler : DelegatingHandler
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthTokenHandler(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
			if (!string.IsNullOrEmpty(token))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}

			return await base.SendAsync(request, cancellationToken);
		}
	}

}
