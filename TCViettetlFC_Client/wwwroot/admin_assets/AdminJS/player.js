// API URL
const apiUrl = 'https://localhost:5000/api/Players';

// Initialize on document ready
$(document).ready(function () {
    loadPlayers();
});

// Load Players List
function loadPlayers() {
    $.ajax({
        url: `${apiUrl}/ListPlayer`,
        method: 'GET',
        success: function (players) {
            const tbody = $('#tbody');
            tbody.empty();

            players.forEach(player => {
                const row = `
                    <tr>
                        <td>${player.fullName}</td>
                        <td>${player.shirtNumber}</td>
                        <td>${player.position}</td>
                        <td>${new Date(player.joinDate).toLocaleDateString()}</td>
                        <td>${player.status === 1 ? 'Hoạt động' : 'Không hoạt động'}</td>
                        <td class="text-center">
                            <button class="btn btn-info" onclick="openModal(${player.playerId})">Sửa</button>
                            <button class="btn btn-danger" onclick="confirmDelete(${player.playerId})">Xóa</button>
                        </td>
                    </tr>`;
                tbody.append(row);
            });
        },
        error: function (error) {
            console.error('Error loading players:', error);
        }
    });
}

// Open Modal for Create or Edit
function openModal(playerId = 0) {
    const modal = $('#modalEditorCreatePlayer');
    modal.modal('show');

    if (playerId === 0) {
        $('#titleModalPlayer').text('Thêm cầu thủ mới');
        resetModalFields();
    } else {
        $('#titleModalPlayer').text('Sửa cầu thủ');
        fetchPlayerById(playerId);
    }
}

// Fetch Player Data by ID
function fetchPlayerById(playerId) {
    $.ajax({
        url: `${apiUrl}/GetPlayerById`,
        method: 'GET',
        data: { id: playerId },
        success: function (player) {
            $('#idCauThu').val(player.playerId);
            $('#tenCauThu').val(player.fullName);
            $('#soAo').val(player.shirtNumber);
            $('#viTri').val(player.position);
            $('#ngayGiaNhap').val(new Date(player.joinDate).toISOString().split('T')[0]);
            $('#statusCauThu').prop('checked', player.status === 1);
        },
        error: function (error) {
            console.error('Error fetching player:', error);
        }
    });
}

// Reset Modal Fields
function resetModalFields() {
    $('#idCauThu').val('');
    $('#tenCauThu').val('');
    $('#soAo').val('');
    $('#viTri').val('');
    $('#ngayGiaNhap').val('');
    $('#statusCauThu').prop('checked', true);
}

// Save Player (Create or Update)
function savePlayer() {
    const playerData = {
        playerId: $('#idCauThu').val(),
        fullName: $('#tenCauThu').val(),
        shirtNumber: $('#soAo').val(),
        position: $('#viTri').val(),
        joinDate: $('#ngayGiaNhap').val(),
        status: $('#statusCauThu').is(':checked') ? 1 : 0
    };

    const url = playerData.playerId ? `${apiUrl}/UpdatePlayer` : `${apiUrl}/AddPlayer`;
    const method = playerData.playerId ? 'PUT' : 'POST';

    $.ajax({
        url: url,
        method: method,
        data: JSON.stringify(playerData),
        contentType: 'application/json',
        success: function () {
            loadPlayers();
            $('#modalEditorCreatePlayer').modal('hide');
        },
        error: function (error) {
            console.error('Error saving player:', error);
        }
    });
}

// Confirm Delete
function confirmDelete(playerId) {
    $('#idXoaPlayer').val(playerId);
    $('#delete_modal_player').modal('show');
}

// Delete Player
$('#btnXoaPlayer').click(function () {
    const playerId = $('#idXoaPlayer').val();

    $.ajax({
        url: `${apiUrl}/DeletePlayer`,
        method: 'DELETE',
        data: { id: playerId },
        success: function () {
            loadPlayers();
            $('#delete_modal_player').modal('hide');
        },
        error: function (error) {
            console.error('Error deleting player:', error);
        }
    });
});
