﻿using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Models;

public class TicketOrderMailData(TicketOrder ticketOrder, Uri websiteBaseUri)
	: TicketOrderViewData(ticketOrder) {
	public Uri BaseUri { get; } = websiteBaseUri;

	public Uri QualifyUri(string path) => new(BaseUri, path);

	public Uri GetHeroImageUri(int width, int height)
		=> QualifyUri(Artist.GetImageUrl(width, height));
}