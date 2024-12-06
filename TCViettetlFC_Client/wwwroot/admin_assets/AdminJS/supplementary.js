
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
    format()
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
    var url = "https://localhost:5000/api/SupplementaryItem/" + id;
    $.ajax({
        url: url,
        method: "Delete",
        success: function (res) {
            debugger;
            $("#delete_modal").modal("hide");
            $(".modal-backdrop").hide();
            showAlert("xóa thành công ");
            loadData();
        },
        error: function (res) {
            alert("xóa thất bại ")
        }
    });

});

function loadData() {
    $.ajax({
        url: "https://localhost:5000/api/SupplementaryItem",
        method: "GET",
        dataType: "json",
        success: function (res) {
           
            var table = $('.datatable').DataTable();

            table.clear();
            $.each(res, function (index, item) {
                var image = item.image ? item.image : "/image/anh_mau_do_an.jpg";


                var rowHtml = `
                    <tr>
                        <td>
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" value="" data-id="${item.itemId}">
                            </div>
                        </td>
                        <td>
                            <h2 class="table-avatar">
                                <a href="profile.html" class="avatar avatar-sm me-2">
                                    <img class="avatar-img rounded-circle"
                                        src="${image}"
                                        alt="User Image">
                                </a>
                                <a href="profile.html">${item.itemName}</a>
                            </h2>
                        </td>
                        <td class="price" data-price="${item.price}">${item.price}</td>
                    
                        <td class="text-center">
                            <div class="status-toggle d-flex justify-content-center">
                                <input type="checkbox" data-mid="${item.itemId}" id="status_${item.itemId}" class="check changeStatus" ${item.status === 1 ? 'checked' : ''}   onchange="changeStatus(this)">
                                <label for="status_${item.itemId}" class="checktoggle">checkbox</label>
                            </div>
                        </td>
                        <td class="text-center">
                            <div class="actions">
                                <a onclick="modalEditOrCreate(${item.itemId})" class="btn btn-sm bg-success-light me-2">
                                    <i class="fe fe-pencil"></i> Sửa
                                </a>
                                <a class="btn btn-sm bg-danger-light" data-bs-toggle="modal" data-id="${item.itemId}" id="confirmXoa" href="#delete_modal">
                                    <i class="fe fe-trash"></i> Xóa
                                </a>
                            </div>
                        </td>
                    </tr>`;

                table.row.add($(rowHtml)); // Thêm từng dòng mới vào bảng
            });

            table.draw();
            format()
        },
        error: function (res) {
            console.error("Error loading data", res);
        }
    });
}

function format() {
    debugger
    function formatCurrency(value) {
        return parseInt(value).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    }

    const prices = document.querySelectorAll('.price');

    prices.forEach(function (priceElement) {
        const priceValue = priceElement.getAttribute('data-price');
        if (priceValue) {
            priceElement.innerText = formatCurrency(priceValue);
        }
    });

}



// change Image
$('.change-photo').on('click', function () {
    $('#fileInputImg').click();
});

$('#fileInputImg').on('change', function (event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#ImgAvt').attr('src', e.target.result);
        };
        reader.readAsDataURL(file);
    }
});


function resetForm() {
    debugger
    $('#errorTen').hide();
    $('#errorGia').hide();

    $('#btnNomal').show();
    $('#btnLoading').hide();

    $('#idSP').val("");
    $('#tenSp').val("");
    $('#giaSp').val("");
    $('#status').prop('checked', true);
    $('#fileInputImg').val(null);
}

