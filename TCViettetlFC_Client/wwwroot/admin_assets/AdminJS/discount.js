
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

function loadData() {
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
                        <td>${item.fromFormatted}</td>

                        <td>${item.untilFormatted}</td>

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

function resetForm() {
    $('#btnNomal').show();
    $('#btnLoading').hide();

    $('#idDiscount').val("");
    $('#maGiamGia').val("");
    $('#percent').val("");

    $('#startDate').val("");

    $('#endDate').val("");

    $('#status').prop('checked', true);
    $('#errorMa').hide();
    $('#errorPercent').hide();
    $('#errorEnd').hide();
    $('#errorStart').hide();

}


function modalEditOrCreate(id) {
    debugger
    resetForm()

    if (id != 0 && id != undefined) {
        $("#titleModal").text("Cập nhật mã giảm giá")

        var url = "https://localhost:5000/api/Discount/GetDiscountById?id=" + id;
        $.ajax({
            url: url,
            method: "get",
            success: function (res) {

                debugger
                $('#idDiscount').val(res.discountId);
                $('#maGiamGia').val(res.discountName);
                $('#percent').val(res.discountPercent);

                let validFrom = res.validFrom;
                let formattedValidFrom = validFrom.split("T")[0];

                $('#startDate').val(formattedValidFrom);

                let validUntil = res.validUntil;
                let formattedValidUntil = validUntil.split("T")[0];

                $('#endDate').val(formattedValidUntil);



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
    if (validation()) {

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
}

function validation() {
    var check = true;

    if ($('#maGiamGia').val().trim() == "" || $('#maGiamGia').val() == null) {
        check = false;
        $('#errorMa').show();

    } else {
        $('#errorMa').hide();

    }
    debugger
    var percent = $('#percent').val().trim();

    if (percent === "" || percent === null) {
        check = false;
        $('#errorPercent').show(); 
    } else {
        $('#errorPercent').hide();
        if (isNaN(percent) || percent < 0 || percent > 100) {
            check = false;
            $('#errorPercent').text("Vui lòng nhập số từ 0 - 100");
            $('#errorPercent').show();
        } else {
            $('#errorPercent').hide();

        }

    }

    var startDate = $('#startDate').val().trim();
    var today = new Date();
    today.setHours(0, 0, 0, 0);
    if (startDate == "" || startDate == null) {
        check = false;
        $('#errorStart').show();

    } else {
        $('#errorStart').hide();

       
        var startDateObj = new Date(startDate);
        if (startDateObj <= today) {
            check = false;
            $('#errorStart').text("Ngày bắt đầu phải lớn hơn hoặc bằng ngày hôm nay");
            $('#errorStart').show();

        }
    }

    // Kiểm tra ngày kết thúc
    var endDate = $('#endDate').val().trim();
    if (endDate == "" || endDate == null) {
        check = false;
        $('#errorEnd').show();
    } else {
        $('#errorEnd').hide();

        var endDateObj = new Date(endDate);
        if (endDateObj <= today) {
            check = false;
            $('#errorEnd').text("Ngày kết thúc phải lớn hơn ngày hôm nay.");
            $('#errorEnd').show();
        }
        else {
            $('#errorEnd').hide();

        }
        if (startDate && endDate && new Date(startDate) >= endDateObj) {
            check = false;
            $('#errorEnd').text("Ngày kết thúc phải lớn hơn ngày bắt đầu.");
            $('#errorEnd').show();
        } else {
            $('#errorEnd').hide();

        }
    }

    return check;
}



function showAlert(mess) {

    $("#alertMessageSuccess").text(mess);
    $("#alertSuccess").show();
    setTimeout(function () {
        $("#alertSuccess").hide(); // Hide the notification box with fade-out effect
    }, 3500); // 4 seconds delay
}



