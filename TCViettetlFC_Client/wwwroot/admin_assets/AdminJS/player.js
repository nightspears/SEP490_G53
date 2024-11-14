//link api
const apiUrl = 'https://localhost:5000/api/Players';

// f run
document.addEventListener('DOMContentLoaded', () => {
    loadPlayers();
});

// list players
async function loadPlayers() {
    try {
        const response = await fetch(`${apiUrl}/ListPlayer`);
        const players = await response.json();

        const tbody = document.getElementById('tbody');
        tbody.innerHTML = '';

        players.forEach(player => {
            const row = document.createElement('tr');

            row.innerHTML = `
                <td>${player.fullName}</td>
                <td>${player.shirtNumber}</td>
                <td>${player.position}</td>
                <td>${new Date(player.joinDate).toLocaleDateString()}</td>
                <td>${player.status === 1 ? 'Hoạt động' : 'Không hoạt động'}</td>
                <td class="text-center">
                    <a href="javascript:void(0)" class="btn btn-info" onclick="modalEditOrCreate(${player.playerId})">Sửa</a>
                    <a href="javascript:void(0)" class="btn btn-danger" onclick="confirmDelete(${player.playerId})">Xóa</a>
                </td>
            `;

            tbody.appendChild(row);
        });
    } catch (error) {
        console.error('Error loading players:', error);
    }
}

// Open modals
function modalEditOrCreate(playerId) {
    if (playerId === 0) {
        document.getElementById('titleModal').textContent = 'Thêm cầu thủ mới';
        document.getElementById('playerId').value = '';
        document.getElementById('fullName').value = '';
        document.getElementById('shirtNumber').value = '';
        document.getElementById('position').value = '';
        document.getElementById('joinDate').value = '';
        document.getElementById('status').value = '1';
    } else {
        // Fetch existing player data
        fetch(`${apiUrl}/GetPlayerById?id=${playerId}`)
            .then(response => response.json())
            .then(player => {
                document.getElementById('titleModal').textContent = 'Sửa cầu thủ';
                document.getElementById('playerId').value = player.playerId;
                document.getElementById('fullName').value = player.fullName;
                document.getElementById('shirtNumber').value = player.shirtNumber;
                document.getElementById('position').value = player.position;
                document.getElementById('joinDate').value = player.joinDate;
                document.getElementById('status').value = player.status;
            })
            .catch(error => console.error('Error fetching player:', error));
    }
}

// Save a player (create or update)
async function savePlayer() {
    const playerId = document.getElementById('playerId').value;
    const playerData = {
        fullName: document.getElementById('fullName').value,
        shirtNumber: document.getElementById('shirtNumber').value,
        position: document.getElementById('position').value,
        joinDate: document.getElementById('joinDate').value,
        status: parseInt(document.getElementById('status').value, 10)
    };

    try {
        if (playerId) {
            // Update player
            await fetch(`${apiUrl}/UpdatePlayer?id=${playerId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(playerData)
            });
        } else {
            // Add new player
            await fetch(`${apiUrl}/AddPlayer`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(playerData)
            });
        }

        loadPlayers();
        document.getElementById('modalEditorCreate').modal('hide');
    } catch (error) {
        console.error('Error saving player:', error);
    }
}

// Confirm player deletion
function confirmDelete(playerId) {
    document.getElementById('playerIdToDelete').value = playerId;
    $('#delete_modal').modal('show');
}

// Delete a player
document.getElementById('btnDeletePlayer').addEventListener('click', async () => {
    const playerId = document.getElementById('playerIdToDelete').value;
    try {
        await fetch(`${apiUrl}/DeletePlayer?id=${playerId}`, {
            method: 'DELETE'
        });

        loadPlayers();
        $('#delete_modal').modal('hide');
    } catch (error) {
        console.error('Error deleting player:', error);
    }
});
