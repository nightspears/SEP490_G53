
$(document).ready(function () {
    loadData();
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
    var url = "https://localhost:5000/api/Matches/DeleteMatches/" + id;
    $.ajax({
        url: url,
        method: "Delete",
        success: function (res) {
            debugger;
            $("#delete_modal").modal("hide");
            alert("xóa thành công ");
            loadData();
        },
        error: function (res) {
            alert("xóa thất bại ")
        }
    });

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

            $.each(res, function (index, item) {
                var tenSan = (item.isHome === true ? 'SVĐ Mỹ Đình' : item.stadiumName);
                var logoUrl = "";
                if (item.logoUrl == null || item.logoUrl == "" || item.logoUrl == undefined) {
                    logoUrl = "/image/imagelogo/icon-image-not-found-free-vector.jpg"
                } else {
                    logoUrl = item.logoUrl;
                }
                debugger
                var html = `
    <tr>

        <td>
            <h2 class="table-avatar">
                <a href="profile.html" class="avatar avatar-sm me-2">
                    <img class="avatar-img rounded-circle"
                        src="${logoUrl}"
                        alt="User Image">
                </a>
                <a href="profile.html">${item.opponentName}</a>
            </h2>
        </td>
        <td> ${tenSan}</td>
        <td>${item.matchDate} <br><small></small></td>
        <td>${item.isHome === true ? 'Sân nhà' : 'Sân khách'}</td>
        <td class="text-center">
            <div class="status-toggle d-flex justify-content-center">
                <input type="checkbox" id="status_${item.id}" class="check" ${item.status === 1 ? 'checked' : ''}>
                    <label for="status_${item.id}" class="checktoggle">checkbox</label>
            </div>
        </td>
        <td class="text-center">
            <div class="actions">
                <a onclick="modalEditOrCreate(${item.id})" class="btn btn-sm bg-success-light me-2">
                    <i class="fe fe-pencil"></i> Sửa
                </a>
                <a class="btn btn-sm bg-danger-light" data-bs-toggle="modal" data-id="${item.id}" id="confirmXoa" href="#delete_modal">
                    <i class="fe fe-trash"></i> Xóa
                </a>
            </div>
        </td>
    </tr>`;
                debugger

                tbody.append(html); // Append the generated HTML to tbody
               
            });
            $('.datatable').DataTable({
                "paging": true,
                "pageLength": 10, // Giảm số dòng trên mỗi trang để thử nghiệm
                "ordering": true,
                "info": true
            });
            debugger; // Pause for further debugging
        },


        error: function (res) {
            debugger;
        }
    });
}
// change Image
$('.change-photo-btn').on('click', function () {
    $('#fileInput').click();
});

$('#fileInput').on('change', function (event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#ImgAvt').attr('src', e.target.result);
        };
        reader.readAsDataURL(file);
    }
});

function modalEditOrCreate(id) {
    debugger
    $('#btnNomal').show();
    $('#btnLoading').hide();

    $('#idTran').val("");
    $('#TenDoiThu').val("");
    $('#tenSan').val("");
    $('#ngayDa').val("");
    $('#status').prop('checked', false);

    if (id != 0 && id != undefined) {
        $("#titleModal").text("Cập nhật trận đấu")

        var url = "https://localhost:5000/api/Matches/GetMatchesById?id=" + id;
        $.ajax({
            url: url,
            method: "get",
            success: function (res) {

                var logoUrl = "";
                if (res.logoUrl == null || res.logoUrl == "" || res.logoUrl == undefined) {
                    logoUrl = "/image/imagelogo/icon-image-not-found-free-vector.jpg"
                } else {
                    logoUrl = res.logoUrl;
                }

                $('#idTran').val(res.id);
                $("#ImgAvt").attr("src", logoUrl);
                $('#TenDoiThu').val(res.opponentName);
                $('#tenSan').val(res.stadiumName);
                $("#tenCLB").text(res.opponentName)
                $('#ngayDa').val(res.matchDate);
                debugger
                res.isHome == true ? $('#isHome').val(1) : $('#isHome').val(2);
                if (res.status == 1) {
                    $('#status').prop('checked', true);
                } else {
                    $('#status').prop('checked', false);

                }
                $(".modal-backdrop").hide();

                $(".modal-backdrop").remove();
                $('#modalEditorCreate').modal("show")

            },
            error: function (res) {
                alert("Lỗi ")
            }
        });

    }
    else {

        $("#titleModal").text("Thêm trận đấu")

        $("#ImgAvt").attr("src", "/image/imagelogo/oip.jpg")
        $("#tenCLB").text("Tên đối thủ")
        $('#idTran').hide()
      
        $(".modal-backdrop").remove();
        $('#modalEditorCreate').modal("show")

    }

}

function EditOrCreate() {
    $('#btnNomal').hide();
    $('#btnLoading').show();
    var id = $('#idTran').val();

    var formData = new FormData();
    var inputElement = $('#fileInput');
    var files = inputElement.prop('files');
    if (files.length > 0) {
        var file = files[0];
        formData.append('LogoUrl', file);
    }

    formData.append('OpponentName', $('#TenDoiThu').val());
    formData.append('StadiumName', $('#tenSan').val());
    formData.append('MatchDate', $('#ngayDa').val());
    if ($('#status').prop('checked')) {
        formData.append('Status', 1);
    } else {
        formData.append('Status', 2);
    }

    formData.append('IsHome', $('#isHome').val() === '1');


    if (id != 0 && id != undefined) {
        $.ajax({
            url: 'https://localhost:5000/api/Matches/UpdateMatches/' + id,
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
            url: 'https://localhost:5000/api/Matches/AddMatches',
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
    }, 400000); // 4 seconds delay
}



