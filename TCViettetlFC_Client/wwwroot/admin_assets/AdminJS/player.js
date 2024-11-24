// Define the API base URL
const apiUrl = '/api/Players';

// Function to load players
function loadPlayers() {
    $.get(apiUrl, function (data) {
        let playersHtml = "";
        data.forEach(player => {
            playersHtml += `
                <tr>
                    <td>${player.fullName}</td>
                    <td>${player.shirtNumber}</td>
                    <td>${player.position}</td>
                    <td>${new Date(player.joinDate).toLocaleDateString()}</td>
                    <td>${player.status ? 'Hoạt động' : 'Không hoạt động'}</td>
                    <td class="text-center">
                        <button class="btn btn-info btn-sm" onclick="editPlayer(${player.playerId})">Sửa</button>
                        <button class="btn btn-danger btn-sm deletePlayerButton" data-id="${player.playerId}">Xóa</button>
                    </td>
                </tr>
            `;
        });
        $("#tbody").html(playersHtml);
    });
}

// Function to open the modal for adding a new player
function openModal() {
    $('#modalEditorCreatePlayer').modal('show');
    $('#idCauThu').val('');
    $('#tenCauThu').val('');
    $('#soAo').val('');
    $('#viTri').val('');
    $('#ngayGiaNhap').val('');
    $('#statusCauThu').prop('checked', false);
    $('#titleModalPlayer').text('Thêm cầu thủ mới');
}

// Function to save or update player
function savePlayer() {
    const playerData = {
        fullName: $('#tenCauThu').val(),
        shirtNumber: $('#soAo').val(),
        position: $('#viTri').val(),
        joinDate: $('#ngayGiaNhap').val(),
        status: $('#statusCauThu').prop('checked'),
        avatar: $('#avatar')[0].files[0]
    };

    const playerId = $('#idCauThu').val();
    const url = playerId ? `${apiUrl}/${playerId}` : apiUrl;

    const formData = new FormData();
    formData.append("fullName", playerData.fullName);
    formData.append("shirtNumber", playerData.shirtNumber);
    formData.append("position", playerData.position);
    formData.append("joinDate", playerData.joinDate);
    formData.append("status", playerData.status);
    if (playerData.avatar) {
        formData.append("avatar", playerData.avatar);
    }

    $.ajax({
        url: url,
        type: playerId ? 'PUT' : 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function () {
            $('#modalEditorCreatePlayer').modal('hide');
            loadPlayers();
        },
        error: function (error) {
            alert('Error saving player: ' + error.responseText);
        }
    });
}

// Function to open the edit modal with player details
function editPlayer(playerId) {
    $.get(`${apiUrl}/${playerId}`, function (data) {
        $('#idCauThu').val(data.playerId);
        $('#tenCauThu').val(data.fullName);
        $('#soAo').val(data.shirtNumber);
        $('#viTri').val(data.position);
        $('#ngayGiaNhap').val(new Date(data.joinDate).toISOString().substring(0, 10));
        $('#statusCauThu').prop('checked', data.status);
        $('#titleModalPlayer').text('Chỉnh sửa cầu thủ');
        $('#modalEditorCreatePlayer').modal('show');
    });
}

// Function to delete player
$(document).on('click', '#btnXoaPlayer', function () {
    const playerId = $('#idXoaPlayer').val();
    $.ajax({
        url: `${apiUrl}/${playerId}`,
        type: 'DELETE',
        success: function () {
            $('#delete_modal_player').modal('hide');
            loadPlayers();
        },
        error: function () {
            alert('Error deleting player');
        }
    });
});

// Show the delete confirmation modal
$(document).on('click', '.deletePlayerButton', function () {
    const playerId = $(this).data('id');
    $('#idXoaPlayer').val(playerId);
    $('#delete_modal_player').modal('show');
});

// Initial load of players
loadPlayers();
