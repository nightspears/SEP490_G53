function modalEditOrCreate(id) {
    // Nếu id = 0 thì có thể xem như đang tạo mới sản phẩm
    if (id === 0) {
        // Đặt các giá trị mặc định cho các trường trong modal nếu cần thiết
        $('#EditingNewId').val(''); // Reset tên sản phẩm
        $('#EditingCreatorId').val(''); // Reset mùa giải
        $('#EditingTitle').val(''); // Reset thể loại
        $('#EditingNewCategory').val(''); // Reset mô tả
        $('#EditingCreateAt').val(''); // Reset giá
        $('#EditingContent').val(''); // Reset chất liệu
      

        // Hiển thị modal
        $('#modalEditorCreate').modal('show');
    } else {
        // Xử lý khi sửa sản phẩm (nếu cần thiết)
        // Bạn có thể tải thông tin sản phẩm từ server để điền vào các trường modal
        // Sau đó mở modal
        $('#modalEditorCreate').modal('show');
    }
}
// Khi click vào icon chỉnh sửa ảnh, hiển thị file input
document.querySelector('.change-photo').addEventListener('click', function () {
    document.getElementById('fileInputImg').click();
});

// Khi người dùng chọn file, cập nhật ảnh preview
document.getElementById('fileInputImg').addEventListener('change', function (event) {
    var file = event.target.files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            // Cập nhật ảnh preview trong thẻ img
            document.getElementById('ImgAvt').src = e.target.result;
        };
        reader.readAsDataURL(file);
    }
});