function modalEditOrCreate(id) {
    debugger
    resetForm();
    if (id != 0 && id != undefined) {
        $("#titleModal").text("Cập nhật đồ đính kèm")

        var url = "https://localhost:5000/api/SupplementaryItem/" + id;
        $.ajax({
            url: url,
            method: "get",
            success: function (res) {
                debugger
                var logoUrl = "";
                if (res.imageUrl == null || res.imageUrl == "" || res.imageUrl == undefined) {
                    logoUrl = "/image/anh_mau_do_an.jpg"
                } else {
                    logoUrl = res.imageUrl;
                }
                debugger
                $('#idSP').val(res.itemId);
                $("#ImgAvt").attr("src", logoUrl);
                $('#tenSp').val(res.itemName);
                $('#giaSp').val(res.price);
                debugger
                res.isHome == true ? $('#isHome').val(1) : $('#isHome').val(2);
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

        $("#titleModal").text("Thêm đồ đính kèm")

        $("#ImgAvt").attr("src", "/image/anh_mau_do_an.jpg")
        $('#idSP').hide()
      
        $(".modal-backdrop").remove();
        $('#modalEditorCreate').modal("show")

    }

}

function EditOrCreate() {
    if (validation()) {

        $('#btnNomal').hide();
        $('#btnLoading').show();
        var id = $('#idSP').val();

        var formData = new FormData();
        var inputElement = $('#fileInputImg');
        var files = inputElement.prop('files');
        if (files.length > 0) {
            var file = files[0];
            formData.append('Imageurl', file);
        }
        formData.append('ItemId', id);
        formData.append('ItemName', $('#tenSp').val());
        formData.append('Price', $('#giaSp').val());
        if ($('#status').prop('checked')) {
            formData.append('Status', 1);
        } else {
            formData.append('Status', 2);
        }

        if (id != 0 && id != undefined) {
            UpdateMatch(formData, id);

        } else {
            AddMatches(formData);
        }

    }

}

//function checkExist(data , callback) {
//    $.ajax({
//        url: 'https://localhost:5000/api/Matches/CheckExist',
//        type: 'post',
//        data: JSON.stringify(data),
//        contentType: 'application/json',
//        success: function (res) {
//            debugger
//            callback(res.exists, res.mess,res.type); // Giả sử API trả về { exists: true/false }
//        },
//        error: function (error) {
//            console.error("Error during checkExist", error);
//            callback(false); // Mặc định là không tồn tại nếu lỗi xảy ra
//        }
//    });
//}

function AddMatches(formData) {

    $('#errorTen').hide();
    $('#errorGia').hide();
    $.ajax({
        url: 'https://localhost:5000/api/SupplementaryItem',
        type: 'post',
        data: formData,
        contentType: false, // Không thiết lập Content-Type
        processData: false, // Không xử lý dữ liệu
        success: function (response) {
            debugger
            loadData();
            showAlert("Thêm mới thành công");
            $("#modalEditorCreate").modal("hide");
        },
        error: function (error) {
            debugger
        }
    });

    //var data = {
    //    NgayDa: formData.get("MatchDate"),
    //    TenDoiThu: formData.get("OpponentName"),
    //    TenSan: formData.get("StadiumName")
    //}
    debugger
    //checkExist(data, function (exist, mess, type) {
    //    debugger
    //    if (exist) {
    //        if (type == 1) {
    //            $('#errorTenSan').text(mess);
    //            $('#errorTenSan').show();
              
    //        }
    //        else if (type == 2) {
    //            $('#errorNgayDa').text(mess);
    //            $('#errorNgayDa').show();
               

    //        }
    //        else if (type == 3) {
    //            $('#errorNgayDa').text(mess);
    //            $('#errorNgayDa').show();
    //        }
    //        $('#btnNomal').show();
    //        $('#btnLoading').hide();
    //    } else {
           
    //    }
    //});

   
}

function UpdateMatch(formData, id) {

    $.ajax({
        url: 'https://localhost:5000/api/SupplementaryItem/' + id,
        type: 'Put',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            debugger
            loadData();
            showAlert("Cập nhật thành công");
            $("#modalEditorCreate").modal("hide");
        },
        error: function (error) {
            debugger
        }
    });

    //var data = {
    //    NgayDa: formData.get("MatchDate"),
    //    TenDoiThu: formData.get("OpponentName"),
    //    TennSan: formData.get("StadiumName")
    //}
    //debugger
    //checkExist(data, function (exist, mess,type) {

    //    if (exist) {
    //        if (type == 1) {
    //            $('#errorTenSan').text(mess);
    //            $('#errorTenSan').show();
    //        }
    //        else if (type == 2) {
    //            $('#errorNgayDa').text(mess);
    //            $('#errorNgayDa').show();

    //        }
    //        else if (type == 3) {
    //            $('#errorNgayDa').text(mess);
    //            $('#errorNgayDa').show();
    //        }
    //        $('#btnNomal').show();
    //        $('#btnLoading').hide();
    //    } else {
    //        $('#errorNgayDa').hide();
    //        $('#errorNgayDa').hide();

           
    //    }
    //});

  
}
function validation() {

    var check = true;
    if ($('#tenSp').val().trim() == "" || $('#tenSp').val() == null) {
        check = false;
        $('#errorTen').show();
    } else {
        $('#errorTen').hide();

    }
    if ($('#giaSp').val().trim() == "" || $('#giaSp').val() == null) {
        check = false;
        $('#errorGia').show();
    } else {
        $('#errorGia').hide();

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

$(document).on("change", ".changeStatus", function () {
    var checkbox = $(this);
    var status = 1;
    if (checkbox.prop('checked')) {
        status = 1
    } else {
        status = 2;
    }
    var id = checkbox.data("mid");
    var formData = new FormData();

    formData.append('id', id);
    formData.append('status', status);
    debugger
    $.ajax({
        url: 'https://localhost:5000/api/SupplementaryItem/updateStatus',
        type: 'post',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            debugger
            loadData();
            showAlert("Cập nhật trạng thái thành công");
        },
        error: function (error) {
            debugger
        }
    });

});

