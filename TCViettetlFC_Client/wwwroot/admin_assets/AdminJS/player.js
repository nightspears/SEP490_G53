const API_URL = "https://localhost:5000/api/Players";

async function loadPlayers() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        const players = await response.json();
        populateTable(players);
    } catch (error) {
        console.error("Error loading players:", error);
    }
}

function populateTable(players) {
    const tbody = document.getElementById("tbody");
    tbody.innerHTML = ""; // Clear previous rows

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


async function savePlayer() {
    // Thu thập dữ liệu từ biểu mẫu
    const fullName = document.getElementById("tenCauThu").value;
    const shirtNumber = document.getElementById("soAo").value;
    const position = document.getElementById("viTri").value;
    const joinDate = document.getElementById("ngayGiaNhap").value;
    const outDate = document.getElementById("ngayRoiDoi").value;
    const status = document.getElementById("statusCauThu").checked ? 1 : 0;
    const description = document.getElementById("description").value;
    const seasonId = document.getElementById("seasonId").value;
    debugger
    // Tạo URL với query string
    const url = new URL("https://localhost:5000/api/Players");
    url.searchParams.append("FullName", fullName);
    url.searchParams.append("ShirtNumber", shirtNumber);
    url.searchParams.append("Position", position);
    url.searchParams.append("JoinDate", joinDate);
    if (outDate) url.searchParams.append("OutDate", outDate); // Chỉ thêm nếu có giá trị
    url.searchParams.append("Description", description);
    url.searchParams.append("Status", status);
    url.searchParams.append("SeasonId", seasonId);

    try {
        const response = await fetch(url, {
            method: "POST"
        });

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);

        const data = await response.json();
        console.log("Player saved successfully:", data);
        loadPlayers(); // Hàm để tải lại danh sách cầu thủ
    } catch (error) {
        console.error("Error saving player:", error);
    }
}



async function deletePlayer(playerId) {
    // Gán ID cầu thủ vào input ẩn trong modal
    document.getElementById("idXoaPlayer").value = playerId;

    // Hiển thị modal xác nhận
    const deleteModal = new bootstrap.Modal(document.getElementById("delete_modal_player"));
    deleteModal.show();
}
//xác nhận xóa 
document.getElementById("btnXoaPlayer").addEventListener("click", async () => {
    const playerId = document.getElementById("idXoaPlayer").value;

    try {
        const response = await fetch(`${API_URL}/${playerId}`, {
            method: "DELETE",
        });

        if (!response.ok) {
                
                throw new Error(`HTTP error! status: ${response.status}`);
            }
        // Đóng modal sau khi xóa thành công
        const deleteModal = bootstrap.Modal.getInstance(document.getElementById("delete_modal_player"));
        deleteModal.hide();

        // Reload danh sách cầu thủ
        loadPlayers();
    } catch (error) {
        console.error("Error deleting player:", error);
        showError("Không thể xóa cầu thủ.");
    }
});


function showError(message) {
    const errorDiv = document.createElement("div");
    errorDiv.className = "alert alert-danger";
    errorDiv.innerText = message;
    document.querySelector(".modal-body").prepend(errorDiv);
    setTimeout(() => errorDiv.remove(), 3000);
}
// Initialize table on page load
document.addEventListener("DOMContentLoaded", loadPlayers);
function openModal(playerId) {
    console.log(`Opening modal for player ID: ${playerId}`);
}


// mở cái chỗ để nhập thông tin 
async function openModal(playerId) {
    const titleModalPlayer = document.getElementById("titleModalPlayer");
    const modalPlayer = new bootstrap.Modal(document.getElementById("modalEditorCreatePlayer"));
    // nếu như bấm nút tạo thì nó sẽ gửi về id = 0 =>
    if (playerId === 0) {
        //tạo form 
        // header 
        titleModalPlayer.textContent = "Thêm cầu thủ mới";

        document.getElementById("idCauThu").value = "";
        document.getElementById("tenCauThu").value = "";
        document.getElementById("soAo").value = "";
        document.getElementById("viTri").value = "";
        document.getElementById("ngayGiaNhap").value = ""; // Clear input
        document.getElementById("ngayRoiDoi").value = "";  // Clear input
        document.getElementById("statusCauThu").checked = false;
    } else {
        // nếu như bấm nút sửa thì nó gửi về id của cầu thủ cần sửa =>
        titleModalPlayer.textContent = "Sửa thông tin cầu thủ";
        try {
            const response = await fetch(`${API_URL}/${playerId}`);
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);

            const player = await response.json();

            document.getElementById("idCauThu").value = player.playerId;
            document.getElementById("tenCauThu").value = player.fullName;
            document.getElementById("soAo").value = player.shirtNumber;
            document.getElementById("viTri").value = player.position;

            // Format and assign join date
            document.getElementById("ngayGiaNhap").value = player.joinDate
                ? new Date(player.joinDate).toISOString().split("T")[0]
                : "";

            // Format and assign out date
            document.getElementById("ngayRoiDoi").value = player.OutDate
                ? new Date(player.OutDate).toISOString().split("T")[0]
                : "";

            document.getElementById("statusCauThu").checked = player.status === 1;
        } catch (error) {
            console.error("Error loading player details:", error);
            showError("Không thể tải thông tin cầu thủ.");
            return;
        }
    }
    modalPlayer.show();
}

