const API_URL = "https://localhost:5000/api";

// Load danh sách cầu thủ khi trang được tải
async function loadPlayers() {
    try {
        const response = await fetch(API_URL + "/Players");
        if (!response.ok) {
            showErrorMessage("Không truy cập được API.");
            return;
        }
        const players = await response.json();
        populateTable(players);
    } catch (error) {
        showErrorMessage("không load được cầu thủ :"+error);
    }
}

// Khởi tạo trang
document.addEventListener("DOMContentLoaded", () => {
    loadPlayers();
    loadSeasons(); // Tải danh sách mùa giải
});

// Hiển thị danh sách cầu thủ lên bảng
function populateTable(players) {
    const tbody = document.getElementById("tbody");
    tbody.innerHTML = ""; // Xóa bảng trước khi điền dữ liệu mới

    players.forEach(player => {
        const maxLength = 30; // Độ dài tối đa của mô tả
        const description = player.description.length > maxLength
            ? player.description.substring(0, maxLength) + "..."
            : player.description;

        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${player.fullName}</td>
            <td>${player.shirtNumber}</td>
            <td>${player.position}</td>
            <td>${description}</td>
            <td>${new Date(player.joinDate).toLocaleDateString()}</td>
            <td>${player.OutDate ? new Date(player.OutDate).toLocaleDateString() : "N/A"}</td>
            <td>${player.status === 1 ? "Hoạt động" : "Ngừng hoạt động"}</td>
            <td class="text-center">
                <button class="btn btn-sm bg-success-light me-2" onclick="openModal(${player.playerId})">Sửa</button>
                <button class="btn btn-sm bg-danger-light" onclick="deletePlayer(${player.playerId})">Xóa</button>
            </td>
        `;
        tbody.appendChild(row);
    });
}



// Xóa cầu thủ
async function deletePlayer(playerId) {
    const confirmDelete = confirm("Bạn chắc chắn muốn xóa cầu thủ này?");
    if (!confirmDelete) return;

    try {
        const response = await fetch(`${API_URL}/Players/${playerId}`, {
            method: "DELETE",
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        loadPlayers(); // Reload danh sách cầu thủ sau khi xóa
    } catch (error) {
        console.error("Error deleting player:", error);
        showErrorMessage("Không thể xóa cầu thủ.");
    }
}

// Hiển thị thông báo lỗi
function showErrorMessage(message) {
    document.getElementById("alertMessage").textContent = message;
    document.getElementById("alertBox").style.display = "block";
    setTimeout(() => {
        document.getElementById("alertBox").style.display = "none";
    }, 3000); // Ẩn sau 3 giây
}

// Hiển thị thông báo thành công
function showSuccessMessage(message) {
    document.getElementById("alertMessageSuccess").textContent = message;
    document.getElementById("alertSuccess").style.display = "block";
    setTimeout(() => {
        document.getElementById("alertSuccess").style.display = "none";
    }, 3000); // Ẩn sau 3 giây
}

// Tải danh sách mùa giải
async function loadSeasons() {
    try {
        const response = await fetch(`${API_URL}/Season/GetSeason`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        const seasons = await response.json();
        populateSeasonSelect(seasons);
    } catch (error) {
        showErrorMessage("tải mùa giải lỗi :" + error);
        console.log(`${API_URL}/Season/GetSeason`);
    }
}

// Điền các mùa giải vào dropdown
function populateSeasonSelect(seasons) {
    const seasonSelect = document.getElementById("seasonId");
    seasonSelect.innerHTML = "";  // Xóa các lựa chọn cũ

    const defaultOption = document.createElement("option");
    defaultOption.value = "";
    defaultOption.textContent = "--Chọn mùa giải--";
    seasonSelect.appendChild(defaultOption);

    seasons.forEach((season, index) => {
        const option = document.createElement("option");
        option.value = season.seasonId;
        option.textContent = season.seasonName;
        seasonSelect.appendChild(option);

        if (index === 0) {
            option.selected = true; // Mặc định chọn mùa giải đầu tiên
        }
    });
}

// Mở modal để chỉnh sửa cầu thủ
async function openModal(playerId) {
    const titleModalPlayer = document.getElementById("titleModalPlayer");
    const modalPlayer = new bootstrap.Modal(document.getElementById("modalEditorCreatePlayer"));

    if (playerId === 0) {
        titleModalPlayer.textContent = "Thêm cầu thủ mới";
        //tạo form 
        // header 
        titleModalPlayer.textContent = "Thêm cầu thủ mới";

        document.getElementById("idCauThu").value = "";
        document.getElementById("ImgAvt").setAttribute("src", "/image/imagelogo/logoviettel.jpg");  // Ảnh mặc định
        document.getElementById("tenCauThu").value = "";
        document.getElementById("soAo").value = "";
        document.getElementById("viTri").value = "";
        document.getElementById("viTri").value = "";
        document.getElementById("description").value = ""; // Clear input
        document.getElementById("ngayRoiDoi").value = "";  // Clear input
        document.getElementById("statusCauThu").checked = false;

    } else {
        titleModalPlayer.textContent = "Sửa thông tin cầu thủ";
        try {
            const response = await fetch(`${API_URL}/Players/${playerId}`);
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);

            const player = await response.json();
            document.getElementById("ImgAvt").setAttribute("src", player.avatar || "default-avatar.jpg");
            document.getElementById("idCauThu").value = player.playerId;
            document.getElementById("tenCauThu").value = player.fullName;
            document.getElementById("soAo").value = player.shirtNumber;
            document.getElementById("viTri").value = player.position;
            document.getElementById("description").value = player.description;
            document.getElementById("ngayGiaNhap").value = new Date(player.joinDate).toISOString().split("T")[0];
            document.getElementById("ngayRoiDoi").value = player.OutDate ? new Date(player.OutDate).toISOString().split("T")[0] : "";

            // Chọn mùa giải
            document.getElementById("seasonId").value = player.seasonId;
            document.getElementById("statusCauThu").checked = player.status === 1;
        } catch (error) {
            console.error("Error loading player details:", error);
            showErrorMessage("Không thể tải thông tin cầu thủ.");
        }
    }

    modalPlayer.show();
}

// Xử lý thay đổi ảnh avatar
function changeAvatar() {
    const fileReader = new FileReader();
    const filePicker = document.createElement('input');
    filePicker.type = 'file';
    filePicker.accept = 'image/*';  // Chỉ chọn ảnh

    filePicker.onchange = function (event) {
        const file = event.target.files[0];  // Lấy tệp ảnh người dùng chọn
        if (file) {
            fileReader.onload = function (e) {
                document.getElementById('ImgAvt').setAttribute('src', e.target.result);  // Hiển thị ảnh trên thẻ img
            };
            fileReader.readAsDataURL(file);  // Chuyển đổi tệp thành Data URL để hiển thị ảnh
        }
    };

    filePicker.click();  // Mở hộp thoại chọn tệp
}


// Lưu cầu thủ mới
function savePlayer() {
    debugger;
    const avatarInput = document.getElementById('avatarInput');
    const avatarFile = avatarInput.files.length > 0 ? avatarInput.files[0] : null; // Nếu không có file, avatar sẽ là null
    const formData = new FormData();

    // Lấy ID cầu thủ từ modal
    const playerId = document.getElementById('idCauThu').value;

    // Nếu ID là null hoặc rỗng, có nghĩa là đây là tạo mới cầu thủ
    if (!playerId) {
        formData.append('FullName', document.getElementById('tenCauThu').value);
        formData.append('ShirtNumber', document.getElementById('soAo').value);
        formData.append('Position', document.getElementById('viTri').value);
        formData.append('JoinDate', document.getElementById('ngayGiaNhap').value);
        formData.append('OutDate', document.getElementById('ngayRoiDoi').value);
        formData.append('Description', document.getElementById('description').value || ''); // Có thể là null hoặc chuỗi rỗng
        formData.append('SeasonId', document.getElementById('seasonId').value);
        formData.append('Status', document.getElementById('statusCauThu').checked ? true : false);

        // Nếu có ảnh đại diện, thêm vào FormData
        if (avatarFile) {
            formData.append('Avatar', avatarFile);
        }

        // Gửi yêu cầu POST để tạo mới cầu thủ
        fetch(`${API_URL}/Players`, {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showSuccessMessage('Cầu thủ mới đã được thêm!');
                } else {
                    showErrorMessage('Có lỗi xảy ra trả về api thêm cầu thủ');
                }
            })
            .catch(error => {
                showErrorMessage('Có lỗi xảy ra khi thêm cầu thủ');
            });

    } else {
        // Nếu ID có sẵn, đây là thao tác cập nhật cầu thủ
        formData.append('id', playerId);
        formData.append('FullName', document.getElementById('tenCauThu').value);
        formData.append('ShirtNumber', document.getElementById('soAo').value);
        formData.append('Position', document.getElementById('viTri').value);
        formData.append('JoinDate', document.getElementById('ngayGiaNhap').value);
        formData.append('OutDate', document.getElementById('ngayRoiDoi').value);
        formData.append('Description', document.getElementById('description').value || ''); // Có thể là null hoặc chuỗi rỗng
        formData.append('SeasonId', document.getElementById('seasonId').value);
        formData.append('Status', document.getElementById('statusCauThu').checked ? true : false);

        // Nếu có ảnh đại diện, thêm vào FormData
        if (avatarFile) {
            formData.append('Avatar', avatarFile);
        }

        // Gửi yêu cầu PUT để cập nhật cầu thủ
        fetch(`${API_URL}/Players/${playerId}`, {
            method: 'PUT',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showSuccessMessage('Cập nhật cầu thủ thành công!');
                } else {
                    showErrorMessage("Có lỗi xảy ra khi lấy trả về api...")
                }
            })
            .catch(error => {
                showErrorMessage("Có lỗi xảy ra khi cập nhật cầu thủ.:" + error)
            });
    }
}
