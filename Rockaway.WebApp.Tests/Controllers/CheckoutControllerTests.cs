using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime.Testing;
using Rockaway.WebApp.Controllers;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Data.Sample;
using Rockaway.WebApp.Models;
using Rockaway.WebApp.Tests.Mail;

namespace Rockaway.WebApp.Tests.Controllers;

public class CheckoutControllerTests {

	private readonly RockawayDbContext db;
	private readonly CheckoutController controller;
	private readonly FakeClock clock = new(SampleData.NOW);

	private readonly FakeMailSender fakeMailSender = new();

	public CheckoutControllerTests() {
		this.db = TestDatabase.Create();

		var ticketMailer = fakeMailSender.CreateTikcetMailer();

		this.controller = new CheckoutController(db, clock, ticketMailer)
			.WithRequestUrl("https://rockaway.dev");
	}

	private async Task<TicketOrder> CreateTestOrderAsync() {
		var show = await db.Shows.Include(show => show.TicketTypes)
			.Include(show => show.HeadlineArtist)
			.FirstAsync();
		var order = show.CreateOrder(show.TicketTypes.ToDictionary(tt => tt.Id, tt => 2), clock.GetCurrentInstant());
		await db.TicketOrders.AddAsync(order);
		await db.SaveChangesAsync();
		return order;
	}

	private async Task<TicketOrder> CreateAndConfirmTestOrderAsync(string name = "Test Customer", string email = "test@example.com") {
		var order = await CreateTestOrderAsync();
		var post = new OrderConfirmationPostData {
			TicketOrderId = order.Id,
			CustomerEmail = email,
			AgreeToPayment = true,
			CustomerName = name
		};
		await controller.Confirm(post.TicketOrderId, post);
		return order;
	}

	[Fact]
	public async Task POST_Confirm_Sends_Tickets_By_Email() {
		fakeMailSender.Messages.ShouldBeEmpty();
		await CreateAndConfirmTestOrderAsync();
		fakeMailSender.Messages.Count.ShouldBe(1);
	}

	[Fact]
	public async Task POST_Confirm_Updates_Database_After_Sending_Email() {
		var order = await CreateAndConfirmTestOrderAsync();
		var db2 = TestDatabase.Connect(this.db.GetSqliteDbName());
		var order2 = await db2.TicketOrders.FindAsync(order.Id);
		order2.ShouldNotBeNull();
		order2.MailSentAt.ShouldBe(clock.GetCurrentInstant());
	}

	[Fact]
	public async Task POST_Confirm_ReturnsNotFound_IfOrderNotFound() {
		var postModel = new OrderConfirmationPostData { TicketOrderId = Guid.NewGuid() };
		var result = await controller.Confirm(postModel.TicketOrderId, postModel);
		result.ShouldBeOfType<NotFoundResult>();
	}

	[Fact]
	public async Task POST_Confirm_ReturnsBadRequest_IfRouteIdAndPostIdMismatch() {
		var guid1 = Guid.NewGuid();
		var guid2 = Guid.NewGuid();
		var postModel = new OrderConfirmationPostData { TicketOrderId = guid1 };
		var result = await controller.Confirm(guid2, postModel);
		result.ShouldBeOfType<BadRequestResult>();
	}

	[Fact]
	public async Task POST_Confirm_Returns_View_When_Model_State_Is_Invalid() {
		var order = await CreateTestOrderAsync();
		var postModel = new OrderConfirmationPostData {
			TicketOrderId = order.Id,
			CustomerEmail = "test@example.com",
			CustomerName = "Test User"
		};
		controller.ModelState.AddModelError("key", "error");
		var result = await controller.Confirm(order.Id, postModel) as ViewResult;
		result.ShouldNotBeNull();
		result.Model.ShouldBeOfType<OrderConfirmationPostData>();
	}

	[Fact]
	public async Task POST_Confirm_Saves_Order() {
		var order = await CreateTestOrderAsync();
		var postModel = new OrderConfirmationPostData {
			TicketOrderId = order.Id,
			CustomerEmail = "test@example.com",
			CustomerName = "Test User"
		};

		await controller.Confirm(order.Id, postModel);
		var updatedOrder = await db.TicketOrders.FindAsync(order.Id);
		updatedOrder.ShouldNotBeNull();
		updatedOrder!.CustomerEmail.ShouldBe("test@example.com");
		updatedOrder.CustomerName.ShouldBe("Test User");
		updatedOrder.CompletedAt.ShouldBe(clock.GetCurrentInstant());
	}

	[Fact]
	public async Task GET_Completed_Returns_NotFound_When_Order_Does_Not_Exist() {
		(await controller.Completed(Guid.NewGuid())).ShouldBeOfType<NotFoundResult>();
	}

	[Fact]
	public async Task GET_Completed_Returns_View() {
		var order = await CreateTestOrderAsync();
		var result = (ViewResult) (await controller.Completed(order.Id))!;
		var model = (TicketOrderViewData) result.Model!;
		model.Id.ShouldBe(order.Id);
		model.CustomerEmail.ShouldBe(order.CustomerEmail);
		model.CustomerName.ShouldBe(order.CustomerName);
	}

	[Fact]
	public async Task GET_Confirm_Returns_NotFound_If_Order_Does_Not_Exist() {
		var result = await controller.Confirm(Guid.NewGuid());
		result.ShouldBeOfType<NotFoundResult>();
	}

	[Fact]
	public async Task GET_Confirm_Returns_View() {
		var ticketOrder = await CreateTestOrderAsync();
		var viewResult = (await controller.Confirm(ticketOrder.Id)).ShouldBeOfType<ViewResult>();
		var model = viewResult.Model.ShouldBeOfType<OrderConfirmationPostData>();
		model.TicketOrderId.ShouldBe(ticketOrder.Id);
		model.TicketOrder.ShouldNotBeNull();
		model.TicketOrder.Id.ShouldBe(ticketOrder.Id);
	}
}

