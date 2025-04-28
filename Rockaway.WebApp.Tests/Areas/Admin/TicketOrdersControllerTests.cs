using Microsoft.AspNetCore.Mvc;
using NodaTime.Testing;
using Rockaway.WebApp.Areas.Admin.Controllers;
using Rockaway.WebApp.Areas.Admin.Models;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Sample;
using Rockaway.WebApp.Tests.Mail;
using System.Diagnostics;

namespace Rockaway.WebApp.Tests.Areas.Admin;

public class TicketOrdersControllerTests {

	private readonly RockawayDbContext db = TestDatabase.Create();
	private readonly TicketOrdersController controller;
	private readonly FakeClock clock = new(SampleData.NOW);

	public TicketOrdersControllerTests() {
		this.controller = new(db, new FakeTicketMailer(),clock);
	}

	[Fact]
	public async Task Index_Returns_Orders() {
		var orders = await controller.Index();
		orders.ShouldNotBeNull();
	}

	[Fact]
	public async Task Index_Returns_Orders_With_Search() {
		var result = (await controller.Index(search: "patrick")).ShouldBeOfType<ViewResult>();
		var model = result.Model.ShouldBeOfType<TicketOrdersViewData>();
		foreach(var order in model.Orders) order.CustomerName.ShouldContain("Patrick");
	}

	[Fact]
	public async Task Index_Returns_Orders_With_Pagination() {
		var result1 = (await controller.Index(index: 0, count: 10)).ShouldBeOfType<ViewResult>();
		var model1 = result1.Model.ShouldBeOfType<TicketOrdersViewData>();
		var result2 = (await controller.Index(index: 1, count: 10)).ShouldBeOfType<ViewResult>();
		var model2 = result2.Model.ShouldBeOfType<TicketOrdersViewData>();
		model1.ShouldNotBeNull();
		model2.ShouldNotBeNull();
		var order1 = model1.Orders[1];
		var order2 = model2.Orders[0];
		order1.Id.ShouldBe(order2.Id);
	}
}
