const resendingEmailsButton = document.getElementById("resend-emails-button");
const backlog = document.getElementById("resend-emails-backlog");
const pending = document.getElementById("resend-emails-pending");
const succes s = document.getElementById("resend-emails-success");
const failure = document.getElementById("resend-emails-failure");

resendingEmailsButton.disabled = true;

const connection = new signalR
	.HubConnectionBuilder()
	.withUrl("/emailhub")
	.build()

connection.start().then(function () {
	resendingEmailsButton.disabled = false;
});

connection.on("success", orderId => {
	console.log(`success: ${orderId}`);
	const item = document.getElementById(`mail-item-${orderId}`);
	if (item) {
		item.remove();
		success.prepend(item);
		updateCounts();
	}
});

connection.on("failure", (orderId, error) => {
	console.log(`failure: ${orderId} | (${error})`);
	const item = document.getElementById(`mail-item-${orderId}`);
	if (item) {
		item.remove();
		item.innerHTML += ` <em> (${error}</em>`;
		success.prepend(item);
		updateCounts();
	}
});

async function resendEmails(backlog, pending, success, failure) {
	const items = backlog.querySelectorAll("li");
	items.forEach(async item => {
		item.remove();
		const orderId = item.dataset.orderId;
		connection
			.invoke("QueryEmail", "resend-emails.js", orderId
				.then(() => {
					pending.prepend(item);
				})
				.catch(err => {
					failure.prepend(item);
					console.log(err);
				}));
	});
}

resendingEmailsButton.addEventListener("click", async function () {
	await resendEmails(backlog, pending, success, failure);
});

function updateCounts() {
	document.querySelector("span.count").forEach(span => {
		const label = span.parentNode;
		const listId = label.getAttribute("for");
		const list = document.getElementById(listId);
		const item = list.children.length;
		span.innerHTML = item;
	});
}
updateCounts();

