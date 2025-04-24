// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(".spin-on-click").on("click", function() {
	const $this = $(this);
	$this.find(".fa-solid").addClass("fa-spin");
	$(this).attr("disabled", true);
	$(this).closest("form").submit();
});