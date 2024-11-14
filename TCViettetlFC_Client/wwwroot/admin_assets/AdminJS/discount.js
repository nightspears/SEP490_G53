
$(document).ready(function () {
    loadData();
    if (!$.fn.DataTable.isDataTable('.datatable')) {
        $('.datatable').DataTable({
            "paging": true,
            "pageLength": 10,
            "ordering": true,
            "info": true
        });
    }
});
function CheckAll(item) {
    var isChecked = $(item).is(':checked');
    var checkboxes = $(item).closest('table').find('.form-check-input');
    checkboxes.prop('checked', isChecked);
};
$(document).on("click", "#confirmXoa", function () {
    debugger
    $("#idXoa").val("");
    var id = $(this).data("id");
    $("#idXoa").val(id);
    debugger

});
$(document).on("click", "#btnXoa", function () {

    var id = $("#idXoa").val();
    debugger
    var url = "https://localhost:5000/api/Discount/DeleteDiscount/" + id;
    $.ajax({
        url: url,
        method: "Delete",
        success: function (res) {
            debugger;
            $("#delete_modal").modal("hide");
            $(".modal-backdrop").hide();
            showAlert("Xóa thành công ");
            loadData();
        },
        error: function (res) {
            alert("xóa thất bại ")
        }
    });

});

function loadData()  {
    debugger

    $.ajax({
        url: "https://localhost:5000/api/Discount/GetDiscount",
        method: "GET",
        dataType: "json",
        success: function (res) {
            debugger
            var table = $('.datatable').DataTable();

            table.clear();
            $.each(res, function (index, item) {

                var rowHtml = `
                    <tr>
                        <td>
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" value="" data-id="${item.discountId}">
                            </div>
                        </td>
                        <td>${item.discountName}</td>
                        <td>${item.discountPercent}</td>
                        <td>${item.validFrom}</td>

                        <td>${item.validUntil}</td>

                        <td class="text-center">
                            <div class="status-toggle d-flex justify-content-center">
                                <input type="checkbox" id="status_${item.discountId}" class="check" ${item.status === 1 ? 'checked' : ''}>
                                <label for="status_${item.discountId}" class="checktoggle">checkbox</label>
                            </div>
                        </td>
                        <td class="text-center">
                            <div class="actions">
                                <a onclick="modalEditOrCreate(${item.discountId})" class="btn btn-sm bg-success-light me-2">
                                    <i class="fe fe-pencil"></i> Sửa
                                </a>
                                <a class="btn btn-sm bg-danger-light" data-bs-toggle="modal" data-id="${item.discountId}" id="confirmXoa" href="#delete_modal">
                                    <i class="fe fe-trash"></i> Xóa
                                </a>
                            </div>
                        </td>
                    </tr>`;

                table.row.add($(rowHtml)); // Thêm từng dòng mới vào bảng
            });

            table.draw();
        },
        error: function (res) {
            console.error("Error loading data", res);
        }
    });
}

// Khởi tạo DataTable khi trang tải lần đầu
$(document).ready(function () {
    if (!$.fn.DataTable.isDataTable('.datatable')) {
        $('.datatable').DataTable({
            "paging": true,
            "pageLength": 10,
            "ordering": true,
            "info": true
        });
    }
});




function modalEditOrCreate(id) {
    debugger
    $('#btnNomal').show();
    $('#btnLoading').hide();

    $('#idDiscount').val("");
    $('#maGiamGia').val("");
    $('#percent').val("");

    $('#startDate').val("");

    $('#endDate').val("");

    $('#status').prop('checked', false);

    if (id != 0 && id != undefined) {
        $("#titleModal").text("Cập nhật mã giảm giá")

        var url = "https://localhost:5000/api/Discount/GetDiscountById?id=" + id;
        $.ajax({
            url: url,
            method: "get",
            success: function (res) {

              
                $('#idDiscount').val(res.discountId);
                $('#maGiamGia').val(res.discountName);
                $('#percent').val(res.discountPercent);
                $('#startDate').val(res.validFrom);
                $('#endDate').val(res.validUntil);

                debugger
                if (res.status == 1) {
                    $('#status').prop('checked', true);
                } else {
                    $('#status').prop('checked', false);

                }
                $(".modal-backdrop").hide();

                $('#modalEditorCreate').modal("show")

            },
            error: function (res) {
                alert("Lỗi ")
            }
        });

    }
    else {

        $("#titleModal").text("Thêm mã giảm giá")

        $('#idDiscount').hide()
      
        $(".modal-backdrop").remove();
        $('#modalEditorCreate').modal("show")

    }

}

function EditOrCreate() {
    $('#btnNomal').hide();
    $('#btnLoading').show();
    var id = $('#idDiscount').val();

    var formData = new FormData();

    formData.append('DiscountName', $('#maGiamGia').val());
    formData.append('DiscountPercent', $('#percent').val());

    formData.append('ValidFrom', $('#startDate').val());

    formData.append('ValidUntil', $('#endDate').val());


    if ($('#status').prop('checked')) {
        formData.append('Status', 1);
    } else {
        formData.append('Status', 2);
    }


    if (id != 0 && id != undefined) {
        $.ajax({
            url: 'https://localhost:5000/api/Discount/UpdateDiscount/' + id,
            type: 'Put',
            data: formData,
            contentType: false, // Không thiết lập Content-Type
            processData: false, // Không xử lý dữ liệu
            success: function (response) {
                loadData();
                showAlert("Cập nhật thành công");
                $("#modalEditorCreate").modal("hide");
            },
            error: function (error) {
                debugger
            }
        });

    } else {
        $.ajax({
            url: 'https://localhost:5000/api/Discount/AddDiscount',
            type: 'post',
            data: formData,
            contentType: false, // Không thiết lập Content-Type
            processData: false, // Không xử lý dữ liệu
            success: function (response) {
                loadData();
                showAlert("Thêm mới thành công");
                $("#modalEditorCreate").modal("hide");
            },
            error: function (error) {
                debugger
            }
        });
    }

}

function validation() {

    var check = true;
    if ($('#TenDoiThu').val().trim() == "" || $('#TenDoiThu').val() == null) {
        check = false;
    }
    if ($('#isHome').val() === '2') {
        if ($('#tenSan').val().trim() == "" || $('#tenSan').val() == null) {
            check = false;
        }
    }
    if ($('#ngayDa').val().trim() == "" || $('#ngayDa').val() == null) {
        check = false;
    }



}


function showAlert(mess) {

    $("#alertMessageSuccess").text(mess);
    $("#alertSuccess").show();
    setTimeout(function () {
        $("#alertSuccess").hide(); // Hide the notification box with fade-out effect
    }, 3500); // 4 seconds delay
}



