$(document).ready(function () {
    loadData();
});

function loadData() {
    debugger;
    $.ajax({
        url: "https://localhost:5000/api/Matches/GetMatches",
        method: "GET",
        dataType: "json", // Corrected the dataType
        success: function (res) {
            debugger; // Pause for debugging
            var tbody = $("#tbody"); // Select the tbody element
            tbody.empty(); // Clear the existing tbody content

            $.each(res.data, function (index, item) {
                var html = ` 
                <tr>
            <td>
                <h2 class="table-avatar">
                    <a href="profile.html" class="avatar avatar-sm me-2">
                        <img class="avatar-img rounded-circle"
                             src="${item.logoUrl}"
                             alt="User Image">
                    </a>
                    <a href="profile.html">${item.opponentName}</a>
                </h2>
            </td>
            <td>Basic Calculation</td>
            <td>${item.matchDate} <br><small></small></td>
            <td class="text-center">
                <div class="status-toggle d-flex justify-content-center">
                    <input type="checkbox" id="status_${item.id}" class="check" ${item.status = 1?'checked' : ''}>
                    <label for="status_${item.id}" class="checktoggle">checkbox</label>
                </div>
            </td>
            <td class="text-center">
                <div class="actions">
                    <a data-bs-toggle="modal" href="#edit_invoice_report" class="btn btn-sm bg-success-light me-2">
                        <i class="fe fe-pencil"></i> Edit
                    </a>
                    <a class="btn btn-sm bg-danger-light" data-bs-toggle="modal" href="#delete_modal">
                        <i class="fe fe-trash"></i> Delete
                    </a>
                </div>
            </td>
        </tr>`;
                debugger

                tbody.append(html); // Append the generated HTML to tbody
            });

            debugger; // Pause for further debugging
        },


        error: function (res) {
            debugger;
        }
    });
}